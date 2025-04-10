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
            GridMap grid = new GridMap(10, 20);
            int agentsCount = 5;
            int simulationTime = 100;
            double infectionTransmissionProbability = 0.8;

            Simulation.Simulate(grid, agentsCount, simulationTime, infectionTransmissionProbability);
        }
    }
}
