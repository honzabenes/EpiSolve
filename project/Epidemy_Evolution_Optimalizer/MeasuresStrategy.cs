using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    internal class MeasuresStrategy
    {
        double LockdownStartThreshold { get; set; }
        double LockdownEndThreshold { get; set; }
        double LockdownReductionFactor { get; set; }
        double LockdownMovementRestriction { get; set; }

        public MeasuresStrategy(Random random)
        {
            this.LockdownReductionFactor = random.NextDouble();
            this.LockdownStartThreshold = random.NextDouble();
            do
            {
                this.LockdownEndThreshold = random.NextDouble();
            } while (this.LockdownStartThreshold <= this.LockdownEndThreshold);
            this.LockdownMovementRestriction = random.NextDouble();
        }


        public MeasuresStrategy(double lockdownStartThreshold, double lockdownEndThreshold,
                                double lockdownReductionFactor, double lockdownMovementRestriction)
        {
            this.LockdownStartThreshold = lockdownStartThreshold;
            this.LockdownEndThreshold = lockdownEndThreshold;
            this.LockdownReductionFactor = lockdownReductionFactor;
            this.LockdownMovementRestriction = lockdownMovementRestriction;
        }


        public MeasuresStrategy Clone()
        {
            return new MeasuresStrategy(this.LockdownStartThreshold, this.LockdownEndThreshold,
                                        this.LockdownReductionFactor, this.LockdownMovementRestriction);
        }
    }
}
