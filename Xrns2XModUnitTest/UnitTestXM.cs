using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Xrns2XMod;

namespace Xrns2XModUnitTest
{
    [TestClass]
    public class UnitTestXM
    {
        SongDataFactory songDataFactory;
        XmSettings settings;
        XMConverter converter;
        SongData songData;

        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext ctx)
        {
            ctx.WriteLine("Initializing XM Unit Test class");
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
            converter = new XMConverter(input);

            settings = new XmSettings();
            settings.Tempo = songData.InitialBPM;
            settings.TicksRow = songData.TicksPerLine;
            converter.Settings = settings;
        }

        [Resource("test_adjust_sample_frequency.xrns")]
        [TestMethod]
        public void AdjustSampleFrequency()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("52e02bc86985f0f83435aca9ee62efe5", output);
        }

        [Resource("test_default_volume.xrns")]
        [TestMethod]
        public void DefaultVolume()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("6774ec1c292beece37a04fc17ac2e45a", output);
        }

        [Resource("test_delay_column.xrns")]
        [TestMethod]
        public void DelayColumn()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("f67a94b3a54bae37d663c05ff4502001", output);
        }

        [Resource("test_fade_volume_compatibility_trick.xrns")]
        [TestMethod]
        public void FadeVolumeCompatibilityTrick()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("0c79bbb58ff4a8e9e1a4c958c6dacd27", output);
        }


        [Resource("test_ft2_mode.xrns")]
        [TestMethod]
        public void FT2Mode()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("1a8ecc9dee9a3678ee4224cc9c346430", output);
        }


        [Resource("test_global_commands.xrns")]
        [TestMethod]
        public void GlobalCommands()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("fc1c3b52e9d722901670049401320e93", output);
        }

        [Resource("test_global_volume_xm_commands.xrns")]
        [TestMethod]
        public void GlobalVolumeXmCommands()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("ae1df1b1cf5a16a7d35cc20624d7fe51", output);
        }

        [Resource("test_instrument_envelopes.xrns")]
        [TestMethod]
        public void InstrumentEnvelopes()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("7c741e1f44514bc4a94b90bc0849d54e", output);
        }

        [Resource("test_instrument_keymap.xrns")]
        [TestMethod]
        public void InstrumentKeymap()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("5e746efcc1493974a218b1ede021dde7", output);
        }

        [Resource("test_instruments_commands.xrns")]
        [TestMethod]
        public void InstrumentsCommands()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("3c450c54fa62f6e7a00a6587203c2988", output);
        }

        [Resource("test_multi_columns.xrns")]
        [TestMethod]
        public void MultiColumns()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("aacd817279de9c55add5f4a5f05736cf", output);
        }

        [Resource("test_panning_column.xrns")]
        [TestMethod]
        public void PanningColumn()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("4107eed8586d6b0395fa03570ead5a74", output);
        }

        [Resource("test_renoise_standard_tpl_mode.xrns")]
        [TestMethod]
        public void RenoiseStandardTPLMode()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("a9cf4bb798ba9d472021a8a8bca598ae", output);
        }

        [Resource("test_sample_commands.xrns")]
        [TestMethod]
        public void SampleCommands()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("0d559b8c089e83e3788e4d95d5d877fd", output);
        }

        [Resource("test_tick_commands.xrns")]
        [TestMethod]
        public void TickCommands()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("513cab45a102ac003cd754a54c8ef14a", output);
        }

        [Resource("test_volume_column.xrns")]
        [TestMethod]
        public void VolumeColumn()
        {
            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("c89468b202520aa6dfec2224cfdffedc", output);
        }

        [Resource("test_volume_scaling_mode.xrns")]
        [TestMethod]
        public void VolumeScalingMode()
        {
            converter.Settings.VolumeScalingMode = VOLUME_SCALING_MODE.COLUMN;

            byte[] bytes = converter.Convert(songData);

            string output = MD5Utils.GenerateMd5Hash(bytes);

            Assert.AreEqual("04f4ae7c82e87c5382c28fc1ef438600", output);            
        }
    }
}
