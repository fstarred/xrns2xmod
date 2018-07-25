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
using Xrns2XMod.Properties;

namespace Xrns2XMod
{
    public class XMConverter : IConverter
    {
        XMUtils xmUtils;
        XrnsManager xrnsManager;


        public XmSettings Settings { get; set; }

        // An event that clients can use to be notified when status change
        public event ProgressHandler EventProgress;

        // Invoke the ReportProgress event; 
        protected virtual void OnReportProgress(EventReportProgressArgs e)
        {
            if (EventProgress != null)
                EventProgress(this, e);
        }

        public XMConverter(string srcFileName)
        {
            //this.inputFilename = srcFileName;
            xrnsManager = new XrnsManager(srcFileName);

        }

        //private string inputFilename;

        private const int maxChannels = 64;

        private const int maxSampleVolume = 64;

        /* write in order:
         * XM HEADER (Song properties)
         * PATTERNS (HEADER, DATA)
         * INSTRUMENTS (HEADER, SAMPLE HEADER, SAMPLE DATA)
         * 
        */
        public byte[] Convert(SongData songData)
        {
            MemoryStream songDataStream = new MemoryStream();

            int ticksPerRow = Settings.TicksRow;

            xmUtils = new XMUtils(songData, ticksPerRow);

            byte[] xmHeader = this.GetXMHeaderData(songData, ticksPerRow);

            byte[] instruments = this.GetAllInstrumentsData(songData.Instruments);

            byte[] patterns = this.GetAllPatternsData(songData.Patterns, songData.Instruments, songData.NumChannels, songData.NumMasterTracksColumns);

            songDataStream.Write(xmHeader, 0, xmHeader.Length);

            songDataStream.Write(patterns, 0, patterns.Length);

            songDataStream.Write(instruments, 0, instruments.Length);

            return songDataStream.ToArray();
        }


        public byte[] GetXMHeaderData(SongData songData, int ticksPerRow)
        {
            OnReportProgress(new EventReportProgressArgs("Processing XM Header"));

            int offset;

            // totale header size
            const int xmHeaderSize = 80 + 256;

            byte[] headerStream = new byte[xmHeaderSize];

            //string progName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            // string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            const string progName = "Xrns2XMod";

            const string idText = "Extended Module: ";
            string trackerName = progName; // + version

            trackerName = trackerName.Length > 20 ? trackerName.Substring(0, 20) : trackerName;

            // value stored on "Header size" part of XM Header, starts at offset 60; its value is given starting from the 
            // starting offset of 60 till the pattern sequence, which is a fixed value of 256
            const int headerSize = 80 - 60 + 256;

            const byte flags = 1;

            int restartPosition = songData.RestartPosition;

            int numPatterns = songData.Patterns.Length;

            int instrumentsLen = songData.Instruments.Length;

            offset = 0;

            // idText            
            Array.Copy(Utility.GetBytesFromString(idText, 17), 0, headerStream, offset, 17);

            offset = 17;

            // module name
            Array.Copy(Utility.GetBytesFromString(songData.Name, 20), 0, headerStream, offset, 20);

            offset = 37;

            // constant 0x1A
            headerStream[offset] = 0x1A;

            offset = 38;

            // tracker name
            Array.Copy(Utility.GetBytesFromString(trackerName, 20), 0, headerStream, offset, 20);

            offset = 58;

            // version number
            headerStream[offset++] = 4;
            headerStream[offset] = 1;

            offset = 60;

            Utility.PutInt4InByteArray(headerSize, headerStream, offset);

            offset = 64;

            Utility.PutInt2InByteArray(songData.PatternOrderTable.Length, headerStream, offset);

            offset = 66;

            Utility.PutInt2InByteArray(restartPosition, headerStream, offset);

            offset = 68;

            Utility.PutInt2InByteArray(songData.NumChannels, headerStream, offset);

            offset = 70;

            Utility.PutInt2InByteArray(songData.Patterns.Length, headerStream, offset);

            offset = 72;

            Utility.PutInt2InByteArray(songData.Instruments.Length, headerStream, offset);

            offset = 74;

            headerStream[offset] = flags;

            offset = 76;

            Utility.PutInt2InByteArray(ticksPerRow, headerStream, offset);

            offset = 78;

            //Util.PutInt2InByteArray(xmUtils.GetInitialTempo(settings.Tempo, songData.LinesPerBeat), headerStream, offset);

            Utility.PutInt2InByteArray(Settings.Tempo, headerStream, offset);

            offset = 80;

            Array.Copy(songData.PatternOrderTable, 0, headerStream, offset, songData.PatternOrderTable.Length);

            return headerStream;
        }

