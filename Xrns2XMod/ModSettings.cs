using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    public class ModSettings : SettingsBase
    {
        //public bool MantainOriginalSampleFreq { get; set; }        
        public bool ForceProTrackerCompatibility { get; set; }
        public int PortamentoLossThreshold { get; set; } // Within this value, portamento is choosen to extra fine portamento whenever there is a loss of accuracy
        public bool NtscMode { get; set; }
    }
}
