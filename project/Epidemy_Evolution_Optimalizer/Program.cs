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
            int agentsCount = 10;
            int simulationTime = 300;
            double infectionTransmissionRate = 1;

            Simulation.Simulate(grid, agentsCount, simulationTime, infectionTransmissionRate);
        }
    }
}
