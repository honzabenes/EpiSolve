using EpiSolve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpiSolve
{
    class SimulationParameters
    {
        public GridMap Grid { get; set; }
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


        public SimulationParameters(GridMap grid, int agentsCount, int simulationTime,
                                   double highRiskRate, double moderateRiskRate,
                                   int minRecoveryTime, double recoveryRate,
                                   int minImunityTime, double imunityLossRate,
                                   double deathRate,
                                   double childWeakerImunityFactor, double elderWeakerImunityFactor)
        {
            this.Grid = grid;
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
            this.ElderWeakerImunityFactor = deathRate;
        }
    }
}
