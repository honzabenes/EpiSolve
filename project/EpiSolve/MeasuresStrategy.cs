using System.Text;

namespace EpiSolve
{
    class MeasuresStrategy
    {
        public double LockdownStartThreshold { get; set; }
        public double LockdownEndThreshold { get; set; }
        public double LockdownInfectionReductionFactor { get; set; }
        public double LockdownMovementRestriction { get; set; }

        public MeasuresStrategy(Random random)
        {
            this.LockdownInfectionReductionFactor = random.NextDouble();
            this.LockdownStartThreshold = random.NextDouble();
            do
            {
                this.LockdownEndThreshold = random.NextDouble();
            } while (this.LockdownStartThreshold <= this.LockdownEndThreshold);
            this.LockdownMovementRestriction = random.NextDouble();
        }


        public MeasuresStrategy(double lockdownStartThreshold, double lockdownEndThreshold,
                                double lockdownInfectionReductionFactor, double lockdownMovementRestriction)
        {
            this.LockdownStartThreshold = lockdownStartThreshold;
            this.LockdownEndThreshold = lockdownEndThreshold;
            this.LockdownInfectionReductionFactor = lockdownInfectionReductionFactor;
            this.LockdownMovementRestriction = lockdownMovementRestriction;
        }


        public MeasuresStrategy Clone()
        {
            return new MeasuresStrategy(this.LockdownStartThreshold, this.LockdownEndThreshold,
                                        this.LockdownInfectionReductionFactor, this.LockdownMovementRestriction);
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Lockdown start threshold: {LockdownStartThreshold}");
            sb.AppendLine($"Lockdown end threshold: {LockdownEndThreshold}");
            sb.AppendLine($"Lockdown infection reduction factor: {LockdownInfectionReductionFactor}");
            sb.AppendLine($"Lockdown movement restriction: {LockdownMovementRestriction}");

            return sb.ToString();
        }
    }
}
