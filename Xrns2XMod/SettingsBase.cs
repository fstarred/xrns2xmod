using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    [Serializable()]
    public enum VOLUME_SCALING_MODE { NONE, SAMPLE, COLUMN };

    public class SettingsBase
    {
        public VOLUME_SCALING_MODE VolumeScalingMode { get; set; }
    }
}
