using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class TransmissionRates
    {
        public double Safe;
        public double ModerateRisk;
        public double HighRisk;

        public TransmissionRates(double safe, double moderateRisk, double highRisk)
        {
            Safe = safe;
            ModerateRisk = moderateRisk;
            HighRisk = highRisk;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // configurable values
            GridMap grid = new GridMap(10, 20);
            int agentsCount = 10;
            int simulationTime = 300;

            double safeRate = 0.001;
            double moderateRiskRate = 0.3;
            double highRiskRate = 0.8;
            TransmissionRates transmissionRates = new TransmissionRates(safeRate, moderateRiskRate, highRiskRate);

            int minRecoveryTime = 50;
            double recoveryRate = 0.8;
            int minImunityTime = 5;
            double imunityLoseRate = 0.8;

            double childImunityFactor = 0.85; // the lower the less imune
            double elderImunityFactor = 0.85; // the lowe the less imune

            double vaccinationSuccesRate = 0.9;

            int lockdownStartThreshold = 7;
            int lockdownEndThreshold = 3;


            Simulation.Simulate(grid, agentsCount, simulationTime, 
                                transmissionRates, 
                                minRecoveryTime, recoveryRate,
                                minImunityTime, imunityLoseRate,
                                childImunityFactor, elderImunityFactor,
                                vaccinationSuccesRate,
                                lockdownStartThreshold, lockdownEndThreshold);
        }
    }
}
