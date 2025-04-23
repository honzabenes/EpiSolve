using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpiSolve
{
    static class FitnessCalculator
    {
        public static double GetFitness(SimulationResult result, MeasuresStrategy strategy, SimulationParameters simParams)
        {
            // result metrics
            int epidemyDuration = result.EpidemyDuration;
            int totalInfected = result.TotalInfected;
            int maxInfected = result.MaxInfected;
            int totalDead = result.TotalDead;
            int lockdownDuration = result.LockdownDuration;

            // strategy metrics
            double lockdownStartThreshold = strategy.LockdownStartThreshold;
            double lockdownEndThreshold = strategy.LockdownEndThreshold;
            double lockdownReductionFactor = strategy.LockdownInfectionReductionFactor;
            double lockdownMovementRestriction = strategy.LockdownMovementRestriction;

            // simulation parameters
            int agentsCount = simParams.AgentsCount;
            int maxSimulationTime = simParams.SimulationTime;



            // normalize metrics
            double normMaxInfected = maxInfected / agentsCount;
            double normTotalInfected = totalInfected / agentsCount;
            double normTotalDead = totalDead / agentsCount;
            double normLockdownDuration = lockdownDuration / maxSimulationTime;
            double normEpidemyDuration = epidemyDuration / maxSimulationTime;

            // weights
            double wTotalDead = 0.4;
            double wMaxInfected = 0.3;
            double wLockdown = 0.2;
            double wTotalInfected = 0.05;
            double wEpidemyDuration = 0.05;

            double fitness =
                wMaxInfected * normMaxInfected +
                wTotalDead * normTotalDead +
                wLockdown * normLockdownDuration +
                wTotalInfected * normTotalInfected +
                wEpidemyDuration * normEpidemyDuration;

            return fitness;
        }
    }
}
