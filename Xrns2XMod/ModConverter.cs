
/*
This file is part of Xrns2XMod.

    Xrns2XMod is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Xrns2XMod is distributed in the hope that it will be useful,
    but WITHANY WARRANTY; witheven the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Xrns2XMod.  If not, see <http://www.gnu.org/licenses/>.
 * */


using System;
using System.IO;
using Un4seen.Bass;
using System.Linq;
using Xrns2XMod.Properties;

namespace Xrns2XMod
{
    public class ModConverter : IConverter
    {

        ModUtils modUtils;
        XrnsManager xrnsManager;

        public ModSettings Settings { get; set; }

        // An event that clients can use to be notified when status change
        public event ProgressHandler EventProgress;

        // Invoke the ReportProgress event; 
        protected virtual void OnReportProgress(EventReportProgressArgs e)
        {
            if (EventProgress != null)
                EventProgress(this, e);
        }

        public ModConverter(string srcFileName)
        {
            //this.inputFilename = srcFileName;
            xrnsManager = new XrnsManager(srcFileName);
        }

        

        private void Init(SongData songData)
        {
            const int initialTickPerRow = 6;

            int ticksPerRow = initialTickPerRow;

            bool forceProTrackerCompatibility = Settings.ForceProTrackerCompatibility;
            int portamentoAccuracyLossThreshold = Settings.PortamentoLossThreshold;
            bool NtscMode = Settings.NtscMode;

            modUtils = new ModUtils(songData, ticksPerRow, forceProTrackerCompatibility, portamentoAccuracyLossThreshold, NtscMode);
        }

        const int maxInstruments = 31;

        const int minChannels = 4;

        //private string inputFilename;

        public byte[] Convert(SongData songData)
        {
            if (CheckRequirements(songData) == false)
                return null;
            
            MemoryStream songDataStream = new MemoryStream();

            Init(songData);

            //offset 0
            byte[] songNameData = this.GetModNameData(songData.Name);

            //offset 20
            byte[] samplesData = this.GetAllSamplesData(songData.Instruments);

            //offset 950
            byte[] songLenData = this.GetSongLengthData(songData.PatternOrderTable.Length);

            //offset 952
            byte[] patternSequences = this.GetPatternSequenceData(songData.PatternOrderTable);

            //offset 1080
            byte[] channelsData = this.GetChannelsNumData(songData.NumChannels);

            //offset 1084
            byte[] patternsData = this.GetAllPatternsData(songData.Patterns, songData.Instruments, songData.NumChannels, songData.NumMasterTracksColumns, patternSequences);

            songDataStream.Write(songNameData, 0, songNameData.Length);

            songDataStream.Write(samplesData, 0, 930);

            songDataStream.Write(songLenData, 0, songLenData.Length);

            songDataStream.Write(patternSequences, 0, patternSequences.Length);

            songDataStream.Write(channelsData, 0, channelsData.Length);

            songDataStream.Write(patternsData, 0, patternsData.Length);

            songDataStream.Write(samplesData, 930, samplesData.Length - 930);

            // sometimes useful if no sample data is stored :)
            //songDataStream.WriteByte(0);

            return songDataStream.ToArray();
        }

        private bool CheckRequirements(SongData songData)
        {
            if (songData.NumChannels < minChannels)
                throw new ConversionException("MOD format requires a minimum of 4 channels");

            return true;
        }

        private byte[] GetModNameData(string name)
        {
            int offset = 0;

            byte[] data = new byte[20];

            Array.Copy(Utility.GetBytesFromString(name, 20), 0, data, offset, 20);

            return data;
        }

