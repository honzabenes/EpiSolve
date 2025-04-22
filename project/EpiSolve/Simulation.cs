using EpiSolve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EpiSolve
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


        public static int Simulate(SimulationParameters simParams, MeasuresStrategy strategy)
        {
            Random random = new Random();
            Agent[] agents = InitAgents(simParams.Grid, simParams.AgentsCount, random);

            // time
            int epidemyDuration = simParams.SimulationTime;

            // infection metric
            int currInfected;
            int maxInfected = 0;
            int totalInfected = 0;
            int totalDead = 0;

            // lockdown
            bool isLockdown = false;
            int lockdownDuration = 0;


            for (int time = 1; time < simParams.SimulationTime + 1; time++)
            {
                currInfected = 0;

                foreach (Agent agent in agents)
                {
                    agent.Move(simParams.Grid, isLockdown, strategy.LockdownMovementRestriction, random);
                    agent.TryRecover(simParams.MinRecoveryTime, time, simParams.RecoveryRate, random);
                    agent.TryLoseImunity(simParams.MinImunityTime, time, simParams.ImunityLossRate, random);
                    agent.TryDie(simParams.DeathProbability, simParams.ChildWeakerImunityFactor,
                                 simParams.ElderWeakerImunityFactor, random);
                    //Console.WriteLine(agent.ToString()); // CONTROL PRINT
                }
                foreach (Agent agent in agents)
                {
                    agent.TryInfect(agents, time, isLockdown,
                                    simParams.HighRiskRate, simParams.ModerateRiskRate,
                                    simParams.ChildWeakerImunityFactor, simParams.ElderWeakerImunityFactor,
                                    strategy.LockdownReductionFactor,
                                    random);

                    if (agent.Status == SIR.Infected) currInfected++;
                }

                if (currInfected > maxInfected) { maxInfected = currInfected; }

                if (currInfected >= (int)Math.Round(agents.Length * strategy.LockdownStartThreshold))
                {
                    isLockdown = true;
                }
                else if (currInfected <= (int)Math.Round(agents.Length * strategy.LockdownEndThreshold))
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

            foreach (Agent agent in agents)
            {
                totalInfected += agent.beenInfected ? 1 : 0;
                totalDead += agent.Status == SIR.Dead ? 1 : 0;
            }

            SimulationResult result = new SimulationResult(totalInfected, maxInfected,
                                                           totalDead, epidemyDuration,
                                                           lockdownDuration);
            Console.WriteLine(result.ToString());

            return maxInfected;
        }
    }
}
