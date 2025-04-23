
namespace EpiSolve
{
    class EA
    {
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
                return other.FitnessScore.CompareTo(this.FitnessScore);
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

            for (int i =  0; i < MaxGenerations; i++)
            {
                List<Individual> nextGeneration = CreateNextGeneration();
                _population = nextGeneration;
                EvaluatePopulation();
            }

            _population.Sort();

            MeasuresStrategy bestStrategy = _population[0].Strategy;

            Console.WriteLine("Evolution finished.\n");
            Console.WriteLine($"Best found strategy: {bestStrategy.ToString()}");

            Simulation.Simulate(bestStrategy, SimParams);

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
            foreach (Individual individual in _population)
            {
                SimulationResult simRes = Simulation.Simulate(individual.Strategy, SimParams);
                individual.FitnessScore = FitnessCalculator.GetFitness(simRes, individual.Strategy, SimParams);
            }
        }


        private Individual SelectParentTournament()
        {
            Individual? bestIndividual = null;

            for (int i = 0; i < PopulationSize; i++)
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
