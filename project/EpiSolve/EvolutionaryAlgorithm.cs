using System.Threading.Tasks;
using ScottPlot;
using System.Drawing;

namespace EpiSolve
{
    class EA
    {
        public bool _print = false;

        public int PopulationSize;
        public int MaxGenerations;
        public double MutationRate;
        public double MutationStrength;
        public double CrossoverRate;
        public int TournamentSize;
        public int ElitismCount;
        public SimulationParameters SimParams;
        public GridMap Grid { get; set; }

        private List<Individual> _population;
        private List<double> _bestFitnessPerGeneration;
        private List<double> _averageFitnessPerGeneration;
        private List<double> _worstFitnessPerGeneration;
        private Random _random;

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

        public EA(
            int populationSize,
            int maxGenerations,
            double mutationRate,
            double mutationStrength,
            double crossoverRate,
            int tournamentSize,
            int elitismCount,
            SimulationParameters simParams)
        {
            PopulationSize = populationSize;
            MaxGenerations = maxGenerations;
            MutationRate = mutationRate;
            MutationStrength = mutationStrength;
            CrossoverRate = crossoverRate;
            TournamentSize = tournamentSize;
            ElitismCount = elitismCount;

            SimParams = simParams;

            _population = new List<Individual>(populationSize);
            _bestFitnessPerGeneration = new List<double>();
            _averageFitnessPerGeneration = new List<double>();
            _worstFitnessPerGeneration = new List<double>();
            _random = new Random();
        }


