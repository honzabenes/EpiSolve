using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class Agent
    {
        public GridPosition Position { get; set; }
        public SIR Status { get; set; }
        public AgentAge Age { get; set; }

        public Agent(SIR status, AgentAge age, GridPosition position)
        {
            this.Status = status;
            this.Age = age; 
            this.Position = position;
        }

        public void Move(GridMap gridMap, Random random)
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

                    if (gridMap.isValidPosition(newY, newX))
                    {
                        GridPosition newPosition = new GridPosition(newY, newX);
                        possibleMoves.Add(newPosition);
                    }
                }
            } 

            int randomIndex = random.Next(possibleMoves.Count);
            this.Position = possibleMoves[randomIndex];
        }

        public void SetGridTileStatus(GridMap grid)
        {
            int x = this.Position.X;
            int y = this.Position.Y;

            if (this.Status == SIR.Infected)
            {
                grid.Tiles[y, x] = TileState.HighRisk;
            }
        }

        public void CloseGridTileStatus(GridMap grid)
        {
            int x = this.Position.X;
            int y = this.Position.Y;

            grid.Tiles[y, x] = TileState.Safe;
        }

        public void TryInfect(GridMap grid, double infectionTransmissionRate, Random random)
        {
            int x = this.Position.X;
            int y = this.Position.Y;

            if (grid.Tiles[y, x] == TileState.HighRisk && 
                random.NextDouble() < infectionTransmissionRate)
            {
                this.Status = SIR.Infected;
            }
        }

        public override string ToString()
        {
            return $"Status: {Status}, Age: {Age}";
        }
    }
}
