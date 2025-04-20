using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    static class Simulation
    {
        private static Agent[] InitAgents(GridMap grid, int agentsCount, Random random)
        {
            Agent[] agents = new Agent[agentsCount];
            bool isSomeoneInfected = false;

            for (int i = 0; i < agentsCount; i++)
            {
                int randomX = random.Next(grid.Width);
                int randomY = random.Next(grid.Height);
                GridPosition position = new GridPosition(randomY, randomX);

                AgentAge age;
                double randomDouble = random.NextDouble();

                if (randomDouble > 0.4)
                {
                    age = AgentAge.Adult;
                }
                else if (randomDouble > 0.2)
                {
                    age = AgentAge.Child;
                }
                else
                {
                    age = AgentAge.Elderly;
                }

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


        public static int Simulate(GridMap grid, int agentsCount, int simulationTime, 
                                   double highRiskRate, double moderateRiskRate, 
                                   int minRecoveryTime, double recoveryRate,
                                   int minImunityTime, double imunityLossRate,
                                   double deathProbability,
                                   double childWeakerImunityFactor, double elderWeakerImunityFactor,
                                   double lockdownStartThreshold, double lockdownEndTreshold, 
                                   double lockdownReductionFactor, double lockdownMovementRestricition)
        {
            Random random = new Random();
            Agent[] agents = InitAgents(grid, agentsCount, random);

            // time
            int epidemyDuration = simulationTime;

            // infection metric
            int currInfected;
            int maxInfected = 0;
            int totalInfected = 0;
            int totalDead = 0;

            // lockdown
            bool isLockdown = false;
            int lockdownDuration = 0;

            
            for (int time = 1; time < simulationTime + 1; time++)
            {
                currInfected = 0;

                foreach (Agent agent in agents)
                {
                    agent.Move(grid, isLockdown, lockdownMovementRestricition, random);
                    agent.TryRecover(minRecoveryTime, time, recoveryRate, random);
                    agent.TryLoseImunity(minImunityTime, time, imunityLossRate, random);
                    agent.TryDie(deathProbability, childWeakerImunityFactor, elderWeakerImunityFactor, random);
                    //Console.WriteLine(agent.ToString()); // CONTROL PRINT
                }
                foreach (Agent agent in agents)
                {
                    agent.TryInfect(agents, time, isLockdown, highRiskRate, moderateRiskRate, 
                                    childWeakerImunityFactor, elderWeakerImunityFactor, 
                                    lockdownReductionFactor, random);

                    if (agent.Status == SIR.Infected) currInfected++;
                }

                if (currInfected > maxInfected) { maxInfected = currInfected; }

                if (currInfected >= (int)Math.Round(agents.Length * lockdownStartThreshold))
                {
                    isLockdown = true;
                }
                else if (currInfected <= (int)Math.Round(agents.Length * lockdownEndTreshold))
                {
                    isLockdown = false;
                }

                if (isLockdown) lockdownDuration++;

                //CONTROL PRINTS
                //Console.Clear();

                //Console.WriteLine($"Time: {time}");
                //Console.WriteLine($"Is lockdown: {isLockdown}");
                //Console.WriteLine($"Infected: {currInfected}");
                //Console.WriteLine($"Max Infected: {maxInfected}\n");
                //grid.PrintGrid(agents);

                //Thread.Sleep(100);

                if (currInfected == 0)
                {
                    epidemyDuration = time;
                    break;
                }
            }

            foreach (Agent agent in agents) {
                totalInfected += agent.beenInfected ? 1 : 0;
                totalDead += agent.Status == SIR.Dead ? 1 : 0;
            }

            SimulationResult result = new SimulationResult(totalInfected, maxInfected, totalDead, epidemyDuration, lockdownDuration);
            Console.WriteLine(result.ToString());

            return maxInfected;
        }
    }
}
