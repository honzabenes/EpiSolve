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
                                   TransmissionRates transmissionRates, 
                                   int minRecoveryTime, double recoveryRate,
                                   int minImunityTime, double imunityLoseRate,
                                   double childImunityFactor, double elderImunityFactor,
                                   double vaccinationSuccessRate,
                                   int lockdownStartTreshold, int lockdownEndTreshold)
        {
            Random random = new Random();
            Agent[] agents = InitAgents(grid, agentsCount, random);

            // time
            int epidemyDuration = simulationTime;

            // infection metric
            int currInfected = 0;
            int maxInfected = 0;
            int totalInfected = 0;

            // vaccination
            double fearOfVaccination = 0.9;
            double changeOfFearFactor;
            bool vaccinationFound = false;
            double vaccinationProgress = 0.0;

            // lockdown
            bool isLockdown = false;
            int lockdownDuration = 0;

            
            for (int time = 1; time < simulationTime + 1; time++)
            {
                currInfected = 0;

                //changeOfFearFactor = (random.NextDouble() * 2 - 1) / 10;
                //vaccinationProgress += random.NextDouble() / 100;

                //if (vaccinationProgress > 1.0) { vaccinationFound = true; }
                //fearOfVaccination += changeOfFearFactor;
               
                //if (fearOfVaccination < 0) { fearOfVaccination = 0; }
                //else if (fearOfVaccination > 1) { fearOfVaccination = 1; }


                foreach (Agent agent in agents)
                {
                    agent.CloseGridTileStatus(grid);
                }
                foreach (Agent agent in agents)
                {
                    agent.TryRecover(minRecoveryTime, time, recoveryRate, random);
                    agent.TryLoseImunity(minImunityTime, time, imunityLoseRate, random);
                    //if (vaccinationFound)
                    //{
                    //    agent.TryVaccinate(fearOfVaccination, time, vaccinationSuccessRate, random);
                    //}
                    agent.Move(grid, isLockdown, random);
                    //Console.WriteLine(agent.ToString()); // CONTROL PRINT
                    agent.SetGridTileStatus(grid);
                }
                foreach (Agent agent in agents)
                {
                    if (agent.Status == SIR.Susceptible) {
                        agent.TryInfect(grid, time, isLockdown, transmissionRates, childImunityFactor, elderImunityFactor, random);
                    }
                    if (agent.Status == SIR.Infected)
                    {
                        agent.beenInfected = true;
                        currInfected++;
                    }
                }

                if (currInfected > maxInfected) { maxInfected = currInfected; }

                if (currInfected >= lockdownStartTreshold) { isLockdown = true; }
                else if (currInfected <= lockdownEndTreshold) { isLockdown = false; }

                if (isLockdown) { lockdownDuration++; }

                // CONTROL PRINTS
                Console.Clear();

                Console.WriteLine($"Time: {time}");
                //Console.WriteLine($"Vac progress: {vaccinationProgress}");
                //Console.WriteLine($"Vac found: {vaccinationFound}");
                //Console.WriteLine($"Fear of vac: {fearOfVaccination}");
                Console.WriteLine($"Is lockdown: {isLockdown}");
                Console.WriteLine($"Infected: {currInfected}");
                Console.WriteLine($"Max Infected: {maxInfected}\n");
                grid.PrintGrid(agents);

                Thread.Sleep(100);

                if (currInfected == 0)
                {
                    epidemyDuration = time;
                    break;
                }
            }

            foreach (Agent agent in agents) { totalInfected += agent.beenInfected ? 1 : 0; }

            SimulationResult result = new SimulationResult(totalInfected, maxInfected, lockdownDuration);
            Console.WriteLine(result.ToString());

            return maxInfected;
        }
    }
}