        private byte[] GetAllInstrumentsData(InstrumentData[] instrumentsData)
        {
            MemoryStream outputStream = new MemoryStream();

            //XrnsReaderUtil xrnsReader = new XrnsReaderUtil(srcFileName);

            for (int ci = 0; ci < instrumentsData.Length; ci++)
            {
                byte[] instrumentHeader = this.GetInstrumentHeaderData(instrumentsData[ci]);

                outputStream.Write(instrumentHeader, 0, instrumentHeader.Length);

                Stream[] encodedSample = new Stream[instrumentsData[ci].Samples.Length];

                for (int si = 0; si < instrumentsData[ci].Samples.Length; si++)
                {
                    OnReportProgress(new EventReportProgressArgs(String.Format("Processing instrument {0}/{1}, sample {2}/{3} ", (ci + 1), instrumentsData.Length, (si + 1), instrumentsData[ci].Samples.Length)));

                    byte[] sampleHeaderBuffer = new byte[0];

                    byte bps = 8;

                    int chans = 1;

                    int sampleRate = 0;

                    int sampleLength = 0;

                    int baseNote = 0;

                    int fineTune = 0;

                    try
                    {
                        //Stream originalSample = xrnsManager.GetSampleStream(ci, si);

                        SampleStreamInfo sampleStreamInfo = xrnsManager.GetSampleStreamInfo(ci, si);

                        // means sample is probably empty
                        if (sampleStreamInfo.Format != FORMAT.NONE)
                        {
                            int handle = BassWrapper.GetBassStream(sampleStreamInfo);
                            
                            BASS_CHANNELINFO bassChannelInfo = BassWrapper.GetBassChannelInfo(handle);

                            int origres = bassChannelInfo.origres;

                            if (origres == 0) // some streams were reported to return undefinied resolution
                            {
                                OnReportProgress(new EventReportProgressArgs("Sample bps detection failed, assuming 16 bits by default", MsgType.WARNING));
                                origres = 16;
                            }                                

                            long originalSampleLength = Bass.BASS_ChannelGetLength(handle);
                            
                            int mixer = BassWrapper.PlugChannelToMixer(handle, bassChannelInfo.freq, bassChannelInfo.chans, origres);

                            if (Settings.VolumeScalingMode == VOLUME_SCALING_MODE.SAMPLE && instrumentsData[ci].Samples[si].Volume != 1.0f)
                            {
                                OnReportProgress(new EventReportProgressArgs(String.Format("Ramping sample volume to value {0}", instrumentsData[ci].Samples[si].Volume)));
                                BassWrapper.AdjustSampleVolume(handle, mixer, instrumentsData[ci].Samples[si].Volume);
                            }

                            Stream stream = BassWrapper.GetXMEncodedSample(mixer, originalSampleLength, bassChannelInfo.chans, origres);

                            Bass.BASS_StreamFree(handle);

                            Bass.BASS_StreamFree(mixer);

                            encodedSample[si] = stream;

                            bps = (byte)(origres > 8 ? 16 : 8);

                            chans = bassChannelInfo.chans;

                            sampleRate = bassChannelInfo.freq;

                            sampleLength = (int)encodedSample[si].Length;

                            xmUtils.StoreSampleInfo(ci, si, sampleLength, sampleRate, chans, bps, instrumentsData[ci].Samples[si].RelNoteNumber, instrumentsData[ci].Samples[si].FineTune, instrumentsData[ci].Samples[si].Transpose);

                            baseNote = xmUtils.GetSampleBaseNote(ci, si);

                            fineTune = xmUtils.GetSampleFineTune(ci, si);
                            
                        }
                    }
                    catch (Exception e)
                    {
                        OnReportProgress(new EventReportProgressArgs(e.Message, MsgType.ERROR));

                        throw e;
                    }

                    sampleHeaderBuffer = GetSampleHeaderData(instrumentsData[ci].Samples[si], baseNote, fineTune,  sampleLength, bps, chans, sampleRate);

                    outputStream.Write(sampleHeaderBuffer, 0, sampleHeaderBuffer.Length);
                }

                for (int si = 0; si < encodedSample.Length; si++)
                {
                    if (encodedSample[si] != null)
                    {
                        encodedSample[si].Seek(0, SeekOrigin.Begin);

                        byte[] encodedSampleBuffer = Utility.GetBytesFromStream(encodedSample[si], encodedSample[si].Length);

                        outputStream.Write(encodedSampleBuffer, 0, encodedSampleBuffer.Length);
                    }
                }

            }

            //xrnsReader.FreeResources();

            return outputStream.ToArray();
        }



