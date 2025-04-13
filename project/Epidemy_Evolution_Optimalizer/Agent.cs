using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class Agent
    {
        public SIR Status { get; set; }
        public int TimeInfected { get; set; }
        public int TimeRecovered { get; set; }
        public AgentAge Age { get; set; }
        public GridPosition Position { get; set; }


        public Agent(SIR status, AgentAge age, GridPosition position)
        {
            this.Status = status;
            this.TimeInfected = 0;
            this.TimeRecovered = 0;
            this.Age = age; 
            this.Position = position;
        }

        public List<GridPosition> GetPossibleMoves(GridMap grid)
        {
            int newX;
            int newY;
            List<GridPosition> possibleMoves = new List<GridPosition>();
            int[] moves = { -1, 0, 1 };

            foreach (int x in moves)
            {
                foreach (int y in moves)
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
            List<GridPosition> possibleMoves = GetPossibleMoves(grid);

            int randomIndex = random.Next(possibleMoves.Count);
            this.Position = possibleMoves[randomIndex];
        }

        public void SetGridTileStatus(GridMap grid)
        {
            int x = this.Position.X;
            int y = this.Position.Y;

            if (this.Status == SIR.Infected)
            {
                List<GridPosition> infectionRadius = GetPossibleMoves(grid);
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
            List<GridPosition> infectionRadius = GetPossibleMoves(grid);

            foreach(GridPosition tile in infectionRadius)
            {
                grid.Tiles[tile.Y, tile.X] = TileState.Safe;
            }
        }

        public void TryInfect(GridMap grid, TransmissionRates transmissionRates, Random random)
        {
            int x = this.Position.X;
            int y = this.Position.Y;
            double randomDouble = random.NextDouble();

            switch(grid.Tiles[y, x])
            {
                case TileState.Safe:
                    if (randomDouble < transmissionRates.Safe)
                    {
                        this.Status = SIR.Infected;
                    }
                    break;

                case TileState.ModerateRisk:
                    if (randomDouble < transmissionRates.ModerateRisk)
                    {
                        this.Status = SIR.Infected;
                    }
                    break;

                case TileState.HighRisk:
                    if (randomDouble < transmissionRates.HighRisk)
                    {
                        this.Status = SIR.Infected;
                    }
                    break;
            }
        }

        public void TryRecover(int minRecoveryTime, int time, double recoveryRate, Random random)
        {
            int delta = time - minRecoveryTime;

            if (this.Status == SIR.Infected && delta > minRecoveryTime)
            {
                if (recoveryRate < random.NextDouble())
                {
                    this.Status = SIR.Recovered;
                }
            }
        }

        public override string ToString()
        {
            return $"Status: {Status}, Age: {Age}, Position {this.Position.ToString()}";
        }
    }
}
