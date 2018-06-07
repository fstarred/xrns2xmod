using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xrns2XMod.Properties;

namespace Xrns2XMod
{
    class ModUtils : ModCommonBase
    {
        class ChannelInfoData
        {
            public int ReachedNote { get; set; }                // renoise reached note
            public int TonePortamentoNote { get; set; }         // renoise tone portamento note
            public int TonePortamentoPeriod { get; set; }       // tone portamento period
            public int CurrentPitch { get; set; }               // pitch of the note (1-15)
            public int CurrentSample { get; set; }              // triggered sample number
            public int CurrentPeriod { get; set; }              // period reached
            public int LastPortamentoCommand { get; set; }      // last portamento command triggered
            public int LastPortamentoValue { get; set; }        // last value triggered
            public int PortamentoDirectionFlag { get; set; }    // portamento direction (to be removed)
        }

        ChannelInfoData[] channelData = new ChannelInfoData[4];

        bool forceProtrackerCompatibility;
        int portamentoAccuracyThreshold;

        public ModUtils(SongData songData, int paramTicksPerRow, bool forceProTrackerCompatibility, int portamentoAccuracyThreshold)
            : base(songData, paramTicksPerRow)
        {
            modCommands = new ModCommands(songData, paramTicksPerRow);

            this.forceProtrackerCompatibility = forceProTrackerCompatibility;
            this.portamentoAccuracyThreshold = portamentoAccuracyThreshold;
            instrumentsList = new InstrumentDataMOD[songData.Instruments.Length];

            for (int i = 0; i < instrumentsList.Length; i++)
            {
                instrumentsList[i] = new InstrumentDataMOD();
                instrumentsList[i].Samples = new InstrumentDataMOD.SampleDataMOD[songData.Instruments[i].Samples.Length];
                for (int a = 0; a < instrumentsList[i].Samples.Length; a++)
                {
                    instrumentsList[i].Samples[a] = new InstrumentDataMOD.SampleDataMOD();
                }
            }
            for (int i = 0; i < channelData.Length; i++)
            {
                ChannelInfoData item = new ChannelInfoData();
                item.ReachedNote = -1;
                channelData[i] = item;
            }

        }

        public ModUtils(SongData songData, int paramTicksPerRow)
            : this(songData, paramTicksPerRow, false, 2)
        {
            
        }
        
        private static readonly int[] PeriodsRange = 
        { 
            1712,1616,1524,1440,1356,1280,1208,1140,1076,1016,960,907,                
	        856,808,762,720,678,640,604,570,538,508,480,453, // standard pro tracker tunes (Amiga freq.)
	        428,404,381,360,339,320,302,285,269,254,240,226, // standard pro tracker tunes (Amiga freq.)
	        214,202,190,180,170,160,151,143,135,127,120,113, // standard pro tracker tunes (Amiga freq.)                
	        107,101,95,90,85,80,75,71,67,63,60,56,
	        53,50,47,45,42,40,37,35,33,31,30,28 
        };

        protected InstrumentDataMOD[] instrumentsList;

        protected ModCommands modCommands;

        public void StoreSampleInfo(int sampleIndex, int originalLength, int length,
            int freq, int OriginalChans, int originalBps, int renoiseBaseNote, int renoiseFineTuning, int transpose)
        {
            int baseNote;
            int fineTune;

            int computedLength = CalculateSampleLength(length, 8, 1);
            int computedOriginalLength = CalculateSampleLength(originalLength, originalBps, OriginalChans);

            GetSampleProperties(renoiseBaseNote, transpose, renoiseFineTuning, freq, out baseNote, out fineTune);

            instrumentsList[sampleIndex].Samples[0].RelatedNote = baseNote;
            instrumentsList[sampleIndex].Samples[0].FineTune = fineTune;
            instrumentsList[sampleIndex].Samples[0].Length = computedLength;
            instrumentsList[sampleIndex].Samples[0].OriginalLength = computedOriginalLength;

        }

        /*
        Lowest four bits represent a signed nibble (-8..7) which is
        the finetune value for the sample. Each finetune step changes
        the note 1/8th of a semitone. Implemented by switching to a
        different table of period-values for each finetune value.
         * */
        public int GetSampleFineTune(int sampleIndex)
        {
            return instrumentsList[sampleIndex].Samples[0].FineTune;
        }

