using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class EA
    {
        private List<Individual> _population;
        private GridMap _grid;
        private Random _random;

        private class Individual : IComparable<Individual>
        {
            public MeasuresStrategy strategy;
            public double FitnessScore = double.MinValue;

            public int CompareTo(Individual other)
            {
                return other.FitnessScore.CompareTo(this.FitnessScore);
            }
        }
    }
}
