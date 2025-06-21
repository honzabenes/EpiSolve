using System.Diagnostics;

namespace EpiSolve
{
    class Program
    {
        static void Main(string[] args)
        {
            // configurable values
            int populationSize = 500;
            int maxGenerations = 30;
            double mutationRate = 0.05;
            double mutationStrength = 0.1;
            double crossoverRate = 1.0;
            int tournamentSize = 15;
            int elitismCount = 10;

            SimulationParameters simParams = new SimulationParameters
                (
                gridHeight: 10,
                gridWidth: 10,
                agentsCount: 35,
                simulationTime: 100,

                moderateRiskRate: 0.3,
                highRiskRate: 0.8,

                minRecoveryTime: 10,
                recoveryRate: 0.8,
                minImunityTime: 20,
                imunityLossRate: 0.8,
                deathRate: 0.001,

                childWeakerImunityFactor: 0.85,
                elderWeakerImunityFactor: 0.85
                );

            EA ea = new EA
                (
                populationSize,
                maxGenerations,
                mutationRate,
                mutationStrength,
                crossoverRate,
                tournamentSize,
                elitismCount,
                simParams
                );

            Stopwatch sw = new Stopwatch();
            sw.Start();

            ea.FindBestStrategy();

            sw.Stop();
            Console.WriteLine($"Elapsed Time: {sw.Elapsed.TotalSeconds} s");
        }
    }
}
