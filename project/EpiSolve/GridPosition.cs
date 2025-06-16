
namespace EpiSolve
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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            GridPosition other = (GridPosition)obj;
            return (this.X == other.X && this.Y == other.Y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }


        private bool Equals(GridPosition other)
        {
            return (this.X == other.X && this.Y == other.Y);
        }


        public static bool operator ==(GridPosition left, GridPosition right)
        {
            return left.Equals(right);
        }


        public static bool operator !=(GridPosition left, GridPosition right)
        {
            return !(left.Equals(right));
        }


        public override string ToString()
        {
            return $"(X: {X}, Y: {Y})";
        }
    }
}
