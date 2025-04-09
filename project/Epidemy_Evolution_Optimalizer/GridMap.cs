using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class GridMap
    {
        public uint Height { get; set; }
        public uint Width { get; set; }
        public TileState[,] Tiles { get; set; }

        public GridMap(uint height, uint width)
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

        public bool isValidPosition(uint x, uint y)
        {
            return (x >= 0 && y >= 0 && x < this.Width && y < this.Height);
        }

        public bool isValidPosition(GridPosition position)
        {
            return isValidPosition(position.X, position.Y);
        }

        public TileState GetTileState(uint x, uint y)
        {
            return Tiles[x, y];
        }

        public TileState GetTileState(GridPosition position)
        {
            return GetTileState(position.X, position.Y);
        }
    }
}