        public int GetModNote(int note, int sampleNumber, int channel)
        {
            //due to lack of base note sample on mod, 
            //note is dinamically computed by related sample note and frequency                                
            int tone2Add = sampleNumber >= 0 ? instrumentsList[sampleNumber].Samples[0].RelatedNote : 0;

            int value = 0;

            if (note >= 0)
            {
                int octave = note / 12;

                int noteIndex = note % 12;

                int finalNote = (octave - 2) * 12 + noteIndex + tone2Add;

                if (finalNote >= 0 && finalNote < PeriodsRange.Length
                    && (forceProtrackerCompatibility == false || finalNote > 11 && finalNote < 48))
                {
                    value = finalNote;
                }
                else
                {
                    throw new ConversionException(String.Format("note {0} is out of range", note));
                }
            }

            return value;
        }

        public bool IsTonePortamentoTriggered(string effect, string volume, string panning)
        {
            bool output = false;

            if (effect != null)
            {
                char[] commandEffectSplitted = effect.ToCharArray();
                char effType = commandEffectSplitted[0];
                char effCom = commandEffectSplitted[1];

                output = (effType == '0' && effCom == 'G');
            }
            if (output == false && volume != null)
            {
                char[] commandEffectSplitted = volume.ToCharArray();
                char effType = commandEffectSplitted[0];

                output = effType == 'G';
                if (output && effect != null)
                    throw new Exception("Critical conversion exception: Found value for fx column and glide value on volume column");
            }
            if (output == false && panning != null)
            {
                char[] commandEffectSplitted = panning.ToCharArray();
                char effType = commandEffectSplitted[0];

                output = effType == 'G';
                if (output && effect != null)
                    throw new Exception("Critical conversion exception: Found value for fx column and glide value on volume column");
            }

            return output;            
        }

        public int GetModNote(string note, int sampleNumber, int channel, bool isTonePortamentoTriggered)
        {
            //due to lack of base note sample on mod, 
            //note is dinamically computed by related sample note and frequency                                
            int tone2Add = sampleNumber >= 0 ? instrumentsList[sampleNumber].Samples[0].RelatedNote : 0;

            int value = 0;

            string tune = note.Substring(0, 2);

            // useful for finding an item into an array
            int originalNoteIndex = Array.FindIndex(notesArray, delegate(string item)
            {
                return item.Equals(tune);
            });
            if (originalNoteIndex >= 0)
            {
                int octave = Int16.Parse(note.Substring(2, 1));

                int noteIndex = (octave - 2) * 12 + originalNoteIndex + tone2Add;

                if (noteIndex >= 0 && noteIndex < PeriodsRange.Length
                    && (forceProtrackerCompatibility == false || noteIndex > 11 && noteIndex < 48))
                {
                    value = PeriodsRange[noteIndex];

                    int renoiseNote = (octave * 12) + originalNoteIndex;

                    // playingChannelData
                    ChannelInfoData playingChannelData = channelData[channel];         
                               
                    playingChannelData.CurrentSample = sampleNumber;
                    if (isTonePortamentoTriggered)
                    {
                        playingChannelData.TonePortamentoNote = renoiseNote;
                        playingChannelData.TonePortamentoPeriod = value;
                    }
                    else
                    {
                        playingChannelData.CurrentPitch = 0;
                        playingChannelData.CurrentPeriod = value;
                        playingChannelData.TonePortamentoPeriod = value;
                        playingChannelData.TonePortamentoNote = renoiseNote;
                        playingChannelData.ReachedNote = renoiseNote;
                    }

                    // debug
                    //if (channel == 2)
                    //    System.Diagnostics.Debug.WriteLine(String.Format("Note: Channel: {0} Period: {1} ModNote: {2} OriginalNote {3}", 
                    //    channel, value, noteIndex, channelData[channel].ReachedNote));
                }
                else
                {
                    throw new ConversionException(String.Format("note {0} is out of range", note));
                }
            }

            return value;
        }

        private int GetSampleLength(int sampleIndex)
        {
            int sampleSize = sampleIndex > 0 ?
                sampleSize = instrumentsList[sampleIndex - 1].Samples[0].Length : 0;            

            return sampleSize;
        }

