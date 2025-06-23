
namespace EpiSolve
{
    static class FitnessCalculator
    {
        public static double GetFitness(SimulationResult result, MeasuresStrategy strategy, SimulationParameters simParams)
        {
            // result metrics
            double maxInfected = result.MaxInfected;
            double totalInfected = result.TotalInfected;
            double totalDead = result.TotalDead;
            double lockdownDuration = result.LockdownDuration;
            double epidemyDuration = result.EpidemyDuration;


            // strategy metrics
            double lockdownStartThreshold = strategy.LockdownStartThreshold;
            double lockdownEndThreshold = strategy.LockdownEndThreshold;
            double lockdownReductionFactor = strategy.LockdownInfectionReductionFactor;
            double lockdownMovementRestriction = strategy.LockdownMovementRestriction;


            // simulation parameters
            double agentsCount = simParams.AgentsCount;
            double maxSimulationTime = simParams.SimulationTime;


            // normalize metrics
            double normMaxInfected = maxInfected / agentsCount;
            double normTotalInfected = totalInfected / agentsCount;
            double normTotalDead = totalDead / agentsCount;
            double normLockdownDuration = lockdownDuration / maxSimulationTime;
            double normEpidemyDuration = epidemyDuration / maxSimulationTime;


            // penalty
            double penalty = 0.0;

            // Penalizace za příliš striktní redukci infekce
            if (strategy.LockdownInfectionReductionFactor < 0.2)
            {
                penalty += (0.3 - strategy.LockdownInfectionReductionFactor);
            }
            // Penalizace za příliš striktní omezení pohybu
            if (strategy.LockdownMovementRestriction > 0.9)
            {
                penalty += (strategy.LockdownMovementRestriction - 0.9);
            }

            // Penalizace za pozdní start lockdownu
            if (strategy.LockdownStartThreshold > 0.3)
            {
                penalty += (strategy.LockdownStartThreshold - 0.3);
            }
            // Penalizace za příliš brzké ukončení lockdownu
            if (strategy.LockdownEndThreshold < 0.1)
            {
                penalty += (0.1 - strategy.LockdownEndThreshold);
            }


            double fitness =
                simParams.WeightMaxInfected * normMaxInfected +
                simParams.WeightTotalInfected * normTotalInfected +
                simParams.WeightTotalDead * normTotalDead +
                simParams.WeightLockdown * normLockdownDuration +
                simParams.WeightEpidemyDuration * normEpidemyDuration +
                penalty;

            return fitness;
        }
    }
}
