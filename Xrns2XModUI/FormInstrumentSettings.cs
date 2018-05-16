using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml.XPath;
using System.Reflection;
using Xrns2XMod;
using System.Collections;

namespace Xrns2XModUI
{
    public partial class FormInstrumentSettings : Form
    {

        private struct InstrumentInfo
        {
            public SampleInfo[] SampleInfo { get; set; }
        }

        private struct SampleInfo
        {
            public int Volume { get; set; }
            public int SampleFreq { get; set; }
            public int SampleFreqOriginal { get; set; }
        }

        private string xrnsFile;
        private SongData songData;
        private InstrumentInfo[] instrumentsInfo;

        public FormInstrumentSettings(SongData songData, string inputFile)
        {
            InitializeComponent();
            this.xrnsFile = inputFile;
            this.songData = songData;
        }

        private void FormSongSettings_Load(object sender, EventArgs e)
        {
            InitInstrumentsTree();
            InitSamplesInfo();
            LoadSamplesSettings();
        }

        private void LoadSamplesSettings()
        {
            IniWrapper iniWrapper = new IniWrapper(xrnsFile, false);

            if (iniWrapper.IsIniLoad)
            {
                for (int ci = 0; ci < treeView1.Nodes.Count; ci++)
                {
                    for (int si = 0; si < treeView1.Nodes[ci].Nodes.Count; si++)
                    {
                        int volume = iniWrapper.ReadDefaultVolumeSample(ci, si);

                        int freq = iniWrapper.ReadFreqSample(ci, si);

                        instrumentsInfo[ci].SampleInfo[si].Volume = volume;

                        if (freq > 0)
                            instrumentsInfo[ci].SampleInfo[si].SampleFreq = freq;
                    }
                }
            }
        }


        private void InitInstrumentsTree()
        {
            FileStream fs = new FileStream(xrnsFile, FileMode.Open, FileAccess.Read);

            ZipFile zipFile = new ZipFile(fs);

            ZipEntry zipEntry = zipFile.GetEntry("Song.xml");

            StreamReader streamReader = new StreamReader(zipFile.GetInputStream(zipEntry));

            XPathDocument docNav;
            XPathNavigator nav;

            // Open the XML.
            docNav = new XPathDocument(streamReader);

            // Create a navigator to query with XPath.
            nav = docNav.CreateNavigator();

            streamReader.Close();

            zipFile.Close();

            const string queryInstruments = "RenoiseSong/Instruments/Instrument";
            const string querySamples = "SampleGenerator/Samples/Sample";

            XPathNodeIterator nodeIteratorInstrument = nav.Select(queryInstruments);
            XPathNodeIterator nodeIteratorSamples;

            int ci = 0;
            int si = 0;

            instrumentsInfo = new InstrumentInfo[nodeIteratorInstrument.Count];

            while (nodeIteratorInstrument.MoveNext())
            {
                si = 0;
                string instrumentName = string.Empty;
                XPathNavigator nodeInstrument = nodeIteratorInstrument.Current;
                XPathNavigator nodeName = nodeInstrument.SelectSingleNode("Name");
                if (nodeName != null)
                    instrumentName = nodeName.Value;
                treeView1.Nodes.Add(ci.ToString(), instrumentName);
                nodeIteratorSamples = nodeInstrument.Select(querySamples);

                instrumentsInfo[ci].SampleInfo = new SampleInfo[nodeIteratorSamples.Count];

                while (nodeIteratorSamples.MoveNext())
                {
                    nodeName = nodeIteratorSamples.Current.SelectSingleNode("Name");
                    string sampleName = string.Empty;
                    if (nodeName != null)
                        sampleName = nodeName.Value;
                    treeView1.Nodes[ci].Nodes.Add(si.ToString(), sampleName);                    
                    
                    si++;
                    
                }
                ci++;
            }

            fs.Close();
        }

        private int[][] GetSampleFreq()
        {
            XrnsManager manager = new XrnsManager(xrnsFile);

            int[][] instrumentFreq = new int[songData.NumInstruments][];

            int ci = 0;
            int si = 0;

            foreach (InstrumentData instrument in songData.Instruments)
            {
                instrumentFreq[ci] = new int[instrument.Samples.Length];

                si = 0;

                foreach (SampleData sample in instrument.Samples)
                {
                    Stream stream = manager.GetSampleStream(ci, si);

                    if (stream != null)
                    {
                        int handle = BassWrapper.GetFlacStream(stream);

                        instrumentFreq[ci][si] = BassWrapper.GetSampleFreq(handle);

                        BassWrapper.FreeStream(handle);

                        stream.Close();
                    }

                    si++;
                }

                ci++;
            }

            return instrumentFreq;

        }

