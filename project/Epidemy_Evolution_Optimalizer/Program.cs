using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{

    class Program
    {
        static void Main(string[] args)
        {
            // configurable values
            GridMap grid = new GridMap(10, 20);
            int agentsCount = 10;
            int simulationTime = 300;

            double moderateRiskRate = 0.3;
            double highRiskRate = 0.8;

            int minRecoveryTime = 50;
            double recoveryRate = 0.8;
            int minImunityTime = 5;
            double imunityLossRate = 0.8;

            double childWeakerImunityFactor = 0.85; // the less the bigger chance of infection
            double elderWeakerImunityFactor = 0.85;

            double vaccinationSuccesRate = 0.9;

            int lockdownStartThreshold = 7;
            int lockdownEndThreshold = 3;
            double lockdownReductionFactor = 0.5;
            double lockdownMovementRestricion = 0.5;


            Simulation.Simulate(grid, agentsCount, simulationTime, 
                                highRiskRate, moderateRiskRate, 
                                minRecoveryTime, recoveryRate,
                                minImunityTime, imunityLossRate,
                                childWeakerImunityFactor, elderWeakerImunityFactor,
                                vaccinationSuccesRate,
                                lockdownStartThreshold, lockdownEndThreshold, 
                                lockdownReductionFactor, lockdownMovementRestricion);
        }
    }
}
