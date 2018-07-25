
using NUnit.Framework;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Xrns2XMod;


namespace Xrns2XModUnitTest
{
    [TestFixture]
    public class UnitTestBass
    {
        public TestContext TestContext { get; set; }

        [TestFixtureSetUp]
        public void ClassInitialize ()
        {
            Console.WriteLine ("Initializing Bass Unit Test class");

            try {
                BassWrapper.InitResources (IntPtr.Zero, null, null);
            } catch (Exception e) {
                Console.WriteLine (e.Message);
				Console.WriteLine (e.StackTrace);
            }
        }

        [TestFixtureTearDown]
        public void ClassCleanup ()
        {
            Console.WriteLine ("Free Bass Unit Test class");
            BassWrapper.FreeResources ();
        }

        static void ReportProgress (object sender, EventReportProgressArgs e)
        {
            Console.WriteLine (e.message);
        }

        [Test]
        public void Resample_16To16 ()
        {
            //-- 1. Create a raw sine sample which shall be 44100 Hz and 16 Bit
            int sampleLength = 500;

            short[] buffer = new short[sampleLength];
            for (int i = 0; i < sampleLength; i++) {
                buffer [i] = (short)(30000.0f * Math.Sin (((float)i / (float)sampleLength) * (Math.PI * 2.0f)));
            }
                
            using (BinaryWriter writerRaw = new BinaryWriter (File.Open ("test1_in_sine_44100_16.raw", FileMode.Create))) {
                for (uint i = 0; i < sampleLength; i++) {
                    writerRaw.Write (buffer [i]);
                }
            }

            GCHandle _hGCFile;

            // now create a pinned handle, so that the Garbage Collector will not move this object
            _hGCFile = GCHandle.Alloc (buffer, GCHandleType.Pinned);

            int handle = Bass.BASS_StreamCreate (44100, 1, BASSFlag.BASS_STREAM_DECODE, BASSStreamProc.STREAMPROC_PUSH);

            int ret = Bass.BASS_StreamPutData (handle, _hGCFile.AddrOfPinnedObject (), buffer.Length * sizeof(short) | (int)BASSStreamProc.BASS_STREAMPROC_END);

            Assert.AreEqual (buffer.Length * sizeof(short), ret);


            //-- 2. Mix it
            int mixer = BassMix.BASS_Mixer_StreamCreate (11025, 1, BASSFlag.BASS_STREAM_DECODE);

            // add channel to mixer
            bool isMixerGood = BassMix.BASS_Mixer_StreamAddChannel (mixer, handle, BASSFlag.BASS_MIXER_NORAMPIN);

            Assert.AreEqual (true, isMixerGood);

            //-- 3. grab Stream and compare
            short[] buffer2 = new short[1000];

            // total data written to the new byte[] buffer
            int totalDataWritten = Bass.BASS_ChannelGetData (mixer, buffer2, (int)1000);

            Console.WriteLine ("totalDataWritten " + totalDataWritten);

            using (BinaryWriter writerRaw = new BinaryWriter (File.Open ("test1_out_sine_11025_16.raw", FileMode.Create))) {
                for (uint i = 0; i < totalDataWritten / 2; i++) {
                    writerRaw.Write (buffer2 [i]);
                }
            }

            byte[] buffer3 = new byte[2000];
            Buffer.BlockCopy (buffer2, 0, buffer3, 0, totalDataWritten);

            string hashGen = MD5Utils.GenerateMd5Hash (buffer3);

            Bass.BASS_StreamFree (handle);
            Bass.BASS_StreamFree (mixer);

            Assert.AreEqual ("2c61dc0767698eb28c7735cb073cea6c", hashGen);
        }


