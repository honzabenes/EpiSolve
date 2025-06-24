using System.Text;

namespace EpiSolve
{
    public class SimulationResult
    {
        public int TotalInfected { get; set; }
        public int MaxInfected { get; set; }
        public int TotalDead { get; set; }
        public int EpidemyDuration { get; set; }
        public int LockdownDuration { get; set; }

        public SimulationResult(int totalInfected, int maxInfected, int totalDead, int epidemyDuration, int lockdownDuration)
        {
            this.TotalInfected = totalInfected;
            this.MaxInfected = maxInfected;
            this.TotalDead = totalDead;
            this.EpidemyDuration = epidemyDuration;
            this.LockdownDuration = lockdownDuration;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Total infected: {this.TotalInfected.ToString()}");
            sb.AppendLine($"Max infected: {this.MaxInfected.ToString()}");
            sb.AppendLine($"Total dead: {this.TotalDead.ToString()}");
            sb.AppendLine($"Epidemy duration: {this.EpidemyDuration.ToString()}");
            sb.AppendLine($"Lockdown duration: {this.LockdownDuration.ToString()}");

            return sb.ToString();
        }
    }
}
