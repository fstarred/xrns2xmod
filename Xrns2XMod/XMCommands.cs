using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    class XMCommands : ModCommands
    {
        public XMCommands(SongData songData, int paramTicksPerRow)
            : base(songData, paramTicksPerRow)
        {

        }

        protected byte GetPanningSlideValue(int originalValue, int ticksPerRow)
        {
            // in mod / xm volume slide is given by value * (current speed - 1), as well for pitch tone

            int divider = ticksPerRow > 1 ? ticksPerRow - 1 : ticksPerRow;

            byte value = (byte)(originalValue / (divider));

            return value;
        }

        public byte[] PlaySampleBackward()
        {
            return new byte[] { 0x21, 0x9F };
        }

        /*
         * Multi retrig Syntax: R + Interval + Volume change This is an extended version of the retrig command. 
         * Volume change: 0 = None 8 = Unused 1 = -1 9 = +1 2 = -2 A = +2 3 = -4 B = +4 4 = -8 C = +8 5 = -16 
         * D = +16 6 = *2/3 E = *3/2 7 = *1/2 F = *2 3.22 Tremor Syntax: T + On time + O
         * */
        public byte[] MultiRetrig(int effVal)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0x1B;
            ret[1] = (byte)effVal;
            return ret;
        }

        // FCxx - Set master volume (00 - FF, -INF to 3 dB).
        public byte[] SetGlobalVolume(int effVal)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0x10;

            if (effVal > 0xC0) effVal = 0xC0;

            // 3 = 0xC0 / 0x40
            effVal /= 3;

            ret[1] = (byte)effVal;
            return ret;
        }

        public byte[] GlobalVolumeSlideUp(int value, int ticksPerRow)
        {
            byte[] ret = new byte[2];
            value = GetVolumeSlideValue(value, ticksPerRow);
            if (value > 0xF) value = 0xF;
            ret[0] = (byte)0x11;            
            ret[1] = (byte)(value << 4);
            return ret;
        }

        public byte[] GlobalVolumeSlideDown(int value, int ticksPerRow)
        {
            byte[] ret = new byte[2];
            value = GetVolumeSlideValue(value, ticksPerRow);
            if (value > 0xF) value = 0xF;
            ret[0] = (byte)0x11;
            ret[1] = (byte)value;
            return ret;
        }

        //F0xx - Set Beats Per Minute (BPM) (20 - FF, 00 = stop song)
        public new byte[] SetTempo(int effVal)
        {
            byte[] ret = new byte[2];
            int effNum = 0x0F;

            // in xm, tempo starts from value 0x20;
            if (effVal < 0x20)
            {
                effNum = 0;
                effVal = 0;
            }

            ret[0] = (byte)effNum;
            ret[1] = (byte)effVal;
            return ret;
        }


        //05xx - Glide to specified note with step xx (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitonee).
        public byte[] TonePortamento(int effVal, int ticksPerRow, bool ignorePitchCompatibilityMode)
        {
            byte[] ret = new byte[2];

            byte value = (byte)GetPortamentoValue(effVal, ticksPerRow, ignorePitchCompatibilityMode);

            if (value == 0 && effVal != 0)
                throw new ConversionException("Converted tone portamento value was discarded because resulted to 0");

            ret[0] = (byte)0x03;
            ret[1] = value;
            return ret;
        }

        /// <summary>
        /// if pitch mode is based on renoise behaviour or ignorePitchCompatibilityMode is true,
        /// the input value is divided by ticks.
        /// 
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="ticksPerRow"></param>
        /// <param name="ignorePitchCompatibilityMode"></param>
        /// <returns></returns>
        protected byte GetPortamentoValue(int originalValue, int ticksPerRow, bool ignorePitchCompatibilityMode)
        {
            byte value;

            int divider = ticksPerRow > 1 ? ticksPerRow - 1 : ticksPerRow;

            if (ignorePitchCompatibilityMode == true || pitchCompatibilityMode == false)
            {
                // Every x unit is 1/16 half tone.
                // A 103 value in effect (xm pitch linear mode) is: 0x03 * (Tick - 1).
                // if tick value is 6, 03 * (6-1) = 15. 15/16 is almost an half tone
                value = (byte)(Math.Round((double)originalValue / divider));

                if (value == 0 && originalValue != 0)
                    throw new ConversionException("Converted portamento value was discarded because resulted to 0");
            }
            else
            {
                value = (byte)originalValue;
            }

            return value;
        }

        // Bx - Play sample in the current note column backwards (0 is backwards, 1 is forwards again).
        public byte[] TransposePlaySampleDirectionVolPanColumn(int effVal)
        {
            return new byte[] { 0x21, (byte)(0x9F - effVal) };
        }


        // on renoise panning is multiplied by 0x10 (<< 4)        
        // on xm, panning column value is multiplied by (tick - 1)
        // on xm, panning slide command value is still multiplied by (tick - 1) Pxy = pRL
        // this effect belongs only from panning column in renoise
        public byte[] TransposePanningSlideLeft(int effVal, int ticksPerRow)
        {
            byte[] ret = new byte[2];

            effVal = GetPanningSlideValue((effVal << 4), ticksPerRow);
            if (effVal > 0x0F) effVal = 0x0F;

            ret[0] = (byte)0x19;
            ret[1] = (byte)(effVal << 4);
            return ret;
        }


        // on renoise panning is multiplied by 0x10 (<< 4)
        // on xm, panning slide column value is multiplied by (tick - 1)
        // on xm, panning slide command value is still multiplied by (tick - 1) Pxy = pRL
        // this effect belongs only from panning column in renoise
        public byte[] TransposePanningSlideRight(int effVal, int ticksPerRow)
        {
            byte[] ret = new byte[2];

            effVal = GetPanningSlideValue(((effVal) << 4), ticksPerRow);
            if (effVal > 0x0F) effVal = 0x0F;

            ret[0] = (byte)0x19;
            ret[1] = (byte)effVal;
            return ret;
        }


        public byte VolumeUpVolumeColumn(int value, int ticksPerRow)
        {
            if (value < 0x05)
            {
                value = FineVolumeUpVolumeColumn(value);
            }
            else
            {
                value = VolumeSlideUpVolumeColumn(value, ticksPerRow);
            }

            return (byte)value;
        }



        public byte VolumeDownVolumeColumn(int value, int ticksPerRow)
        {
            if (value < 0x05)
            {
                value = FineVolumeDownVolumeColumn(value);
            }
            else
            {
                value = VolumeSlideDownVolumeColumn(value, ticksPerRow);
            }

            return (byte)value;
        }


        // volume slide up from volume column in renoise is max 0xF0
        // in xm slide is based on tick value
        public byte VolumeSlideUpVolumeColumn(int effVal, int ticksPerRow)
        {
            effVal = ((effVal & 0xF) << 4);
            effVal = base.GetVolumeSlideValue(effVal, ticksPerRow);
            if (effVal > 0x0F) effVal = 0x0F;
            effVal += 0x70;
            return (byte)effVal;
        }

        // volume slide down from volume column in renoise is max 0xF0
        // in xm slide is based on tick value
        public byte VolumeSlideDownVolumeColumn(int effVal, int ticksPerRow)
        {
            effVal = ((effVal & 0xF) << 4);
            effVal = base.GetVolumeSlideValue(effVal, ticksPerRow);
            if (effVal > 0x0F) effVal = 0x0F;
            effVal += 0x60;
            return (byte)effVal;
        }

        public byte FineVolumeUpVolumeColumn(int effVal)
        {
            effVal = ((effVal & 0xF) << 2);
            if (effVal > 0x0F) effVal = 0x0F;
            effVal += 0x90;
            return (byte)effVal;
        }

        //01xx - Pitch slide up (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitone).
        //02xx - Pitch slide down (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitone).
        //public new byte[] Portamento(int effNum, int effVal, int ticksPerRow)
        //{
        //    byte[] ret = new byte[2];

        //    bool useFinePortamentoCommand = false;

        //    if (pitchCompatibilityMode == false)
        //    {
        //        useFinePortamentoCommand = IsFinePortamentoCommandCloserToValue(effVal, ticksPerRow);
        //    }

        //    if (useFinePortamentoCommand)
        //        ret = FinePortamentoUpDown(effNum, effVal);
        //    else
        //        ret = PortamentoUpDown(effNum, effVal, ticksPerRow);

        //    return ret;
        //}

        //public new byte[] PortamentoUpDown(int effNum, int effVal, int ticksPerRow)
        //{
        //    byte[] ret = new byte[2];

        //    ret[0] = (byte)effNum;
        //    ret[1] = (byte)GetPitchNoteValue(effVal, ticksPerRow, false);

        //    return ret;
        //}

        //05xx - Glide to specified note with step xx (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitonee).
        //public new byte[] TonePortamento(int effVal, int ticksPerRow)
        //{
        //    byte[] ret = new byte[2];

        //    byte value = (byte)GetPitchNoteValue(effVal, ticksPerRow, false);

        //    if (value == 0 && effVal != 0)
        //        throw new ConversionException("Converted tone portamento value was discarded because resulted to 0");

        //    ret[0] = (byte)0x03;
        //    ret[1] = value;
        //    return ret;
        //}

        public byte[] PortamentoUpDown(int effNum, int effVal, int ticksPerRow, bool ignorePitchCompatibilityMode)
        {            
            byte[] ret = new byte[2];

            ret[0] = (byte)effNum;
            ret[1] = (byte)GetPortamentoValue(effVal, ticksPerRow, ignorePitchCompatibilityMode);

            return ret;
        }

        //01xx - Pitch slide up (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitone).
        //02xx - Pitch slide down (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitone).
        public byte[] Portamento(int effNum, int effVal, int ticksPerRow, bool ignorePitchCompatibilityMode)
        {
            byte[] ret = new byte[2];

            const int accuracyLossTreshold = 0;

            bool useFinePortamentoCommand = false;

            if (pitchCompatibilityMode == false)
            {
                useFinePortamentoCommand = IsFinePortamentoCommandCloserToValue(effVal, ticksPerRow, accuracyLossTreshold);
            }
            
            if (useFinePortamentoCommand)
                ret = FinePortamentoUpDown(effNum, effVal);
            else
                ret = PortamentoUpDown(effNum, effVal, ticksPerRow, ignorePitchCompatibilityMode);

            return ret;
        }

        public new byte[] TransposeGlideVolPanColumn(int effVal, int ticksPerRow)
        {
            const byte MAX_VALUE = 0xFF;
            byte[] ret = new byte[2];
            ret[0] = (byte)0x03;
            ret[1] = (byte)(effVal < 0x0F ? GetPortamentoValue((effVal << 4), ticksPerRow, true) : MAX_VALUE);
            return ret;
        }

        // Renoise: Slide pitch up by x semitones.
        // in xm slide is based on tick value
        public new byte[] TransposePortamentoUpVolPanColumn(int effVal, int ticksPerRow)
        {

            byte[] ret = new byte[2];

            ret[0] = (byte)1;
            effVal <<= 4;
            ret[1] = (byte)GetPortamentoValue(effVal, ticksPerRow, true);

            return ret;
        }

        // Renoise: Slide pitch down by x semitones.
        // in xm slide is based on tick value
        public byte[] TransposePortamentoDownVolPanColumn(int effVal, int ticksPerRow)
        {

            byte[] ret = new byte[2];

            ret[0] = (byte)2;
            effVal <<= 4;
            ret[1] = (byte)GetPortamentoValue(effVal, ticksPerRow, true);

            return ret;
        }

        public byte FineVolumeDownVolumeColumn(int effVal)
        {
            effVal = ((effVal & 0xF) << 2);
            if (effVal > 0x0F) effVal = 0x0F;
            effVal += 0x80;
            return (byte)effVal;
        }

        public byte SetVolumeVolumeColumn(int effVal)
        {
            effVal >>= 1;
            effVal += 0x10;
            return (byte)effVal;
        }

        public byte SetPanningVolumeColumn(int effVal)
        {
            effVal = effVal >> 3;
            if (effVal > 0xf) effVal = 0xf;
            effVal += 0xC0;
            return (byte)effVal;
        }

        // Panning Slide from Panning Column in renoise has a max value of 0xF0, 
        // but range from 0x90 to 0xF0 is pretty useless
        // in xm slide is based on tick value
        public byte PanSlideLeftVolumeColumn(int effVal, int ticksPerRow)
        {
            if (effVal > 0x08) effVal = 0x08;
            effVal <<= 4;
            effVal = GetPanningSlideValue(effVal, ticksPerRow);
            if (effVal > 0x0F) effVal = 0x0F;
            effVal += 0xD0;
            return (byte)effVal;
        }


        // Panning Slide from Panning Column in renoise has a max value of 0xF0, 
        // but range from 0x90 to 0xF0 is pretty useless
        // in xm slide is based on tick value
        public byte PanSlideRightVolumeColumn(int effVal, int ticksPerRow)
        {
            if (effVal > 0x08) effVal = 0x08;
            effVal <<= 4;
            effVal = GetPanningSlideValue(effVal, ticksPerRow);
            if (effVal > 0x0F) effVal = 0x0F;
            effVal += 0xE0;
            return (byte)effVal;
        }

    }
}
