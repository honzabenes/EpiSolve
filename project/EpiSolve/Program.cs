using EpiSolve;
using System;
using System.Collections.Generic;
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
            SimulationParameters simParams = new SimulationParameters
                (
                grid: new GridMap(100, 100),
                agentsCount: 300,
                simulationTime: 1000,

                moderateRiskRate: 0.3,
                highRiskRate: 0.8,

                minRecoveryTime: 50,
                recoveryRate: 0.8,
                minImunityTime: 5,
                imunityLossRate: 0.8,
                deathProbability: 0.001,

                childWeakerImunityFactor: 0.85,
                elderWeakerImunityFactor: 0.85
                );

            MeasuresStrategy strategy = new MeasuresStrategy
                (
                lockdownStartThreshold: 0.3,
                lockdownEndThreshold: 0.05,
                lockdownReductionFactor: 0.5,
                lockdownMovementRestriction: 0.5
                );


            GridMap grid = new GridMap(100, 100);
            int agentsCount = 300;
            int simulationTime = 2000;

            double moderateRiskRate = 0.3;
            double highRiskRate = 0.8;

            int minRecoveryTime = 50;
            double recoveryRate = 0.8;
            int minImunityTime = 5;
            double imunityLossRate = 0.8;
            double deathProability = 0.001;

            double childWeakerImunityFactor = 0.85; // the less the bigger chance of infection
            double elderWeakerImunityFactor = 0.85;

            double lockdownStartThreshold = 0.3;
            double lockdownEndThreshold = 0.05;
            double lockdownReductionFactor = 0.5;
            double lockdownMovementRestricion = 0.5;


            Simulation.Simulate(simParams, strategy);
        }
    }
}