        private int GetEffectivePortamento(byte command, byte value)
        {
            int output = 0;
            switch (command)
            {
                case 0x01:
                case 0x02:
                case 0x03:
                    output = value * (ticksPerRow - 1);
                    break;
                case 0x0E:
                    output = value & 0x0F;
                    break;                
            }
            //
            return output;
        }
        
        private int GetTonePortamentoFromChannelPeriod(int value, int channel, bool isNoteTriggered, bool ignorePitchCompatibilityMode)
        {
            ChannelInfoData playingChannelData = channelData[channel];

            if (value == 0 && playingChannelData.LastPortamentoValue > 0)
                value = playingChannelData.LastPortamentoValue;

            if (value == 0 || playingChannelData.CurrentPeriod == playingChannelData.TonePortamentoPeriod)
            {
                if (isNoteTriggered)
                    return 0;
                else
                    throw new ConversionException("tone portamento value equals 0");
            }            

            const int portamentoCommand = 0x03;

            int tonePortamentoPeriod = playingChannelData.TonePortamentoPeriod;
            
            int realValue = ignorePitchCompatibilityMode == false && pitchCompatibilityMode ? value * (ticksPerRow > 1 ? ticksPerRow - 1 : 1) : value;
            int directionFlag = playingChannelData.CurrentPeriod < tonePortamentoPeriod ? -1 : +1;
            int currentPitch = playingChannelData.CurrentPitch + (realValue * directionFlag);
            int currentRenoiseNote = (int)Math.Truncate(((double)currentPitch / 16) + playingChannelData.ReachedNote);
            int currentPeriod = playingChannelData.CurrentPeriod;
            int modNote = GetModNote(currentRenoiseNote, playingChannelData.CurrentSample, channel);            
            int relativePitch;

            relativePitch = currentPitch % 0x10;
            if (relativePitch < 0)
                relativePitch = relativePitch + 0x10;

            if (currentPeriod == 0)
                throw new ConversionException("no previous note triggered on this channel");

            int firstperiod = PeriodsRange[modNote];

            int secondperiod = PeriodsRange[modNote + 1];

            int delta = firstperiod - secondperiod;

            int portamento = (relativePitch * delta) / 0x10;

            int targetPeriod = firstperiod - portamento;            

            portamento = (currentPeriod - targetPeriod) * directionFlag;

            playingChannelData.ReachedNote = currentRenoiseNote;
            playingChannelData.CurrentPitch = relativePitch;
            playingChannelData.LastPortamentoCommand = portamentoCommand;
            playingChannelData.LastPortamentoValue = value;
            playingChannelData.PortamentoDirectionFlag = directionFlag;
            

            return portamento;
        }

        /*
        private int GetTonePortamentoFromChannelPeriodOld(int value, int channel)
        {
            ChannelInfoData playingChannelData = channelData[channel];

            if (value == 0 && playingChannelData.LastPortamentoValue > 0)
                value = playingChannelData.LastPortamentoValue;

            if (value == 0)
                throw new ConversionException("tone portamento value equals 0");


            if (channel == 1)
            {
                // debug
                System.Diagnostics.Debug.WriteLine("Current Period: " + playingChannelData.CurrentPeriod);
            }

            const int portamentoCommand = 0x03;

            int tonePortamentoPeriod = PeriodsRange[GetModNote(playingChannelData.TonePortamentoNote, playingChannelData.CurrentSample, channel)];

            int directionFlag = playingChannelData.CurrentPeriod < tonePortamentoPeriod ? -1 : +1;
            int currentPitch = playingChannelData.CurrentPitch + (value * directionFlag);
            int currentRenoiseNote = (int)Math.Truncate(((double)currentPitch / 16) + playingChannelData.ReachedNote);

            if ((directionFlag > 0 && currentRenoiseNote >= playingChannelData.TonePortamentoNote) ||
                (directionFlag < 0 && currentRenoiseNote < playingChannelData.TonePortamentoNote))
            {
                currentRenoiseNote = playingChannelData.TonePortamentoNote;
                currentPitch = 0;
            }

            int modNote = GetModNote(currentRenoiseNote, playingChannelData.CurrentSample, channel);
            int currentPeriod = playingChannelData.CurrentPeriod;
            int relativePitch;

            relativePitch = currentPitch % 0x10;
            if (relativePitch < 0)
                relativePitch = relativePitch + 0x10;

            if (currentPeriod == 0)
                throw new ConversionException("no previous note triggered on this channel");

            int firstperiod = PeriodsRange[modNote];

            int secondperiod = PeriodsRange[modNote + 1];

            int delta = firstperiod - secondperiod;

            int portamento = (relativePitch * delta) / 0x10;

            int targetPeriod = firstperiod - portamento;

            portamento = (currentPeriod - targetPeriod) * directionFlag;

            playingChannelData.ReachedNote = currentRenoiseNote;
            playingChannelData.CurrentPitch = relativePitch;
            playingChannelData.LastPortamentoCommand = portamentoCommand;
            playingChannelData.LastPortamentoValue = value;
            playingChannelData.PortamentoDirectionFlag = directionFlag;

            if (channel == 0)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("channel: {0} effect: {1} value: {2} note: {3} pitch: {4} portamento: {5}",
                channel, portamentoCommand, value, currentRenoiseNote, relativePitch, portamento));
                // debug
            }

            return portamento;
        }
        */

