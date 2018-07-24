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
			ConversionTest ("test_adjust_sample_frequency.xrns", "98ffd26ebce463439a0e544a020289d4");
		}

        [Test]
        public void DefaultVolume ()
        {
			ConversionTest ("test_default_volume.xrns", "8c2e69e9c96d0b6df4ece91306b43304");
        }

        [Test]
        public void DelayColumn ()
        {
			ConversionTest ("test_delay_column.xrns", "ac5c9f2d6ac31564ccfe3c12274875e2");
        }

        [Test]
        public void FadeVolumeCompatibilityTrick ()
        {
			ConversionTest ("test_fade_volume_compatibility_trick.xrns", "ce0d0185fcf148ffe80d5fd615ab911f");
        }

        [Test]
		public void FT2Mode ()
        {
			ConversionTest ("test_ft2_mode.xrns", "259feb4a31eebc4545c7ed217193adfc");
        }

        [Test]
		public void GlobalCommands ()
        {
			ConversionTest ("test_global_commands.xrns", "a1a9f3754d0298aa791b636a8b5ac60c");
        }

        [Test]
        public void GlobalVolumeXmCommands ()
        {
			ConversionTest ("test_global_volume_xm_commands.xrns", "223815be794edc61871a79c285dcd80b");
        }

        [Test]
        public void InstrumentEnvelopes ()
        {
			ConversionTest ("test_instrument_envelopes.xrns", "233130acf3cc3b28f42b00b56e403c5c");
        }

        [Test]
        public void InstrumentKeymap ()
        {
			ConversionTest ("test_instrument_keymap.xrns", "193f232d955d07ac27c0bd6793b28dec");
        }

        [Test]
        public void InstrumentsCommands ()
        {
			ConversionTest ("test_instruments_commands.xrns", "a4949e2d65bce6c29ca4a95e8456f8b9");
        }

        [Test]
        public void MultiColumns ()
        {
			ConversionTest ("test_multi_columns.xrns", "fe062d09adc179dd38f4152dfc2c018a");
        }

        [Test]
        public void PanningColumn ()
        {
			ConversionTest ("test_panning_column.xrns", "6e058660aaf84f69fa0c5e8c0fe52d91");
        }

        [Test]
        public void RenoiseStandardTPLMode ()
        {
			ConversionTest ("test_renoise_standard_tpl_mode.xrns", "494c2ce8d03e1774c92401ca2bd482d4");
        }

        [Test]
        public void SampleCommands ()
        {
			ConversionTest ("test_sample_commands.xrns", "36ecf7ec6aa674574b88623870a3f692");
        }

        [Test]
        public void TickCommands ()
        {
			ConversionTest ("test_tick_commands.xrns", "373b612b604d514c653468d70cbd51d2");
        }


        [Test]
        public void VolumeColumn ()
        {
			ConversionTest ("test_volume_column.xrns", "001cfe7fc3063ed7a1f878c9de538688");
        }

        [Test]
        public void VolumeScalingMode ()
        {
			ConversionTest ("test_volume_scaling_mode.xrns", "eaec9b7acfb06cb203ef0e94324401a0");
        }


    }
}