        private byte[] GetSampleHeaderData(SampleData sampleData, int baseNote, int fineTune,  int sampleLen, byte bitsPerSample, int chans, int sampleRate)
        {
            MemoryStream stream = new MemoryStream();

            BinaryWriter writer = new BinaryWriter(stream);

            const byte deltaPackedSample = 0;
            const int maxNameLen = 22;

            bool isStereo = chans > 1;

            //int fineTune = 0;
            //int relNoteNumber = 0;

            //ModCommons.GetRelNoteAndFTuneProperties(sampleData.RelNoteNumber, sampleData.FineTune, sampleRate, out relNoteNumber, out fineTune);

            int offset;

            offset = 0;
            writer.Seek(offset, SeekOrigin.Begin);

            writer.Write(Utility.MakeByte4FromInt(sampleLen));

            offset = 4;
            writer.Seek(offset, SeekOrigin.Begin);

            writer.Write(Utility.MakeByte4FromInt(XMUtils.GetSampleLoopValue(sampleData.LoopStart, bitsPerSample, isStereo)));

            offset = 8;
            writer.Seek(offset, SeekOrigin.Begin);
            // Loop End value is relative to LoopStart
            writer.Write(Utility.MakeByte4FromInt(XMUtils.GetSampleLoopValue((sampleData.LoopEnd - sampleData.LoopStart), bitsPerSample, isStereo)));

            offset = 12;
            writer.Seek(offset, SeekOrigin.Begin);

            writer.Write((byte)sampleData.DefaultVolume);

            offset = 13;
            writer.Seek(offset, SeekOrigin.Begin);

            writer.Write((sbyte)fineTune);

            offset = 14;
            writer.Seek(offset, SeekOrigin.Begin);

            byte loopMode = XMUtils.GetSampleLoopMode(sampleData.LoopMode);

            byte type = (byte)(loopMode + (bitsPerSample) + (isStereo ? 0x20 : 0));

            writer.Write(type);

            offset = 15;
            writer.Seek(offset, SeekOrigin.Begin);

            writer.Write(XMUtils.GetPanning(sampleData.Panning));

            offset = 16;
            writer.Seek(offset, SeekOrigin.Begin);

            writer.Write((sbyte)baseNote);

            //writer.Write((sbyte)XMUtil.GetRelNoteNumber(sampleData.RelNoteNumber, sampleRate, relNoteNumber, fineTune));

            offset = 17;
            writer.Seek(offset, SeekOrigin.Begin);

            writer.Write(deltaPackedSample);

            offset = 18;
            writer.Seek(offset, SeekOrigin.Begin);

            writer.Write(Utility.GetBytesFromString(sampleData.Name, maxNameLen));

            writer.Seek(0, SeekOrigin.Begin);

            return Utility.GetBytesFromStream(stream, stream.Length);
        }


