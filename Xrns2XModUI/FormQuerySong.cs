using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.XPath;
using Xrns2XMod;

namespace Xrns2XModUI
{
    public partial class FormQuerySong : Form
    {

        private string inputFilename;
        private XPathDocument docNav;
        private XPathNavigator nav;        
        private const int maxResults = 100;
        private SongData songData;

        private const string LABEL_TIMING_MODEL_SPEED = "TIMING_MODEL_SPEED";
        private const string LABEL_TIMING_MODEL_LPB = "TIMING_MODEL_LPB";

        public FormQuerySong(string filePath, SongData songData)
        {
            this.inputFilename = filePath;
            this.songData = songData;
            InitializeComponent();            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void FormInfoSong_Load(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(inputFilename, FileMode.Open, FileAccess.Read);

            ZipFile zipFile = new ZipFile(fs);

            ZipEntry zipEntry = zipFile.GetEntry("Song.xml");

            StreamReader streamReader = new StreamReader(zipFile.GetInputStream(zipEntry));

            // Open the XML.
            this.docNav = new XPathDocument(streamReader);

            // Create a navigator to query with XPath.
            nav = docNav.CreateNavigator();

            streamReader.Close();

            zipFile.Close();

            txtXQuery.Text = "Write your XPath query here";

            txtXQuery.SelectAll();

            txtResult.Text = "Some examples: \r\n" +
                "//SequencerTrack/Name \r\n" +
                "//SequencerTrack//Devices//Volume/Value \r\n" +
                "//Patterns/Pattern/NumberOfLines \r\n" +
                "//Patterns/Pattern[1]//Tracks//Line \r\n" +
                "//Patterns/Pattern[1]//Tracks//Line[@index=\"0\"] \r\n";

            labelFilename.Text = inputFilename;

            if (this.songData.PlaybackEngineVersion != Constants.MOD_VERSION_COMPATIBLE)
            {
                this.buttonDowngrade.Enabled = true;
                this.checkBoxReplaceZK.Enabled = true;
                this.labelModelTiming.Text = LABEL_TIMING_MODEL_LPB;
            }
            else
                this.labelModelTiming.Text = LABEL_TIMING_MODEL_SPEED;
        }


        private void executeQuery()
        {
            string query = txtXQuery.Text;

            txtResult.Clear();

            try
            {
                XPathNodeIterator nodeIterator = nav.Select(query);

                if (nodeIterator.Count == 0)
                {
                    txtResult.Text = "No Result";
                }

                if (nodeIterator.Count > maxResults)
                {
                    txtResult.Text = "Too many results";
                    return;
                }

                while (nodeIterator.MoveNext())
                {
                    XPathNavigator nodesNavigator = nodeIterator.Current;

                    XPathNodeIterator nodesText = nodesNavigator.SelectDescendants(XPathNodeType.Text, false);

                    if (nodesText.Count > maxResults)
                    {
                        txtResult.Text += "Too many results in the node \r\n";

                    }
                    else
                    {
                        while (nodesText.MoveNext())
                        {
                            txtResult.Text += nodesText.Current.Value + "\r\n";
                        }
                    }
                    
                }

            }
            catch (XPathException err)
            {
                MessageBox.Show(err.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {            
            this.Dispose();
            this.Close();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            executeQuery();
        }

        private void buttonDowngrade_Click(object sender, EventArgs e)
        {
            XrnsManager xrnsManager = new XrnsManager(inputFilename);

            bool replaceZK = checkBoxReplaceZK.Checked;

            try
            {
                bool result = xrnsManager.DowngradeSong(replaceZK);

                if (result)
                {
                    buttonDowngrade.Enabled = false;
                    this.checkBoxReplaceZK.Enabled = false;
                    this.labelModelTiming.Text = LABEL_TIMING_MODEL_SPEED;                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

    }
}
