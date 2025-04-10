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

        public void PrintGrid()
        {
            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    switch (this.Tiles[y, x])
                    {
                        case TileState.Safe:
                            sb.Append('-');
                            break;

                        case TileState.HighRisk:
                            sb.Append('R');
                            break;
                    }
                }
                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());
        }
    }
}