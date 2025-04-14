using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class SimulationResult
    {
        public int TotalInfected { get; set; }
        public int MaxInfected { get; set; }
        public int LockdownDuration { get; set; }

        public SimulationResult(int totalInfected, int maxInfected, int lockdownDuration)
        {
            this.TotalInfected = totalInfected;
            this.MaxInfected = maxInfected;
            this.LockdownDuration = lockdownDuration;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Total infected: {this.TotalInfected.ToString()}");
            sb.AppendLine($"Max infected: {this.MaxInfected.ToString()}");
            sb.AppendLine($"Lockdown duration: {this.LockdownDuration.ToString()}");

            return sb.ToString();
        }
    }
}
