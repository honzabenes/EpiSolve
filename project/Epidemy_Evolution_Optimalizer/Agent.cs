using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class Agent
    {
        public GridPosition Position { get; set; }
        public SIR Status { get; set; }
        public AgentAge Age { get; set; }

        public Agent(SIR status, AgentAge age, GridPosition position)
        {
            this.Status = status;
            this.Age = age;
            Position = position;
        }

        public override string ToString()
        {
            return $"Status: {Status}, Age: {Age}";
        }
    }
}