        /*
         * This take all sample data (sample info + data)
         * info part is stored from index 0 to 929 
         * data starts from 930
         * */
        private byte[] GetAllSamplesData(InstrumentData[] instruments)
        {
            const int sampleInfoSize = 930;

            /*
             * Informations about frequency is based on http://www.pouet.net/topic.php?which=8628
             * For a PAL machine:
             * SampleRate = 7093789.2 / (Period * 2)
             * C2 428 -> 8287.13691 Hz
             * C3 214 -> 16574.2738 Hz
             * For a NTSC machine:
             * SampleRate = 7159090 / (Period * 2)
             * C2 428 -> 8363.423 Hz
             * C3 214 -> 16726.846 Hz
             */

            const int NtscC2Frequency = 8363;
            const int PalC2Frequency = 8287;
            int freqC2 = Settings.NtscMode ? NtscC2Frequency : PalC2Frequency;

            const int maxSampleLengthMOD = 65536;

            int totalInstruments = instruments.Length;

            int offset;

            byte[] sampleInfo = new byte[sampleInfoSize];

            byte[] allSampleData;

            //XrnsReaderUtil xrnsReader = new XrnsReaderUtil(srcFileName);

            //XrnsManager xrnsManager = new XrnsManager();

            MemoryStream ms4SampleData = new MemoryStream();

            if (totalInstruments > maxInstruments) totalInstruments = maxInstruments;

            offset = 0;

            // initialize end loop values of all samples with value 1 (avoid crashes in Protracker)
            for (int i = 0; i < sampleInfo.Length; i += 30)
            {
                sampleInfo[i + 29] = 1;
            }

            for (int ci = 0; ci < totalInstruments; ci++)
            {
                OnReportProgress(new EventReportProgressArgs(String.Format("Processing Sample {0}/{1}", (ci + 1), totalInstruments)));

                if (instruments[ci].Samples.Length > 1)
                {
                    OnReportProgress(new EventReportProgressArgs(String.Format("More samples detected on instrument {0}", (ci + 1)), MsgType.ERROR));
                }

                byte[] sampleData = new byte[0];

                try
                {
                    SampleStreamInfo originalSample = xrnsManager.GetSampleStreamInfo(ci, 0);

                    //Stream originalSample = xrnsReader.GetInstrumentSample(ci, 0);

                    // means sample is probably empty
                    if (originalSample.Format != FORMAT.NONE)
                    {
                        int handle = BassWrapper.GetBassStream(originalSample);

                        BASS_CHANNELINFO bassChannelInfo = BassWrapper.GetBassChannelInfo(handle);

                        int origres = bassChannelInfo.origres;

                        if (origres == 0) // some streams were reported to return undefinied resolution
                        {
                            OnReportProgress(new EventReportProgressArgs("Sample bps detection failed, assuming 8 bits by default", MsgType.WARNING));
                            origres = 8;
                        }

                        long sampleLength = Bass.BASS_ChannelGetLength(handle);

                        // samplerate may be:
                        // 1) same as original
                        // 2) taken from song settings
                        int sampleRate = freqC2;
                        
                        int freqFromIni = instruments[ci].Samples[0].SampleFreq;

                        if (freqFromIni > 0)
                        {
                            OnReportProgress(new EventReportProgressArgs(String.Format("Sample {0} frequency adjusted to: {1} Hz", (ci + 1), freqFromIni), MsgType.INFO));
                            sampleRate = freqFromIni;
                        }
                        else
                        {
                            OnReportProgress (new EventReportProgressArgs (String.Format ("Sample {0} frequency stays C3 frequency {1} Hz", (ci + 1), sampleRate), MsgType.INFO));
                            sampleRate = freqC2;
                        }

#if DEBUG
                        Console.WriteLine("sampleRate "+ sampleRate+ "   sampleLength "+ sampleLength);
                        Console.WriteLine("bassChannelInfo.freq " + bassChannelInfo.freq);
                        Console.WriteLine("bassChannelInfo.chans " + bassChannelInfo.chans);
                        Console.WriteLine("origres " + origres);
#endif
                        //Enforce 16 Bit Samples here as 8 Bit samples are corrupted (only on Linux?).
                        //The 8 least significant bits are removed later.
                        int mixer = BassWrapper.PlugChannelToMixer (handle, sampleRate, 1, 16);

                        if (Settings.VolumeScalingMode == VOLUME_SCALING_MODE.SAMPLE && instruments[ci].Samples[0].Volume != 1.0f)
                        {
                            OnReportProgress(new EventReportProgressArgs(String.Format("Ramping sample volume to value {0}", instruments[ci].Samples[0].Volume)));
                            BassWrapper.AdjustSampleVolume(handle, mixer, instruments[ci].Samples[0].Volume);
                        }

                        Stream stream = BassWrapper.GetModEncodedSample(mixer, sampleLength, Settings.ForceProTrackerCompatibility);

                        Bass.BASS_StreamFree(handle);

                        Bass.BASS_StreamFree(mixer);

                        int originalChans = bassChannelInfo.chans;

                        int originalBps = origres;

                        modUtils.StoreSampleInfo(ci, (int)sampleLength, (int)stream.Length, sampleRate, originalChans, originalBps, 
                            instruments[ci].Samples[0].RelNoteNumber, instruments[ci].Samples[0].FineTune, instruments[ci].Samples[0].Transpose);

                        if (stream.Length > maxSampleLengthMOD)
                        {
                            throw new ApplicationException(String.Format("Sample number {0} is too large: max size for mod is {1}", (ci + 1), maxSampleLengthMOD));
                        }

                        // sample data will be stored only if sample doesn't exceed length of 65536 bytes
                        sampleData = Utility.GetBytesFromStream(stream, stream.Length);

                        stream.Close();
                    }

                }
                catch (Exception e)
                {
                    OnReportProgress(new EventReportProgressArgs(e.Message, MsgType.ERROR));
                }

                if (instruments[ci].Samples.Length > 0)
                {
                    Array.Copy(Utility.GetBytesFromString(instruments[ci].Name, 22), 0, sampleInfo, offset, 22);

                    offset += 22;

                    // stored as a word number in big endian
                    Utility.PutInt2InByteArray((sampleData.Length / 2), true, sampleInfo, offset);

                    offset += 2;

                    // for any doubt just see in mod specs how fineTune is stored
                    sampleInfo[offset++] = (byte)(modUtils.GetSampleFineTune(ci) >> 4 & 0x0F);

                    // default volume
                    sampleInfo[offset++] = instruments[ci].Samples[0].DefaultVolume;

                    //if (ModUtil.IsLoopSample(instruments[ci].Samples[0].LoopMode))
                    if (sampleData.Length > 0 &&
                        instruments[ci].Samples[0].LoopMode.Equals("Off", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        Utility.PutInt2InByteArray(modUtils.GetLoopValue(instruments[ci].Samples[0].LoopStart, ci), true, sampleInfo, offset);

                        offset += 2;

                        Utility.PutInt2InByteArray(modUtils.GetLoopValue(instruments[ci].Samples[0].LoopEnd - instruments[ci].Samples[0].LoopStart, ci), true, sampleInfo, offset);

                        offset += 2;
                    }
                    else
                    {
                        offset += 4;
                    }

                    ms4SampleData.Write(sampleData, 0, sampleData.Length);
                }
                else
                {
                    offset += 30;
                }
                
            }

            //xrnsReader.FreeResources();

            ms4SampleData.Seek(0, SeekOrigin.Begin);

            byte[] sampleChunkData = Utility.GetBytesFromStream(ms4SampleData, ms4SampleData.Length);

            allSampleData = new byte[sampleInfoSize + sampleChunkData.Length];

            Array.Copy(sampleInfo, 0, allSampleData, 0, sampleInfo.Length);

            Array.Copy(sampleChunkData, 0, allSampleData, sampleInfoSize, sampleChunkData.Length);

            return allSampleData;
        }

        private byte[] GetSongLengthData(int numPatterns)
        {
            byte[] data = new byte[2];

            const int byteFixed = 0x7F;

            int offset = 0;

            data[offset++] = (byte)numPatterns;
            data[offset] = byteFixed;

            return data;
        }

        private byte[] GetPatternSequenceData(byte[] patternOrderTable)
        {
            const int patternSeqLen = 128;

            byte[] data = new byte[patternSeqLen];

            int offset = 0;

            for (int i = 0; i < patternOrderTable.Length; i++)
            {
                data[offset++] = patternOrderTable[i];
            }

            return data;

        }


        private byte[] GetAllPatternsData(PatternData[] patternsData, InstrumentData[] instruments, 
            int numChannels, int numMasterTrackColumns, byte[] patternSequences)
        {
            int maxPatternNumberToStore = 0;

            foreach (int value in patternSequences)
                if (maxPatternNumberToStore < value) maxPatternNumberToStore = value;

            MemoryStream patternsStreamData = new MemoryStream();

            /*
            for (int i = 0; i <= maxPatternNumberToStore; i++)
            {
                OnReportProgress(new EventReportProgressArgs(String.Format("Processing pattern {0}/{1}", i, maxPatternNumberToStore)));

                byte[] data = GetPatternData(patternsData[i], numChannels, numMasterTrackColumns);

                patternsStreamData.Write(data, 0, data.Length);
            }
            */

            // patterns are processed following predefinied sequence in order to keep correct period values
            byte[] distinctPatternSequences = patternSequences.Distinct().ToArray();

            for (int i = 0; i <= maxPatternNumberToStore; i++)
            {
                int currentPattern = distinctPatternSequences[i];

                OnReportProgress(new EventReportProgressArgs(String.Format("Processing pattern {0}/{1}", currentPattern, maxPatternNumberToStore)));

                byte[] data = GetPatternData(patternsData[currentPattern], numChannels, numMasterTrackColumns);

                patternsStreamData.Seek(data.Length * distinctPatternSequences[i], SeekOrigin.Begin);

                patternsStreamData.Write(data, 0, data.Length);
            }


            return patternsStreamData.ToArray();
        }

        private byte[] GetPatternData(PatternData patternData, int numChannels, int numMasterTrackColumns)
        {
            const int numRows4Pattern = 64;
            const int totalData4EachTrackLine = 4;

            int maxTrackLinesInPattern = numRows4Pattern * numChannels;

            byte[] data = new byte[numRows4Pattern * totalData4EachTrackLine * numChannels];

            int cyclesToDo = patternData.NumRows * numChannels;

            if (cyclesToDo > maxTrackLinesInPattern) cyclesToDo = maxTrackLinesInPattern;

            int offset = 0;

            //int currentMasterTrackColumnToParse = 0;
            byte[] masterTrackCommand = new byte[2];
            bool isMasterTrackCommandUsed = false;

            // force number of mastertrack columns to parse at x value; any column beyond the x value will be ignored
            const int maxMasterTrackColumnToParse = 0;

            int numMasterTrackColumnsToParse = maxMasterTrackColumnToParse;

            // numMasterTrackColumns -- count of mt columns in module
            // numMasterTrackColumnsToParse -- count of mt columns to parse
            if (numMasterTrackColumnsToParse > numMasterTrackColumns)
                numMasterTrackColumnsToParse = numMasterTrackColumns;

            int currentMasterTrackIndex = 0;
            int masterTrackIndexLimitForCurrentRow = 0;

            for (int i = 0; i < cyclesToDo; i++)
            {
                modUtils.ComputeTickPerRowForCurrentLine(patternData.TracksLineData, i, numChannels);

                int currentRow = i / numChannels;
                int currentChannel = i % numChannels + 1;

                if (currentChannel == 1)
                {
                    if (isMasterTrackCommandUsed)
                    {
                        string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, "Some MasterTrack command were not used due to missing free command effects slots");
                        OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                    }
                    //currentMasterTrackColumnToParse = 0;
                    isMasterTrackCommandUsed = false;
                    currentMasterTrackIndex = currentRow * numMasterTrackColumns;
                    masterTrackIndexLimitForCurrentRow = currentMasterTrackIndex + numMasterTrackColumnsToParse;
                }

                while (currentMasterTrackIndex < masterTrackIndexLimitForCurrentRow && !isMasterTrackCommandUsed)
                {
                    MasterTrackLineData masterTrackLineData = patternData.MasterTrackLineData[currentMasterTrackIndex];

                    if (masterTrackLineData.IsSet)
                    {
                        masterTrackCommand = modUtils.GetCommandsFromMasterTrack(masterTrackLineData.EffectNumber, masterTrackLineData.EffectValue);

                        // test
                        //masterTrackCommand = new byte[] { 1, 5 };

                        if (masterTrackCommand[0] + masterTrackCommand[1] > 0)
                        {
                            isMasterTrackCommandUsed = true;
                            //break;
                        }
                    }

                    //currentMasterTrackColumnToParse++;
                    currentMasterTrackIndex++;
                }

                TrackLineData trackLineData = patternData.TracksLineData[i];

                offset = i * totalData4EachTrackLine;

                int sampleNumber = 0;
                int period = 0;
                int effectNum = 0;
                int effectVal = 0;

                // check if any effect command used on this row;
                // otherwise, tries to transpose some renoise volume commands to mod effect command
                bool isEffectCommandUsed = false;

                if (trackLineData.IsSet || isMasterTrackCommandUsed)
                {
                    if (trackLineData.Instrument != null)
                    {
                        sampleNumber = Int16.Parse(trackLineData.Instrument, System.Globalization.NumberStyles.HexNumber) + 1;
                    }

                    if (trackLineData.Note != null)
                    {
                        try
                        {
                            bool isTonePortamentoTriggered = false;

                            isTonePortamentoTriggered = modUtils.IsTonePortamentoTriggered(trackLineData.EffectNumber, trackLineData.Volume, trackLineData.Panning);                            

                            period = modUtils.GetModNote(trackLineData.Note, sampleNumber - 1, currentChannel - 1, isTonePortamentoTriggered);
                        }
                        catch (Exception e)
                        {                                                        
                            string errorMessage = string.Format("row {0}, instrument {1}, channel {2}: {3}", currentRow, (sampleNumber-1), currentChannel, e.Message);
                            OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                        }
                    }

                    // means for effect value too
                    if (trackLineData.EffectNumber != null)
                    {
                        try
                        {
                            byte[] values = modUtils.GetModEffect(trackLineData.EffectNumber, trackLineData.EffectValue, sampleNumber, currentChannel - 1, period != 0);
                            if ((values[0] + values[1]) > 0)
                            {
                                isEffectCommandUsed = true;
                                effectNum = values[0];
                                effectVal = values[1];
                            }
                        }
                        catch (ConversionException e)
                        {
                            string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, e.Message);
                            OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                        }
                    }

                    // check for any volume / panning command to transpose
                    if (isEffectCommandUsed == false)
                    {
                        // volume column
                        if (trackLineData.Volume != null)
                        {
                            try
                            {
                                byte[] values = modUtils.TransposeVolumeToCommandEffect(trackLineData.Volume, sampleNumber, currentChannel -1, period != 0);
                                if ((values[0] + values[1]) > 0)
                                {
                                    isEffectCommandUsed = true;
                                    effectNum = values[0];
                                    effectVal = values[1];
                                }
                            }
                            catch (ConversionException e)
                            {
                                string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, e.Message);
                                OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                            }
                        }
                        // delay column
                        if (isEffectCommandUsed == false && trackLineData.Delay != null)
                        {
                            try
                            {
                                byte[] values = modUtils.TransposeDelayToCommandEffect(trackLineData.Delay);
                                if ((values[0] + values[1]) > 0)
                                {
                                    isEffectCommandUsed = true;
                                    effectNum = values[0];
                                    effectVal = values[1];
                                }
                            }
                            catch (ConversionException e)
                            {
                                string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, e.Message);
                                OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                            }
                        }
                        // panning column
                        if (isEffectCommandUsed == false && trackLineData.Panning != null)
                        {
                            try
                            {
                                byte[] values = modUtils.TransposePanningToCommandEffect(trackLineData.Panning, sampleNumber, currentChannel - 1, period != 0);
                                if ((values[0] + values[1]) > 0)
                                {
                                    isEffectCommandUsed = true;
                                    effectNum = values[0];
                                    effectVal = values[1];
                                }
                            }
                            catch (ConversionException e)
                            {
                                string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, e.Message);
                                OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                            }
                        }                        
                        // apply global commands to the current effect column
                        if (isMasterTrackCommandUsed && !isEffectCommandUsed)
                        {
                            isEffectCommandUsed = true;
                            effectNum = masterTrackCommand[0];
                            effectVal = masterTrackCommand[1];

                            //currentMasterTrackColumnToParse++;
                            //currentMasterTrackIndex++;
                            isMasterTrackCommandUsed = false;
                        } 
                    }

                }

                data[offset] = (byte)((sampleNumber & 0xf0) + ((period & 0xf00) >> 8));
                data[offset + 1] = (byte)(System.Convert.ToByte(period & 0xff));
                data[offset + 2] = (byte)(System.Convert.ToByte(((sampleNumber & 0xf) << 4) + (effectNum)));
                data[offset + 3] = (byte)(effectVal);

            }

            return data;
        }

        private byte[] GetChannelsNumData(int numChannels)
        {
            byte[] data = new byte[4];

            char[] ncstr = new char[4];

            if (numChannels == 4)
            {
                ncstr = "M.K.".ToCharArray();
            }
            else
            {
                ncstr = ((numChannels.ToString() + "CHN")).ToCharArray();
            }
            byte[] buffer = Utility.GetBytesFromString(new string(ncstr, 0, ncstr.Length), 4);

            Array.Copy(buffer, 0, data, 0, buffer.Length);

            return data;
        }
    }
}