        [Test]
        public void Resample_8To8 () //broken only on Linux?
        {

            //-- 1. Create a raw sine sample which shall be 44100 Hz and 16 Bit
            int sampleLength = 500;

            byte[] buffer = new byte[sampleLength];
            for (int i = 0; i < sampleLength; i++) {
                buffer [i] = (byte)(128 + 120.0f * Math.Sin (((float)i / (float)sampleLength) * (Math.PI * 2.0f)));
            }

            using (BinaryWriter writerRaw = new BinaryWriter (File.Open ("test2_in_sine_44100_8.raw", FileMode.Create))) {
                for (uint i = 0; i < sampleLength; i++) {
                    writerRaw.Write (buffer [i]);
                }
            }

            GCHandle _hGCFile;

            // now create a pinned handle, so that the Garbage Collector will not move this object
            _hGCFile = GCHandle.Alloc (buffer, GCHandleType.Pinned);

            int handle = Bass.BASS_StreamCreate (44100, 1, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_8BITS, BASSStreamProc.STREAMPROC_PUSH);

            int ret = Bass.BASS_StreamPutData (handle, _hGCFile.AddrOfPinnedObject (), buffer.Length * sizeof(byte) | (int)BASSStreamProc.BASS_STREAMPROC_END);

            Assert.AreEqual (buffer.Length * sizeof(byte), ret);


            //-- 2. Mix it
            int mixer = BassMix.BASS_Mixer_StreamCreate (11025, 1, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_8BITS);

            // add channel to mixer
            bool isMixerGood = BassMix.BASS_Mixer_StreamAddChannel (mixer, handle, BASSFlag.BASS_MIXER_NORAMPIN);

            Assert.AreEqual (true, isMixerGood);

            //-- 3. grab Stream and compare
            byte[] buffer2 = new byte[1000];

            // total data written to the new byte[] buffer
            int totalDataWritten = Bass.BASS_ChannelGetData (mixer, buffer2, (int)1000);

            Console.WriteLine ("totalDataWritten " + totalDataWritten);

            using (BinaryWriter writerRaw = new BinaryWriter (File.Open ("test2_out_sine_11025_8.raw", FileMode.Create))) {
                for (uint i = 0; i < totalDataWritten; i++) {
                    writerRaw.Write (buffer2 [i]);
                }
            }
                
            string hashGen = MD5Utils.GenerateMd5Hash (buffer2);

            Bass.BASS_StreamFree (handle);
            Bass.BASS_StreamFree (mixer);

			Assert.AreEqual ("50bce9c536e98667534747f0a5f06ebd", hashGen);
        }

        [Test]
        public void Resample_16To8 () //broken only on Linux?
        {
            //-- 1. Create a raw sine sample which shall be 44100 Hz and 16 Bit
            int sampleLength = 500;

            short[] buffer = new short[sampleLength];
            for (int i = 0; i < sampleLength; i++) {
				buffer [i] = (short)(30000.0f * Math.Sin (((float)i / (float)sampleLength) * (Math.PI * 2.0f)));
            }

            using (BinaryWriter writerRaw = new BinaryWriter (File.Open ("test3_in_sine_44100_16.raw", FileMode.Create))) {
                for (uint i = 0; i < sampleLength; i++) {
                    writerRaw.Write (buffer [i]);
                }
            }

            GCHandle _hGCFile;

            // now create a pinned handle, so that the Garbage Collector will not move this object
            _hGCFile = GCHandle.Alloc (buffer, GCHandleType.Pinned);

            int handle = Bass.BASS_StreamCreate (44100, 1, BASSFlag.BASS_STREAM_DECODE, BASSStreamProc.STREAMPROC_PUSH);
            int ret = Bass.BASS_StreamPutData (handle, _hGCFile.AddrOfPinnedObject (), buffer.Length * sizeof(short) | (int)BASSStreamProc.BASS_STREAMPROC_END);

            Assert.AreEqual (buffer.Length * sizeof(short), ret);

            //-- 2. Mix it
            int mixer = BassMix.BASS_Mixer_StreamCreate (11025, 1, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_8BITS);

            // add channel to mixer
            bool isMixerGood = BassMix.BASS_Mixer_StreamAddChannel (mixer, handle, BASSFlag.BASS_MIXER_NORAMPIN);

            Assert.AreEqual (true, isMixerGood);

            //-- 3. grab Stream and compare
            byte[] buffer2 = new byte[1000];

            // total data written to the new byte[] buffer
            int totalDataWritten = Bass.BASS_ChannelGetData (mixer, buffer2, (int)1000);

            Console.WriteLine ("totalDataWritten " + totalDataWritten);

            using (BinaryWriter writerRaw = new BinaryWriter (File.Open ("test3_out_sine_11025_8.raw", FileMode.Create))) {
                for (uint i = 0; i < totalDataWritten; i++) {
                    writerRaw.Write (buffer2 [i]);
                }
            }
				
            string hashGen = MD5Utils.GenerateMd5Hash (buffer2);

            Bass.BASS_StreamFree (handle);
            Bass.BASS_StreamFree (mixer);

			Assert.AreEqual ("8303df5b4a649645690fc87489b0f6e0", hashGen);
        }


