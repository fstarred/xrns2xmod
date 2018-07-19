using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Aac;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Mix;

namespace Xrns2XMod
{
    public static class BassWrapper
    {
#if DEBUG
        static int testCnt = 0;
#endif

		private static bool isInitialized = false;

        public static void InitResources(IntPtr win, string bassEmail, string bassCode)
        {
			//This function shall only be executed once! At least NUnit has a problem if executed multiple times.
			if (isInitialized)
				return;
			
            string targetPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //if (Utils.Is64Bit)
            //    targetPath = Path.Combine(targetPath, "libs/x64");
            //else
            //    targetPath = Path.Combine(targetPath, "libs/x86");            

            if (!string.IsNullOrEmpty(bassEmail) && !string.IsNullOrEmpty(bassCode))
            {
                BassNet.Registration(bassEmail, bassCode);
            }

            if (Utility.IsWindowsOS())
            {
                Bass.LoadMe();
                BassMix.LoadMe();
                BassFlac.LoadMe();
                BassAac.LoadMe();
            }

            bool isBassInit = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, win);

            if (!isBassInit)
                throw new ApplicationException("Some errors occurred while initializing audio dll");

			isInitialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetBassStream(SampleStreamInfo input)
        {
            int handle;

#if DEBUG
            Console.WriteLine ("getBassStream " + testCnt + " "+ input.Stream.Length);
#endif
            Stream stream = input.Stream;

#if DEBUG
            stream.Seek(0, System.IO.SeekOrigin.Begin);

            using (FileStream file = new FileStream("getBassStream "+testCnt+".bin", FileMode.Create, System.IO.FileAccess.Write))
                stream.CopyTo(file);
#endif			
            stream.Seek (0, System.IO.SeekOrigin.Begin);

            byte[] buffer = Utility.GetBytesFromStream(stream, stream.Length);

            long length = buffer.Length;

            GCHandle _hGCFile;

            // now create a pinned handle, so that the Garbage Collector will not move this object
            _hGCFile = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            Func<IntPtr, long, long, BASSFlag, int> actionStreamCreateFile;

            switch (input.Format)
            {
                case FORMAT.WAV:
                case FORMAT.AIFF:
                case FORMAT.MP3:
                case FORMAT.OGG:
                    actionStreamCreateFile = Bass.BASS_StreamCreateFile;
                    break;
                case FORMAT.FLAC:
                    actionStreamCreateFile = BassFlac.BASS_FLAC_StreamCreateFile;                    
                    break;
                case FORMAT.AAC:
                    //actionStreamCreateFile = BassAac.BASS_AAC_StreamCreateFile;
                    throw new NotImplementedException("AAC extension not supported");                    
                default:
                    throw new NotImplementedException("Sample sxtension is not supported");
            }

            handle = actionStreamCreateFile(_hGCFile.AddrOfPinnedObject(), 0L, length, BASSFlag.BASS_STREAM_DECODE);
            
            return handle;
        }

        [Obsolete("Use GetBassStream instead")]
        public static int GetFlacStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            byte[] buffer = Utility.GetBytesFromStream(stream, stream.Length);

            long length = buffer.Length;

            GCHandle _hGCFile;

            // now create a pinned handle, so that the Garbage Collector will not move this object
            _hGCFile = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            // create the flac stream (AddrOfPinnedObject delivers the necessary IntPtr)            
            int handle = BassFlac.BASS_FLAC_StreamCreateFile(_hGCFile.AddrOfPinnedObject(), 0L, length, BASSFlag.BASS_STREAM_DECODE);

            return handle;
        }
        

        public static Stream GetXMEncodedSample(int handle, long sampleLength, int chans, int bps)
        {
            byte[] rawDataBuffer = new byte[sampleLength];

            // retrieve raw data of the source on byte[]
            int channelTotalData = Bass.BASS_ChannelGetData(handle, rawDataBuffer, (int)sampleLength);

            if (bps == 8)
            {
                if (chans == 2)
                    return AudioEncUtil.EncodeDelta8BitStereoSample(rawDataBuffer);
                else
                    return AudioEncUtil.EncodeDelta8BitMonoSample(rawDataBuffer);
            }
            else
            {
                if (chans == 2)
                    return AudioEncUtil.EncodeDelta16BitStereoSample(rawDataBuffer);
                else
                    return AudioEncUtil.EncodeDelta16BitMonoSample(rawDataBuffer);
            }
        }

