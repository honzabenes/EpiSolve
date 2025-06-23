using System.Threading.Tasks;
using ScottPlot;
using System.Drawing;

namespace EpiSolve
{
    /// <summary>
    /// Implements the core logic of the Evolutionary Algorithm to find an optimal epidemic control strategy.
    /// </summary>
    class EA
    {
        public AppConfig Config;
        public EAParameters EAParams;
        public SimulationParameters SimParams;
        public GridMap Grid { get; set; }

        private List<Individual> _population;
        private List<double> _bestFitnessPerGeneration;
        private List<double> _averageFitnessPerGeneration;
        private List<double> _worstFitnessPerGeneration;
        private Random _random;

        /// <summary>
        /// Represents an individual in the evolutionary algorithm population,
        /// holding a strategy and its evaluated fitness score.
        /// </summary>
        class Individual : IComparable<Individual>
        {
            public MeasuresStrategy Strategy;
            public double FitnessScore = double.MinValue;

            public Individual(MeasuresStrategy strategy)
            {
                this.Strategy = strategy;
            }

            public int CompareTo(Individual other)
            {
                return this.FitnessScore.CompareTo(other.FitnessScore);
            }
        }

        public EA(AppConfig config)
        {
            EAParams = config.EAParameters;
            SimParams = config.SimulationParameters;

            _population = new List<Individual>(EAParams.PopulationSize);
            _bestFitnessPerGeneration = new List<double>();
            _averageFitnessPerGeneration = new List<double>();
            _worstFitnessPerGeneration = new List<double>();
            _random = new Random();
        }

        /// <summary>
        /// Runs the Evolutionary Algorithm to find the best mitigation strategy.
        /// Initializes the population, runs generations of evaluation, selection, crossover, and mutation.
        /// Records generation statistics and plots the fitness evolution.
        /// Simulates the best found strategy for visualization.
        /// </summary>
        public MeasuresStrategy FindBestStrategy()
        {
            InitializePopulation();

            for (int i = 0; i < EAParams.MaxGenerations; i++)
            {
                Console.WriteLine("Evolution in proces...");
                Console.WriteLine($"Generation: {i + 1}/{EAParams.MaxGenerations}");

                EvaluatePopulation();
                RecordGenerationStats();
                List<Individual> nextGeneration = CreateNextGeneration();
                _population = nextGeneration;

                Console.Clear();
            }

            EvaluatePopulation();
            RecordGenerationStats();
            _population.Sort();

            MeasuresStrategy bestStrategy = _population[0].Strategy;

            int finalDisplaySeed;
            lock (_random)
            {
                finalDisplaySeed = _random.Next();
            }
            SimulationResult bestResult = Simulation.Simulate(bestStrategy, SimParams, finalDisplaySeed, true);

            Console.WriteLine("Evolution finished.\n");
            Console.WriteLine($"Best found strategy:\n{bestStrategy.ToString()}");
            Console.WriteLine($"Simulation result:\n{bestResult.ToString()}");
            Console.WriteLine($"Best fitness: {_population[0].FitnessScore}");

            EAGraphPlotter.PlotEvolutionGraph(_bestFitnessPerGeneration, _worstFitnessPerGeneration, _averageFitnessPerGeneration);

            return bestStrategy;
        }


        private List<Individual> CreateNextGeneration()
        {
            _population.Sort();
            List<Individual> newPopulation = new List<Individual>(EAParams.PopulationSize);

            for (int i = 0; i < EAParams.ElitismCount; i++)
            {
                MeasuresStrategy eliteStrategy = _population[i].Strategy.Clone();
                Individual eliteIndividual = new Individual(eliteStrategy);
                newPopulation.Add(eliteIndividual);
            }

            while (newPopulation.Count < EAParams.PopulationSize)
            {
                Individual parent1 = SelectParentTournament();
                Individual parent2 = SelectParentTournament();

                Individual child = Crossover(parent1, parent2);

                child = Mutate(child);

                newPopulation.Add(child);
            }

            return newPopulation;
        }


