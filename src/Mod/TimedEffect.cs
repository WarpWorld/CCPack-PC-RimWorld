using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrowdControl {
    public abstract class TimedEffect : Effect {
        public bool IsActive = false;
        protected DateTime startTime;

        public abstract void Tick();
    }
}
