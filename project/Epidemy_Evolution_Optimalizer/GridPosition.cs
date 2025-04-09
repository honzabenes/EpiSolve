using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class GridPosition
    {
        public uint X { get; set; }
        public uint Y { get; set; }

        public GridPosition(uint x, uint y)
        {
            this.X = x;
            this.Y = y;
        }

        private bool Equals(GridPosition other)
        {
            return (this.X == other.X && this.Y == other.Y);
        }

        public static bool operator == (GridPosition left, GridPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator != (GridPosition left, GridPosition right)
        {
            return !(left.Equals(right));
        }

        public override string ToString()
        {
            return $"(X: {X}, Y: {Y})";
        }
    }
}
