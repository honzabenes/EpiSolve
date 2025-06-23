
namespace EpiSolve
{
    /// <summary>
    /// Static class responsible for running a single simulation of the epidemic
    /// based on given parameters and a strategy.
    /// </summary>
    static class Simulation
    {
        /// <summary>
        /// Initializes the agents for the simulation with random positions, ages,
        /// and exactly one initially infected agent.
        /// </summary>
        /// <returns>An array of initialized <see cref="Agent"/> objects.</returns>
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


        /// <summary>
        /// Runs a full epidemic simulation based on specified parameters and a strategy.
        /// Tracks simulation metrics like infected counts, duration, and lockdown duration.
        /// Can optionally visualize the simulation grid step-by-step.
        /// </summary>
        /// <returns>A <see cref="SimulationResult"/> object containing the simulation metrics.</returns>
        public static SimulationResult Simulate(MeasuresStrategy strategy, SimulationParameters simParams, int seed, bool visualise = false)
        {
            Random random = new Random(seed);
            Agent[] agents = InitAgents(simParams.Grid, simParams.AgentsCount, random);
            

            int epidemyDuration = simParams.SimulationTime;

            int currInfected;
            int maxInfected = 0;
            int totalInfected = 0;
            int totalDead = 0;

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
                    agent.TryDie(simParams.DeathRate, simParams.ChildWeakerImunityFactor,
                                 simParams.ElderWeakerImunityFactor, random);
                    //Console.WriteLine(agent.ToString()); // CONTROL PRINT
                }

                foreach (Agent agent in agents)
                {
                    agent.TryInfect(agents, simParams.Grid,
                                    time, isLockdown, simParams.HighRiskRate, simParams.ModerateRiskRate,
                                    simParams.ChildWeakerImunityFactor, simParams.ElderWeakerImunityFactor,
                                    strategy.LockdownInfectionReductionFactor,
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

                // VISUAL SIMULATION
                if (visualise)
                {
                    Console.Clear();
                    Console.WriteLine("Simulation of the best strategy.");
                    simParams.Grid.PrintGrid(agents);
                    Thread.Sleep(100);
                }

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
            //Console.WriteLine(result.ToString());

            return result;
        }
    }
}
