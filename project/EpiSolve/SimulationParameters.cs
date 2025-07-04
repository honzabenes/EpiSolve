﻿using System;
using System.IO;
using System.Text.Json;

namespace EpiSolve
{
    public class SimulationParameters
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public GridMap Grid { get; set; }

        public int GridHeight { get; set; }
        public int GridWidth { get; set; }
        public int AgentsCount { get; set; }
        public int SimulationTime { get; set; }
        public double ModerateRiskRate { get; set; }
        public double HighRiskRate { get; set; }
        public int MinRecoveryTime { get; set; }
        public double RecoveryRate { get; set; }
        public int MinImunityTime { get; set; }
        public double ImunityLossRate { get; set; }
        public double DeathRate { get; set; }
        public double ChildWeakerImunityFactor { get; set; }
        public double ElderWeakerImunityFactor { get; set; }
        public double WeightTotalDead { get; set; }
        public double WeightMaxInfected { get; set; }
        public double WeightLockdown { get; set; }
        public double WeightTotalInfected { get; set; }
        public double WeightEpidemyDuration { get; set; }


        public SimulationParameters() { }

        public SimulationParameters(int gridHeight, int gridWidth, int agentsCount, int simulationTime,
                                   double highRiskRate, double moderateRiskRate,
                                   int minRecoveryTime, double recoveryRate,
                                   int minImunityTime, double imunityLossRate,
                                   double deathRate,
                                   double childWeakerImunityFactor, double elderWeakerImunityFactor,
                                   double weightTotalDead, double weightMaxInfected, double weightLockdown,
                                   double weightTotalInfected, double weightEpidemyDuration)
        {
            this.Grid = new GridMap(gridHeight, gridWidth);
            this.GridHeight = gridHeight;
            this.GridWidth = gridWidth;
            this.AgentsCount = agentsCount;
            this.SimulationTime = simulationTime;
            this.ModerateRiskRate = moderateRiskRate;
            this.HighRiskRate = highRiskRate;
            this.MinRecoveryTime = minRecoveryTime;
            this.RecoveryRate = recoveryRate;
            this.MinImunityTime = minImunityTime;
            this.ImunityLossRate = imunityLossRate;
            this.DeathRate = deathRate;
            this.ChildWeakerImunityFactor = childWeakerImunityFactor;
            this.ElderWeakerImunityFactor = elderWeakerImunityFactor;
            this.WeightTotalDead = weightTotalDead;
            this.WeightMaxInfected = weightMaxInfected;
            this.WeightLockdown = weightLockdown;
            this.WeightTotalInfected = weightTotalInfected;
            this.WeightEpidemyDuration = weightEpidemyDuration;
        }

        public void InitializeGrid()
        {
            this.Grid = new GridMap(this.GridHeight, this.GridWidth);
        }
    }
}