        private int GetPortamentoFromChannelPeriod(char command, int value, int channel, bool ignorePitchCompatibilityMode)
        {
            ChannelInfoData playingChannelData = channelData[channel];

            if (value == 0 && playingChannelData.LastPortamentoValue > 0)
                value = playingChannelData.LastPortamentoValue;

            // new
            int directionFlag;
            int portamentoCommand;
            switch (command)
            {
                case 'U':
                    directionFlag = 1;
                    portamentoCommand = 1;
                    break;
                case 'D':
                    directionFlag = -1;
                    portamentoCommand = 2;
                    break;
                default:
                    throw new ConversionException("command not valid");
            }

            int realValue = ignorePitchCompatibilityMode == false && pitchCompatibilityMode ? value * (ticksPerRow > 1 ? ticksPerRow - 1 : 1) : value;
            int currentPitch = playingChannelData.CurrentPitch + (realValue * directionFlag);
            int currentRenoiseNote = (int)Math.Truncate(((double)currentPitch / 16) + playingChannelData.ReachedNote);
            int modNote = GetModNote(currentRenoiseNote, playingChannelData.CurrentSample, channel);
            int currentPeriod = playingChannelData.CurrentPeriod;
            int relativePitch = currentPitch % 0x10;
            if (relativePitch < 0)
                relativePitch = relativePitch + 0x10;
            
            int finalNote = modNote + 1;

            if (currentPeriod == 0)
                throw new ConversionException("no previous note triggered on this channel");            

            int firstperiod = PeriodsRange[modNote];
            
            int secondperiod = PeriodsRange[modNote + 1];

            int delta = firstperiod - secondperiod;

            int portamento = (relativePitch * delta) / 0x10;

            int targetPeriod = firstperiod - portamento;

            portamento = (currentPeriod - targetPeriod) * directionFlag;
            
            playingChannelData.ReachedNote = currentRenoiseNote;
            playingChannelData.CurrentPitch = relativePitch;
            playingChannelData.LastPortamentoCommand = portamentoCommand;
            playingChannelData.LastPortamentoValue = value;
            playingChannelData.PortamentoDirectionFlag = directionFlag;

            if (portamento == 0)
                throw new ConversionException("Portamento value resulted to 0 value, no effect was triggered there");

            //if (channel == 2)
            //{
            //    System.Diagnostics.Debug.WriteLine(String.Format("channel: {0} effect: {1} value: {2} note: {3} pitch: {4} portamento: {5}",
            //    channel, portamentoCommand, value, currentRenoiseNote, relativePitch, portamento));
            //    // debug
            //}

            return portamento;
        }

