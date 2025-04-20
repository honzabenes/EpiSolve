using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class Agent
    {
        public SIR Status { get; set; }
        public AgentAge Age { get; set; }
        public GridPosition Position { get; set; }
        public bool beenInfected { get; set; }
        public int TimeInfected { get; set; }
        public int TimeRecovered { get; set; }
        public int TimeVaccinated { get; set; }


        public Agent(SIR status, AgentAge age, GridPosition position)
        {
            this.Status = status;
            this.Age = age;
            this.Position = position;
            this.beenInfected = false;
            this.TimeInfected = 1;
            this.TimeRecovered = 1;
            this.TimeVaccinated = 1;
        }


        public List<GridPosition> GetPossibleMoves(GridMap grid, int[] movesRanges)
        {
            int newX;
            int newY;
            List<GridPosition> possibleMoves = new List<GridPosition>();

            foreach (int x in movesRanges)
            {
                foreach (int y in movesRanges)
                {
                    newX = this.Position.X + x;
                    newY = this.Position.Y + y;

                    if (grid.isValidPosition(newY, newX))
                    {
                        GridPosition newPosition = new GridPosition(newY, newX);
                        possibleMoves.Add(newPosition);
                    }
                }
            }

            return possibleMoves;
        }


        public void Move(GridMap grid, bool isLockdown, Random random)
        {
            int[] movesRanges;

            if (isLockdown)
            {
                movesRanges = new int[] { -1, 0, 1 };
            }
            else
            {
                movesRanges = new int[] { -2, -1, 0, 1, 2 };
            }

            List<GridPosition> possibleMoves = GetPossibleMoves(grid, movesRanges);

            int randomIndex = random.Next(possibleMoves.Count);
            this.Position = possibleMoves[randomIndex];
        }


        public void SetGridTileStatus(GridMap grid)
        {
            int x = this.Position.X;
            int y = this.Position.Y;

            if (this.Status == SIR.Infected)
            {
                int[] movesRanges = { -1, 0, 1 };
                List<GridPosition> infectionRadius = GetPossibleMoves(grid, movesRanges);

                foreach (GridPosition tile in infectionRadius)
                {
                    if (grid.Tiles[tile.Y, tile.X] == TileState.Safe)
                    {
                        grid.Tiles[tile.Y, tile.X] = TileState.ModerateRisk;
                    }
                }
                grid.Tiles[y, x] = TileState.HighRisk;
            }
        }


        public void CloseGridTileStatus(GridMap grid)
        {
            int[] movesRanges = { -1, 0, 1 };
            List<GridPosition> infectionRadius = GetPossibleMoves(grid, movesRanges);

            foreach(GridPosition tile in infectionRadius)
            {
                grid.Tiles[tile.Y, tile.X] = TileState.Safe;
            }
        }


        private void Infect(int time)
        {
            this.Status = SIR.Infected;
            this.TimeInfected = time;
            this.beenInfected = true;
        }


        private void Recover(int time)
        {
            this.Status = SIR.Recovered;
            this.TimeRecovered = time;
        }


        private void Vaccinate(int time)
        {
            this.Status = SIR.Vaccinated;
            this.TimeVaccinated = time;
        }


        public void TryInfect(Agent[] agents, int time, bool isLockdown, double highRiskRate, double moderateRiskRate, 
                              double childInfectionRiskFactor, double elderInfectionRiskFactor,
                              double lockdownReductionFactor, Random random)
        {
            int x = this.Position.X;
            int y = this.Position.Y;

            int numberOfModerateRiskNeighbors = 0;
            int numberOfHighRiskNeighbors = 0;

            foreach (Agent otherAgent in agents)
            {
                if (otherAgent == this) continue;

                if (otherAgent.Status != SIR.Infected) continue;

                int dx = Math.Abs(otherAgent.Position.X - x);
                int dy = Math.Abs(otherAgent.Position.Y - y);

                if (dx == 0 && dy == 0)
                {
                    numberOfHighRiskNeighbors++;
                }
                else if (Math.Max(dx, dy) == 1)
                {
                    numberOfModerateRiskNeighbors++;
                }
            }


            double probOfInfection = 0.0;

            if (numberOfModerateRiskNeighbors > 0 || numberOfHighRiskNeighbors > 0)
            {
                if (isLockdown)
                {
                    moderateRiskRate *= lockdownReductionFactor;
                    highRiskRate *= lockdownReductionFactor;
                }

                switch (this.Age)
                {
                    case AgentAge.Child: 
                    {
                        moderateRiskRate *= Math.Min(childInfectionRiskFactor, 1);
                        highRiskRate *= Math.Min(childInfectionRiskFactor, 1);
                        break;
                    }
                    case AgentAge.Elderly:
                    {
                        moderateRiskRate *= Math.Min(elderInfectionRiskFactor, 1);
                        highRiskRate *= Math.Min(elderInfectionRiskFactor, 1);
                        break;
                    }
                }

                double probOfNoInfection = Math.Pow(1.0 - moderateRiskRate, numberOfModerateRiskNeighbors) *
                                           Math.Pow(1.0 - highRiskRate, numberOfHighRiskNeighbors);

                probOfInfection = 1.0 - probOfNoInfection;
            }

            if (random.NextDouble() < probOfInfection)
            {
                Infect(time);
            }
        }


        private void TryChangeStatus(SIR statusToChangeFrom, int statusChangedTime, int minDurationInStatus, int time, double probabilityOfChangePerStep, Random random)
        {
            int delta = time - statusChangedTime;

            if (this.Status == statusToChangeFrom && delta > minDurationInStatus)
            {
                if (random.NextDouble() < probabilityOfChangePerStep)
                {
                    switch (statusToChangeFrom)
                    {
                        case SIR.Infected:
                            Recover(time);
                            break;

                        case SIR.Recovered:
                            this.Status = SIR.Susceptible;
                            break;
                    }
                }
            }
        }


        public void TryRecover(int minRecoveryTime, int time, 
                               double recoveryRate, Random random)
        {
            TryChangeStatus(SIR.Infected, this.TimeInfected, minRecoveryTime, time, recoveryRate, random);
        }


        public void TryLoseImunity(int minImunityTime, int time, 
                                   double imunityLoseRate, Random random)
        {
            TryChangeStatus(SIR.Recovered, this.TimeRecovered, minImunityTime, time, imunityLoseRate, random);
        }


        public void TryVaccinate(double fearOfVaccination, int time,
                                 double vaccinationSuccessRate, Random random)
        {
            if (random.NextDouble() > fearOfVaccination && 
                random.NextDouble() < vaccinationSuccessRate)
            {
                Vaccinate(time);
            }
        }


        public override string ToString()
        {
            return $"Status: {Status}, Time infected: {TimeInfected}, Time recovered: {TimeRecovered}, Age: {Age}, Position {this.Position.ToString()}";
        }
    }
}
