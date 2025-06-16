
namespace EpiSolve
{
    class EA
    {
        public bool _print = false;

        public int PopulationSize;
        public int MaxGenerations;
        public double MutationRate;
        public double CrossoverRate;
        public int TournamentSize;
        public int ElitismCount;
        public SimulationParameters SimParams;
        public GridMap Grid { get; set; }

        private List<Individual> _population;
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
            double crossoverRate,
            int tournamentSize,
            int elitismCount,
            SimulationParameters simParams)
        {
            PopulationSize = populationSize;
            MaxGenerations = maxGenerations;
            MutationRate = mutationRate;
            CrossoverRate = crossoverRate;
            TournamentSize = tournamentSize;
            ElitismCount = elitismCount;

            SimParams = simParams;

            _population = new List<Individual>(populationSize);
            _random = new Random();
        }


        public MeasuresStrategy FindBestStrategy()
        {
            InitializePopulation();
            EvaluatePopulation();

            this._print = true;

            for (int i =  0; i < MaxGenerations; i++)
            {
                List<Individual> nextGeneration = CreateNextGeneration();
                _population = nextGeneration;
                EvaluatePopulation();
            }

            _population.Sort();

            MeasuresStrategy bestStrategy = _population[0].Strategy;
            SimulationResult bestResult = Simulation.Simulate(bestStrategy, SimParams, true);
            Console.WriteLine("Evolution finished.\n");
            Console.WriteLine($"Best found strategy:\n{bestStrategy.ToString()}");
            Console.WriteLine($"The result: {bestResult.ToString()}");
            Console.WriteLine($"Fitness: {_population[0].FitnessScore}");

            return bestStrategy;
        }


        private List<Individual> CreateNextGeneration()
        {
            _population.Sort();
            List<Individual> newPopulation = new List<Individual>(PopulationSize);

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
            int numberOfRunsPerIndividual = 5;

            foreach (Individual individual in _population)
            {
                List<SimulationResult> runResults = new List<SimulationResult>();
                for (int i = 0; i < numberOfRunsPerIndividual; i++)
                {
                    runResults.Add(Simulation.Simulate(individual.Strategy, SimParams));
                }

                SimulationResult averagedResult = AverageSimulationResults(runResults);
                individual.FitnessScore = FitnessCalculator.GetFitness(averagedResult, individual.Strategy, SimParams);


                // CONTROL PRINT
                //if (true)
                //{
                //    Console.WriteLine(individual.FitnessScore);
                //    Console.WriteLine();
                //}
            }
            this._print = false;

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
            if (_random.NextDouble() < MutationRate)
            {
                MeasuresStrategy strategy = individual.Strategy.Clone();

                double randomDouble = _random.NextDouble();
                strategy.LockdownStartThreshold *= randomDouble;
                strategy.LockdownEndThreshold *= randomDouble;

                strategy.LockdownInfectionReductionFactor *= _random.NextDouble();
                strategy.LockdownMovementRestriction *= _random.NextDouble();

                Individual mutatedIndivdual = new Individual(strategy);

                return mutatedIndivdual;
            }
            else return individual;
        }
    }
}
