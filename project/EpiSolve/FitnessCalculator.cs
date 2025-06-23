
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


            // weights
            double wTotalDead = 0.6;
            double wMaxInfected = 0.4;
            double wLockdown = 0.3;
            double wTotalInfected = 0.05;
            double wEpidemyDuration = 0.05;


            // penalty
            double penalty = 0.0;
            double penaltyFactor = 1;


            // Penalizace za příliš striktní redukci infekce
            if (strategy.LockdownInfectionReductionFactor < 0.2)
            {
                penalty += (0.3 - strategy.LockdownInfectionReductionFactor) * penaltyFactor;
            }
            // Penalizace za příliš striktní omezení pohybu
            if (strategy.LockdownMovementRestriction > 0.9)
            {
                penalty += (strategy.LockdownMovementRestriction - 0.9) * penaltyFactor;
            }

            // Penalizace za pozdní start lockdownu
            if (strategy.LockdownStartThreshold > 0.3)
            {
                penalty += (strategy.LockdownStartThreshold - 0.3) * penaltyFactor;
            }
            // Penalizace za příliš brzké ukončení lockdownu
            if (strategy.LockdownEndThreshold < 0.1)
            {
                penalty += (0.1 - strategy.LockdownEndThreshold) * penaltyFactor;
            }


            double fitness =
                wMaxInfected * normMaxInfected +
                wTotalInfected * normTotalInfected +
                wTotalDead * normTotalDead +
                wLockdown * normLockdownDuration +
                wEpidemyDuration * normEpidemyDuration +
                penalty;


            //double fMaxInfected = wMaxInfected * normMaxInfected;
            //double fTotalInfected = wTotalInfected * normTotalInfected;
            //double fTotalDead = wTotalDead * normTotalDead;
            //double fLockdown = wLockdown * normLockdownDuration;
            //double fEpidemyDuration = wEpidemyDuration * normEpidemyDuration;

            //double[] results = new double[] { fMaxInfected, fTotalInfected, fTotalDead, fLockdown, fEpidemyDuration };


            return fitness;
        }
    }
}
