using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xrns2XMod;

namespace Xrns2XModUnitTest
{
    [TestClass]
    public class ModTests
    {
        const string RESOURCE_DIRECTORY = @"resources/examples/";
        SongDataFactory songDataFactory;
        ModSettings settings;
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext ctx)
        {
            BassWrapper.InitResources(IntPtr.Zero, null, null);
        }

        [TestInitialize]
        public void InitializeMethod()
        {
            songDataFactory = new SongDataFactory();
            settings = new ModSettings();
            settings.ForceProTrackerCompatibility = true;
            settings.PortamentoLossThreshold = 2;
            settings.VolumeScalingMode = VOLUME_SCALING_MODE.SAMPLE;
        }

        [TestMethod]
        public void AdjustSampleFrequency()
        {
            const string input = RESOURCE_DIRECTORY + "test_adjust_sample_frequency.xrns";
            
            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);
            
            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "2e36279920d991ad20241d5812f035bd");
        }

        [TestMethod]
        public void DefaultVolume()
        {
            const string input = RESOURCE_DIRECTORY + "test_default_volume.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "8a04aaaa005b76f33f145f5ba61200ed");
        }

        [TestMethod]
        public void DelayColumn()
        {
            const string input = RESOURCE_DIRECTORY + "test_delay_column.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "34a34fcf4fb6bf036c298a773102101a");
        }

        [TestMethod]
        public void FadeVolumeCompatibilityTrick()
        {
            const string input = RESOURCE_DIRECTORY + "test_fade_volume_compatibility_trick.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "435ada05f16686e29198a963e276fb8f");
        }

        [TestMethod]
        public void FT2Mode()
        {
            const string input = RESOURCE_DIRECTORY + "test_ft2_mode.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "48a7a50abfd384b5ed318c940be623e6");
        }

        [TestMethod]
        public void GlobalCommands()
        {
            const string input = RESOURCE_DIRECTORY + "test_global_commands.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "433db7eaf631d2a3c6b2172518ee8442");
        }


        [TestMethod]
        public void InstrumentsCommands()
        {
            const string input = RESOURCE_DIRECTORY + "test_instruments_commands.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "fe95f07432539b31e25446863e22f4e7");
        }

        [TestMethod]
        public void ModSampleConversion()
        {
            const string input = RESOURCE_DIRECTORY + "test_mod_sample_conversion.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "56c9c667a857b823411cea57edaf12cf");
        }

        [TestMethod]
        public void MultiColumns()
        {
            const string input = RESOURCE_DIRECTORY + "test_multi_columns.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "2c7591e96f820b08b2f3ce3ab5c3a7a2");
        }

        [TestMethod]
        public void PanningColumn()
        {
            const string input = RESOURCE_DIRECTORY + "test_panning_column.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "20646321e170f66f1933d69f074fc829");
        }

        [TestMethod]
        public void RenoiseStandardTPLMode()
        {
            const string input = RESOURCE_DIRECTORY + "test_renoise_standard_tpl_mode.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "341120418fdbcaf06bd46bef7c0c9844");
        }

        [TestMethod]
        public void SampleCommands()
        {
            const string input = RESOURCE_DIRECTORY + "test_sample_commands.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "4ad75feec9349ffc6f26d25c7d89392e");
        }

        [TestMethod]
        public void TickCommands()
        {
            const string input = RESOURCE_DIRECTORY + "test_tick_commands.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "0d4e432d6d758463713ee637db6ecbec");
        }


        [TestMethod]
        public void VolumeColumn()
        {
            const string input = RESOURCE_DIRECTORY + "test_volume_column.xrns";

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);

            SongData songData = songDataFactory.ExtractSongData(renoiseSong, input);

            ModConverter converter = new ModConverter(input);

            converter.Settings = settings;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual(output, "83057508c6d46bdac9b588998a6a5252");
        }        
    }
}
