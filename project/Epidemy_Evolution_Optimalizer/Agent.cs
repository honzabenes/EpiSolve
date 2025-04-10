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

        public override string ToString()
        {
            return $"Status: {Status}, Age: {Age}";
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

                    if (gridMap.isValidPosition(newX, newY))
                    {
                        possibleMoves.Append(new GridPosition(newX, newY));
                    }
                }
            } 

            int randomIndex = random.Next(possibleMoves.Count);
            this.Position = possibleMoves[randomIndex];
        }
    }
}
