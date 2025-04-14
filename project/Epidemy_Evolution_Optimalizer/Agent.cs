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
        public int TimeInfected { get; set; }
        public int TimeRecovered { get; set; }
        public int TimeVaccinated { get; set; }
        public AgentAge Age { get; set; }
        public GridPosition Position { get; set; }


        public Agent(SIR status, AgentAge age, GridPosition position)
        {
            this.Status = status;
            this.TimeInfected = 1;
            this.TimeRecovered = 1;
            this.TimeVaccinated = 1;
            this.Age = age; 
            this.Position = position;
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


        public void Move(GridMap grid, Random random)
        {
            int[] movesRanges = { -2, -1, 0, 1, 2 };
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


        public void TryInfect(GridMap grid, int time, TransmissionRates transmissionRates, 
                              double childImunityFactor, double elderImunityFactor, Random random)
        {
            int x = this.Position.X;
            int y = this.Position.Y;
            double randomDouble = random.NextDouble();

            switch (this.Age)
            {
                case AgentAge.Child: randomDouble *= childImunityFactor; break;
                case AgentAge.Elderly: randomDouble *= elderImunityFactor; break;
            }

            switch(grid.Tiles[y, x])
            {
                case TileState.Safe:
                    if (randomDouble < transmissionRates.Safe)
                    {
                        Infect(time);
                    }
                    break;

                case TileState.ModerateRisk:
                    if (randomDouble < transmissionRates.ModerateRisk)
                    {
                        Infect(time);
                    }
                    break;

                case TileState.HighRisk:
                    if (randomDouble < transmissionRates.HighRisk)
                    {
                        Infect(time);
                    }
                    break;
            }
        }


        private void TryChangeStatus(SIR statusToChangeFrom, int statusChangedTime, int minStatusTime, int time, double statusChangeRate, Random random)
        {
            int delta = time - statusChangedTime;

            if (this.Status == statusToChangeFrom && delta > minStatusTime)
            {
                if (random.NextDouble() < statusChangeRate)
                {
                    switch (statusToChangeFrom)
                    {
                        case SIR.Infected:
                            Recover(time);
                            //Console.WriteLine("RECOVERED !!");
                            break;

                        case SIR.Recovered:
                            this.Status = SIR.Susceptible;
                            //Console.WriteLine("LOST IMUNITY !!");
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
