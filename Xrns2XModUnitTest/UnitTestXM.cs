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
            ConversionTest ("test_adjust_sample_frequency.xrns", "52e02bc86985f0f83435aca9ee62efe5");
        }

        [Test]
        public void DefaultVolume ()
        {
            ConversionTest ("test_default_volume.xrns", "6774ec1c292beece37a04fc17ac2e45a");
        }

        [Test]
        public void DelayColumn ()
        {
            ConversionTest ("test_delay_column.xrns", "f67a94b3a54bae37d663c05ff4502001");
        }

        [Test]
        public void FadeVolumeCompatibilityTrick ()
        {
            ConversionTest ("test_fade_volume_compatibility_trick.xrns", "0c79bbb58ff4a8e9e1a4c958c6dacd27");
        }

        [Test]
        public void FT2Mode ()
        {
            ConversionTest ("test_ft2_mode.xrns", "1a8ecc9dee9a3678ee4224cc9c346430");
        }

        [Test]
        public void GlobalCommands ()
        {
            ConversionTest ("test_global_commands.xrns", "fc1c3b52e9d722901670049401320e93");
        }

        [Test]
        public void GlobalVolumeXmCommands ()
        {
            ConversionTest ("test_global_volume_xm_commands.xrns", "ae1df1b1cf5a16a7d35cc20624d7fe51");
        }

        [Test]
        public void InstrumentEnvelopes ()
        {
            ConversionTest ("test_instrument_envelopes.xrns", "7c741e1f44514bc4a94b90bc0849d54e");
        }

        [Test]
        public void InstrumentKeymap ()
        {
            ConversionTest ("test_instrument_keymap.xrns", "5e746efcc1493974a218b1ede021dde7");
        }

        [Test]
        public void InstrumentsCommands ()
        {
            ConversionTest ("test_instruments_commands.xrns", "3c450c54fa62f6e7a00a6587203c2988");
        }

        [Test]
        public void MultiColumns ()
        {
            ConversionTest ("test_multi_columns.xrns", "aacd817279de9c55add5f4a5f05736cf");
        }

        [Test]
        public void PanningColumn ()
        {
            ConversionTest ("test_panning_column.xrns", "4107eed8586d6b0395fa03570ead5a74");
        }

        [Test]
        public void RenoiseStandardTPLMode ()
        {
            ConversionTest ("test_renoise_standard_tpl_mode.xrns", "a9cf4bb798ba9d472021a8a8bca598ae");
        }

        [Test]
        public void SampleCommands ()
        {
            ConversionTest ("test_sample_commands.xrns", "0d559b8c089e83e3788e4d95d5d877fd");
        }

        [Test]
        public void TickCommands ()
        {
            ConversionTest ("test_tick_commands.xrns", "513cab45a102ac003cd754a54c8ef14a");
        }


        [Test]
        public void VolumeColumn ()
        {
            ConversionTest ("test_volume_column.xrns", "c89468b202520aa6dfec2224cfdffedc");
        }

        [Test]
        public void VolumeScalingMode ()
        {
            ConversionTest ("test_volume_scaling_mode.xrns", "04f4ae7c82e87c5382c28fc1ef438600");
        }

    }
}
