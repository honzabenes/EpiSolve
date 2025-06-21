
namespace EpiSolve
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
                    GridPosition currentPos = new GridPosition(y, x);
                    Agent agentOnTile = agents.FirstOrDefault(agent => agent.Position == currentPos);

                    if (agentOnTile != null)
                    {
                        switch (agentOnTile.Status)
                        {
                            case SIR.Susceptible: Console.ForegroundColor = ConsoleColor.Green; Console.Write('S'); break;
                            case SIR.Infected: Console.ForegroundColor = ConsoleColor.Red; Console.Write('I'); break;
                            case SIR.Recovered: Console.ForegroundColor = ConsoleColor.Blue; Console.Write('R'); break;
                            case SIR.Dead: Console.ForegroundColor = ConsoleColor.White; Console.Write('D'); break;
                            default: Console.Write('?'); break;
                        }
                    }
                    else
                    {
                        Console.Write('-');
                    }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
    }
}