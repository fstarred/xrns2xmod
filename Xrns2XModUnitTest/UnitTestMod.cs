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

            //Console.WriteLine("ConversionTest....");

            //string input = this.TestContext.Properties["resource_path"].ToString() + resourceAttribute.Path;
            string input = "resources/examples/" + path;
            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong (input);
            songData = songDataFactory.ExtractSongData (renoiseSong, input);
            converter = new ModConverter (input);

            settings = new ModSettings ();
            settings.ForceProTrackerCompatibility = PROTRACKER_COMPATIBILITY_MODE.NONE;
            settings.NtscMode = true;
            settings.PortamentoLossThreshold = 2;
            settings.VolumeScalingMode = VOLUME_SCALING_MODE.SAMPLE;

            converter.EventProgress += ReportProgress;

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
            ConversionTest ("test_adjust_sample_frequency.xrns", "9612c8a677c60fb344ce39bdc06b38b7");
        }

        [Test]
        public void DefaultVolume ()
        {
            ConversionTest ("test_default_volume.xrns", "2b31dd4ffbcb05faf49bd37b32e12376");
        }

        [Test]
        public void DelayColumn ()
        {
            ConversionTest ("test_delay_column.xrns", "719fcee8301d63f9cc36dc97646915f1");
        }

        [Test]
        public void FadeVolumeCompatibilityTrick ()
        {
            ConversionTest ("test_fade_volume_compatibility_trick.xrns", "de5b6857e8bbc80b7c7ca88e20175bbf");
        }

        [Test]
        public void FT2Mode ()
        {
            ConversionTest ("test_ft2_mode.xrns", "1d168eea0804e9780c6cb338b1ecd037");
        }

        [Test]
        public void GlobalCommands ()
        {
            ConversionTest ("test_global_commands.xrns", "433db7eaf631d2a3c6b2172518ee8442");
        }

        [Test]
        public void InstrumentsCommands ()
        {
            ConversionTest ("test_instruments_commands.xrns", "2057154adc09482a055302ad247fd928");
        }

        [Test]
        public void ModSampleConversion ()
        {
            ConversionTest ("test_mod_sample_conversion.xrns", "81cedf315d33a6b28366ce8fbf5e1701");
        }

        [Test]
        public void MultiColumns ()
        {
            ConversionTest ("test_multi_columns.xrns", "c777728033171415574524055d7150be");
        }

        [Test]
        public void PanningColumn ()
        {
            ConversionTest ("test_panning_column.xrns", "07834df5c4769a8c889b0f1b54c113b2");
        }

        [Test]
        public void RenoiseStandardTPLMode ()
        {
            ConversionTest ("test_renoise_standard_tpl_mode.xrns", "6546ebe6b7ab35ac8d308740d794ff97");
        }

        [Test]
        public void SampleCommands ()
        {
            ConversionTest ("test_sample_commands.xrns", "0263b0abe687bca2f892a4b7464c1a03");
        }

        [Test]
        public void TickCommands ()
        {
            ConversionTest ("test_tick_commands.xrns", "3e54be8104bfd22e719f2e29a317c660");
        }

        [Test]
        public void VolumeColumn ()
        {
            ConversionTest ("test_volume_column.xrns", "0e31af884e4b81dfdf574e1da82a6fad");
        }
    }
}
