using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    public class ModSettings : SettingsBase
    {
        //public bool MantainOriginalSampleFreq { get; set; }        
		public PROTRACKER_COMPATIBILITY_MODE ForceProTrackerCompatibility { get; set; }
        public int PortamentoLossThreshold { get; set; } // Within this value, portamento is choosen to extra fine portamento whenever there is a loss of accuracy
        public bool NtscMode { get; set; }

		public void printSettings()
		{
			Console.WriteLine ("System Frequency : " + (NtscMode ? "NTSC (60 Hz)" : "PAL (50 Hz)"));

			switch (ForceProTrackerCompatibility) {
			case PROTRACKER_COMPATIBILITY_MODE.NONE:
				Console.WriteLine ("Force ProTracker Compatibility : Disabled");
				break;
			case PROTRACKER_COMPATIBILITY_MODE.B3MAX:
				Console.WriteLine ("Force ProTracker Compatibility : Highest note is B-3");
				break;
			case PROTRACKER_COMPATIBILITY_MODE.A3MAX:
				Console.WriteLine ("Force ProTracker Compatibility : Highest note is A-3 (to avoid Amiga DMA problems)");
				break;
			}

			Console.WriteLine ("Volume Scaling Column "+ VolumeScalingMode);
			Console.WriteLine ("PortamentoLossThreshold "+ PortamentoLossThreshold);
		}
    }
}
