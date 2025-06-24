using NUnit.Framework;
using EpiSolve;

namespace EpiSolve.Tests
{
    class FitnessCalculatorTests
    {
        // Test základního výpočtu fitness bez penalizací, váhy = 1, norm. max = 1
        [Test]
        public void GetFitness_BasicCalculation_ReturnsCorrectFitness()
        {
            var result = new SimulationResult(
                totalInfected: 100, // normTotalInfected = 100/100 = 1.0
                maxInfected: 50,    // normMaxInfected = 50/100 = 0.5
                totalDead: 10,      // normTotalDead = 10/100 = 0.1
                epidemyDuration: 80,// normEpidemyDuration = 80/100 = 0.8
                lockdownDuration: 30 // normLockdownDuration = 30/100 = 0.3
            );

            var strategy = new MeasuresStrategy( // Parametry nezpůsobující penalizace pro tento test
                lockdownStartThreshold: 0.2,
                lockdownEndThreshold: 0.15,
                lockdownInfectionReductionFactor: 0.5, // Nad 0.3 -> bez penalizace
                lockdownMovementRestriction: 0.8 // Pod 0.9 -> bez penalizace
            );

            var simParams = new SimulationParameters
            {
                AgentsCount = 100,
                SimulationTime = 100,
                WeightTotalDead = 1.0,
                WeightMaxInfected = 1.0,
                WeightLockdown = 1.0,
                WeightTotalInfected = 1.0,
                WeightEpidemyDuration = 1.0,
                // Další parametry nejsou pro tento výpočet fitness relevantní
                HighRiskRate = 0,
                ModerateRiskRate = 0,
                MinRecoveryTime = 0,
                RecoveryRate = 0,
                MinImunityTime = 0,
                ImunityLossRate = 0,
                DeathRate = 0,
                ChildWeakerImunityFactor = 1,
                ElderWeakerImunityFactor = 1
            };

            double expectedFitness = 2.7;

            double actualFitness = FitnessCalculator.GetFitness(result, strategy, simParams);

            double delta = 0.0001;

            Assert.AreEqual(expectedFitness, actualFitness, delta);
        }


        // Test výpočtu fitness s nenulovými vahami a penalizacemi
        [Test]
        public void GetFitness_WithWeightsAndPenalties_ReturnsCorrectFitness()
        {
            var result = new SimulationResult(
                totalInfected: 200, // normTotalInfected = 200/200 = 1.0
                maxInfected: 150,    // normMaxInfected = 150/200 = 0.75
                totalDead: 20,      // normTotalDead = 20/200 = 0.1
                epidemyDuration: 120,// normEpidemyDuration = 120/200 = 0.6
                lockdownDuration: 100 // normLockdownDuration = 100/200 = 0.5
            );

            var strategy = new MeasuresStrategy(
                lockdownStartThreshold: 0.5, // > 0.3 -> Penalty (0.5 - 0.3) = 0.2
                lockdownEndThreshold: 0.05, // < 0.1 -> Penalty (0.1 - 0.05) = 0.05
                lockdownInfectionReductionFactor: 0.1, // < 0.3 -> Penalty (0.3 - 0.1) = 0.2
                lockdownMovementRestriction: 0.95 // > 0.9 -> Penalty (0.95 - 0.9) = 0.05
            );

            var simParams = new SimulationParameters
            {
                AgentsCount = 200,
                SimulationTime = 200,
                WeightTotalDead = 0.8,
                WeightMaxInfected = 0.6,
                WeightLockdown = 0.5,
                WeightTotalInfected = 0.1,
                WeightEpidemyDuration = 0.2,
                // Další parametry nejsou pro tento výpočet fitness relevantní
                HighRiskRate = 0,
                ModerateRiskRate = 0,
                MinRecoveryTime = 0,
                RecoveryRate = 0,
                MinImunityTime = 0,
                ImunityLossRate = 0,
                DeathRate = 0,
                ChildWeakerImunityFactor = 1,
                ElderWeakerImunityFactor = 1
            };

            double expectedFitness = 1.5;

            double actualFitness = FitnessCalculator.GetFitness(result, strategy, simParams);

            double delta = 0.0001;

            Assert.AreEqual(expectedFitness, actualFitness, delta);
        }


        // Test, že fitness je 0, když jsou všechny výsledky 0 (a nejsou penalizace)
        [Test]
        public void GetFitness_AllResultsZeroAndNoPenalties_ReturnsZero()
        {
            var result = new SimulationResult(0, 0, 0, 0, 0); // Všechny metriky 0

            var strategy = new MeasuresStrategy( // Parametry nezpůsobující penalizace
                lockdownStartThreshold: 0.2,
                lockdownEndThreshold: 0.15,
                lockdownInfectionReductionFactor: 0.5,
                lockdownMovementRestriction: 0.8
            );

            var simParams = new SimulationParameters
            {
                AgentsCount = 100,
                SimulationTime = 100,
                WeightTotalDead = 1.0,
                WeightMaxInfected = 1.0,
                WeightLockdown = 1.0,
                WeightTotalInfected = 1.0,
                WeightEpidemyDuration = 1.0,
                HighRiskRate = 0,
                ModerateRiskRate = 0,
                MinRecoveryTime = 0,
                RecoveryRate = 0,
                MinImunityTime = 0,
                ImunityLossRate = 0,
                DeathRate = 0,
                ChildWeakerImunityFactor = 1,
                ElderWeakerImunityFactor = 1
            };

            double expectedFitness = 0.0;

            double actualFitness = FitnessCalculator.GetFitness(result, strategy, simParams);

            double delta = 0.0001;

            Assert.AreEqual(expectedFitness, actualFitness, delta);
        }


        // Test, že penalizace fungují, i když výsledky jsou 0
        [Test]
        public void GetFitness_ZeroResultsButWithPenalties_ReturnsPenaltyValue()
        {
            var result = new SimulationResult(0, 0, 0, 0, 0); // Všechny metriky 0

            var strategy = new MeasuresStrategy( // Parametry ZPŮSOBUJÍCÍ penalizace
                lockdownStartThreshold: 0.5, // > 0.3 -> Penalty (0.5 - 0.3) = 0.2
                lockdownEndThreshold: 0.05, // < 0.1 -> Penalty (0.1 - 0.05) = 0.05
                lockdownInfectionReductionFactor: 0.1, // < 0.3 -> Penalty (0.3 - 0.1) = 0.2
                lockdownMovementRestriction: 0.95 // > 0.9 -> Penalty (0.95 - 0.9) = 0.05
            );

            var simParams = new SimulationParameters
            {
                AgentsCount = 100,
                SimulationTime = 100,
                WeightTotalDead = 1.0,
                WeightMaxInfected = 1.0,
                WeightLockdown = 1.0,
                WeightTotalInfected = 1.0,
                WeightEpidemyDuration = 1.0,
                HighRiskRate = 0,
                ModerateRiskRate = 0,
                MinRecoveryTime = 0,
                RecoveryRate = 0,
                MinImunityTime = 0,
                ImunityLossRate = 0,
                DeathRate = 0,
                ChildWeakerImunityFactor = 1,
                ElderWeakerImunityFactor = 1
            };

            double expectedFitness = 0.5;

            double actualFitness = FitnessCalculator.GetFitness(result, strategy, simParams);

            double delta = 0.0001;

            Assert.AreEqual(expectedFitness, actualFitness, delta);
        }
    }
}
