using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    [Serializable()]
    public enum VOLUME_SCALING_MODE { NONE, SAMPLE, COLUMN };

	[Serializable()]
	public enum PROTRACKER_COMPATIBILITY_MODE
	{
		NONE,
		B3MAX, //B-3 is maximum allowed note value
		A3MAX, //A-3 is maximum allowed note value as a real Amiga can't get that high
	};

    public class SettingsBase
    {
        public VOLUME_SCALING_MODE VolumeScalingMode { get; set; }
    }
}
