using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    static class FitnessCalculator
    {
        public static double GetFitness(SimulationResult result, MeasuresStrategy strategy)
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
            double lockdownReductionFactor = strategy.LockdownReductionFactor;
            double lockdownMovementRestriction = strategy.LockdownMovementRestriction;
        }
    }
}
