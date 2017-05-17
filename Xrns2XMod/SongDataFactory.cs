using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Xrns2XMod.Extensions;

namespace Xrns2XMod
{
    public class SongDataFactory
    {
        public event ProgressHandler ReportProgress;

        public static readonly int[] COMPATIBILITY_SCHEMA_LIST = new int[] { 54, 63 };

        // Invoke the ReportProgress event; 
        protected void OnReportProgress(EventReportProgressArgs e)
        {
            if (ReportProgress != null)
                ReportProgress(null, e);
        }


        private int GetMasterTracksTotalColumns(SequencerMasterTrack[] sequencerMasterTrack)
        {
            return sequencerMasterTrack[0].NumberOfVisibleEffectColumns;
        }

        public RenoiseSong ExtractRenoiseSong(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

            ZipFile zipFile = new ZipFile(fs);

            ZipEntry zipEntry = zipFile.GetEntry("Song.xml");

            StreamReader songStream = new StreamReader(zipFile.GetInputStream(zipEntry));

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RenoiseSong));

            RenoiseSong renoiseSong = (RenoiseSong)xmlSerializer.Deserialize(songStream);

            try
            {
                int docVersion = renoiseSong.doc_version;

                if (Array.BinarySearch(COMPATIBILITY_SCHEMA_LIST, docVersion) < 0)
                    throw new XrnsException("Song version is not supported by this Xrns2XMod version. Try to save it with a " +
                            "renoise compatible version (check the homepage for more info)");
            }
            finally
            {
                if (fs != null)
                    fs.Close();

                if (songStream != null)
                    songStream.Close();

                if (zipFile != null)
                    zipFile.Close();
            }

