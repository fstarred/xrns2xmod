using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    static class XMExtras
    {
        

        const int maxSampleVolume = 0x40;

        public static bool IsVolumeSetOnVolumeColumn(byte volume)
        {
            const int delta = 0x10;

            volume -= delta;

            return volume <= maxSampleVolume;
        }

        public static bool IsVolumeSetOnEffectColumn(byte command)
        {
            return command == 0x0C;
        }

        public static byte ScaleVolumeFromVolumeCommand(float volumeFactor)
        {
            const int delta = 0x10;

            return ScaleVolumeFromVolumeCommand(maxSampleVolume + delta, volumeFactor);
        }

        public static byte ScaleVolumeFromVolumeCommand(byte value, float volumeFactor)
        {
            const int delta = 0x10;

            int originalvalue = value - delta;

            int scaledvalue = (int)(originalvalue * volumeFactor);

            if (scaledvalue > maxSampleVolume)
                throw new ConversionException(String.Format( "Volume scaling failed, result value: {0}", scaledvalue));

            int returnvalue = scaledvalue + delta;

            return (byte)returnvalue;
        }

        public static byte[] ScaleVolumeFromEffectCommand(float volumeFactor)
        {
            byte value = ScaleVolumeFromEffectCommand(maxSampleVolume, volumeFactor);

            byte command = 0x0C;

            return new byte[] { command, value };
        }

        public static byte ScaleVolumeFromEffectCommand(byte value, float volumeFactor)
        {
            byte result = (byte)((float)value * volumeFactor);
            if (result > maxSampleVolume)
                throw new ConversionException(String.Format("Volume scaling failed, result value: {0}", result));

            return result;
        }

    }
}
