using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class GridPosition
    {
        public int Y { get; set; }
        public int X { get; set; }


        public GridPosition(int y, int x)
        {
            this.Y = y;
            this.X = x;
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
