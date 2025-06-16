using System.Diagnostics;

namespace EpiSolve
{
    class Program
    {
        static void Main(string[] args)
        {
            // configurable values
            int populationSize = 100;
            int maxGenerations = 30;
            double mutationRate = 0.05;
            double crossoverRate = 1.0;
            int tournamentSize = 20;
            int elitismCount = 5;

            SimulationParameters simParams = new SimulationParameters
                (
                gridHeight: 10,
                gridWidth: 10,
                agentsCount: 25,
                simulationTime: 50,

                moderateRiskRate: 0.3,
                highRiskRate: 0.8,

                minRecoveryTime: 10,
                recoveryRate: 0.8,
                minImunityTime: 5,
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
