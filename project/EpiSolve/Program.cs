using System.Diagnostics;

namespace EpiSolve
{
    class Program
    {
        static void Main(string[] args)
        {
            EAParameters eaParams = new EAParameters
                (
                populationSize: 500,
                maxGenerations: 5,
                mutationRate: 0.05,
                mutationStrength: 0.1,
                crossoverRate: 1.0,
                tournamentSize: 15,
                elitismCount: 10,
                numberOfRunsForAveraging: 20
                );

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

            EA ea = new EA(eaParams, simParams);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            ea.FindBestStrategy();

            sw.Stop();
            Console.WriteLine($"Elapsed Time: {sw.Elapsed.TotalSeconds} s\n");
        }
    }
}
