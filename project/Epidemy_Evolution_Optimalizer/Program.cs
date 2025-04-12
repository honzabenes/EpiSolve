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
            GridMap grid = new GridMap(10, 20);
            int agentsCount = 10;
            int simulationTime = 100;

            double safeRate = 0.0;
            double moderateRiskRate = 0.3;
            double highRiskRate = 0.8;
            TransmissionRates transmissionRates = new TransmissionRates(safeRate, moderateRiskRate, highRiskRate);

            Simulation.Simulate(grid, agentsCount, simulationTime, transmissionRates);
        }
    }
}
