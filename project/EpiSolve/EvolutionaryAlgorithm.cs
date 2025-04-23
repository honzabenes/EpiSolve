using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
