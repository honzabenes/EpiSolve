using System;
using System.Text.Json;

namespace EpiSolve
{
    class AppConfig
    {
        public EAParameters EAParameters { get; set; }
        public SimulationParameters SimulationParameters { get; set; }
    }


    /// <summary>
    /// Static class responsible for loading and managing the application configuration from a JSON file.
    /// Provides default configurations if the file is not found or loading fails.
    /// </summary>
    static class ConfigLoader
    {
        /// <summary>
        /// Loads the application configuration from a specified JSON file.
        /// If the file is not found, it creates a default configuration and saves it.
        /// Handles potential errors during file reading or deserialization by providing an emergency default config.
        /// </summary>
        public static AppConfig LoadConfig(string pathToFile = "config.json")
        {
            try
            {
                if (!File.Exists(pathToFile))
                {
                    Console.WriteLine($"Konfigurační soubor '{pathToFile}' nenalezen. Používám výchozí hodnoty nebo vytvářím nový.");
                    return CreateDefaultConfigAndSave(pathToFile);
                }

                string jsonString = File.ReadAllText(pathToFile);
                AppConfig config = JsonSerializer.Deserialize<AppConfig>(jsonString);

                config?.SimulationParameters?.InitializeGrid();

                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při načítání konfigurace z '{pathToFile}': {ex.Message}");
                Console.WriteLine("Používám nouzové výchozí hodnoty.");
                return CreateEmergencyDefaultConfig();
            }
        }

        private static AppConfig CreateDefaultConfigAndSave(string filePath)
        {
            var defaultConfig = new AppConfig
            {
                EAParameters = new EAParameters(
                    populationSize: 500, maxGenerations: 15, mutationRate: 0.05,
                    mutationStrength: 0.1, crossoverRate: 0.8, tournamentSize: 15,
                    elitismCount: 10, numberOfRunsForAveraging: 20
                ),
                SimulationParameters = new SimulationParameters(
                    gridHeight: 10, gridWidth: 10, agentsCount: 35, simulationTime: 100,
                    highRiskRate: 0.8, moderateRiskRate: 0.3, minRecoveryTime: 10,
                    recoveryRate: 0.8, minImunityTime: 20, imunityLossRate: 0.5,
                    deathRate: 0.001, childWeakerImunityFactor: 0.85, elderWeakerImunityFactor: 0.85,
                    weightTotalDead: 0.8, weightMaxInfected: 0.6, weightLockdown: 0.5, weightTotalInfected: 0.1, weightEpidemyDuration: 0.2
                )
            };
            defaultConfig.SimulationParameters.InitializeGrid();

            try
            {
                string jsonString = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonString);
                Console.WriteLine($"Výchozí konfigurační soubor vytvořen: '{filePath}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při ukládání výchozí konfigurace: {ex.Message}");
            }
            return defaultConfig;
        }

        private static AppConfig CreateEmergencyDefaultConfig()
        {
            var emergencyConfig = new AppConfig
            {
                EAParameters = new EAParameters(50, 20, 0.1, 0.1, 0.7, 5, 2, 3),
                SimulationParameters = new SimulationParameters(5, 5, 10, 50, 0.5, 0.1, 5, 0.5, 10, 0.2, 0.01, 0.9, 0.9, 0.8, 0.6, 0.5, 0.1, 0.2)
            };
            emergencyConfig.SimulationParameters.InitializeGrid();
            return emergencyConfig;
        }
    }
}