            return renoiseSong;
        }

        private int GetNumChannels(SequencerTrack[] sequencerTracks)
        {
            return sequencerTracks.Sum((o) => o.NumberOfVisibleNoteColumns);
        }

        public SongData ExtractSongData(RenoiseSong renoiseSong, string filename)
        {
            SongData songData = new SongData();

            //int[] tracksPositionMap = GenerateChannelsPositionMapAccordingToSubColumns(renoiseSong.Tracks.SequencerTrack);
            int masterTracksTotalColumns = GetMasterTracksTotalColumns(renoiseSong.Tracks.SequencerMasterTrack);

            songData.Name = renoiseSong.GlobalSongData.SongName;
            songData.LinesPerBeat = renoiseSong.GlobalSongData.LinesPerBeat;
            songData.SampleOffsetCompatibilityMode = renoiseSong.GlobalSongData.SampleOffsetCompatibilityMode;
            songData.PitchCompatibilityMode = renoiseSong.GlobalSongData.PitchEffectsCompatibilityMode;
            songData.PlaybackEngineVersion = renoiseSong.GlobalSongData.PlaybackEngineVersion;
            songData.TicksPerLine = renoiseSong.GlobalSongData.TicksPerLine;
            songData.NumChannels = GetNumChannels(renoiseSong.Tracks.SequencerTrack);
            songData.NumInstruments = renoiseSong.Instruments.Instrument.Length;
            songData.NumMasterTracksColumns = masterTracksTotalColumns;
            songData.InitialBPM = Convert.ToInt16(renoiseSong.GlobalSongData.BeatsPerMin);
            songData.LinesPerBeat = renoiseSong.GlobalSongData.LinesPerBeat;
            songData.RestartPosition = renoiseSong.PatternSequence.LoopSelection.CursorPos;
            songData.PatternOrderTable = GetPatternOrderTable(renoiseSong.PatternSequence.SequenceEntries.SequenceEntry);

            songData.Instruments = GetInstrumentsData(renoiseSong.Instruments.Instrument);
            songData.Patterns = GetPatternData(renoiseSong.PatternPool.Patterns.Pattern, renoiseSong.Tracks.SequencerTrack, masterTracksTotalColumns);

            LoadFromIni(songData, filename);

            return songData;
        }

        private static int GetPatternIndex(int columnCount, int row, int column)
        {
            return column + (row * columnCount);
        }
        private static TrackLineData GetPatternCell(IEnumerable<TrackLineData> pattern, int columnCount, int row, int column)
        {
            return pattern.ElementAt(GetPatternIndex(columnCount, row, column));
        }
        private static void SetPatternCell(List<TrackLineData> pattern, int columnCount, int row, int column, TrackLineData val)
        {
            pattern[GetPatternIndex(columnCount, row, column)] = val;
        }

        //private ChannelProperties[] GetChannelProperties(RenoiseSongTracks tracks, int numChannels)
        //{

        //    var emptyChannelProperties = new ChannelProperties();
        //    emptyChannelProperties.gainRatio = 1.0f;
        //    emptyChannelProperties.panning = 0.5f;
        //    IList<ChannelProperties> channels = Enumerable.Repeat(emptyChannelProperties, numChannels).ToList();

        //    for (int i = 0; i < tracks.SequencerTrack.Length; ++i)
        //    {
        //        var track = tracks.SequencerTrack[i];
        //        System.Diagnostics.Debug.WriteLine(string.Format("Seq Track: {0} / {1}", i, track.Name));
        //    }

        //    for (int i = 0; i < channels.Count; ++i)
        //    {
        //        var x = channels[i];
        //        System.Diagnostics.Debug.WriteLine(string.Format("V1 channel property: {0} gain {1} pan {2}", i, x.gainRatio, x.panning));
        //    }

        //    // ok let's try to fake delay effects by creating shifted patterns.
        //    for (int ixrnsSeqTrack = tracks.SequencerTrack.Length - 1; ixrnsSeqTrack >= 0; --ixrnsSeqTrack)// go in reverse order so the column indices always match
        //    {
        //        var xrnsTrack = tracks.SequencerTrack[ixrnsSeqTrack];
        //        if (xrnsTrack.FilterDevices.Devices.DelayDevice == null)
        //            continue;
        //        if (xrnsTrack.FilterDevices.Devices.DelayDevice.Length < 1)
        //            continue;
        //        var delayDevice = xrnsTrack.FilterDevices.Devices.DelayDevice[0];
        //        if (!Utility.IsNearlyEqual(delayDevice.IsActive.Value, 1.0, 0.05))
        //            continue;
        //        if (!Utility.IsNearlyEqual(delayDevice.LineSync.Value, 1.0, 0.05))// only care when it's line-sync'd.
        //            continue;
        //        int lineDelay = (int)delayDevice.LSyncTime.Value;
        //        float attenuationRatio = delayDevice.TrackSend.Value / 127.0f;// TrackSend is from 0-127, representing -inf to 0.0db.
        //        float panning = (delayDevice.LTapPan.Value + delayDevice.RTapPan.Value) / 2;// 0 = left, 1 = right

        //        for (int ixrnsNoteColumn = xrnsTrack.NumberOfVisibleNoteColumns - 1; ixrnsNoteColumn >= 0; --ixrnsNoteColumn)
        //        {
        //            System.Diagnostics.Debug.WriteLine(string.Format("Applying delay to track {4}, notecol:{5} / {0} linedelay {1} gain {2} pan {3}", ixrnsSeqTrack, xrnsTrack.Name, lineDelay, attenuationRatio, panning, ixrnsNoteColumn));

        //            int oldColumnIndex = tracks.SequencerTrack.Take(ixrnsSeqTrack).Sum(o => o.NumberOfVisibleNoteColumns);
        //            int newColumnIndex = oldColumnIndex + 1;

        //            // generate a new track!
        //            List<TrackLineData> carryoverEntries = new List<TrackLineData>();
        //            for (int pi = 0; pi < songData.Patterns.Length; ++pi)
        //            {
        //                var pattern = songData.Patterns[pi];
        //                List<TrackLineData> newPatternData = new List<TrackLineData>();
        //                int newColumnCount = numChannels + 1;
        //                // it's column-major. so indices to tracksLineData are like this:
        //                // |  0  |  1  |  2  |
        //                // |-----+-----+-----+
        //                // |  3  |  4  |  5  |
        //                // so to add a new column of data, just insert a blank cell right next to the item. simplest algorithm is to copy items one-by-one, inserting when necessary.
        //                for (int iold = 0; iold < pattern.TracksLineData.Length; ++iold)
        //                {
        //                    var entry = pattern.TracksLineData[iold];
        //                    int row = iold / numChannels;
        //                    int column = iold - (row * numChannels);
        //                    newPatternData.Add(entry);
        //                    if (column == oldColumnIndex)
        //                    {
        //                        newPatternData.Add(new TrackLineData());
        //                    }
        //                }

        //                // the new column index is always oldindex+1.
        //                // the column has been added to this pattern so copy items
        //                for (int oldrow = 0; oldrow < pattern.NumRows - lineDelay; ++oldrow)
        //                {
        //                    int newRow = oldrow + lineDelay;
        //                    TrackLineData newEntry = GetPatternCell(newPatternData, newColumnCount, oldrow, oldColumnIndex);
        //                    SetPatternCell(newPatternData, newColumnCount, newRow, newColumnIndex, newEntry);
        //                }

        //                // copy previous carryover
        //                for (int icarryover = 0; icarryover < carryoverEntries.Count; ++icarryover)
        //                {
        //                    SetPatternCell(newPatternData, newColumnCount, icarryover, newColumnIndex, carryoverEntries[icarryover]);
        //                }

        //                // and populate new carryover
        //                carryoverEntries = new List<TrackLineData>(lineDelay);

        //                for (int ioldrow = pattern.NumRows - lineDelay; ioldrow < pattern.NumRows; ++ioldrow)
        //                {
        //                    carryoverEntries.Add(GetPatternCell(newPatternData, newColumnCount, ioldrow, oldColumnIndex));
        //                }


        //                // save our work!
        //                songData.Patterns[pi].TracksLineData = newPatternData.ToArray();

        //            }// iterate patterns

        //            var newChannelProperty = channels[oldColumnIndex];
        //            newChannelProperty.gainRatio *= attenuationRatio;// you can just multiply ratios and everything is safe.
        //            newChannelProperty.panning = panning;// no need to take into consideration the old panning. that would actually make it impossible to pan past 50% L or R.
        //            channels.Insert(newColumnIndex, newChannelProperty);
        //            songData.NumChannels++;
        //        }
        //    }


        //    for (int i = 0; i < songData.ChannelProperties.Count; ++i)
        //    {
        //        var x = songData.ChannelProperties[i];
        //        System.Diagnostics.Debug.WriteLine(string.Format("Final channel property: {0} gain {1} pan {2}", i, x.gainRatio, x.panning));
        //    }
        //    return songData;

        //}

        private void LoadFromIni(SongData songData, string inputFile)
        {
            IniWrapper iniWrapper = new IniWrapper(inputFile, false);

            if (iniWrapper.IsIniLoad)
            {
                OnReportProgress(new EventReportProgressArgs("loading song configuration...", MsgType.INFO));
                for (int ci = 0; ci < songData.NumInstruments; ci++)
                {
                    for (int si = 0; si < songData.Instruments[ci].Samples.Length; si++)
                    {
                        songData.Instruments[ci].Samples[si].DefaultVolume = (byte)iniWrapper.ReadDefaultVolumeSample(ci, si);
                        songData.Instruments[ci].Samples[si].SampleFreq = iniWrapper.ReadFreqSample(ci, si);
                    }
                }
            }

        }



        private byte[] GetPatternOrderTable(PatternSequenceEntry[] sequenceEntry)
        {
            byte[] patternSequence = new byte[sequenceEntry.Length];

            for (int i = 0; i < sequenceEntry.Length; i++)
            {
                patternSequence[i] = (byte)sequenceEntry[i].Pattern;
            }

            return patternSequence;
        }

        private InstrumentData[] GetInstrumentsData(RenoiseInstrument[] renoiseInstruments)
        {
            InstrumentData[] instrumentsData = new InstrumentData[renoiseInstruments.Length];

            for (int ci = 0; ci < instrumentsData.Length; ci++)
            {
                instrumentsData[ci].Name = Utility.NullToString(renoiseInstruments[ci].Name);

                SampleModulationSet sms = renoiseInstruments[ci].SampleGenerator.ModulationSets.ModulationSet[0];

                if (sms.Devices != null)
                {
                    if (sms.Devices.SampleCompatibilityModulationDevice != null)
                    {
                        foreach (SampleCompatibilityModulationDevice scmd in sms.Devices.SampleCompatibilityModulationDevice)
                        {
                            switch (scmd.Target)
                            {
                                case SampleCompatibilityModulationDeviceTarget.Volume:
                                    // volume         

                                    instrumentsData[ci].VolumeEnabled = Convert.ToBoolean(scmd.IsActive.Value);
                                    instrumentsData[ci].EnvVolumePoints = scmd.EnvelopeNodes.Points;
                                    instrumentsData[ci].VolumeFadeOut = (int)scmd.EnvelopeDecay.Value;

                                    instrumentsData[ci].VolumeSustainEnabled = scmd.EnvelopeSustainIsActive;

                                    instrumentsData[ci].VolumeSustainPoint = scmd.EnvelopeSustainPos;

                                    instrumentsData[ci].VolumeLoopEnabled = scmd.EnvelopeLoopMode != SampleCompatibilityModulationDeviceEnvelopeLoopMode.Off;

                                    instrumentsData[ci].VolumeLoopStart = scmd.EnvelopeLoopStart;
                                    instrumentsData[ci].VolumeLoopEnd = scmd.EnvelopeLoopEnd;

                                    break;
                                case SampleCompatibilityModulationDeviceTarget.Panning:

                                    //panning
                                    instrumentsData[ci].PanningEnabled = Convert.ToBoolean(scmd.IsActive.Value);

                                    instrumentsData[ci].EnvPanningPoints = scmd.EnvelopeNodes.Points;
                                    instrumentsData[ci].PanningLoopStart = scmd.EnvelopeLoopStart;
                                    instrumentsData[ci].PanningLoopEnd = scmd.EnvelopeLoopEnd;

                                    instrumentsData[ci].PanningSustainEnabled = scmd.EnvelopeSustainIsActive;

                                    instrumentsData[ci].PanningSustainPoint = scmd.EnvelopeSustainPos;

                                    instrumentsData[ci].PanningLoopEnabled = scmd.EnvelopeLoopMode != SampleCompatibilityModulationDeviceEnvelopeLoopMode.Off;

                                    break;
                            }
                        }
                    }
                    else if (sms.Devices.SampleEnvelopeModulationDevice != null)
                    {
                        const double DeltaValue = 10.665;

                        foreach (SampleEnvelopeModulationDevice semd in sms.Devices.SampleEnvelopeModulationDevice)
                        {
                            switch (semd.Target)
                            {
                                case SampleEnvelopeModulationDeviceTarget.Volume:
                                    // volume         

                                    instrumentsData[ci].VolumeEnabled = Convert.ToBoolean(semd.IsActive.Value);

                                    if (semd.Nodes.Points != null)
                                        instrumentsData[ci].EnvVolumePoints = semd.Nodes.Points.Select((x) => ((int)Math.Round(Int32.Parse(x.Split(',')[0]) / DeltaValue)).ToString() + ',' + x.Split(',')[1]).ToArray<string>();

                                    instrumentsData[ci].VolumeSustainEnabled = semd.SustainIsActive;
                                    instrumentsData[ci].VolumeSustainPoint = (int)(semd.SustainPos / DeltaValue);
                                    instrumentsData[ci].VolumeLoopEnabled = semd.LoopMode != SampleEnvelopeModulationDeviceLoopMode.Off;
                                    instrumentsData[ci].VolumeLoopStart = (int)(semd.LoopStart / DeltaValue);
                                    instrumentsData[ci].VolumeLoopEnd = (int)(semd.LoopEnd / DeltaValue);

                                    instrumentsData[ci].VolumeFadeOut = (int)semd.Decay.Value;

                                    break;
                                case SampleEnvelopeModulationDeviceTarget.Panning:

                                    //panning
                                    instrumentsData[ci].PanningEnabled = Convert.ToBoolean(semd.IsActive.Value);

                                    if (semd.Nodes.Points != null)
                                        instrumentsData[ci].EnvPanningPoints = semd.Nodes.Points.Select((x) => ((int)Math.Round(Int32.Parse(x.Split(',')[0]) / DeltaValue)).ToString() + ',' + x.Split(',')[1]).ToArray<string>();

                                    instrumentsData[ci].PanningSustainEnabled = semd.SustainIsActive;
                                    instrumentsData[ci].PanningSustainPoint = (int)(semd.SustainPos / DeltaValue);
                                    instrumentsData[ci].PanningLoopEnabled = semd.LoopMode != SampleEnvelopeModulationDeviceLoopMode.Off;
                                    instrumentsData[ci].PanningLoopStart = (int)(semd.LoopStart / DeltaValue);
                                    instrumentsData[ci].PanningLoopEnd = (int)(semd.LoopEnd / DeltaValue);

                                    break;
                            }
                        }
                    }

                }

                // keymap - only noteOn are taken
                instrumentsData[ci].KeyMap = new int[120];
                int sampleIndex = 0;

                bool sampleExists = renoiseInstruments[ci].SampleGenerator.Samples != null;

                if (sampleExists)
                {
                    // keymap
                    foreach (Sample noteMap in renoiseInstruments[ci].SampleGenerator.Samples.Sample)
                    {
                        for (int i = noteMap.Mapping.NoteStart; i <= noteMap.Mapping.NoteEnd; i++)
                            instrumentsData[ci].KeyMap[i] = sampleIndex;
                        sampleIndex++;
                    }

                    instrumentsData[ci].Samples = GetSamplesData(renoiseInstruments[ci].SampleGenerator.Samples.Sample);
                }
                else
                {
                    // if instrument has no sample, create an empty array
                    instrumentsData[ci].Samples = new SampleData[0];
                }

            }

            return instrumentsData;
        }

        private SampleData[] GetSamplesData(Sample[] renoiseSamples)
        {
            const int DefaultSampleVolume = 64;

            SampleData[] samples = new SampleData[renoiseSamples.Length];

            for (int si = 0; si < samples.Length; si++)
            {
                samples[si].Name = renoiseSamples[si].Name;
                samples[si].Volume = Math.Abs(renoiseSamples[si].Volume);
                samples[si].LoopStart = renoiseSamples[si].LoopStart;
                samples[si].LoopEnd = renoiseSamples[si].LoopEnd;
                samples[si].Panning = renoiseSamples[si].Panning;
                samples[si].FineTune = renoiseSamples[si].Finetune;
                samples[si].LoopMode = renoiseSamples[si].LoopMode.ToString();
                samples[si].Transpose = renoiseSamples[si].Transpose;
                samples[si].RelNoteNumber = (sbyte)renoiseSamples[si].Mapping.BaseNote;
                samples[si].DefaultVolume = DefaultSampleVolume;
            }


            //private ChannelProperties

            // SampleSplitMapping might be not of the same length of InstrumentSample
            // due to the different handling of keymaps, xrns SHOULD NOT USE same samples with different basenote

            //bool warningKeyMapFound = false;

            //for (int si = 0; si < noteOnMappings.Length; si++)
            //{
            //    //int sampleIndex = noteOnMappings[si].SampleIndex;
            //    int sampleIndex = si;
            //    if (samples[sampleIndex].RelNoteNumber == 0)
            //        samples[sampleIndex].RelNoteNumber = (sbyte)noteOnMappings[si].BaseNote;
            //    else if (noteOnMappings[si].BaseNote != samples[sampleIndex].RelNoteNumber)
            //        warningKeyMapFound = true;                    
            //}

            //if (warningKeyMapFound)
            //    OnReportProgress(new EventReportProgressArgs("Found Keymaps with same samples and different basenote", MsgType.WARNING));

            return samples;
        }

        private PatternData[] GetPatternData(Pattern[] renoisePatterns, SequencerTrack[] sequencerTracks, int numMasterTrackColumns)
        {
            PatternData[] patternsData = new PatternData[renoisePatterns.Length];

            for (int cp = 0; cp < renoisePatterns.Length; cp++)
            {
                TrackLineData[] tracksLineData = GetLineTrackDataFromPattern(renoisePatterns[cp], sequencerTracks);

                MasterTrackLineData[] masterTracksLineData = GetMasterTrackDataFromPattern(renoisePatterns[cp], numMasterTrackColumns);

                patternsData[cp].TracksLineData = tracksLineData;

                patternsData[cp].MasterTrackLineData = masterTracksLineData;

                patternsData[cp].NumRows = renoisePatterns[cp].NumberOfLines;

            }

            return patternsData;

        }

        private MasterTrackLineData[] GetMasterTrackDataFromPattern(Pattern renoisePattern, int numColumns)
        {
            int numRows = renoisePattern.NumberOfLines;
            int totalMasterTracksData = numRows * numColumns;

            MasterTrackLineData[] masterTrackStructsData = new MasterTrackLineData[totalMasterTracksData];

            if (renoisePattern.Tracks.PatternMasterTrack[0].Lines != null)
            {
                for (int cl = 0; cl < renoisePattern.Tracks.PatternMasterTrack[0].Lines.Length; cl++)
                {
                    for (int cc = 0; cc < renoisePattern.Tracks.PatternMasterTrack[0].Lines[cl].EffectColumns.EffectColumn.Length; cc++)
                    {
                        // row index starts from 0
                        int row = renoisePattern.Tracks.PatternMasterTrack[0].Lines[cl].index;

                        if (row >= numRows) continue;

                        string effectnumber = null;
                        string effectvalue = null;

                        effectnumber = renoisePattern.Tracks.PatternMasterTrack[0].Lines[cl].EffectColumns.EffectColumn[cc].Number;
                        effectvalue = renoisePattern.Tracks.PatternMasterTrack[0].Lines[cl].EffectColumns.EffectColumn[cc].Value;

                        if ((effectnumber == null && effectvalue == null) == false)
                        {
                            int posInPattern = (numColumns * row) + cc;

                            masterTrackStructsData[posInPattern].IsSet = true;

                            masterTrackStructsData[posInPattern].EffectNumber = ConvertNullEffectToZero(effectnumber);
                            masterTrackStructsData[posInPattern].EffectValue = ConvertNullEffectToZero(effectvalue);
                        }
                    }
                }
            }



            return masterTrackStructsData;


        }

        // Renoise 3.0
        private string ConvertNullEffectToZero(string value)
        {
            return value ?? "00";
        }

        /*
         *  Store all the row/track data in an array with size = (pattern.rows * numChannels)
         * */
        private TrackLineData[] GetLineTrackDataFromPattern(Pattern renoisePattern, SequencerTrack[] sequencerTracks)
        {
            int numRows = renoisePattern.NumberOfLines;
            int numChannels = GetNumChannels(sequencerTracks);
            int patternDataSize = numRows * numChannels;

            TrackLineData[] patternStructsData = new TrackLineData[patternDataSize];

            // this is the channel index based to all renoise columns for track            
            int channelIndex = 0;

            // iterate tracks
            foreach (var patternTrackIterator in renoisePattern.Tracks.PatternTrack.WithIndex())
            {
                int track = patternTrackIterator.Index;
                PatternTrack patternTrack = patternTrackIterator.Value;
                int numVisibleColumnsTrack = sequencerTracks[track].NumberOfVisibleNoteColumns;
                int maxVisibleColumnIndex = numVisibleColumnsTrack - 1;

                if (patternTrack.Lines != null)
                {
                    // iterate all lines (and subcolumns) for current track
                    foreach (PatternLineNode line in patternTrack.Lines)
                    {
                        // row index starts from 0
                        int row = line.index;

                        if (row >= numRows) 
                            continue;

                        // note columns section
                        if (line.NoteColumns != null)
                        {                            
                            // iterate note columns for current line
                            foreach (var noteColumnIterator in line.NoteColumns.NoteColumn.WithIndex())
                            {
                                int column = noteColumnIterator.Index;

                                if (column > maxVisibleColumnIndex)
                                    continue;

                                PatternLineNoteColumnNode noteColumn = noteColumnIterator.Value;

                                string note = noteColumn.Note;
                                string instrument = noteColumn.Instrument;
                                string volume = noteColumn.Volume;
                                string panning = noteColumn.Panning;
                                string delay = noteColumn.Delay;

                                int trackAssigned = channelIndex + column;
                                int posInPattern = (row * numChannels) + trackAssigned;

                                if (note != null || instrument != null || volume != null || panning != null || delay != null)
                                {
                                    patternStructsData[posInPattern].IsSet = true;

                                    patternStructsData[posInPattern].Track = trackAssigned;
                                    patternStructsData[posInPattern].Row = row;
                                    patternStructsData[posInPattern].Note = note;
                                    patternStructsData[posInPattern].Instrument = instrument;
                                    patternStructsData[posInPattern].Volume = volume;
                                    patternStructsData[posInPattern].Panning = panning;
                                    patternStructsData[posInPattern].Delay = delay;

                                    System.Diagnostics.Debug.WriteLine(String.Format(
                                        "row: {0} column: {1} track: {2} note: {3} volume {4} panning {5} delay {6}", 
                                            row, column, track, note, volume, panning, delay)
                                    );

                                }
                            }
                        }

                        // effect columns section
                        if (line.EffectColumns != null)
                        {
                            // iterate effect columns for current line
                            foreach (var noteColumnIterator in line.EffectColumns.EffectColumn.WithIndex())
                            {
                                int column = noteColumnIterator.Index;

                                if (column > 0 || column > maxVisibleColumnIndex)
                                    continue;

                                PatternLineEffectColumnNode noteColumn = noteColumnIterator.Value;

                                string effectnumber = noteColumn.Number;
                                string effectvalue = noteColumn.Value;

                                int trackAssigned = channelIndex;

                                if (effectnumber != null || effectvalue != null)
                                {
                                    // spreads column effects for all the track sub-columns
                                    for (int iCol = 0; iCol < numVisibleColumnsTrack; iCol++)
                                    {
                                        int posInPattern = (row * numChannels) + trackAssigned + iCol;

                                        patternStructsData[posInPattern].IsSet = true;

                                        patternStructsData[posInPattern].Track = trackAssigned;
                                        patternStructsData[posInPattern].Row = row;
                                        patternStructsData[posInPattern].EffectNumber = ConvertNullEffectToZero(effectnumber);
                                        patternStructsData[posInPattern].EffectValue = ConvertNullEffectToZero(effectvalue);

                                        System.Diagnostics.Debug.WriteLine(String.Format(
                                            "row: {0} column: {1} track: {2} effect: {3} value: {4}", row, iCol, track, effectnumber, effectvalue)
                                        );
                                    }
                                }
                            }
                        }
                    }
                }

                channelIndex += sequencerTracks[track].NumberOfVisibleNoteColumns;

            }

            return patternStructsData;

        }

        /*
         *  Store all the row/track data in an array with size = (pattern.rows * totalChannels)
         * */
        private TrackLineData[] GetLineTrackDataFromPatternOld(Pattern renoisePattern, int[] assignedTrackPosition)
        {
            int numRows = renoisePattern.NumberOfLines;
            int numChannels = assignedTrackPosition[assignedTrackPosition.Length - 1];
            int patternDataSize = numRows * numChannels;

            TrackLineData[] patternStructsData = new TrackLineData[patternDataSize];

            // scan each track on pattern
            for (int iCurrentTrack = 0; iCurrentTrack < renoisePattern.Tracks.PatternTrack.Length; iCurrentTrack++)
            {
                if (renoisePattern.Tracks.PatternTrack[iCurrentTrack].Lines != null)
                {
                    // scan each lines filled on track
                    for (int iCurrentLine = 0; iCurrentLine < renoisePattern.Tracks.PatternTrack[iCurrentTrack].Lines.Length; iCurrentLine++)
                    {
                        // row index starts from 0
                        int row = renoisePattern.Tracks.PatternTrack[iCurrentTrack].Lines[iCurrentLine].index;

                        if (row >= numRows) continue;

                        string note = null;
                        string instrument = null;
                        string volume = null;
                        string panning = null;
                        string effectNumber = null;
                        string effectValue = null;

                        // fill pattern values for note columns
                        if (renoisePattern.Tracks.PatternTrack[iCurrentTrack].Lines[iCurrentLine].NoteColumns != null)
                        {

                            // avoids columns not visible to be written on pattern
                            int trackTotalVisibleColumns = assignedTrackPosition[iCurrentTrack + 1] - assignedTrackPosition[iCurrentTrack];

                            int loopEnd = renoisePattern.Tracks.PatternTrack[iCurrentTrack].Lines[iCurrentLine].NoteColumns.NoteColumn.Length;

                            if (trackTotalVisibleColumns < loopEnd)
                                loopEnd = trackTotalVisibleColumns;

                            // scan every columns on track                            
                            for (int iCurrentColumn = 0; iCurrentColumn < loopEnd; iCurrentColumn++)
                            {
                                note = renoisePattern.Tracks.PatternTrack[iCurrentTrack].Lines[iCurrentLine].NoteColumns.NoteColumn[iCurrentColumn].Note;
                                instrument = renoisePattern.Tracks.PatternTrack[iCurrentTrack].Lines[iCurrentLine].NoteColumns.NoteColumn[iCurrentColumn].Instrument;
                                volume = renoisePattern.Tracks.PatternTrack[iCurrentTrack].Lines[iCurrentLine].NoteColumns.NoteColumn[iCurrentColumn].Volume;
                                panning = renoisePattern.Tracks.PatternTrack[iCurrentTrack].Lines[iCurrentLine].NoteColumns.NoteColumn[iCurrentColumn].Panning;

                                int trackAssigned = assignedTrackPosition[iCurrentTrack] + iCurrentColumn;
                                int posInPattern = (row * numChannels) + trackAssigned;

                                patternStructsData[posInPattern].IsSet = true;

                                patternStructsData[posInPattern].Track = trackAssigned;
                                patternStructsData[posInPattern].Row = row;
                                patternStructsData[posInPattern].Note = note;
                                patternStructsData[posInPattern].Instrument = instrument;
                                patternStructsData[posInPattern].Volume = volume;
                                patternStructsData[posInPattern].Panning = panning;
                            }
                        }
                        // fill pattern values for effect columns
                        if (renoisePattern.Tracks.PatternTrack[iCurrentTrack].Lines[iCurrentLine].EffectColumns != null)
                        {
                            effectNumber = renoisePattern.Tracks.PatternTrack[iCurrentTrack].Lines[iCurrentLine].EffectColumns.EffectColumn[0].Number;
                            effectValue = renoisePattern.Tracks.PatternTrack[iCurrentTrack].Lines[iCurrentLine].EffectColumns.EffectColumn[0].Value;

                            int trackAssigned = assignedTrackPosition[iCurrentTrack];

                            int totalNoteColumnsForThisTrack;

                            if (iCurrentTrack == (renoisePattern.Tracks.PatternTrack.Length - 1))
                            {
                                totalNoteColumnsForThisTrack = numChannels - trackAssigned;
                            }
                            else
                            {
                                totalNoteColumnsForThisTrack = assignedTrackPosition[iCurrentTrack + 1] - trackAssigned;
                            }

                            // spreads volume column effects for all columns in track
                            for (int iCurrentColumn = 0; iCurrentColumn < totalNoteColumnsForThisTrack; iCurrentColumn++)
                            {
                                int posInPattern = (row * numChannels) + trackAssigned + iCurrentColumn;

                                patternStructsData[posInPattern].IsSet = true;

                                patternStructsData[posInPattern].Track = trackAssigned;
                                patternStructsData[posInPattern].Row = row;
                                patternStructsData[posInPattern].EffectNumber = ConvertNullEffectToZero(effectNumber);
                                patternStructsData[posInPattern].EffectValue = ConvertNullEffectToZero(effectValue);
                            }
                        }
                    }
                }
            }

            return patternStructsData;

        }
    }
}