        private byte[] GetInstrumentHeaderData(InstrumentData instrumentData)
        {
            //const int instrumentSize = 29;
            const int instrumentSize = 0x107;
            const int instrumentNameLen = 22;
            const byte instrumentType = 0;

            int numberOfSamples = instrumentData.Samples.Length;

            byte[] instrumentHeaderStream = new byte[instrumentSize];

            int offset;

            offset = 0;

            // instrument size
            Utility.PutInt4InByteArray(instrumentSize, instrumentHeaderStream, offset);

            offset = 4;

            Array.Copy(Utility.GetBytesFromString(instrumentData.Name, instrumentNameLen), 0, instrumentHeaderStream, offset, instrumentNameLen);

            offset = 26;

            instrumentHeaderStream[offset] = instrumentType;

            offset = 27;

            Utility.PutInt2InByteArray(numberOfSamples, instrumentHeaderStream, offset);

            if (numberOfSamples > 0)
            {
                const int sampleHeaderSize = 0x28;

                offset = 29;

                Utility.PutInt4InByteArray(sampleHeaderSize, instrumentHeaderStream, offset);

            }

            offset = 33;

            for (int i = 0; i < 96; i++)
            {
                instrumentHeaderStream[offset++] = (byte)instrumentData.KeyMap[i];
            }

            const int maxEnvPoints = 12;

            offset = 129;

            byte[] envVolumePoints = XMUtils.GetEnvelopePointsValue(instrumentData.EnvVolumePoints,
                instrumentData.VolumeSustainPoint, instrumentData.VolumeLoopStart, instrumentData.VolumeLoopEnd,
                instrumentData.VolumeSustainEnabled, instrumentData.VolumeLoopEnabled);

            int totalEnvVolumePoints = envVolumePoints.Length / 4;

            if (totalEnvVolumePoints > maxEnvPoints)
            {
                totalEnvVolumePoints = maxEnvPoints;
            }

            Array.Copy(envVolumePoints, 0, instrumentHeaderStream, offset, totalEnvVolumePoints * 4);

            offset = 177;

            byte[] envPanningPoints = XMUtils.GetEnvelopePointsValue(instrumentData.EnvPanningPoints,
                instrumentData.PanningSustainPoint, instrumentData.PanningLoopStart, instrumentData.PanningLoopEnd,
                instrumentData.PanningSustainEnabled, instrumentData.PanningLoopEnabled);

            int totalEnvPanningPoints = envPanningPoints.Length / 4;

            if (totalEnvPanningPoints > maxEnvPoints)
            {
                totalEnvPanningPoints = maxEnvPoints;
            }

            Array.Copy(envPanningPoints, 0, instrumentHeaderStream, offset, totalEnvPanningPoints * 4);

            offset = 225;

            instrumentHeaderStream[offset++] = (byte)(totalEnvVolumePoints);
            instrumentHeaderStream[offset++] = (byte)(totalEnvPanningPoints);

            offset = 227;
            instrumentHeaderStream[offset++] = XMUtils.GetPointNumber(envVolumePoints, instrumentData.VolumeSustainPoint);
            instrumentHeaderStream[offset++] = XMUtils.GetPointNumber(envVolumePoints, instrumentData.VolumeLoopStart);
            instrumentHeaderStream[offset++] = XMUtils.GetPointNumber(envVolumePoints, instrumentData.VolumeLoopEnd);
            instrumentHeaderStream[offset++] = XMUtils.GetPointNumber(envPanningPoints, instrumentData.PanningSustainPoint);
            instrumentHeaderStream[offset++] = XMUtils.GetPointNumber(envPanningPoints, instrumentData.PanningLoopStart);
            instrumentHeaderStream[offset++] = XMUtils.GetPointNumber(envPanningPoints, instrumentData.PanningLoopEnd);

            offset = 233;

            instrumentHeaderStream[offset++] = XMUtils.GetVolumePanningType(instrumentData.VolumeEnabled, instrumentData.VolumeSustainEnabled, instrumentData.VolumeLoopEnabled);
            instrumentHeaderStream[offset++] = XMUtils.GetVolumePanningType(instrumentData.PanningEnabled, instrumentData.PanningSustainEnabled, instrumentData.PanningLoopEnabled);

            offset = 239;

            Utility.PutInt2InByteArray(instrumentData.VolumeFadeOut, instrumentHeaderStream, offset);

            return instrumentHeaderStream;

        }


        private byte[] GetAllPatternsData(PatternData[] patternsData, InstrumentData[] instruments, int numChannels, int numMasterTrackColumns)
        {

            MemoryStream totalPatternDataStream = new MemoryStream();

            for (int i = 0; i < patternsData.Length; i++)
            {
                OnReportProgress(new EventReportProgressArgs(String.Format("Processing pattern {0}/{1}", (i + 1), patternsData.Length)));

                byte[] patternDataStream = GetPatternData(patternsData[i], instruments, numChannels, numMasterTrackColumns);

                byte[] patternHeaderStream = GetPatternHeaderData(patternDataStream.Length, patternsData[i].NumRows);

                totalPatternDataStream.Write(patternHeaderStream, 0, patternHeaderStream.Length);

                totalPatternDataStream.Write(patternDataStream, 0, patternDataStream.Length);
            }

            return totalPatternDataStream.ToArray();
        }


