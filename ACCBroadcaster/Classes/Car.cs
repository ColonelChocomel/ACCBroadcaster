using ksBroadcastingNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCBroadcaster.Classes
{
    internal class Car
    {
        public int Position { get; set; }
        public int RaceNumber { get; set; }
        public string DriverName { get; set; }
        public CarLocationEnum Location { get; set; }
        public string LapDelta { get; set; }
        public string CurrentLap { get; set; }
        public string LastLap { get; set; }
        public string BestLap { get; set; }
    }
}