        [Test]
        public void Resample_8To16 ()
        {
            //-- 1. Create a raw sine sample which shall be 44100 Hz and 16 Bit
            int sampleLength = 500;

            byte[] buffer = new byte[sampleLength];
            for (int i = 0; i < sampleLength; i++) {
                buffer [i] = (byte)(128 + 120.0f * Math.Sin (((float)i / (float)sampleLength) * (Math.PI * 2.0f)));
            }

            using (BinaryWriter writerRaw = new BinaryWriter (File.Open ("test4_in_sine_44100_8.raw", FileMode.Create))) {
                for (uint i = 0; i < sampleLength; i++) {
                    writerRaw.Write (buffer [i]);
                }
            }

            GCHandle _hGCFile;

            // now create a pinned handle, so that the Garbage Collector will not move this object
            _hGCFile = GCHandle.Alloc (buffer, GCHandleType.Pinned);

            int handle = Bass.BASS_StreamCreate (44100, 1, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_8BITS, BASSStreamProc.STREAMPROC_PUSH);

            int ret = Bass.BASS_StreamPutData (handle, _hGCFile.AddrOfPinnedObject (), buffer.Length * sizeof(byte) | (int)BASSStreamProc.BASS_STREAMPROC_END);

            Assert.AreEqual (buffer.Length * sizeof(byte),ret);


            //-- 2. Mix it
            int mixer = BassMix.BASS_Mixer_StreamCreate (11025, 1, BASSFlag.BASS_STREAM_DECODE);

            // add channel to mixer
            bool isMixerGood = BassMix.BASS_Mixer_StreamAddChannel (mixer, handle, BASSFlag.BASS_MIXER_NORAMPIN);

            Assert.AreEqual (true,isMixerGood);

            //-- 3. grab Stream and compare
            short[] buffer2 = new short[1000];

            // total data written to the new byte[] buffer
            int totalDataWritten = Bass.BASS_ChannelGetData (mixer, buffer2, (int)1000);

            Console.WriteLine ("totalDataWritten " + totalDataWritten);

            using (BinaryWriter writerRaw = new BinaryWriter (File.Open ("test4_out_sine_11025_16.raw", FileMode.Create))) {
                for (uint i = 0; i < totalDataWritten / 2; i++) {
                    writerRaw.Write (buffer2 [i]);
                }
            }

            byte[] buffer3 = new byte[2000];
            Buffer.BlockCopy (buffer2, 0, buffer3, 0, totalDataWritten);
            string hashGen = MD5Utils.GenerateMd5Hash (buffer3);

            Bass.BASS_StreamFree (handle);
            Bass.BASS_StreamFree (mixer);

            Assert.AreEqual ("5623f37646fdb253c444f712f4827d1a",hashGen);
        }


        [Test]
        public void Resample_8ToFloat ()
        {
            //-- 1. Create a raw sine sample which shall be 44100 Hz and 16 Bit
            int sampleLength = 500;

            byte[] buffer = new byte[sampleLength];
            for (int i = 0; i < sampleLength; i++) {
                buffer [i] = (byte)(128 + 120.0f * Math.Sin (((float)i / (float)sampleLength) * (Math.PI * 2.0f)));
            }

            using (BinaryWriter writerRaw = new BinaryWriter (File.Open ("test5_in_sine_44100_8.raw", FileMode.Create))) {
                for (uint i = 0; i < sampleLength; i++) {
                    writerRaw.Write (buffer [i]);
                }
            }

            GCHandle _hGCFile;

            // now create a pinned handle, so that the Garbage Collector will not move this object
            _hGCFile = GCHandle.Alloc (buffer, GCHandleType.Pinned);

            int handle = Bass.BASS_StreamCreate (44100, 1, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_8BITS, BASSStreamProc.STREAMPROC_PUSH);

            int ret = Bass.BASS_StreamPutData (handle, _hGCFile.AddrOfPinnedObject (), buffer.Length * sizeof(byte) | (int)BASSStreamProc.BASS_STREAMPROC_END);

            Assert.AreEqual (buffer.Length * sizeof(byte),ret);


            //-- 2. Mix it
            int mixer = BassMix.BASS_Mixer_StreamCreate (11025, 1, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);

            // add channel to mixer
            bool isMixerGood = BassMix.BASS_Mixer_StreamAddChannel (mixer, handle, BASSFlag.BASS_MIXER_NORAMPIN);

            Assert.AreEqual (true,isMixerGood);

            //-- 3. grab Stream and compare
            float[] buffer2 = new float[1000];

            // total data written to the new byte[] buffer
            int totalDataWritten = Bass.BASS_ChannelGetData (mixer, buffer2, (int)1000);

            Console.WriteLine ("totalDataWritten " + totalDataWritten);

            using (BinaryWriter writerRaw = new BinaryWriter (File.Open ("test5_out_sine_11025_float.raw", FileMode.Create))) {
                for (uint i = 0; i < totalDataWritten / 4; i++) {
                    writerRaw.Write (buffer2 [i]);
                }
            }

            byte[] buffer3 = new byte[2000];
            Buffer.BlockCopy (buffer2, 0, buffer3, 0, totalDataWritten);
            string hashGen = MD5Utils.GenerateMd5Hash (buffer3);

            Bass.BASS_StreamFree (handle);
            Bass.BASS_StreamFree (mixer);

            Assert.AreEqual ("303f97ff4f022feb29bd3944b3c300bb", hashGen);

        }

    }
}
