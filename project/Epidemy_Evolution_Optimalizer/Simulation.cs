using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    static class Simulation
    {
        private static Agent[] InitAgents(GridMap grid, int agentsCount)
        {
            Agent[] agents = new Agent[agentsCount];
            Random random = new Random();
            bool isSomeoneInfected = false;

            for (int i = 0; i < agentsCount; i++)
            {
                int randomX = random.Next(grid.Width);
                int randomY = random.Next(grid.Height);
                GridPosition position = new GridPosition(randomY, randomX);

                AgentAge[] ages = Enum.GetValues(typeof(AgentAge)).Cast<AgentAge>().ToArray();
                int randomAgeIndex = random.Next(ages.Length);
                AgentAge age = ages[randomAgeIndex];

                SIR status;
                if (!isSomeoneInfected)
                {
                    status = SIR.Infected;
                    isSomeoneInfected = true;
                }
                else
                {
                    status = SIR.Susceptible;
                }

                agents[i] = new Agent(status, age, position);
            }

            return agents;
        }

        private static void SetGridTileStatus(GridMap grid, Agent agent)
        {
            int x = agent.Position.X;
            int y = agent.Position.Y;

            if (agent.Status == SIR.Infected)
            {
                grid.Tiles[y, x] = TileState.HighRisk;
            }
        }

        private static void CloseGridTileStatus(GridMap grid, Agent agent)
        {
            int x = agent.Position.X;
            int y = agent.Position.Y;

            grid.Tiles[y, x] = TileState.Safe;
        }

        public static int Simulate(GridMap grid, int agentsCount, int simulationTime, double infectionTransmissionProbabiltiy)
        {
            Random random = new Random();
            int maxInfected = 0;
            Agent[] agents = InitAgents(grid, agentsCount);
            
            for (int i = 0; i < simulationTime; i++)
            {
                foreach (Agent agent in agents)
                {
                    CloseGridTileStatus(grid, agent);
                    agent.Move(grid, random);
                    SetGridTileStatus(grid, agent);
                }
                foreach (Agent agent in agents)
                {
                    bool isInfected = agent.TryInfect(grid, infectionTransmissionProbabiltiy, random);
                    if (isInfected)
                    {
                        maxInfected++;
                    }
                }
                grid.PrintGrid();
            }
            
            return maxInfected;
        }
    }
}