        private byte[] GetPatternHeaderData(int patternDataSize, int numRows)
        {
            int patternHeaderLength = 9;
            byte packingType = 0;
            int offset;

            byte[] patternHeader = new byte[patternHeaderLength];

            offset = 0;

            patternHeader[offset] = (byte)patternHeaderLength;

            offset = 4;

            patternHeader[offset] = packingType;

            offset = 5;

            Utility.PutInt2InByteArray(numRows, patternHeader, offset);

            offset = 7;

            Utility.PutInt2InByteArray(patternDataSize, patternHeader, offset);

            return patternHeader;
        }


        private byte[] GetPatternData(PatternData patternData, InstrumentData[] instruments, int numChannels, int numMasterTrackColumns)
        {
            byte noteBit = 1;
            byte instrumentBit = 2;
            byte volumeColBit = 4;
            byte effectTypeBit = 8;
            byte effectParamBit = 16;
            byte emptyBit = 128;
            byte allValuesFilledBit = (byte)(noteBit + instrumentBit + volumeColBit + effectTypeBit + effectParamBit + emptyBit);

            MemoryStream patternDataStream = new MemoryStream();

            // A Carl Corcoran idea, useful to know which is the last sample played by an x channel
            System.Collections.Generic.Dictionary<int, SampleData?> playingSamplesMap = new System.Collections.Generic.Dictionary<int, SampleData?>();
            
            byte[] masterTrackCommand = new byte[2];            
            bool isMasterTrackCommandUsed = false;

            // force number of mastertrack columns to parse at x value, therefore any column beyond the x value will be ignored
            // NOTE: if applied parseOnlyGlobalVolumeFromMT, other effect but Global Volume will be ignored
            const int maxMasterTrackColumnToParse = 1;

            int numMasterTrackColumnsToParse = maxMasterTrackColumnToParse;

            // numMasterTrackColumns -- count of MT columns in module
            // numMasterTrackColumnsToParse -- count of MT columns to parse
            if (numMasterTrackColumnsToParse > numMasterTrackColumns)
                numMasterTrackColumnsToParse = numMasterTrackColumns;

            // parse only global volume from MasterTrack
            const bool parseOnlyGlobalVolumeFromMT = false;

            int currentMasterTrackIndex = 0;
            int masterTrackIndexLimitForCurrentRow = 0;

            for (int i = 0; i < patternData.TracksLineData.Length; i++)
            {
                xmUtils.ComputeTickPerRowForCurrentLine(patternData.TracksLineData, i, numChannels);

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
                        masterTrackCommand = xmUtils.GetCommandsFromMasterTrack(masterTrackLineData.EffectNumber, masterTrackLineData.EffectValue, parseOnlyGlobalVolumeFromMT);
                        
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

                if (trackLineData.IsSet || isMasterTrackCommandUsed)
                {
                    byte compressionValue = emptyBit;
                    byte xmNote = 0;
                    byte xmInstrument = 0;
                    byte xmVolume = 0;
                    byte xmEffectNumber = 0;
                    byte xmEffectValue = 0;

                    bool isEffectCommandUsed = false;
                    bool isVolumeCommandUsed = false;
                    bool isPanningCommandUsed = false;

                    if (trackLineData.Note != null)
                    {
                        try
                        {
                            xmNote = XMUtils.GetXMNote(trackLineData.Note);
                            compressionValue = (byte)(compressionValue + noteBit);
                        }
                        catch (ConversionException e)
                        {                                                        
                            string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, e.Message);
                            OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                        }
                    }
                    if (trackLineData.Instrument != null)
                    {
                        compressionValue = (byte)(compressionValue + instrumentBit);
                        xmInstrument = (byte)(Int16.Parse(trackLineData.Instrument, System.Globalization.NumberStyles.HexNumber) + 1);
                        if (xmNote != 0)
                        {
                            int xmSample = xmUtils.GetPlayedSampleFromKeymap(xmNote, xmInstrument);                                                        
                            // figure out which sample will play for this.
                            if (instruments[xmInstrument - 1].Samples.Length > xmSample )
                            {
                                playingSamplesMap[currentChannel] = instruments[xmInstrument - 1].Samples[xmSample];
                            }
                            
                        }                        
                    }
                    
                    // the currently playing sample in the channel
                    SampleData? currentlyPlayingSample = null;
                    if (playingSamplesMap.ContainsKey(currentChannel))
                        currentlyPlayingSample = playingSamplesMap[currentChannel];

                    int sampleDefaultVolume = currentlyPlayingSample != null ? 
                        currentlyPlayingSample.Value.DefaultVolume : maxSampleVolume;

                    float sampleVolume = 1.0f;

                    if (currentlyPlayingSample != null)
                    {
                        sampleVolume = currentlyPlayingSample.Value.Volume;
                    }

                    if (trackLineData.EffectNumber != null)
                    {
                        try
                        {
                            byte[] values = xmUtils.GetXMEffect(trackLineData.EffectNumber, trackLineData.EffectValue, xmNote, xmInstrument);

                            if ((values[0] + values[1]) > 0)
                            {
                                isEffectCommandUsed = true;
                                xmEffectNumber = values[0];
                                xmEffectValue = values[1];
                            }
                        }
                        catch (ConversionException e)
                        {
                            string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, e.Message);
                            OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                        }
                    }

                    // volume column (volume column got priority before panning)
                    if (trackLineData.Volume != null)
                    {
                        xmVolume = xmUtils.GetVolumeColumnEffectFromVolume(trackLineData.Volume);
                        isVolumeCommandUsed = xmVolume > 0;

                        if (isVolumeCommandUsed == false && isEffectCommandUsed == false)
                        {
                            // transpose possible parseable command from volume to effect columns
                            // G|U|D|I|O|B|Q|R|Y|C
                            byte[] values = xmUtils.TransposeVolumeToCommandEffect(trackLineData.Volume);
                            if ((values[0] + values[1]) > 0)
                            {
                                isEffectCommandUsed = true;
                                xmEffectNumber = values[0];
                                xmEffectValue = values[1];
                            }
                        }
                    }

                    // only with VOLUME_SCALING_MODE = COLUMN
                    if (Settings.VolumeScalingMode == VOLUME_SCALING_MODE.COLUMN)
                    {
                        bool sampleNeedsVolumeScaling =
                            currentlyPlayingSample != null && currentlyPlayingSample.Value.Volume != 1.0;
                        bool doesTriggerSample = trackLineData.Note != null && trackLineData.Instrument != null;
                        
                        // if volume column command is used, then scale it
                        if (sampleNeedsVolumeScaling && isVolumeCommandUsed && XMExtras.IsVolumeSetOnVolumeColumn(xmVolume))
                        {
                            try
                            {
                                xmVolume = XMExtras.ScaleVolumeFromVolumeCommand(xmVolume, sampleVolume);
                            }
                            catch (ConversionException e)
                            {
                                string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, e.Message);
                                OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                            }
                            sampleNeedsVolumeScaling = false;
                        }

                        // if effect command is used, then scale it
                        if (sampleNeedsVolumeScaling && isEffectCommandUsed && XMExtras.IsVolumeSetOnEffectColumn(xmEffectNumber))
                        {
                            try
                            {
                                xmEffectValue = XMExtras.ScaleVolumeFromEffectCommand(xmEffectValue, sampleVolume);
                            }
                            catch (ConversionException e)
                            {
                                string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, e.Message);
                                OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                            }
                            sampleNeedsVolumeScaling = false;
                        }

