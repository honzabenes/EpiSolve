using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    class Agent
    {
        public SIR Status { get; set; }
        public ushort Age { get; set; }

        public Agent(SIR status, ushort age)
        {
            this.Status = status;
            this.Age = age;
        }

        public override string ToString()
        {
            return $"Status: {Status}, Age: {Age}";
        }
    }
}
