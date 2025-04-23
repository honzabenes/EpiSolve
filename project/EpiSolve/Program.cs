using EpiSolve;
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
            GridMap grid = new GridMap(100, 100);


            // configurable values
            SimulationParameters simParams = new SimulationParameters
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

            MeasuresStrategy strategy = new MeasuresStrategy
                (
                lockdownStartThreshold: 20,
                lockdownEndThreshold: 5,
                lockdownReductionFactor: 0.5,
                lockdownMovementRestriction: 0.5
                );


            Stopwatch sw = new Stopwatch();
            sw.Start();

            Simulation.Simulate(grid, simParams, strategy);

            sw.Stop();
            Console.WriteLine($"Elapsed Time: {sw.Elapsed.TotalSeconds} s");
        }
    }
}
