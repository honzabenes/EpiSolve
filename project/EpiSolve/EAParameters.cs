
namespace EpiSolve
{
    public class EAParameters
    {
        public int PopulationSize { get; set; }
        public int MaxGenerations { get; set; }
        public double MutationRate { get; set; }
        public double MutationStrength { get; set; }
        public double CrossoverRate { get; set; }
        public int TournamentSize { get; set; }
        public int ElitismCount { get; set; }
        public int NumberOfRunsForAveraging { get; set; }


        public EAParameters() { }

        public EAParameters(int populationSize, int maxGenerations, double mutationRate,
                            double mutationStrength, double crossoverRate, int tournamentSize,
                            int elitismCount, int numberOfRunsForAveraging)
        {
            PopulationSize = populationSize;
            MaxGenerations = maxGenerations;
            MutationRate = mutationRate;
            MutationStrength = mutationStrength;
            CrossoverRate = crossoverRate;
            TournamentSize = tournamentSize;
            ElitismCount = elitismCount;
            NumberOfRunsForAveraging = numberOfRunsForAveraging;
        }
    }
}
