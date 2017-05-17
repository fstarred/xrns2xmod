using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    class ModCommands
    {
        protected bool pitchCompatibilityMode;
        protected bool sampleOffsetCompatibilityMode;

        public ModCommands(SongData songData, int paramTicksPerRow)
        {
            sampleOffsetCompatibilityMode = songData.SampleOffsetCompatibilityMode;
            pitchCompatibilityMode = songData.PitchCompatibilityMode;
        }

        /// <summary>
        /// if pitch mode is based on renoise behaviour or ignorePitchCompatibilityMode is true,
        /// the input value is divided by ticks.
        /// 
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="ticksPerRow"></param>
        /// <returns></returns>
        protected byte GetPortamentoValue(int originalValue, int ticksPerRow)
        {
            byte value;

            int divider = ticksPerRow > 1 ? ticksPerRow - 1 : ticksPerRow;
            
            // Every x unit is 1/16 half tone.
            // A 103 value in effect (xm pitch linear mode) is: 0x03 * (Tick - 1).
            // if tick value is 6, 03 * (6-1) = 15. 15/16 is almost an half tone
            value = (byte)(Math.Round((double)originalValue / divider));

            if (value == 0 && originalValue != 0)
                value = 1;
                //throw new ConversionException("Converted portamento value was discarded because resulted to 0");
            

            return value;
        }

        private byte GetSampleOffsetValue(int originalOffsetValue, int sampleSize)
        {
            byte value = (byte)originalOffsetValue;

            if (sampleOffsetCompatibilityMode == false && sampleSize > 0)
            {
                // first takes the sample used in the instrument

                // 65536 is composed by
                // 256 is the fraction in which the sample is divided by in renoise
                // 256 because in mod/xm every unit value for this effect is 256 bytes.
                int modOffset = (sampleSize * originalOffsetValue) >> 16;
                value = (byte)(modOffset > Byte.MaxValue ? Byte.MaxValue : modOffset);

            }

            return value;
        }

        // in renoise volume has a range of 0 - 0xFF, 4 times more accurated than module
        protected byte GetVolumeSlideValue(int originalValue, int ticksPerRow)
        {
            // in mod / xm volume slide is given by value * (current speed - 1), as well for pitch tone
            //byte value = (byte)((originalValue / (ticksPerRow - 1)) >> 2);

            int divider = ticksPerRow > 1 ? ticksPerRow - 1 : ticksPerRow;

            byte value = (byte)(Math.Round((double)(originalValue >> 2) / divider));

            return value;
        }


        //Arpeggio (x = first note offset, y = second note offset).
        public byte[] Arpeggio(int effVal)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0x0;
            ret[1] = (byte)effVal;
            return ret;
        }

        // return precision loss (also negative value)
        public int GetPrecisionLostInPitchSlide(int effVal, int ticksPerRow)
        {
            int divider = ticksPerRow > 1 ? ticksPerRow - 1 : ticksPerRow;
            int precisionLoss = effVal % divider;
            if (precisionLoss > (divider >> 1))
                precisionLoss = precisionLoss - divider;
            else if (effVal == precisionLoss)
                precisionLoss = divider - effVal;

            return precisionLoss;
        }


        // return precision loss (also negative value)
        public int GetPrecisionLostInVolumeSlide(int effVal, int ticksPerRow)
        {
            int divider = ticksPerRow > 1 ? ticksPerRow - 1 : ticksPerRow;
            int precisionLoss = effVal % divider;
            if (precisionLoss > (divider >> 1))
                precisionLoss = precisionLoss - divider;

            return precisionLoss;
        }

        //01xx - Pitch slide up (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitone).
        //02xx - Pitch slide down (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitone).
        public byte[] Portamento(int effNum, int effVal, int ticksPerRow, int accuracyLossTreshold)
        {
            byte[] ret = new byte[2];

            bool useFinePortamentoCommand = false;

            if (pitchCompatibilityMode == false)
            {
                useFinePortamentoCommand = IsFinePortamentoCommandCloserToValue(effVal, ticksPerRow, accuracyLossTreshold);
            }

            //useFinePortamentoCommand = false;

            if (useFinePortamentoCommand)
                ret = FinePortamentoUpDown(effNum, effVal);
            else
                ret = PortamentoUpDown(effNum, effVal, ticksPerRow);

            return ret;
        }

        public byte[] PortamentoUpDown(int effNum, int effVal, int ticksPerRow)
        {
            byte[] ret = new byte[2];

            ret[0] = (byte)effNum;
            ret[1] = (byte)GetPortamentoValue(effVal, ticksPerRow);

            return ret;
        }

        public byte[] FinePortamentoUpDown(int effNum, int effVal)
        {
            byte[] ret = new byte[2];

            if (effVal > 0xF) effVal = 0xF;

            ret[0] = (byte)(0x0E);
            ret[1] = (byte)((0x10 * effNum) + effVal);

            return ret;
        }

        //03xx - Set channel volume (represents range from -60 to +3db).
        public byte[] SetVolume(int effVal)
        {
            byte[] ret = new byte[2];

            // add 1 to grant a better conversion ( 0xF is correctly converted to 0x40)
            effVal = (effVal + 1) >> 2;

            ret[0] = (byte)0xC;
            ret[1] = (byte)effVal;
            return ret;
        }

        //05xx - Glide to specified note with step xx (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitonee).
        public byte[] TonePortamento(int effVal, int ticksPerRow)
        {
            byte[] ret = new byte[2];

            byte value = (byte)GetPortamentoValue(effVal, ticksPerRow);

            if (value == 0 && effVal != 0)
                throw new ConversionException("Converted tone portamento value was discarded because resulted to 0");

            ret[0] = (byte)0x03;
            ret[1] = value;
            return ret;
        }

        //06xx - Volume slide up with step xx (0601 inserted 256 times will slide from 0 to full volume, 067F inserted twice will do the same).
        public byte[] VolumeUp(int effVal, int ticksPerRow)
        {
            byte[] ret = new byte[2];

            bool useFineVolumeCommand = IsFineVolumeCommandCloserToValue(effVal, ticksPerRow);

            if (useFineVolumeCommand)
                ret = VolumeFineUp(effVal);
            else
                ret = VolumeSlideUp(effVal, ticksPerRow);

            return ret;
        }

        //private delegate byte[] FineVolumeUpDown(int effVal, int lossPrecisionVolumeSlide);


        private bool IsFineVolumeCommandCloserToValue(int effVal, int ticksPerRow)
        {
            int lossPrecisionVolumeSlide = Math.Abs(GetPrecisionLostInVolumeSlide(effVal, ticksPerRow));

            const int maxFineVolume = 0xF << 2;
            const int margineFineVolume = maxFineVolume << 1;

            bool ret = false;

            if (lossPrecisionVolumeSlide > 0 && effVal < margineFineVolume)
            {
                int lossPrecisionVolumeFine = effVal - maxFineVolume;
                if (lossPrecisionVolumeFine < lossPrecisionVolumeSlide)
                {
                    ret = true;
                }
            }

            return ret;
        }

        protected bool IsFinePortamentoCommandCloserToValue(int effVal, int ticksPerRow, int accuracyLossTreshold)
        {
            bool ret = false;

            if (effVal > 0)
            {
                int precisionLossPortamentoSlide = Math.Abs(GetPrecisionLostInPitchSlide(effVal, ticksPerRow));

                int portamentoUnit = ticksPerRow > 1 ? ticksPerRow - 1 : 1;

                const int maxModTempo = 20;
                const int maxFinePortamentoValue = 0xF;
                const int marginFinePortamento = maxModTempo - 1;
                
                if (effVal < marginFinePortamento &&
                    precisionLossPortamentoSlide > accuracyLossTreshold)
                {
                    int precisionLossPortamentoFine = effVal - maxFinePortamentoValue;

                    if (precisionLossPortamentoFine < precisionLossPortamentoSlide)
                    {
                        ret = true;
                    }
                }
            }
            
            return ret;
        }


        public byte[] VolumeSlideUp(int effVal, int ticksPerRow)
        {
            byte[] ret = new byte[2];
            effVal = GetVolumeSlideValue(effVal, ticksPerRow);
            if (effVal > 0xF) effVal = 0xF;
            effVal <<= 4;
            ret[0] = (byte)0xA;
            ret[1] = (byte)effVal;
            return ret;
        }

        public byte[] VolumeFineUp(int effVal)
        {
            byte[] ret = new byte[2];
            effVal >>= 2;
            if (effVal > 0xF) effVal = 0xF;
            effVal += 0xA0;
            ret[0] = (byte)0xE;
            ret[1] = (byte)effVal;
            return ret;
        }

        //07xx - Volume slide down with step xx.
        public byte[] VolumeDown(int effVal, int ticksPerRow)
        {
            byte[] ret = new byte[2];

            bool useFineVolumeCommand = IsFineVolumeCommandCloserToValue(effVal, ticksPerRow);

            if (useFineVolumeCommand)
                ret = VolumeFineDown(effVal);
            else
                ret = VolumeSlideDown(effVal, ticksPerRow);

            return ret;
        }

        public byte[] VolumeSlideDown(int effVal, int ticksPerRow)
        {
            byte[] ret = new byte[2];
            effVal = GetVolumeSlideValue(effVal, ticksPerRow);
            if (effVal > 0xF) effVal = 0xF;
            ret[0] = (byte)0xA;
            ret[1] = (byte)effVal;
            return ret;
        }

        public byte[] VolumeFineDown(int effVal)
        {
            byte[] ret = new byte[2];
            effVal >>= 2;
            if (effVal > 0xF) effVal = 0xF;
            effVal += 0xB0;
            ret[0] = (byte)0xE;
            ret[1] = (byte)effVal;
            return ret;
        }

        //08xx - Change the track pre-mixer's panning (00 = full left, 80 = center, FF = full right).
        public byte[] SetPanning(int effVal)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0x08;
            ret[1] = (byte)effVal;
            return ret;
        }

        //09xx - Trigger sample at slice number xx, (00 is the sample's start, FF the last slice).
        public byte[] SetSampleOffset(int effVal, int sampleSize)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0x9;
            ret[1] = (byte)GetSampleOffsetValue(effVal, sampleSize);
            return ret;
        }

        //0Dxx - Delay all notes by xx ticks.
        public byte[] NoteDelay(int effVal)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0xE;
            ret[1] = (byte)(0xD0 + (effVal > 0xF ? 0xF : effVal));
            return ret;
        }

        //0Exy - Retrigger a note every y ticks with volume x.
        public byte[] RetrigNote(int effVal)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0xE;
            ret[1] = (byte)(0x90 + (effVal & 0x0F));
            return ret;
        }

        //0Fxy Vibrato (x = speed, y = depth).
        public byte[] Vibrato(int effVal)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0x04;
            ret[1] = (byte)effVal;
            return ret;
        }

        // Set tremolo (regular volume variation), x = speed, y = depth.
        public byte[] Tremolo(int value)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0x07;
            ret[1] = (byte)value;
            return ret;
        }

        //F2xx - Set Ticks Per Line (TPL) (01 - 10).
        public byte[] SetSpeed(int effVal)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0x0F;
            ret[1] = (byte)effVal;
            return ret;
        }

        //F0xx - Set Beats Per Minute (BPM) (20 - FF, 00 = stop song)
        public byte[] SetTempo(int effVal)
        {
            byte[] ret = new byte[2];
            int effNum = 0x0F;

            // in mod, tempo starts from value 0x21;
            if (effVal <= 0x20)
            {
                effNum = 0;
                effVal = 0;
            }

            ret[0] = (byte)effNum;
            ret[1] = (byte)effVal;
            return ret;
        }

        //ZDxx - Delay(pause) pattern playback by xx lines
        public byte[] PatternDelay(int val)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0xE;
            if (val > 0x0F)
                val = 0x0F;
            ret[1] = (byte)(0xE0 + val);
            return ret;
        }

        //FBxx - Pattern break. The current pattern finishes immediately and jumps to next pattern at line xx (hex).
        public byte[] PatternBreak(int effVal)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0xD;
            ret[1] = (byte)(int.Parse(effVal.ToString(), System.Globalization.NumberStyles.HexNumber));
            return ret;
        }

        // this effect belongs only from volume / panning column
        public byte[] NoteCut(int effVal)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0xE;
            ret[1] = (byte)(0xC0 + effVal);
            return ret;
        }

        // Gx - Glide towards given note by x semitones. A value of F will make the slide instant.
        [Obsolete("Do not use this")]
        public byte[] TransposeGlideVolPanColumn(int effVal, int ticksPerRow)
        {
            const byte MAX_VALUE = 0xFF;
            byte[] ret = new byte[2];
            ret[0] = (byte)0x03;
            ret[1] = (byte)(effVal < 0x0F ? GetPortamentoValue((effVal << 4), ticksPerRow) : MAX_VALUE);
            return ret;
        }

        // Renoise: Slide pitch up by x semitones.
        // in xm slide is based on tick value
        [Obsolete("Do not use this")]
        public byte[] TransposePortamentoUpVolPanColumn(int effVal, int ticksPerRow)
        {

            byte[] ret = new byte[2];

            ret[0] = (byte)1;
            effVal <<= 4;
            ret[1] = (byte)GetPortamentoValue(effVal, ticksPerRow);

            return ret;
        }

        public byte[] TransposeSetVolumeFromVolume(int effVal)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0x0C;
            ret[1] = (byte)(effVal >> 1);
            return ret;
        }

        public byte[] TransposeVolumeSlideUpFromVolume(int effVal, int ticksPerRow)
        {
            byte[] ret = new byte[2];
            effVal = effVal * 0x10;
            ret = VolumeUp(effVal, ticksPerRow);
            return ret;
        }

        public byte[] TransposeVolumeSlideDownFromVolume(int effVal, int ticksPerRow)
        {
            byte[] ret = new byte[2];
            effVal = effVal * 0x10;
            ret = VolumeSlideDown(effVal, ticksPerRow);
            return ret;
        }

        public byte[] TransposeNoteDelayFromVolume(int effVal)
        {
            byte[] ret = new byte[2];
            effVal = (effVal & 0x0F);
            ret = NoteDelay(effVal);
            return ret;
        }

        public byte[] TransposeNoteDelayFromVolumeNew(int effVal)
        {
            byte[] ret = new byte[2];
            ret = NoteDelay(effVal);
            return ret;
        }


        public byte[] TransposeRetrigNoteFromVolume(int effVal)
        {
            byte[] ret = new byte[2];
            ret = RetrigNote(effVal);
            return ret;
        }

        public byte[] TransposeSetPanningFromPanning(int effVal)
        {
            byte[] ret = new byte[2];
            effVal = effVal << 1;
            if (effVal > 0xFF) effVal = 0xFF;
            ret = SetPanning(effVal);
            return ret;
        }

    }

}