                        // if sample is triggered and needs volume scaling, check for any free slot
                        if (sampleNeedsVolumeScaling && doesTriggerSample)
                        {
                            // the real sample volume is relative with the default volume
                            sampleVolume *= (float)currentlyPlayingSample.Value.DefaultVolume / (float)maxSampleVolume;

                            // try to fill on volume column first
                            if (isVolumeCommandUsed == false)
                            {
                                try
                                {
                                    xmVolume = XMExtras.ScaleVolumeFromVolumeCommand(sampleVolume);
                                    isVolumeCommandUsed = true;
                                }
                                catch (ConversionException e)
                                {
                                    string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, e.Message);
                                    OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                                }
                                sampleNeedsVolumeScaling = false;
                            }
                            
                            // try to fill on effect column
                            if (sampleNeedsVolumeScaling && isEffectCommandUsed == false)
                            {
                                byte[] values = new byte[2];

                                // transpose possible parseable command from volume to effect columns
                                // G|U|D|J|K|Q|B|R|Y|C
                                try
                                {
                                    values = XMExtras.ScaleVolumeFromEffectCommand(sampleVolume);
                                }
                                catch (Exception e)
                                {
                                    string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, e.Message);
                                    OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                                }
                                if ((values[0] + values[1]) > 0)
                                {
                                    isEffectCommandUsed = true;
                                    xmEffectNumber = values[0];
                                    xmEffectValue = values[1];
                                }
                                sampleNeedsVolumeScaling = false;
                            }
                        }

                        // if still sample needs scaling a conversion error is thrown
                        if (sampleNeedsVolumeScaling)
                        {
                            // no empty slot free, a log error conversion is thrown
                            string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, "Cannot apply scaled volume for this channel due to missing free slots");
                            OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                        }

                    }

                    // delay column
                    if (trackLineData.Delay != null)
                    {
                        if (isEffectCommandUsed == false)
                        {
                            byte[] values = xmUtils.TransposeDelayToCommandEffect(trackLineData.Delay);
                            isEffectCommandUsed = true;
                            xmEffectNumber = values[0];
                            xmEffectValue = values[1];
                        }
                        else
                        {
                            // no empty slot free, a log error conversion is thrown
                            string errorMessage = string.Format("row {0}, channel {1}: {2}", currentRow, currentChannel, "Cannot apply delay for this channel due to missing free slots");
                            OnReportProgress(new EventReportProgressArgs(errorMessage, MsgType.ERROR));
                        }
                    }

                    // panning column
                    if (trackLineData.Panning != null)
                    {
                        if (isVolumeCommandUsed == false)
                        {
                            xmVolume = xmUtils.GetVolumeColumnEffectFromPanning(trackLineData.Panning);
                            isPanningCommandUsed = xmVolume > 0;
                        }
                        if (isPanningCommandUsed == false && isEffectCommandUsed == false)
                        {
                            byte[] values = xmUtils.TransposePanningToCommandEffect(trackLineData.Panning);
                            if ((values[0] + values[1]) > 0)
                            {
                                isEffectCommandUsed = true;
                                xmEffectNumber = values[0];
                                xmEffectValue = values[1];
                            }
                        }
                    }
                    
                    // apply global commands to the current effect column
                    if (isMasterTrackCommandUsed && !isEffectCommandUsed)
                    {
                        isEffectCommandUsed = true;                        
                        xmEffectNumber = masterTrackCommand[0];
                        xmEffectValue = masterTrackCommand[1];
                        
                        //currentMasterTrackColumnToParse++;                        
                        isMasterTrackCommandUsed = false;
                    } 
                    
                    // xm volume column binary switch                    
                    if (isPanningCommandUsed || isVolumeCommandUsed)                    
                        compressionValue = (byte)(compressionValue + volumeColBit);                    
                    if (xmEffectNumber > 0) 
                        compressionValue = (byte)(compressionValue + effectTypeBit);
                    if (xmEffectValue > 0) 
                        compressionValue = (byte)(compressionValue + effectParamBit);

                    // this might require a little explanation.
                    // row/track data, wherever is not completely filled with note, instrument, volume col, 
                    // effect type, effect value 
                    // is packaged in this way: 
                    // the first byte means the type of values, and from second the values to put.                   
                    // an empty row/track data byte is byte value 128 
                    // the values order, starting from the less significant bit are
                    // 
                    // 1        1           1           1           1
                    // note     instrument  volume col  effect type effect value
                    // 
                    // so, for example, a value of 26 in bit is 11010 that means note, instrument and effect type filled.
                    // Therefore the first byte will be 154 (128 + 26)
                    // See the specs for a better idea

                    // writes the package byte only if not all values are valorized                    
                    if (compressionValue != allValuesFilledBit)
                    {
                        patternDataStream.WriteByte(compressionValue);
                    }
                    // checks for every bit of package byte to understand which values has to store in
                    if (((compressionValue & 0x1)) > 0)
                    {
                        patternDataStream.WriteByte(xmNote);
                    }
                    if (((compressionValue & 0x3) >> 1) > 0)
                    {
                        patternDataStream.WriteByte(xmInstrument);
                    }
                    if (((compressionValue & 0x7) >> 2) > 0)
                    {
                        patternDataStream.WriteByte(xmVolume);
                    }
                    if (((compressionValue & 0xf) >> 3) > 0)
                    {
                        patternDataStream.WriteByte(xmEffectNumber);
                    }
                    if (((compressionValue & 0x1f) >> 4) > 0)
                    {
                        patternDataStream.WriteByte(xmEffectValue);
                    }

                }
                else
                {
                    patternDataStream.WriteByte(emptyBit);
                }
            }

            return patternDataStream.ToArray();

        }

    }
}
