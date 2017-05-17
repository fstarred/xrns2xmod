using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xrns2XMod.Properties;

namespace Xrns2XMod
{
    /*
     * An abstract class for mod / xm class utils
     * */
    abstract class ModCommonBase
    {

        

        protected static string[] notesArray = { "C-", "C#", "D-", "D#", "E-", "F-", "F#", "G-", "G#", "A-", "A#", "B-" };

        protected ModCommonBase(SongData songData, int paramTicksPerRow)
        {
            sampleOffsetCompatibilityMode = songData.SampleOffsetCompatibilityMode;
            pitchCompatibilityMode = songData.PitchCompatibilityMode;
            playbackEngineVersion = songData.PlaybackEngineVersion;
            ticksPerRow = paramTicksPerRow;
        }

        protected static CultureInfo Culture = new CultureInfo("EN-GB");

        protected bool sampleOffsetCompatibilityMode { get; private set; }
        protected bool pitchCompatibilityMode { get; private set; }
        protected int ticksPerRow { get; private set; }
        protected int playbackEngineVersion { get; private set; }

        protected static int CalculateSampleLength(int size, int bps, int chans)
        {
            //size /= (bps / 8);

            if (bps > 8) size /= 2;

            size /= chans;

            return size;
        }


        protected static void GetSampleProperties(int renBaseNote, int transpose, int renFineTuning, int sampleRate, out int relativeTone, out int fineTune)
        {
            // range value in Renoise starts from 0 to 119

            // Thanks to Jojo of OpenMPT for giving me this hint

            const int defaultNote = 48; /* C-4 for Renoise */

            relativeTone = 0;
            fineTune = 0;

            int renoiseValue2Add = defaultNote - renBaseNote + transpose;

            int f2t = (int)(1536.0 * (Math.Log((double)sampleRate / 8363.0) / Math.Log(2.0)));
            int transp = f2t >> 7;
            int ftune = f2t & 0x7F; //0x7F == 111 1111 

            ftune += renFineTuning;
            if (ftune > 80)
            {
                transp++;
                ftune -= 128;
            }
            if (transp > 127) transp = 127;
            if (transp < -127) transp = -127;

            relativeTone = transp;
            fineTune = ftune;

            relativeTone += renoiseValue2Add;
        }


        public void ComputeTickPerRowForCurrentLine(TrackLineData[] trackLineData, int currentRow, int numChannels)
        {
            if (currentRow % numChannels == 0)
            {
                for (int i = 0; i < numChannels; i++)
                {
                    int track = currentRow++;

                    TrackLineData trackData = trackLineData[track];

                    if (trackData.IsSet)
                    {
                        if (trackData.EffectNumber != null)
                        {
                            char[] commandEffectSplitted = trackData.EffectNumber.ToCharArray();
                            char effType = commandEffectSplitted[0];
                            char effCom = commandEffectSplitted[1];
                            
                            int effVal = Int16.Parse(trackData.EffectValue, System.Globalization.NumberStyles.HexNumber);

                            char commandForTicks = 
                                playbackEngineVersion == Constants.MOD_VERSION_COMPATIBLE ? 
                                'L' : 'K';

                            if (effCom.Equals(commandForTicks))
                            {
                                ticksPerRow = effVal;
                            }
                        }
                    }
                }
            }
        }
    }




    




    



}