        private void InitializePopulation()
        {
            _population.Clear();

            for (int i = 0; i < EAParams.PopulationSize; i++)
            {
                MeasuresStrategy strategy = new MeasuresStrategy(_random);
                _population.Add(new Individual(strategy));
            }
        }


        private void EvaluatePopulation()
        {
            Parallel.ForEach(_population, individual =>
            {
                List<SimulationResult> runResults = new List<SimulationResult>();

                for (int i = 0; i < EAParams.NumberOfRunsForAveraging; i++)
                {
                    int runSeed;
                    lock (_random)
                    {
                        runSeed = _random.Next();
                    }
                    runResults.Add(Simulation.Simulate(individual.Strategy, SimParams, runSeed));
                }

                SimulationResult averagedResult = AverageSimulationResults(runResults);
                individual.FitnessScore = FitnessCalculator.GetFitness(averagedResult, individual.Strategy, SimParams);
            });
        }


        private SimulationResult AverageSimulationResults(List<SimulationResult> results)
        {
            // CONTROL PRINT
            //foreach (SimulationResult result in results)
            //{
            //    Console.WriteLine(result.ToString());
            //}

            double avgTotalInfected = results.Average(r => r.TotalInfected);
            double avgMaxInfected = results.Average(r => r.MaxInfected);
            double avgTotalDead = results.Average(r => r.TotalDead);
            double avgEpidemyDuration = results.Average(r => r.EpidemyDuration);
            double avgLockdownDuration = results.Average(r => r.LockdownDuration);

            SimulationResult avgResult = new SimulationResult(
                (int)Math.Round(avgTotalInfected),
                (int)Math.Round(avgMaxInfected),
                (int)Math.Round(avgTotalDead),
                (int)Math.Round(avgEpidemyDuration),
                (int)Math.Round(avgLockdownDuration)
            );

            // CONTROL PRINT
            //Console.WriteLine($"avg: {avgResult.ToString()}");
            //Console.WriteLine();

            return avgResult;
        }

        /// <summary>
        /// Selects an individual from the population using tournament selection.
        /// Randomly selects a subset (tournament size) of individuals and returns the best one among them.
        /// </summary>
        /// <returns>The best <see cref="Individual"/> found within the random tournament subset.</returns>
        private Individual SelectParentTournament()
        {
            Individual? bestIndividual = null;

            for (int i = 0; i < EAParams.TournamentSize; i++)
            {
                int randomIndex = _random.Next(EAParams.PopulationSize);
                Individual candidate = _population[randomIndex];

                if (bestIndividual == null || candidate.FitnessScore < bestIndividual.FitnessScore)
                {
                    bestIndividual = candidate;
                }
            }

            return bestIndividual;
        }


        /// <summary>
        /// Performs crossover between two parent strategies to create a child strategy.
        /// Averages the corresponding parameters from the two parents.
        /// </summary>
        /// <returns>A new <see cref="Individual"/> representing the child with the combined strategy.</returns>
        private Individual Crossover(Individual parent1, Individual parent2)
        {
            MeasuresStrategy strategy1 = parent1.Strategy;
            MeasuresStrategy strategy2 = parent2.Strategy;

            double lockdownStartThreshold = Average(strategy1.LockdownStartThreshold, strategy2.LockdownStartThreshold);
            double lockdownEndThreshold = Average(strategy1.LockdownEndThreshold, strategy2.LockdownEndThreshold);
            double lockdownInfectionReductionFactor = Average(strategy1.LockdownInfectionReductionFactor, strategy2.LockdownInfectionReductionFactor);
            double lockdownMovementRestriction = Average(strategy1.LockdownMovementRestriction, strategy2.LockdownMovementRestriction);

            MeasuresStrategy childStrategy = new MeasuresStrategy(lockdownStartThreshold, lockdownEndThreshold,
                                                          lockdownInfectionReductionFactor, lockdownMovementRestriction);

            Individual child = new Individual(childStrategy);

            return child;
        }


        private double Average(params double[] numbers)
        {
            double sum = 0;
            foreach (double num in numbers) { sum += num; }

            return sum / numbers.Length;
        }


