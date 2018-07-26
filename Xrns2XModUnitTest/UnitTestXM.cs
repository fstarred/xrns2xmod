using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;
using Xrns2XMod;

namespace Xrns2XModUnitTest
{
    [TestFixture]
    public class UnitTestXM
    {
        SongDataFactory songDataFactory;
        XmSettings settings;
        XMConverter converter;
        SongData songData;

        public TestContext TestContext { get; set; }

        [TestFixtureSetUp]
        public static void ClassInitialize ()
        {
            Console.WriteLine ("Initializing XM Unit Test class");
            BassWrapper.InitResources (IntPtr.Zero, null, null);
        }

        [TestFixtureTearDown]
        public static void ClassCleanup ()
        {
            Console.WriteLine ("Free XM Unit Test class");
            BassWrapper.FreeResources ();
        }


        public void ConversionTest (string path, string hash)
        {
            songDataFactory = new SongDataFactory ();            
			songDataFactory.ReportProgress += ReportProgress;

            string input = "resources/examples/" + path;
            
            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong (input);
            songData = songDataFactory.ExtractSongData (renoiseSong, input);
            converter = new XMConverter (input);

            settings = new XmSettings ();
            settings.Tempo = songData.InitialBPM;
            settings.TicksRow = songData.TicksPerLine;
            converter.Settings = settings;

            converter.EventProgress += ReportProgress;

            byte[] bytes = converter.Convert (songData);

            string hashGen = MD5Utils.GenerateMd5Hash (bytes);

            //Write file for later investigation
            string outputFile = input.Remove (input.Length - 5); // default output file, same as input without .xrns extension

            outputFile = System.Text.RegularExpressions.Regex.Match (outputFile, @"(?:(?!\.(mod|xm)$).)*").Value;

            string outputFileExt = System.Text.RegularExpressions.Regex.Match (outputFile, @"\.(mod | xm)$").Value;
            string destType = "xm";

            // add extension to output file in case user has not already specified it
            if (!outputFileExt.Equals ("." + destType, StringComparison.CurrentCultureIgnoreCase)) {
                outputFile += '.' + destType;
            }

            Utility.SaveByteArrayToFile (outputFile, bytes);

            //So is it what we wanted?
            Assert.AreEqual (hash, hashGen);

        }

        static void ReportProgress (object sender, EventReportProgressArgs e)
        {
            Console.WriteLine (e.message);
        }

        [Test]
        public void AdjustSampleFrequency ()
        {
			ConversionTest ("test_adjust_sample_frequency.xrns", "5bb4d0e1f0f8e78b573e0d8625c85259");
		}

        [Test]
        public void DefaultVolume ()
        {
			ConversionTest ("test_default_volume.xrns", "88f236fcadec11ca1b50c4fd31e49c86");
        }

        [Test]
        public void DelayColumn ()
        {
			ConversionTest ("test_delay_column.xrns", "60dd6b5506a556047bbb07231504fbe6");
        }

        [Test]
        public void FadeVolumeCompatibilityTrick ()
        {
			ConversionTest ("test_fade_volume_compatibility_trick.xrns", "7332c804c21fd696eb86a9fcbb23ecff");
        }

        [Test]
		public void FT2Mode ()
        {
			ConversionTest ("test_ft2_mode.xrns", "1d5625dd578504b2c949f391add67c5a");
        }

        [Test]
		public void GlobalCommands ()
        {
			ConversionTest ("test_global_commands.xrns", "1a7e740900e80a53393799668e4ecce4");
        }

        [Test]
        public void GlobalVolumeXmCommands ()
        {
			ConversionTest ("test_global_volume_xm_commands.xrns", "952366929aef04dcdf4dee27dedc9f24");
        }

        [Test]
        public void InstrumentEnvelopes ()
        {
			ConversionTest ("test_instrument_envelopes.xrns", "9611b90736a11ae2d9cf6849bdffd6ae");
        }

        [Test]
        public void InstrumentKeymap ()
        {
			ConversionTest ("test_instrument_keymap.xrns", "90b5bf094698f34a0b1e73defda20ded");
        }

        [Test]
        public void InstrumentsCommands ()
        {
			ConversionTest ("test_instruments_commands.xrns", "d1ce14d20853eb8ca62aa5da5c7f965b");
        }

        [Test]
        public void MultiColumns ()
        {
			ConversionTest ("test_multi_columns.xrns", "886d8aee962f6e42e6a727cc78b6f2c4");
        }

        [Test]
        public void PanningColumn ()
        {
			ConversionTest ("test_panning_column.xrns", "e40767b74223c625d8e003505f9d31e3");
        }

        [Test]
        public void RenoiseStandardTPLMode ()
        {
			ConversionTest ("test_renoise_standard_tpl_mode.xrns", "92abe7b7b8e2386b9fa09e7d0fb7b3a8");
        }

        [Test]
        public void SampleCommands ()
        {
			ConversionTest ("test_sample_commands.xrns", "eb5808f2e424fe535474c9d2ac260417");
        }

        [Test]
        public void TickCommands ()
        {
			ConversionTest ("test_tick_commands.xrns", "d11a166a291580271d88e7c7a99a7830");
        }


        [Test]
        public void VolumeColumn ()
        {
			ConversionTest ("test_volume_column.xrns", "85e2f2fec2fb93901ee182e15c86dc00");
        }

        [Test]
        public void VolumeScalingMode ()
        {
			ConversionTest ("test_volume_scaling_mode.xrns", "667d32d0b13c6d3ebe3fe0308215f8d4");
        }


    }
}