        public MeasuresStrategy FindBestStrategy()
        {
            InitializePopulation();

            this._print = true;

            for (int i = 0; i < MaxGenerations; i++)
            {
                Console.WriteLine($"Generation: {i + 1}/{MaxGenerations}"); // CONTROL PRINT

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
            Console.WriteLine($"The result:\n{bestResult.ToString()}");
            Console.WriteLine($"Fitness: {_population[0].FitnessScore}");

            PlotEvolutionGraph();

            return bestStrategy;
        }


        private List<Individual> CreateNextGeneration()
        {
            _population.Sort();
            List<Individual> newPopulation = new List<Individual>(PopulationSize);

            //for (int i = 0; i < 5; i++)
            //{
            //    MeasuresStrategy strategy = new MeasuresStrategy(_random);
            //    newPopulation.Add(new Individual(strategy));
            //}

            for (int i = 0; i < ElitismCount; i++)
            {
                MeasuresStrategy eliteStrategy = _population[i].Strategy.Clone();
                Individual eliteIndividual = new Individual(eliteStrategy);
                newPopulation.Add(eliteIndividual);
            }

            while (newPopulation.Count < PopulationSize)
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

            for (int i = 0; i < PopulationSize; i++)
            {
                MeasuresStrategy strategy = new MeasuresStrategy(_random);
                _population.Add(new Individual(strategy));
            }
        }


        private void EvaluatePopulation()
        {
            int numberOfRunsPerIndividual = 20;

            Parallel.ForEach(_population, individual =>
            {
                List<SimulationResult> runResults = new List<SimulationResult>();

                for (int i = 0; i < numberOfRunsPerIndividual; i++)
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
                //Console.WriteLine(individual.FitnessScore);
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


        private Individual SelectParentTournament()
        {
            Individual? bestIndividual = null;

            for (int i = 0; i < TournamentSize; i++)
            {
                int randomIndex = _random.Next(PopulationSize);
                Individual candidate = _population[randomIndex];

                if (bestIndividual == null || candidate.FitnessScore < bestIndividual.FitnessScore)
                {
                    bestIndividual = candidate;
                }
            }

            return bestIndividual;
        }


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


        private Individual Mutate(Individual individual)
        {
            MeasuresStrategy mutatedStrategy = individual.Strategy.Clone();
            bool mutated = false;

            if (_random.NextDouble() < MutationRate)
            {
                mutated = true;

                // LockdownStartThreshold
                double change = (_random.NextDouble() * 2.0 - 1.0) * MutationStrength;
                mutatedStrategy.LockdownStartThreshold += change;
                mutatedStrategy.LockdownStartThreshold = Math.Clamp(mutatedStrategy.LockdownStartThreshold, 0.0, 1.0);

                // LockdownEndThreshold
                double maxPossibleEndThreshold = Math.Max(0.0, mutatedStrategy.LockdownStartThreshold - 0.001);
                change = (_random.NextDouble() * 2.0 - 1.0) * MutationStrength;
                mutatedStrategy.LockdownEndThreshold += change;
                mutatedStrategy.LockdownEndThreshold = Math.Clamp(mutatedStrategy.LockdownEndThreshold, 0.0, maxPossibleEndThreshold);

            }

            if (_random.NextDouble() < MutationRate)
            {
                mutated = true;

                // LockdownInfectionReductionFactor
                double change = (_random.NextDouble() * 2.0 - 1.0) * MutationStrength;
                change = (_random.NextDouble() * 2.0 - 1.0) * MutationStrength;
                mutatedStrategy.LockdownInfectionReductionFactor += change;
                mutatedStrategy.LockdownInfectionReductionFactor = Math.Clamp(mutatedStrategy.LockdownInfectionReductionFactor, 0.0, 1.0);
            }

            if (_random.NextDouble() < MutationRate)
            {
                mutated = true;

                // LockdownMovementRestriction
                double change = (_random.NextDouble() * 2.0 - 1.0) * MutationStrength;
                change = (_random.NextDouble() * 2.0 - 1.0) * MutationStrength;
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
                foreach (Individual ind in _population)
                {
                    sumFitness += ind.FitnessScore;
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



        private void PlotEvolutionGraph()
        {
            if (!_bestFitnessPerGeneration.Any())
            {
                Console.WriteLine("No data to plot for evolution graph.");
                return;
            }

            var plt = new ScottPlot.Plot();

            double[] generations = Enumerable.Range(0, _bestFitnessPerGeneration.Count)
                                             .Select(i => (double)i)
                                             .ToArray();

            double[] bestFitnessData = _bestFitnessPerGeneration.ToArray();
            double[] averageFitnessData = _averageFitnessPerGeneration.ToArray();
            double[] worstFitnessData = _worstFitnessPerGeneration.ToArray();


            // Přidání křivky nejlepší fitness
            var bestFitnessLine = plt.Add.Scatter(generations, bestFitnessData);
            bestFitnessLine.Label = "Best Fitness";
            bestFitnessLine.Color = ScottPlot.Colors.Green;
            bestFitnessLine.LineWidth = 2;

            // Přidání křivky average fitness
            var averageFitnessLine = plt.Add.Scatter(generations, averageFitnessData);
            averageFitnessLine.Label = "Average Fitness";
            averageFitnessLine.Color = ScottPlot.Colors.Orange;
            averageFitnessLine.LineWidth = 2;

            // Přidání křivky nejhorší fitness
            var worstFitnessLine = plt.Add.Scatter(generations, worstFitnessData);
            worstFitnessLine.Label = "Worst Fitness";
            worstFitnessLine.Color = ScottPlot.Colors.Red;
            worstFitnessLine.LineWidth = 2;

            // Nastavení popisků os a titulku
            plt.Title("Evolution of Fitness Over Generations");


            plt.Axes.Bottom.Label.Text = "Generation";
            plt.Axes.Left.Label.Text = "Fitness Score (Lower is Better)";

            // Povolení legendy
            plt.Legend.IsVisible = true;
            plt.Legend.Location = ScottPlot.Alignment.UpperRight;

            // Uložení grafu jako obrázek
            string filePath = "../../../../../plots/fitness_graph.png";
            try
            {
                plt.Save(filePath, 600, 400);
                Console.WriteLine($"Evolution graph saved to {System.IO.Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving graph: {ex.Message}");
            }
        }
    }
}