        private void StoreChannelPeriod(int portamento, char command, int channel)
        {
            ChannelInfoData playingChannelData = channelData[channel];
            int directionFlag = playingChannelData.PortamentoDirectionFlag * -1;
            int currentPeriod = playingChannelData.CurrentPeriod + (portamento * directionFlag);

            if (command == 'G')
            {
                if ((directionFlag > 0 && currentPeriod > playingChannelData.TonePortamentoPeriod) ||
                    (directionFlag < 0 && currentPeriod < playingChannelData.TonePortamentoPeriod))
                {
                    currentPeriod = playingChannelData.TonePortamentoPeriod;
                    playingChannelData.ReachedNote = playingChannelData.TonePortamentoNote;
                    playingChannelData.CurrentPitch = 0;
                }
            }

            playingChannelData.CurrentPeriod = currentPeriod;
            //System.Diagnostics.Debug.WriteLine("current period: " + channelData[channel].CurrentPeriod);
        }

        private byte[] GetSampleCommands(char command, int value, int sampleIndex, int channel, bool isNoteTriggered)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0;
            ret[1] = (byte)0;

            switch (command)
            {
                case 'A': //Arpeggio (x = first note offset, y = second note offset).
                    ret = modCommands.Arpeggio(value);
                    break;
                case 'U': //01xx - Pitch slide up (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitone).                        
                    int portamentoUpValue = GetPortamentoFromChannelPeriod(command, value, channel, false);
                    ret = modCommands.Portamento(1, portamentoUpValue, ticksPerRow, portamentoAccuracyThreshold);
                    portamentoUpValue = GetEffectivePortamento(ret[0], ret[1]);
                    StoreChannelPeriod(portamentoUpValue, command, channel);
                    break;
                case 'D': //02xx - Pitch slide down (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitone).
                    int portamentoDownValue = GetPortamentoFromChannelPeriod(command, value, channel, false);
                    ret = modCommands.Portamento(2, portamentoDownValue, ticksPerRow, portamentoAccuracyThreshold);
                    portamentoDownValue = GetEffectivePortamento(ret[0], ret[1]);
                    StoreChannelPeriod(portamentoDownValue, command, channel);                    
                    break;
                case 'M': //03xx - Set channel volume (represents range from -60 to +3db).
                    ret = modCommands.SetVolume(value);
                    break;
                case 'C': //04xy - Volume slicer. x = volume factor (0 = 0%, F = 100%), applied at tick number y.                    
                    break;
                case 'G': //05xx - Glide to specified note with step xx (01 is 1/16th semitone, 08 is a half semitone, 10 is one semitonee).
                    int tonePortamento = GetTonePortamentoFromChannelPeriod(value, channel, isNoteTriggered, false);
                    ret = modCommands.TonePortamento(tonePortamento, ticksPerRow);
                    tonePortamento = GetEffectivePortamento(ret[0], ret[1]);
                    StoreChannelPeriod(tonePortamento, command, channel);
                    break;
                case 'I': //06xx - Volume slide up with step xx (0601 inserted 256 times will slide from 0 to full volume, 067F inserted twice will do the same).
                    ret = modCommands.VolumeUp(value, ticksPerRow);
                    break;
                case 'O': //07xx - Volume slide down with step xx.
                    ret = modCommands.VolumeDown(value, ticksPerRow);
                    break;
                case 'P': //08xx - Change the track pre-mixer's panning (00 = full left, 80 = center, FF = full right).
                    ret = modCommands.SetPanning(value);
                    break;
                case 'S': //09xx - Trigger sample at slice number xx, (00 is the sample's start, FF the last slice).
                    ret = modCommands.SetSampleOffset(value, GetSampleLength(sampleIndex));
                    break;
                case 'W': //0Axx - Track surround width.
                    break;
                case 'B': //0Bxx - Play sample backwards (B00) or forwards again (B01 during play when sample is playing backwards). Can also be combined with the 09xx effect.
                    break;
                case 'L': //0Cxx - Change the track pre-mixer's volume (represents range from -INF to +3db).
                    break;
                case 'Q': //0Dxx - Delay all notes by xx ticks.                    
                    ret = modCommands.NoteDelay(value);
                    break;
                case 'R': //0Exy - Retrigger a note every y ticks with volume x.                    
                    ret = modCommands.RetrigNote(value);
                    break;
                case 'V': //0Fxy Vibrato (x = speed, y = depth).
                    ret = modCommands.Vibrato(value);
                    break;
                case 'T':
                    ret = modCommands.Tremolo(value);
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


        protected byte[] GetGlobalCommands(char command, int value)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0;
            ret[1] = (byte)0;

            switch (command)
            {
                case 'T': //F0xx - Set Beats Per Minute (BPM) (20 - FF, 00 = stop song)
                    ret = modCommands.SetTempo(value);
                    break;
                case 'L': //F1xx - Set Lines Per Beat (LPB) (01 - FF, 00 = stop song).
                    if (playbackEngineVersion == Constants.MOD_VERSION_COMPATIBLE)
                    {
                        ret = modCommands.SetSpeed(value);
                    }
                    break;
                case 'K': //F2xx - Set Ticks Per Line (TPL) (01 - 10).
                    ret = modCommands.SetSpeed(value);
                    break;                
                case 'G': //F4xx - Toggle song groove on/off (see Song Settings) (00 = turn off, 01 or higher = turn on).
                    break;
                case 'B': //FBxx - Pattern break. The current pattern finishes immediately and jumps to next pattern at line xx (hex).
                    ret = modCommands.PatternBreak(value);
                    break;                
                case 'D': //FDxx - Pause pattern playback by xx lines.
                    ret = modCommands.PatternDelay(value);
                    break;
                default:
                    break;
            }

            return ret;
        }


