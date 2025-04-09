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
            Position = position;
        }

        public override string ToString()
        {
            return $"Status: {Status}, Age: {Age}";
        }

        public void Move(GridMap gridMap)
        {
            int newX;
            int newY;
            List<GridPosition> possibleMoves = new List<GridPosition>();
            int[] moves = { -1, 1 };

            foreach (int move in moves)
            {
                newX = this.Position.X + move;
                newY = this.Position.Y + move;

                if (gridMap.isValidPosition(newX, this.Position.Y))
                {
                    possibleMoves.Append(new GridPosition(newX, this.Position.Y));
                }

                if (gridMap.isValidPosition(this.Position.X, newY))
                {
                    possibleMoves.Append(new GridPosition(this.Position.X, newY));
                }
            }

            Random random = new Random();
            int randomIndex = random.Next(possibleMoves.Count);

            this.Position = possibleMoves[randomIndex];
        }
    }
}
