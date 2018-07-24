using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Xrns2XMod;

namespace Xrns2XModUnitTest
{
    [TestFixture]
    public class UnitTestMod
    {
        SongDataFactory songDataFactory;
        ModSettings settings;
        ModConverter converter;
        SongData songData;

        public TestContext TestContext { get; set; }

        [TestFixtureSetUp]
        public void ClassInitialize ()
        {
            Console.WriteLine ("Initializing MOD Unit Test class");

            try {
                BassWrapper.InitResources (IntPtr.Zero, null, null);
            } catch (Exception e) {
                Console.WriteLine (e.Message);
            }
        }

        [TestFixtureTearDown]
        public void ClassCleanup ()
        {
            Console.WriteLine ("Free MOD Unit Test class");
            BassWrapper.FreeResources ();
        }

        public void ConversionTest (string path, string hash)
        {
            songDataFactory = new SongDataFactory ();
			songDataFactory.ReportProgress += ReportProgress;

            //Console.WriteLine("ConversionTest....");

            //string input = this.TestContext.Properties["resource_path"].ToString() + resourceAttribute.Path;
            string input = "resources/examples/" + path;
            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong (input);
            songData = songDataFactory.ExtractSongData (renoiseSong, input);

            converter = new ModConverter (input);
			converter.EventProgress += ReportProgress;

            settings = new ModSettings ();
            settings.ForceProTrackerCompatibility = PROTRACKER_COMPATIBILITY_MODE.NONE;
            settings.NtscMode = true;
            settings.PortamentoLossThreshold = 2;
            settings.VolumeScalingMode = VOLUME_SCALING_MODE.SAMPLE;

            

            converter.Settings = settings;

            byte[] bytes = converter.Convert (songData);

            string hashGen = MD5Utils.GenerateMd5Hash (bytes);


            //Write file for later investigation
            string outputFile = input.Remove (input.Length - 5); // default output file, same as input without .xrns extension

            outputFile = System.Text.RegularExpressions.Regex.Match (outputFile, @"(?:(?!\.(mod|xm)$).)*").Value;

            string outputFileExt = System.Text.RegularExpressions.Regex.Match (outputFile, @"\.(mod | xm)$").Value;
            string destType = "mod";

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
			ConversionTest ("test_adjust_sample_frequency.xrns", "26aba2ff997acedf404a074171603c55");
        }

        [Test]
        public void DefaultVolume ()
        {
			ConversionTest ("test_default_volume.xrns", "308d2a1916693a28edc45d2117b7a718");
        }

        [Test]
        public void DelayColumn ()
        {
			ConversionTest ("test_delay_column.xrns", "d43f4326f62f719588f29081027980fa");
        }

        [Test]
        public void FadeVolumeCompatibilityTrick ()
        {
			ConversionTest ("test_fade_volume_compatibility_trick.xrns", "4d8a362b0bed4d8ca5010426ec641b3e");
        }

        [Test]
		public void FT2Mode ()
        {
			ConversionTest ("test_ft2_mode.xrns", "ff23ac24bcb7854a9c4fe443036a9251");
        }

        [Test]
        public void GlobalCommands ()
        {
			ConversionTest ("test_global_commands.xrns", "433db7eaf631d2a3c6b2172518ee8442");
        }

        [Test]
		public void InstrumentsCommands ()
        {
			ConversionTest ("test_instruments_commands.xrns", "fe04aee3202757ddbbf45134d38db66b");
        }

        [Test]
		public void ModSampleConversion ()
        {
			ConversionTest ("test_mod_sample_conversion.xrns", "6b4326980afecd55cb7c6ae21f8d1226");
        }

		[Test]
        public void MultiColumns ()
        {
			ConversionTest ("test_multi_columns.xrns", "9201efb2392b256b80bd38798fc7e943");
        }

        [Test]
        public void PanningColumn ()
        {
			ConversionTest ("test_panning_column.xrns", "a3da9f5d5716c757eacc56bb36fdd297");
        }

		[Test]
        public void RenoiseStandardTPLMode ()
		{
			ConversionTest ("test_renoise_standard_tpl_mode.xrns", "59f14889b73980d1401d07776a79448b");
        }

		[Test]
        public void SampleCommands ()
        {
			ConversionTest ("test_sample_commands.xrns", "04034cf7fba96ed0156bc97660c11e39");
        }

		[Test]
		public void SincInterpolation ()
		{
			ConversionTest ("test_sample_sinc.xrns", "eaeebe5d4903b525bdfc6a4c6091858f");
		}
			
        [Test]
        public void TickCommands ()
        {
			ConversionTest ("test_tick_commands.xrns", "7cbbfd87d073575dc2f7515d62e82d30");
        }

        [Test]
        public void VolumeColumn ()
        {
			ConversionTest ("test_volume_column.xrns", "094586988400dda4110b3f525e403acd");
        }

	

    }
}