        public byte[] GetModEffect(string xrnsEffNum, string xrnsEffVal, int sampleIndex, int channel, bool isNoteTriggered)
        {
            char[] commandEffectSplitted = xrnsEffNum.ToCharArray();
            char effType = commandEffectSplitted[0];
            char effCom = commandEffectSplitted[1];
            int effVal = Int16.Parse(xrnsEffVal, System.Globalization.NumberStyles.HexNumber);

            byte[] ret = new byte[2];
            ret[0] = (byte)0;
            ret[1] = (byte)0;

            switch (effType)
            {
                case '0': // Sample Commands
                    ret = GetSampleCommands(effCom, effVal, sampleIndex, channel, isNoteTriggered);
                    break;
                case 'Z': // Global Commands
                    ret = GetGlobalCommands(effCom, effVal);
                    break;
            }            

            return ret;
        }

        public byte[] GetCommandsFromMasterTrack(string xrnsEffNum, string xrnsEffVal)
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)0;
            ret[1] = (byte)0;

            char[] commandEffectSplitted = xrnsEffNum.ToCharArray();
            char effType = commandEffectSplitted[0];
            char effCom = commandEffectSplitted[1];
            int effVal = Int16.Parse(xrnsEffVal, System.Globalization.NumberStyles.HexNumber);

            if (effType == 'Z') // Global Commands
                ret = GetGlobalCommands(effCom, effVal);                                