        /// <summary>
        /// Mutates the strategy of an individual based on the mutation rate and strength.
        /// Randomly adjusts the strategy parameters within defined bounds.
        /// </summary>
        /// <returns>A new <see cref="Individual"/> with the mutated strategy, or the original individual if no mutation occurred.</returns>
        private Individual Mutate(Individual individual)
        {
            MeasuresStrategy mutatedStrategy = individual.Strategy.Clone();
            bool mutated = false;

            if (_random.NextDouble() < EAParams.MutationRate)
            {
                mutated = true;

                // LockdownStartThreshold
                double change = (_random.NextDouble() * 2.0 - 1.0) * EAParams.MutationStrength;
                mutatedStrategy.LockdownStartThreshold += change;
                mutatedStrategy.LockdownStartThreshold = Math.Clamp(mutatedStrategy.LockdownStartThreshold, 0.0, 1.0);

                // LockdownEndThreshold
                double maxPossibleEndThreshold = Math.Max(0.0, mutatedStrategy.LockdownStartThreshold - 0.001);
                change = (_random.NextDouble() * 2.0 - 1.0) * EAParams.MutationStrength;
                mutatedStrategy.LockdownEndThreshold += change;
                mutatedStrategy.LockdownEndThreshold = Math.Clamp(mutatedStrategy.LockdownEndThreshold, 0.0, maxPossibleEndThreshold);

            }

            if (_random.NextDouble() < EAParams.MutationRate)
            {
                mutated = true;

                // LockdownInfectionReductionFactor
                double change = (_random.NextDouble() * 2.0 - 1.0) * EAParams.MutationStrength;
                change = (_random.NextDouble() * 2.0 - 1.0) * EAParams.MutationStrength;
                mutatedStrategy.LockdownInfectionReductionFactor += change;
                mutatedStrategy.LockdownInfectionReductionFactor = Math.Clamp(mutatedStrategy.LockdownInfectionReductionFactor, 0.0, 1.0);
            }

            if (_random.NextDouble() < EAParams.MutationRate)
            {
                mutated = true;

                // LockdownMovementRestriction
                double change = (_random.NextDouble() * 2.0 - 1.0) * EAParams.MutationStrength;
                change = (_random.NextDouble() * 2.0 - 1.0) * EAParams.MutationStrength;
                mutatedStrategy.LockdownMovementRestriction += change;
                mutatedStrategy.LockdownMovementRestriction = Math.Clamp(mutatedStrategy.LockdownMovementRestriction, 0.0, 1.0);
            }

            if (mutated)
            {
                if (mutatedStrategy.LockdownStartThreshold <= mutatedStrategy.LockdownEndThreshold)
                {
                    if (mutatedStrategy.LockdownStartThreshold > 0.01)
                    {
                        mutatedStrategy.LockdownEndThreshold = Math.Max(0.0, mutatedStrategy.LockdownStartThreshold * _random.NextDouble() * 0.9);
                    }
                    else
                    {
                        mutatedStrategy.LockdownEndThreshold = 0.0;
                    }
                    mutatedStrategy.LockdownEndThreshold = Math.Clamp(mutatedStrategy.LockdownEndThreshold, 0.0, Math.Max(0.0, mutatedStrategy.LockdownStartThreshold - 0.001));
                }
                return new Individual(mutatedStrategy);
            }


            return individual;
        }


        private void RecordGenerationStats()
        {
            if (_population.Any())
            {
                _population.Sort();
                _bestFitnessPerGeneration.Add(_population[0].FitnessScore);
                _worstFitnessPerGeneration.Add(_population[_population.Count - 1].FitnessScore);

                double sumFitness = 0;
                foreach (Individual individual in _population)
                {
                    sumFitness += individual.FitnessScore;
                }

                _averageFitnessPerGeneration.Add(sumFitness / _population.Count);
            }
            else
            {
                _bestFitnessPerGeneration.Add(double.NaN);
                _worstFitnessPerGeneration.Add(double.NaN);
                _averageFitnessPerGeneration.Add(double.NaN);
            }
        }
    }
}
