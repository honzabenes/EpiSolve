using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class Simulation
    {
        public GridMap Grid { get; set; }
        public int AgentsCount { get; set; }


        public Simulation(GridMap grid, int agentsCount)
        {
            this.Grid = grid;
            this.AgentsCount = agentsCount;
        }

        private Agent[] InitAgents()
        {
            Agent[] agents = new Agent[AgentsCount];
            Random random = new Random();

            for (int i = 0; i < AgentsCount; i++)
            {
                int randomX = random.Next(Grid.Width);
                int randomY = random.Next(Grid.Height);
                GridPosition position = new GridPosition(randomX, randomY);

                AgentAge[] ages = Enum.GetValues(typeof(AgentAge)).Cast<AgentAge>().ToArray();
                int randomAgeIndex = random.Next(ages.Length);
                AgentAge age = ages[randomAgeIndex];

                SIR status = SIR.Susceptible;

                agents[i] = new Agent(status, age, position);
            }

            return agents;
        }
    }
}
