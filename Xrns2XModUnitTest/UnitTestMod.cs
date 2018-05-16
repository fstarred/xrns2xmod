using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;
using Xrns2XMod;

namespace Xrns2XModUnitTest
{
    [TestClass]
    public class UnitTestMod
    {
        SongDataFactory songDataFactory;
        ModSettings settings;
        ModConverter converter;
        SongData songData;

        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext ctx)
        {
            ctx.WriteLine("Initializing MOD Unit Test class");
            BassWrapper.InitResources(IntPtr.Zero, null, null);
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            BassWrapper.FreeResources();
        }

        [TestInitialize]
        public void InitializeMethod()
        {
            songDataFactory = new SongDataFactory();
            
            string theClassName = TestContext.FullyQualifiedTestClassName;
            string testName = TestContext.TestName;

            // NOTE: You might have to use AppDomain.CurrentDomain.GetAssemblies() 
            // and then call GetTypes on each assembly if this code
            // resides in a baseclass in another assembly. 
            var currentlyRunningClassType = this.GetType()
                .Assembly
                .GetTypes()
                .FirstOrDefault(f => f.FullName == theClassName);

            var currentlyRunningMethod = currentlyRunningClassType.GetMethod(testName);

            var resourceAttribute = currentlyRunningMethod.
                GetCustomAttributes<Resource>()
                .FirstOrDefault();

            string input = this.TestContext.Properties["resource_path"].ToString() + resourceAttribute.Path;
            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(input);
            songData = songDataFactory.ExtractSongData(renoiseSong, input);
            converter = new ModConverter(input);

            settings = new ModSettings();
            settings.ForceProTrackerCompatibility = true;
            settings.PortamentoLossThreshold = 2;
            settings.VolumeScalingMode = VOLUME_SCALING_MODE.SAMPLE;

            converter.Settings = settings;
        }

        [Resource("test_adjust_sample_frequency.xrns")]
        [TestMethod]
        public void AdjustSampleFrequency()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("2e36279920d991ad20241d5812f035bd", output);
        }

        [Resource("test_default_volume.xrns")]
        [TestMethod]
        public void DefaultVolume()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("8a04aaaa005b76f33f145f5ba61200ed", output);
        }

        [Resource("test_delay_column.xrns")]
        [TestMethod]
        public void DelayColumn()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("34a34fcf4fb6bf036c298a773102101a", output);
        }

        [Resource("test_fade_volume_compatibility_trick.xrns")]
        [TestMethod]
        public void FadeVolumeCompatibilityTrick()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("435ada05f16686e29198a963e276fb8f", output);
        }

        [Resource("test_ft2_mode.xrns")]
        [TestMethod]
        public void FT2Mode()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("48a7a50abfd384b5ed318c940be623e6", output);
        }

        [Resource("test_global_commands.xrns")]
        [TestMethod]
        public void GlobalCommands()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("433db7eaf631d2a3c6b2172518ee8442", output);
        }

        [Resource("test_instruments_commands.xrns")]
        [TestMethod]
        public void InstrumentsCommands()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("fe95f07432539b31e25446863e22f4e7", output);
        }

        [Resource("test_mod_sample_conversion.xrns")]
        [TestMethod]
        public void ModSampleConversion()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("56c9c667a857b823411cea57edaf12cf", output);
        }

        [Resource("test_multi_columns.xrns")]
        [TestMethod]
        public void MultiColumns()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("2c7591e96f820b08b2f3ce3ab5c3a7a2", output);
        }

        [Resource("test_panning_column.xrns")]
        [TestMethod]
        public void PanningColumn()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("20646321e170f66f1933d69f074fc829", output);
        }

        [Resource("test_renoise_standard_tpl_mode.xrns")]
        [TestMethod]
        public void RenoiseStandardTPLMode()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("341120418fdbcaf06bd46bef7c0c9844", output);
        }

        [Resource("test_sample_commands.xrns")]
        [TestMethod]
        public void SampleCommands()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("4ad75feec9349ffc6f26d25c7d89392e", output);
        }

        [Resource("test_tick_commands.xrns")]
        [TestMethod]
        public void TickCommands()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("0d4e432d6d758463713ee637db6ecbec", output);
        }

        [Resource("test_volume_column.xrns")]
        [TestMethod]
        public void VolumeColumn()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("83057508c6d46bdac9b588998a6a5252", output);
        }        
    }
}
