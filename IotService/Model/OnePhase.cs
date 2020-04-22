using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IotService.Model
{
    public class OnePhase
    {
        public float RVoltage { get; set; }
        public float Current { get; set; }
        public float Active { get; set; }
        public float Reactive { get; set; }
    }
}
