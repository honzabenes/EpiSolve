using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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

        private GridMap _grid { get; set; }
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
            int elitismCount)
        {
            PopulationSize = populationSize;
            MaxGenerations = maxGenerations;
            MutationRate = mutationRate;
            CrossoverRate = crossoverRate;
            TournamentSize = tournamentSize;
            ElitismCount = elitismCount;

            SimParams = new SimulationParameters
                (
                agentsCount: 300,
                simulationTime: 1000,

                moderateRiskRate: 0.3,
                highRiskRate: 0.8,

                minRecoveryTime: 50,
                recoveryRate: 0.8,
                minImunityTime: 5,
                imunityLossRate: 0.8,
                deathRate: 0.001,

                childWeakerImunityFactor: 0.85,
                elderWeakerImunityFactor: 0.85
                );

            _population = new List<Individual>(populationSize);
            _random = new Random();
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
                SimulationResult simRes = Simulation.Simulate(_grid, individual.Strategy, SimParams);
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


        private double Average(double num1, double num2)
        {
            return (num1 + num2) / 2.0;
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
