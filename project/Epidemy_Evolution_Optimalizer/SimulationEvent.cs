using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemy_Evolution_Optimalizer
{
    public abstract class SimulationEvent
    {
        /// <summary>
        /// Čas, kdy se má událost vykonat.
        /// </summary>
        public double Timestamp { get; private set; }

        /// <summary>
        /// Konstruktor základní události.
        /// </summary>
        /// <param name="timestamp">Čas vykonání události.</param>
        protected SimulationEvent(double timestamp)
        {
            if (timestamp < 0) throw new ArgumentOutOfRangeException(nameof(timestamp), "Čas události nesmí být záporný.");
            Timestamp = timestamp;
        }

        /// <summary>
        /// Metoda, která vykoná logiku události. Musí být implementována v odvozených třídách.
        /// </summary>
        /// <param name="engine">Instance simulačního jádra pro přístup ke stavu a plánování.</param>
        public abstract void Execute(SimulationEngine engine);
    }
}
