using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Xrns2XMod
{
    class XMUtils : ModUtils
    {
        public XMUtils(SongData songData, int paramTicksPerRow)
            : base(songData, paramTicksPerRow)
        {
            instrumentsList = new InstrumentDataXM[songData.Instruments.Length];

            for (int i = 0; i < instrumentsList.Length; i++)
            {
                instrumentsList[i] = new InstrumentDataXM();
                instrumentsList[i].Samples = new InstrumentDataBase.SampleDataBase[songData.Instruments[i].Samples.Length];
                instrumentsList[i].KeyMaps = songData.Instruments[i].KeyMap;
                for (int a = 0; a < instrumentsList[i].Samples.Length; a++)
                {
                    instrumentsList[i].Samples[a] = new InstrumentDataBase.SampleDataBase();
                }
            }

            xmCommands = new XMCommands(songData, paramTicksPerRow);
        }

        private XMCommands xmCommands;

        protected new InstrumentDataXM[] instrumentsList;

        /*
        -128..+127 (-128 = -1 halftone, +127 = +127/128 halftones)
         * */
        public int GetSampleFineTune(int instrumentIndex, int sampleIndex)
        {
            return instrumentsList[instrumentIndex].Samples[sampleIndex].FineTune;
        }

        public int GetSampleBaseNote(int instrumentIndex, int sampleIndex)
        {
            return instrumentsList[instrumentIndex].Samples[sampleIndex].RelatedNote;
        }

        public int GetPlayedSampleFromKeymap(int note, int instrument)
        {
            return instrumentsList[instrument - 1].KeyMaps[note - 1];
        }

        private int GetSampleLength(int xmNote, int xmInstrument)
        {
            int sampleSize = 0;

            if (xmNote > 0 && xmInstrument > 0)
            {
                // get the sample, according to keymap value
                int sampleUsedInKeyMap = GetPlayedSampleFromKeymap(xmNote, xmInstrument);
                if (sampleUsedInKeyMap >= 0)
                {
                    // first takes the sample used in the instrument
                    sampleSize = instrumentsList[xmInstrument - 1].Samples[sampleUsedInKeyMap].Length;

                    //int xmOffset = (sampleSize * originalValue) >> 16;
                    //value = (byte)(xmOffset > Byte.MaxValue ? Byte.MaxValue : xmOffset);
                }
            }

            return sampleSize;
        }

        public new void StoreSampleInfo(int instrumentIndex, int sampleIndex, int length,
            int freq, int chans, int bps, int renoiseBaseNote, int renoiseFineTuning, int transpose)
        {
            int realLength = CalculateSampleLength(length, bps, chans);

            int baseNote;
            int fineTune;

            GetSampleProperties(renoiseBaseNote, transpose, renoiseFineTuning, freq, out baseNote, out fineTune);

            instrumentsList[instrumentIndex].Samples[sampleIndex].RelatedNote = baseNote;
            instrumentsList[instrumentIndex].Samples[sampleIndex].FineTune = fineTune;
            instrumentsList[instrumentIndex].Samples[sampleIndex].Length = realLength;

        }

        public short GetInitialTempo(int renoiseBPM, int linesPerBeat)
        {
            short tempo;

            // more accurate in one passage, no loss of decimal points in type conversion
            tempo = (short)((renoiseBPM * linesPerBeat / 4) * ticksPerRow / 6);

            return tempo;
        }

        public static byte GetSampleLoopMode(string renoiseLoopMode)
        {
            byte loopMode = 0;

            if (renoiseLoopMode.Equals("Forward", StringComparison.OrdinalIgnoreCase))
            {
                loopMode = 1;
            }
            else if (renoiseLoopMode.Equals("PingPong", StringComparison.OrdinalIgnoreCase))
            {
                loopMode = 2;
            }

            return loopMode;
        }

        public static uint GetSampleLoopValue(uint value, int bps, bool isStereo)
        {
            value *= (uint)(bps / 8);

            value *= (uint)(isStereo ? 2 : 1);

            return value;
        }

        public static byte GetPanning(float panning)
        {
            return (byte)Math.Abs(Byte.MaxValue * panning + 1);
        }        

        public static byte GetVolumePanningType(bool volumeEnabled, bool sustainEnabled, bool loopEnabled)
        {
            int value = 0;

            byte volumeOnBit = 1;
            byte sustainOnBit = 2;
            byte loopOnBit = 4;

            value += volumeEnabled ? volumeOnBit : 0;
            value += sustainEnabled ? sustainOnBit : 0;
            value += loopEnabled ? loopOnBit : 0;

            return (byte)value;
        }

        public static byte GetXMNote(string note)
        {
            byte value = 0;

            if (note.Equals("OFF"))
            {
                value = 97;
            }
            else
            {
                string tune = note.Substring(0, 2);

                // really useful for finding an array value !!
                int index = Array.FindIndex(notesArray, delegate(string item)
                {
                    return item.Equals(tune);
                });
                if (index >= 0)
                {
                    int octave = Int16.Parse(note.Substring(2, 1));
                    //if (octave == 0) octave = 1;
                    //if (octave == 8) octave = 8;

                    if (octave < 8)
                        value = (byte)((12 * (octave /*- 1*/)) + (index + 1));
                    else
                        throw new ConversionException(String.Format("note {0} is out of range", note));
                }
            }

            return value;
        }

        private byte GetVolumeColumnEffectFromVolume(int hexval, float sampleVolumeFactor)
        {
            // so yea full volume. apply attenuation and return either 0 for full volume (00 in effect column) or volume column effect [00-7F]
            int hexVal = (int)(0x80 * sampleVolumeFactor);
            if (hexVal == 0x80)
                return 0;
            if (hexVal > 0x80)
                throw new NotImplementedException("Cannot scale positive instrument volumes");
            if (hexVal < 0)
                throw new IndexOutOfRangeException("Attenuated instrument volume ended up less than zero, wtf.");
            return xmCommands.SetVolumeVolumeColumn(hexVal);
        }
        

        public byte GetVolumeColumnEffectFromVolume(string xrnsColVolEff)
        {
            char[] commandEffectSplitted = xrnsColVolEff.ToCharArray();
            char command = commandEffectSplitted[0];
            char hexvalue = commandEffectSplitted[1];

            int value = Int16.Parse(hexvalue.ToString(), System.Globalization.NumberStyles.HexNumber);

            switch (command)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8': // 00 - 7F - Set note volume (velocity) of current note column.
                    int hexVal = Int16.Parse(xrnsColVolEff, System.Globalization.NumberStyles.HexNumber);
                    value = xmCommands.SetVolumeVolumeColumn(hexVal);
                    break;
                case 'I': //  Volume fade in in the current note column, with step x * 10 (91 = 0I10 in effect column, 92 = 0I20 in effect column etc.)
                    value = xmCommands.VolumeUpVolumeColumn(value, ticksPerRow);
                    break;
                case 'O': //  Volume fade out in the current note column, with step x * 10 (A1 = 0O10, A2 = 0O20 etc.)
                    value = xmCommands.VolumeDownVolumeColumn(value, ticksPerRow);
                    break;
                case 'B': // Play sample in the current note column backwards (0 is backwards, 1 is forwards again).                    
                case 'Q': // Delay a note by x ticks (0 - F).                    
                case 'R': // Retrigger a note every x ticks (0 - F).                    
                case 'C': // Cut the note after x ticks (0 - F).                    
                default:
                    value = 0;
                    break;

            }

            return (byte)value;

        }


        public byte GetVolumeColumnEffectFromPanning(string xrnsColPanEff)
        {

            char[] commandEffectSplitted = xrnsColPanEff.ToCharArray();
            char command = commandEffectSplitted[0];
            char hexvalue = commandEffectSplitted[1];

            int value = Int16.Parse(hexvalue.ToString(), System.Globalization.NumberStyles.HexNumber);

            switch (command)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8': // Set panning of current note column: (00 = full left, 40 = center, 80 = full right).
                    int hexVal = Int16.Parse(xrnsColPanEff, System.Globalization.NumberStyles.HexNumber);
                    value = xmCommands.SetPanningVolumeColumn(hexVal);
                    break;
                case 'J': //  Panning slide left with step x (0 - F).
                    value = xmCommands.PanSlideLeftVolumeColumn(value, ticksPerRow);
                    break;
                case 'K': //   Panning slide right with step x (0 - F).
                    value = xmCommands.PanSlideRightVolumeColumn(value, ticksPerRow);
                    break;
                case 'B': // Play sample in the current note column backwards (0 is backwards, 1 is forwards again).                    
                case 'Q': // Delay a note by x ticks (0 - F).                    
                case 'R': // Retrigger a note every x ticks (0 - F).                    
                case 'C': // Cut the note after x ticks (0 - F).                    
                default:
                    value = 0;
                    break;

            }

            return (byte)value;

        }

        public static byte GetPointNumber(byte[] envPoints, int value)
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(envPoints)))
            {
                for (int i = 0; i < envPoints.Length; i += 2)
                {
                    short x = reader.ReadInt16();
                    if (x == value)
                    {
                        return (byte)(i / 4);
                    }
                }
            }

            return 0;
        }

        public static byte[] GetEnvelopePointsValue(string[] envPoints, int sustainPoint, int loopStart, int loopEnd,
                bool isSustainEnabled, bool isLoopEnabled)
        {
            /*
             * Reproduces envelope points from renoise.
             * Because of sustain / loop points different logic, 
             * might be necessary adding new point with computed y 
             * in order to make these effects work properly
             * 
             * */

            if (envPoints == null) return new byte[0];

            List<EnvelopePoint> listPoints = new List<EnvelopePoint>();

            foreach (string input in envPoints)
            {
                string[] xyValues = input.Split(',');

                int x = Int16.Parse(xyValues[0]);

                double temp = Double.Parse(xyValues[1], Culture);

                int y = ((int)Math.Abs(SByte.MaxValue * temp) / 2);

                EnvelopePoint point = new EnvelopePoint(x, y);

                listPoints.Add(point);
            }

            // sort is maybe not necessary because points are already sorted
            listPoints.Sort();

            if (isSustainEnabled)
            {
                /* Check if this point already exists in Renoise, in ordet to know if it's 
                * necessary or not to create a new one
                */
                if (Array.BinarySearch(listPoints.ToArray(), sustainPoint) < 0)
                {
                    AddNewPointInList(listPoints, sustainPoint);
                }
            }
            if (isLoopEnabled)
            {
                /* Check if this point already exists in Renoise, in ordet to know if it's 
                * necessary or not to create a new one
                */
                if (Array.BinarySearch(listPoints.ToArray(), loopStart) < 0)
                {
                    AddNewPointInList(listPoints, loopStart);
                }
                /* Check if this point already exists in Renoise, in ordet to know if it's 
                * necessary or not to create a new one
                */
                if (Array.BinarySearch(listPoints.ToArray(), loopEnd) < 0)
                {
                    AddNewPointInList(listPoints, loopEnd);
                }
            }


            byte[] points = new byte[listPoints.Count * 4];

            int offset = 0;

            for (int i = 0; i < listPoints.Count; i++)
            {
                Utility.PutInt2InByteArray(listPoints[i].X, points, offset);

                offset += 2;

                Utility.PutInt2InByteArray(listPoints[i].Y, points, offset);

                offset += 2;
            }

            return points;
        }

        public byte GetSampleOffsetValue(int originalValue, byte xmNote, byte xmInstrument)
        {
            byte value = (byte)originalValue;

            if (sampleOffsetCompatibilityMode == false && xmNote > 0 && xmInstrument > 0)
            {
                // get the sample, according to keymap value
                int sampleUsedInKeyMap = instrumentsList[xmInstrument - 1].KeyMaps[xmNote - 1];
                int sampleSize = 0;
                if (sampleUsedInKeyMap >= 0)
                {
                    // first takes the sample used in the instrument
                    sampleSize = instrumentsList[xmInstrument - 1].Samples[sampleUsedInKeyMap].Length;

                    // 65536 is composed by
                    // 256 is the fraction in which the sample is divided by in renoise
                    // 256 because in mod/xm every unit value for this effect is 256 bytes.
                    int xmOffset = (sampleSize * originalValue) >> 16;
                    value = (byte)(xmOffset > Byte.MaxValue ? Byte.MaxValue : xmOffset);
                }
            }

            return value;
        }

        // xmNote and xmEffect are required for a correct parsing of 09 sample offset value 
        public byte[] GetXMEffect(string xrnsEffNum, string xrnsEffVal, byte xmNote, byte xmInstrument)
        {
            char[] commandEffectSplitted = xrnsEffNum.ToCharArray();
            char effType = commandEffectSplitted[0];
            char command = commandEffectSplitted[1];
            int value = Int16.Parse(xrnsEffVal, System.Globalization.NumberStyles.HexNumber);

            byte[] result = new byte[2];
            result[0] = (byte)0;
            result[1] = (byte)0;

            switch (effType)
            {
                case '0': // Sample Commands
                    result = GetSampleCommands(command, value, xmNote, xmInstrument);
                    break;
                case 'Z': // Global Commands
                    result = GetGlobalCommands(command, value);
                    break;
            }

            return result;
        }

        public byte[] GetCommandsFromMasterTrack(string xrnsEffNum, string xrnsEffVal, bool parseOnlyGlobalVolume)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0;
            ret[1] = (byte)0;

            char[] commandEffectSplitted = xrnsEffNum.ToCharArray();
            char commandType = commandEffectSplitted[0];
            char command = commandEffectSplitted[1];
            int value = Int16.Parse(xrnsEffVal, System.Globalization.NumberStyles.HexNumber);

            switch (commandType)
            {
                case '0': // Sample Commands
                    switch (command)
                    {
                        case 'M':
                            ret = xmCommands.SetGlobalVolume(value);
                            break;
                        case 'I':
                            ret = xmCommands.GlobalVolumeSlideUp(value, ticksPerRow);
                            break;
                        case 'O':
                            ret = xmCommands.GlobalVolumeSlideDown(value, ticksPerRow);
                            break;
                        default:
                            break;
                    }
                    break;
                case 'Z': // Global Commands
                    if (parseOnlyGlobalVolume == false)
                        ret = GetGlobalCommands(command, value);
                    break;
            }

            return ret;
        }


        private byte[] GetSampleCommands(char command, int value, int xmNote, int xmInstrument)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0;
            ret[1] = (byte)0;

            switch (command)
            {
                case 'A': //Arpeggio (x = first note offset, y = second note offset).
                    ret = xmCommands.Arpeggio(value);
                    break;
                case 'U': //01xx - Pitch slide up (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitone).                     
                    ret = xmCommands.Portamento(1, value, ticksPerRow, false);
                    break;
                case 'D': //02xx - Pitch slide down (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitone).
                    ret = xmCommands.Portamento(2, value, ticksPerRow, false);
                    break;
                case 'M': //03xx - Set channel volume (represents range from -60 to +3db).
                    ret = xmCommands.SetVolume(value);
                    break;
                case 'C': //04xy - Volume slicer. x = volume factor (0 = 0%, F = 100%), applied at tick number y.                    
                    break;
                case 'G': //05xx - Glide to specified note with step xx (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitonee).
                    ret = xmCommands.TonePortamento(value, ticksPerRow, false);
                    break;
                case 'I': //06xx - Volume slide up with step xx (0601 inserted 256 times will slide from 0 to full volume, 067F inserted twice will do the same).
                    ret = xmCommands.VolumeUp(value, ticksPerRow);
                    break;
                case 'O': //07xx - Volume slide down with step xx.
                    ret = xmCommands.VolumeDown(value, ticksPerRow);
                    break;
                case 'P': //08xx - Change the track pre-mixer's panning (00 = full left, 80 = center, FF = full right).
                    ret = xmCommands.SetPanning(value);
                    break;
                case 'S': //09xx - Trigger sample at slice number xx, (00 is the sample's start, FF the last slice).
                    ret = xmCommands.SetSampleOffset(value, GetSampleLength(xmNote, xmInstrument));
                    break;
                case 'W': //0Axx - Track surround width.
                    break;
                case 'B': //0Bxx - Play sample backwards (B00) or forwards again (B01 during play when sample is playing backwards). Can also be combined with the 09xx effect.
                    ret = xmCommands.PlaySampleBackward();
                    break;
                case 'L': //0Cxx - Change the track pre-mixer's volume (represents range from -INF to +3db).
                    break;
                case 'Q': //0Dxx - Delay all notes by xx ticks.                    
                    ret = xmCommands.NoteDelay(value);
                    break;
                case 'R': //0Exy - Retrigger a note every y ticks with volume x.                    
                    ret = xmCommands.MultiRetrig(value);
                    break;
                case 'V': //0Fxy Vibrato (x = speed, y = depth).
                    ret = xmCommands.Vibrato(value);
                    break;
                case 'T':
                    ret = xmCommands.Tremolo(value);
                    break;
                case 'N':
                    break;
                case 'E':
                    break;
                case 'J':
                    break;
                case 'X':
                    break;
                default:
                    break;
            }

            return ret;
        }


        private new byte[] GetGlobalCommands(char command, int value)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0;
            ret[1] = (byte)0;

            switch (command)
            {
                case 'T': //F0xx - Set Beats Per Minute (BPM) (20 - FF, 00 = stop song)
                    ret = xmCommands.SetTempo(value);
                    break;
                case 'L': //F1xx - Set Lines Per Beat (LPB) (01 - FF, 00 = stop song).                    
                    if (playbackEngineVersion == Constants.MOD_VERSION_COMPATIBLE)
                    {
                        ret = xmCommands.SetSpeed(value);
                    }
                    break;
                case 'K': //F2xx - Set Ticks Per Line (TPL) (01 - 10).
                    ret = xmCommands.SetSpeed(value);
                    break;
                case 'G': //F4xx - Toggle song groove on/off (see Song Settings) (00 = turn off, 01 or higher = turn on).
                    break;
                case 'B': //FBxx - Pattern break. The current pattern finishes immediately and jumps to next pattern at line xx (hex).
                    ret = xmCommands.PatternBreak(value);
                    break;
                case 'D': //FDxx - Pause pattern playback by xx lines.
                    ret = xmCommands.PatternDelay(value);
                    break;
                default:
                    break;
            }

            return ret;
        }

        public byte[] TransposeVolPanEffectColumnToEffectColumn(char command, int value, int ticksPerRow)
        {
            byte[] ret = new byte[2];

            switch (command)
            {
                case 'U':
                    ret = xmCommands.TransposePortamentoUpVolPanColumn(value, ticksPerRow);
                    break;
                case 'D':
                    ret = xmCommands.TransposePortamentoDownVolPanColumn(value, ticksPerRow);
                    break;
                case 'G':
                    ret = xmCommands.TransposeGlideVolPanColumn(value, ticksPerRow);
                    break;
                case 'B': // Play sample in the current note column backwards (0 is backwards, 1 is forwards again).                    
                    ret = xmCommands.TransposePlaySampleDirectionVolPanColumn(value);
                    break;
                case 'Q': // Delay a note by x ticks (0 - F).                    
                    // same as volume column
                    ret = xmCommands.TransposeNoteDelayFromVolumeNew(value);
                    break;
                case 'R': // Retrigger a note every x ticks (0 - F).      
                    // same as volume column
                    ret = xmCommands.TransposeRetrigNoteFromVolume(value);
                    break;
                case 'C': // Cut the note after x ticks (0 - F).                    
                    ret = xmCommands.NoteCut(value);
                    break;
                default:
                    break;
            }

            return ret;
        }

        public byte[] TransposeVolumeToCommandEffect(string xrnsColVolEff)
        {
            byte[] ret = new byte[2];
            ret[0] = 0;
            ret[1] = 0;

            char[] commandEffectSplitted = xrnsColVolEff.ToCharArray();
            char command = commandEffectSplitted[0];
            char effVal = commandEffectSplitted[1];

            int value = Int16.Parse(effVal.ToString(), System.Globalization.NumberStyles.HexNumber);

            switch (command)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8': // 00 - 7F - Set note volume (velocity) of current note column.
                    int hexVal = Int16.Parse(xrnsColVolEff, System.Globalization.NumberStyles.HexNumber);
                    ret = xmCommands.TransposeSetVolumeFromVolume(hexVal);
                    break;
                case 'I': //  Volume fade in in the current note column, with step x * 10 (91 = 0I10 in effect column, 92 = 0I20 in effect column etc.)
                    ret = xmCommands.TransposeVolumeSlideUpFromVolume(value, ticksPerRow);
                    break;
                case 'O': //  Volume fade out in the current note column, with step x * 10 (A1 = 0O10, A2 = 0O20 etc.)
                    ret = xmCommands.TransposeVolumeSlideDownFromVolume(value, ticksPerRow);
                    break;
                default:
                    ret = TransposeVolPanEffectColumnToEffectColumn(command, value, ticksPerRow);
                    break;

            }

            return ret;

        }        

        public byte[] TransposePanningToCommandEffect(string xrnsColPanEff)
        {
            byte[] ret = new byte[2];
            ret[0] = 0;
            ret[1] = 0;

            char[] commandEffectSplitted = xrnsColPanEff.ToCharArray();
            char command = commandEffectSplitted[0];
            char hexvalue = commandEffectSplitted[1];

            int value = Int16.Parse(hexvalue.ToString(), System.Globalization.NumberStyles.HexNumber);

            switch (command)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8': // Set panning of current note column: (00 = full left, 40 = center, 80 = full right).
                    int hexVal = Int16.Parse(xrnsColPanEff, System.Globalization.NumberStyles.HexNumber);
                    ret = xmCommands.TransposeSetPanningFromPanning(hexVal);
                    break;
                case 'J': //  Panning slide left with step x (0 - F).
                    ret = xmCommands.TransposePanningSlideLeft(value, ticksPerRow);
                    break;
                case 'K': //   Panning slide right with step x (0 - F).
                    ret = xmCommands.TransposePanningSlideRight(value, ticksPerRow);
                    break;
                default:
                    ret = TransposeVolPanEffectColumnToEffectColumn(command, value, ticksPerRow);
                    break;

            }

            return ret;
        }

        private struct EnvelopePoint : IComparable<EnvelopePoint>, IComparable
        {
            public EnvelopePoint(int x, int y)
                : this()
            {
                X = x;
                Y = y;
            }

            // used to sort
            public int CompareTo(EnvelopePoint other)
            {
                return X.CompareTo(other.X);
            }

            // used to search
            public int CompareTo(object other)
            {
                return X.CompareTo((int)other);
            }

            public int X { get; private set; }
            public int Y { get; private set; }

        }

        private static void AddNewPointInList(List<EnvelopePoint> listPoints, int x)
        {
            /*
             * Add new envelopes point in existing list
             * */
            int y = 0;

            int index = 0;

            for (index = 0; index < listPoints.Count; index++)
            {
                if (x < listPoints[index].X)
                {
                    if (index > 0)
                    {
                        /*
                         * This code calculate the y axis according to the x position of the line
                         * Thanks to my bro for giving me this algo
                         * */
                        double x1 = listPoints[index - 1].X;
                        double x2 = listPoints[index].X;
                        double y1 = listPoints[index - 1].Y;
                        double y2 = listPoints[index].Y;

                        double a = (y2 - y1) / (x2 - x1);
                        double b = y1 - (a * x1);
                        y = (int)((a * x) + b);

                    }
                    else
                    {
                        y = listPoints[index].Y;
                    }

                    break;
                }
                else
                {
                    /* if argument point is the last one and is not the only one, 
                     * it takes the y value the from previous existing point
                     * */
                    if (index == listPoints.Count - 1)
                    {
                        y = listPoints[index].Y;
                        index++;
                        break;
                    }
                }
            }

            // index is used to insert the point in the right position of the array
            EnvelopePoint newPoint = new EnvelopePoint(x, y);
            listPoints.Insert(index, newPoint);
        }
    }
}
