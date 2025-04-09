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

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    this.Tiles[y, x] = TileState.Safe;
                }
            }
        }

        public bool isValidPosition(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < this.Width && y < this.Height);
        }

        public bool isValidPosition(GridPosition position)
        {
            return isValidPosition(position.X, position.Y);
        }

        public TileState GetTileState(int x, int y)
        {
            return Tiles[x, y];
        }

        public TileState GetTileState(GridPosition position)
        {
            return GetTileState(position.X, position.Y);
        }
    }
}