        private void InitSamplesInfo()
        {
            int[][] originalSampleFreq = GetSampleFreq();

            for (int ci = 0; ci < originalSampleFreq.Length; ci++)
                for (int si = 0; si < originalSampleFreq[ci].Length; si++)
                {
                    instrumentsInfo[ci].SampleInfo[si].Volume = 64;
                    instrumentsInfo[ci].SampleInfo[si].SampleFreqOriginal = originalSampleFreq[ci][si];
                    instrumentsInfo[ci].SampleInfo[si].SampleFreq = originalSampleFreq[ci][si];
                }
        }

        private void trackBarVolume_ValueChanged(object sender, EventArgs e)
        {
            nudVolume.Value = ((TrackBar)sender).Value;
        }

        private void nudVolume_ValueChanged(object sender, EventArgs e)
        {
            trackBarVolume.Value = (int)((NumericUpDown)sender).Value;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView tree = (TreeView)sender;

            TreeNode node = tree.SelectedNode;

            if (node.Level == 1)
            {
                nudVolume.Enabled = true;

                cmbFrequency.Enabled = true;

                int instrIndex = node.Parent.Index;

                int sampleIndex = node.Index;

                nudVolume.Value = instrumentsInfo[instrIndex].SampleInfo[sampleIndex].Volume;

                cmbFrequency.Text = instrumentsInfo[instrIndex].SampleInfo[sampleIndex].SampleFreq.ToString();

            }
            else
            {
                nudVolume.Enabled = false;

                cmbFrequency.Enabled = false;
            }
        }

        private void SaveValuesInSampleInfo()
        {
            TreeNode node = treeView1.SelectedNode;

            if (node != null && node.Level == 1)
            {
                int instrIndex = node.Parent.Index;

                int sampleIndex = node.Index;

                int volume = (int)nudVolume.Value;

                ValidateFrequency(cmbFrequency.Text);

                int freq = Int32.Parse(cmbFrequency.Text);

                instrumentsInfo[instrIndex].SampleInfo[sampleIndex].Volume = volume;

                instrumentsInfo[instrIndex].SampleInfo[sampleIndex].SampleFreq = freq;
            }
            
        }
            
        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            SaveValuesInSampleInfo();
        }

        /*
         * Saves only volume values < 64
         * */
        private void SaveAllArrayValuesToFile()
        {
            SaveValuesInSampleInfo();

            IniWrapper iniWrapper = new IniWrapper(xrnsFile, true);

            string iniFolder = Path.GetDirectoryName(iniWrapper.IniPath);

            if (Directory.Exists(iniFolder) == false)
            {
                Directory.CreateDirectory(iniFolder);
            }

            File.Delete(iniWrapper.IniPath);

            for (int ci = 0; ci < instrumentsInfo.Length; ci++)
            {
                for (int si = 0; si < instrumentsInfo[ci].SampleInfo.Length; si++)
                {
                    //int volume = (int)volumeArray[ci][si];

                    //if (volume < 64)
                    //    iniWrapper.SaveDefaultVolumeSample(ci, si, volume);

                    //if (oldSamplesFreq[ci][si] != samplesFreq[ci][si])
                    //    iniWrapper.SaveNewFreqSample(ci, si, samplesFreq[ci][si]);

                    SampleInfo sampleInfo = instrumentsInfo[ci].SampleInfo[si];

                    if (sampleInfo.Volume < 64)
                        iniWrapper.SaveDefaultVolumeSample(ci, si, sampleInfo.Volume);

                    if (sampleInfo.SampleFreq != sampleInfo.SampleFreqOriginal)
                        iniWrapper.SaveNewFreqSample(ci, si, sampleInfo.SampleFreq);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveAllArrayValuesToFile();
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            ResetAll();
        }

        private void ResetAll()
        {
            treeView1.SelectedNode = null;
            nudVolume.Value = nudVolume.Maximum;

            for (int ci=0; ci < instrumentsInfo.Length; ci++)
            {
                for (int si = 0; si < instrumentsInfo[ci].SampleInfo.Length; si++)
                {
                    instrumentsInfo[ci].SampleInfo[si].Volume = 64;
                    instrumentsInfo[ci].SampleInfo[si].SampleFreq = instrumentsInfo[ci].SampleInfo[si].SampleFreqOriginal;
                }
            }            
        }

        private void cmbFrequency_TextChanged(object sender, EventArgs e)
        {
            
                
        }

        private void ValidateFrequency(string txtfreq)
        {
            int value = 0;

            try
            {
                value = Int32.Parse(txtfreq);
            }
            catch (Exception) { }

            if (value < 5000 || value > 96000)
            {
                TreeNode node = treeView1.SelectedNode;

                int instrIndex = node.Parent.Index;

                int sampleIndex = node.Index;

                cmbFrequency.Text = instrumentsInfo[instrIndex].SampleInfo[sampleIndex].SampleFreq.ToString();
            }
        }


    }
}
