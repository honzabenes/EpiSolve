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

        public static int Simulate(GridMap grid, int agentsCount, int simulationTime, TransmissionRates transmissionRates)
        {
            Random random = new Random();
            int maxInfected = 0;
            Agent[] agents = InitAgents(grid, agentsCount);
            
            for (int i = 0; i < simulationTime; i++)
            {
                maxInfected = 0;

                foreach (Agent agent in agents)
                {
                    agent.CloseGridTileStatus(grid);
                }
                foreach (Agent agent in agents)
                {
                    agent.Move(grid, random);
                    agent.SetGridTileStatus(grid);
                }
                foreach (Agent agent in agents)
                {
                    if (agent.Status == SIR.Susceptible) {
                        agent.TryInfect(grid, transmissionRates, random);
                    }
                    if (agent.Status == SIR.Infected)
                    {
                        maxInfected++;
                    }
                }
                // CONTROL PRINTS
                Console.WriteLine($"Time: {i + 1}");
                Console.WriteLine($"Infected: {maxInfected}\n");
                grid.PrintGrid(agents);
            }
            
            return maxInfected;
        }
    }
}
