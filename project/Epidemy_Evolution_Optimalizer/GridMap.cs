using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class GridMap
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public TileState[,] Tiles { get; set; }


        public GridMap(int height, int width)
        {
            this.Height = height;
            this.Width = width;
            this.Tiles = new TileState[height, width];

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    this.Tiles[y, x] = TileState.Safe;
                }
            }
        }


        public bool isValidPosition(int y, int x)
        {
            return (x >= 0 && y >= 0 && x < this.Width && y < this.Height);
        }


        public bool isValidPosition(GridPosition position)
        {
            return isValidPosition(position.Y, position.X);
        }


        public TileState GetTileState(int x, int y)
        {
            return Tiles[y, x];
        }


        public TileState GetTileState(GridPosition position)
        {
            return GetTileState(position.Y, position.X);
        }


        public void PrintGrid(Agent[] agents)
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    GridPosition position = new GridPosition(y, x);
                    bool isAgentHere = false;

                    switch (this.Tiles[y, x])
                    {
                        case TileState.Safe:

                            foreach (Agent agent in agents)
                            {
                                if (agent.Position == position && !isAgentHere)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write('x');
                                    isAgentHere = true;
                                }
                            }
                            if (!isAgentHere) { Console.Write('-'); }
                            break;

                        case TileState.ModerateRisk:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write('x');
                            break;

                        case TileState.HighRisk:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write('x');
                            break;
                    }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
    }
}