            return ret;
        }


        public byte[] TransposeVolPanEffectColumnToEffectColumn(char command, int value, int ticksPerRow, int sampleIndex, int channel, bool isNoteTriggered)
        {
            byte[] ret = new byte[2];

            switch (command)
            {
                case 'U':
                    //ret = modCommands.TransposePortamentoUpVolPanColumn(value, ticksPerRow);
                    int portamentoUpValue = GetPortamentoFromChannelPeriod(command, value << 4, channel, true);
                    ret = modCommands.Portamento(1, portamentoUpValue, ticksPerRow, portamentoAccuracyThreshold);
                    portamentoUpValue = GetEffectivePortamento(ret[0], ret[1]);
                    StoreChannelPeriod(portamentoUpValue, command, channel);
                    break;
                case 'D':
                    //ret = modCommands.TransposePortamentoDownVolPanColumn(value, ticksPerRow);
                    int portamentoDownValue = GetPortamentoFromChannelPeriod(command, value << 4, channel, true);
                    ret = modCommands.Portamento(2, portamentoDownValue, ticksPerRow, portamentoAccuracyThreshold);
                    portamentoDownValue = GetEffectivePortamento(ret[0], ret[1]);
                    StoreChannelPeriod(portamentoDownValue, command, channel);
                    break;
                case 'G':
                    //ret = modCommands.TransposeGlideVolPanColumn(value, ticksPerRow);
                    int tonePortamento = GetTonePortamentoFromChannelPeriod(value << 4, channel, isNoteTriggered, true);
                    ret = modCommands.TonePortamento(tonePortamento, ticksPerRow);
                    tonePortamento = GetEffectivePortamento(ret[0], ret[1]);
                    StoreChannelPeriod(tonePortamento, command, channel);
                    break;
                case 'B': // Play sample in the current note column backwards (0 is backwards, 1 is forwards again).                    
                    break;
                case 'Q': // Delay a note by x ticks (0 - F).         
                    ret = modCommands.TransposeNoteDelayFromVolumeNew(value);
                    break;
                case 'R': // Retrigger a note every x ticks (0 - F).   
                    ret = modCommands.TransposeRetrigNoteFromVolume(value);
                    break;
                case 'C': // Cut the note after x ticks (0 - F).       
                    ret = modCommands.NoteCut(value);
                    break;
                default:
                    break;
            }

            return ret;
        }

        public byte[] TransposeVolumeToCommandEffect(string xrnsColVolEff, int sampleIndex, int channel, bool isNoteTriggered)
        {

            byte[] ret = new byte[2];
            ret[0] = 0;
            ret[1] = 0;

            char[] commandEffectSplitted = xrnsColVolEff.ToCharArray();
            char effCom = commandEffectSplitted[0];
            char effVal = commandEffectSplitted[1];

            int value = Int16.Parse(effVal.ToString(), System.Globalization.NumberStyles.HexNumber);

            switch (effCom)
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
                    ret = modCommands.TransposeSetVolumeFromVolume(hexVal);
                    break;
                case 'I': //  Volume fade in in the current note column, with step x * 10 (91 = 0I10 in effect column, 92 = 0I20 in effect column etc.)
                    ret = modCommands.TransposeVolumeSlideUpFromVolume(value, ticksPerRow);
                    break;
                case 'O': //  Volume fade out in the current note column, with step x * 10 (A1 = 0O10, A2 = 0O20 etc.)
                    ret = modCommands.TransposeVolumeSlideDownFromVolume(value, ticksPerRow);
                    break;                
                default:
                    ret = TransposeVolPanEffectColumnToEffectColumn(effCom, value, ticksPerRow, sampleIndex, channel, isNoteTriggered);
                    break;

            }

            return ret;
        }


        public byte[] TransposeDelayToCommandEffect(string xrnsColPanEff)
        {
            byte[] ret = new byte[2];
            ret[0] = 0;
            ret[1] = 0;

            int value = Int16.Parse(xrnsColPanEff, System.Globalization.NumberStyles.HexNumber);

            int result = (int)Math.Round((double)(value * ticksPerRow) / 0xff);

            if (result == ticksPerRow)
            {
                result--;
            }

            if (result > 0)
                ret = modCommands.NoteDelay(result);

            return ret;
        }

        public byte[] TransposePanningToCommandEffect(string xrnsColPanEff, int sampleIndex, int channel, bool isNoteTriggered)
        {
            byte[] ret = new byte[2];
            ret[0] = 0;
            ret[1] = 0;

            char[] commandEffectSplitted = xrnsColPanEff.ToCharArray();
            char effCom = commandEffectSplitted[0];
            char effVal = commandEffectSplitted[1];

            int value = Int16.Parse(effVal.ToString(), System.Globalization.NumberStyles.HexNumber);

            switch (effCom)
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
                    // int hexVal = Int16.Parse(xrnsColPanEff, System.Globalization.NumberStyles.HexNumber);
                    // ret = modCommands.TransposeSetPanningFromPanning(hexVal);
                    // NOTE: this effect is not more converted because MOD envolve whole channel whereas XRNS does only the current note
                    break;
                case 'J': //  Panning slide left with step x (0 - F).                    
                    break;
                case 'K': //   Panning slide right with step x (0 - F).                    
                    break;               
                default:
                    ret = TransposeVolPanEffectColumnToEffectColumn(effCom, value, ticksPerRow, sampleIndex, channel, isNoteTriggered);
                    break;

            }

            return ret;
        }


        public int GetLoopValue(uint loopValue, int sampleNumber)
        {
            int originalSampleLength = instrumentsList[sampleNumber].Samples[0].OriginalLength;
            int sampleLength = instrumentsList[sampleNumber].Samples[0].Length;

            int value = 0;

            if (sampleLength > 0)
            {
                value = (int)(loopValue * sampleLength / originalSampleLength) / 2;                
            }

            return value;
        }

    }
}