        public static BASS_CHANNELINFO GetBassChannelInfo(int handle)
        {
            BASS_CHANNELINFO output = Bass.BASS_ChannelGetInfo(handle);

            return output;
        }

        public static int GetSampleFreq(int handle)
        {
            return Bass.BASS_ChannelGetInfo(handle).freq;
        }

        public static void FreeStream(int handle)
        {
            Bass.BASS_StreamFree(handle);
        }

		public static Stream GetModEncodedSample(int handle, long sampleLength, PROTRACKER_COMPATIBILITY_MODE ptCompatibility)
        {
            byte[] buffer = new byte[sampleLength];

            // total data written to the new byte[] buffer
            int totalDataWritten = Bass.BASS_ChannelGetData(handle, buffer, (int)sampleLength);

#if DEBUG
            using (BinaryWriter writerRaw = new BinaryWriter (File.Open ("fileRaw" + testCnt + ".bin", FileMode.Create)))
            {
                for (uint i = 0; i < totalDataWritten; i++)
                {
                    writerRaw.Write(buffer[i]);
                }
            }
#endif
            MemoryStream inputSample = new MemoryStream(buffer);

            MemoryStream outputStream = new MemoryStream();

            BinaryReader reader = new BinaryReader(inputSample);

            BinaryWriter writer = new BinaryWriter(outputStream);

#if DEBUG
            Console.WriteLine ("sampleLen " + sampleLength);
            testCnt++;

            BinaryWriter writer2 = new BinaryWriter (File.Open ("fileB" + testCnt + ".bin", FileMode.Create));
            BinaryWriter writer3 = new BinaryWriter (File.Open ("fileD" + testCnt + ".bin", FileMode.Create));
            inputSample.Seek(0, SeekOrigin.Begin);
            inputSample.Seek (0, SeekOrigin.Begin);
#endif

            // Amiga ProTracker compatibility
            // all samples with no loop should begin with two bytes of 0 value (Thanks to Jojo of OpenMPT for the hints)            
			if (ptCompatibility != PROTRACKER_COMPATIBILITY_MODE.NONE)
            {
                for (int i = 0; i < 2; i++)
                {
                    short value = reader.ReadInt16();
                    if (value != 0)
                        writer.Write ((sbyte)0);
                }

                inputSample.Seek(0, SeekOrigin.Begin);
            }
#if DEBUG
            Console.WriteLine ("totalDataWritten " + totalDataWritten);
#endif
            for (uint i = 0; i < totalDataWritten; i += 2)
            {
                short value = reader.ReadInt16 ();
                sbyte newValue = (sbyte)(value / 256);
                writer.Write (newValue);
#if DEBUG
                writer2.Write (newValue);
#endif
            }
#if DEBUG
    		writer2.Close();
    		writer3.Close();
#endif
            // sample length must be even, because its value is stored divided by 2
			if (outputStream.Length % 2 != 0)
            {
                writer.Write((sbyte)0);
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }


        public static void AdjustSampleVolume(int handle, int mixer, float value)
        {
            BASS_MIXER_NODE[] nodes = 
              {
                new BASS_MIXER_NODE(0, value),
              };
            bool isVolGood = BassMix.BASS_Mixer_ChannelSetEnvelope(handle, BASSMIXEnvelope.BASS_MIXER_ENV_VOL, nodes);
        }


        public static int PlugChannelToMixer(int handle, int freq, int chans, int res)
        {
            BASSFlag bassFlag = BASSFlag.BASS_STREAM_DECODE;

            if (res == 8)
                bassFlag |= BASSFlag.BASS_SAMPLE_8BITS;

            // this will be the final mixer output stream being played   
            int mixer = BassMix.BASS_Mixer_StreamCreate(freq, chans, bassFlag);

            // add channel to mixer
            bool isMixerGood = BassMix.BASS_Mixer_StreamAddChannel(mixer, handle, BASSFlag.BASS_MIXER_NORAMPIN);
#if DEBUG
            Console.WriteLine (freq + " "+chans+" "+res+" isMixerGood " + isMixerGood);
#endif
            return mixer;
        }        

        public static void FreeResources()
        {
            Bass.FreeMe();
            BassMix.FreeMe();
            BassFlac.FreeMe();

        }
    }
}
