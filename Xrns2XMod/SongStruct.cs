/*
 * SongData
 *  PatternData[]
 *   TrackLineData[]
 *   MasterTrackLineData[]
 *  Instrument[]
 *   Sample[]
 *   
 *   */
public struct SongData
{
    public string Name { get; set; }
    public int RestartPosition { get; set; }
    public int NumChannels { get; set; }
    public int NumInstruments { get; set; }
    public int NumMasterTracksColumns { get; set; }
    public int InitialBPM { get; set; }
    public int LinesPerBeat { get; set; }
    public int TicksPerLine { get; set; }
    public bool SampleOffsetCompatibilityMode { get; set; }
    public bool PitchCompatibilityMode { get; set; }
    public int PlaybackEngineVersion { get; set; }
    public byte[] PatternOrderTable { get; set; }    
    public PatternData[] Patterns { get; set; }    
    public InstrumentData[] Instruments { get; set; }
}

public struct PatternData
{
    public int NumRows { get; set; }
    public TrackLineData[] TracksLineData { get; set; }
    public MasterTrackLineData[] MasterTrackLineData { get; set; }
}

public struct InstrumentData
{    
    public string Name { get; set; }
    public int[] KeyMap { get; set; }
    public bool VolumeEnabled { get; set; }
    public string[] EnvVolumePoints { get; set; }
    public int VolumeFadeOut { get; set; }
    public int VolumeSustainPoint { get; set; }
    public int VolumeLoopStart { get; set; }
    public int VolumeLoopEnd { get; set; }
    public bool VolumeSustainEnabled { get; set; }
    public bool VolumeLoopEnabled { get; set; }
    public string[] EnvPanningPoints { get; set; }
    public int PanningSustainPoint { get; set; }
    public int PanningLoopStart { get; set; }
    public int PanningLoopEnd { get; set; }
    public bool PanningSustainEnabled { get; set; }
    public bool PanningLoopEnabled { get; set; }
    public bool PanningEnabled { get; set; }
    public SampleData[] Samples { get; set; }
}

public struct SampleData
{
    public byte DefaultVolume { get; set; } 
    public string SampleFreq { get; set; } // handled by Ini settings
	public int SincInterpolationPoints { get; set; } // windowed sinc interpolation points, handled by Ini settings
    public sbyte Transpose { get; set; }
    public string Name { get; set; }
    public uint LoopStart { get; set; }
    public uint LoopEnd { get; set; }
    public float Volume { get; set; }
    public sbyte FineTune { get; set; }
    public float Panning { get; set; }
    public sbyte RelNoteNumber { get; set; }
    public string LoopMode { get; set; }
}

public struct MasterTrackLineData
{
    public bool IsSet { get; set; } // this variable help to speed up the conversion processing. if set to false, the line is empty
    public int Row { get; set; }
    public string EffectNumber { get; set; }
    public string EffectValue { get; set; }
}

public struct TrackLineData
{
    public bool IsSet { get; set; } // this variable help to speed up the conversion processing. if set to false, the line is empty
    public int Row { get; set; }
    public int Track { get; set; }
    public string Note { get; set; }
    public string Instrument { get; set; }
    public string Volume { get; set; }
    public string Panning { get; set; }
    public string Delay { get; set; }
    public string EffectNumber { get; set; }
    public string EffectValue { get; set; }

}