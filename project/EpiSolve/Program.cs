using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EpiSolve
{
    class Program
    {
        static void Main(string[] args)
        {
            // configurable values
            GridMap grid = new GridMap(10, 10);

            int populationSize = 10;
            int maxGenerations = 30;
            double mutationRate = 0.05;
            double crossoverRate = 1.0;
            int tournamentSize = 2;
            int elitismCount = 1;

            SimulationParameters simParams = new SimulationParameters
                (
                grid: grid,
                agentsCount: 20,
                simulationTime: 100,

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
