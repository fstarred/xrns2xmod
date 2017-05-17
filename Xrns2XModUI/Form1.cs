/*
This file is part of Xrns2XMod.

    Xrns2XMod is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Xrns2XMod is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Xrns2XMod.  If not, see <http://www.gnu.org/licenses/>.
 * */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Xrns2XMod;
using Xrns2XModUI.Properties;
using System.Security.Cryptography;
using System.Threading;

namespace Xrns2XModUI
{
    public partial class Form1 : Form
    {
        private ProcessInfo processInfo;

        private const string LogFileName = "tmpErrLog.txt";

        private const string MESSAGE_SELECT_VALID_SOURCE = "Select a valid source first";

        // DISABLED. To enable, simply set to true
        private bool firstTimeOperation = false;

        // used for function save / saveAs
        private bool isConversionTypeChanged = true;

        private long inputfileLastModified = 0;

        private string outputFileName = string.Empty;
        private string inputFilename = string.Empty;

        private SongData songData;

        private bool isSongDataCorrectlyLoad = false;

        private bool isDowngradeEnabled = false;

        private enum FileType
        {
            MOD,
            XM
        };

        private class ProcessInfo
        {
            public string Log;
            public bool hasErrors;
        }

        private class ConvertXrnsArgs
        {
            public string FileName;
            public FileType FileType;
        }

        private BackgroundWorker BackgroundWorker1;

        public Form1()
        {
            InitializeComponent();

            InitVariables();
        }


        private void InitVariables()
        {
            BackgroundWorker1 = new BackgroundWorker();

            BackgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);

            BackgroundWorker1.WorkerReportsProgress = true;

            //backgroundWorker1.WorkerSupportsCancellation = true;
        }

        private void btOpenDlg_Click(object sender, EventArgs e)
        {
            OpenRenoiseSongFile();
        }

        private void OpenRenoiseSongFile()
        {
            if (BackgroundWorker1.IsBusy) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Renoise module (*.xrns)|*.xrns";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog.FileName;

                ChangeInputFilename(filename);

                LoadXrnsFile(filename);
            }
        }

        private void ChangeInputFilename(string filename)
        {
            this.txtFileName.Text = Path.GetFileName(filename);

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(this.txtFileName, this.txtFileName.Text);
        }

        private void LoadXrnsFile(string fileName)
        {
            if (BackgroundWorker1.IsBusy) return;

            try
            {
                processInfo = new ProcessInfo();

                inputfileLastModified = 0;

                //MainFactory.ReportProgress -= new ProgressHandler(_converterFactory_ReportProgress);
                //MainFactory.ReportProgress -= new ProgressHandler(_loadModule_ReportProgress);
                //MainFactory.ReportProgress += new ProgressHandler(_loadModule_ReportProgress);

                RunBWorker4LoadXrns(fileName);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string openSaveDlg(FileType fileType)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            string filter;
            if (fileType == FileType.MOD)
                filter = "mod file (*.mod)|*.mod";
            else
                filter = "xm file (*.xm)|*.xm";

            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(inputFilename);
            saveFileDialog1.Filter = filter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog1.FileName;
            }
            else
            {
                return null;
            }
        }

        #region backgroundWorker init and run functions

        private void RunBWorker4WritingModule(string fileName, FileType fileType)
        {
            BackgroundWorker1.DoWork -= backgroundWorker1_DoWork_ProcessModule;
            BackgroundWorker1.DoWork -= backgroundWorker1_DoWork_LoadFile;            
            BackgroundWorker1.DoWork += backgroundWorker1_DoWork_ProcessModule;
            
            BackgroundWorker1.RunWorkerCompleted -= backgroundWorker1_RunWorkerCompleted_LoadFile;
            BackgroundWorker1.RunWorkerCompleted -= backgroundWorker1_RunWorkerCompleted_ProcessModule;
            BackgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted_ProcessModule;
            
            ConvertXrnsArgs bwArgs = new ConvertXrnsArgs();

            bwArgs.FileName = fileName;
            bwArgs.FileType = fileType;

            // call backgroundWorker1_DoWork_ProcessModule
            // at end backgroundWorker1_RunWorkerCompleted_ProcessModule
            BackgroundWorker1.RunWorkerAsync(bwArgs);            
        }

        private void RunBWorker4LoadXrns(string fileName)
        {
            BackgroundWorker1.DoWork -= backgroundWorker1_DoWork_ProcessModule;
            BackgroundWorker1.DoWork -= backgroundWorker1_DoWork_LoadFile;
            BackgroundWorker1.DoWork += backgroundWorker1_DoWork_LoadFile;

            BackgroundWorker1.RunWorkerCompleted -= backgroundWorker1_RunWorkerCompleted_ProcessModule;
            BackgroundWorker1.RunWorkerCompleted -= backgroundWorker1_RunWorkerCompleted_LoadFile;
            BackgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted_LoadFile;

            // call backgroundWorker1_DoWork_LoadFile
            BackgroundWorker1.RunWorkerAsync(fileName);
        }

        #endregion

        #region backgroundworker progressChanged event (common)

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            if (e.ProgressPercentage == 0)
            {
                toolStripProgressBar1.ProgressBar.Style = ProgressBarStyle.Marquee;
            }


            if (e.UserState != null)
                toolStripStatusLabel1.Text = e.UserState.ToString();
        }

        #endregion

        #region backgroundWorker Load .xrns file events

        private void backgroundWorker1_DoWork_LoadFile(object sender, DoWorkEventArgs e)
        {
            string filename = (string)e.Argument;

            BackgroundWorker bw = sender as BackgroundWorker;

            bw.ReportProgress(0, "Loading xrns data...");

            SongDataFactory songDataFactory = new SongDataFactory();

            songDataFactory.ReportProgress += load_XrnsEvent;

            RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(filename);

            songData = songDataFactory.ExtractSongData(renoiseSong, filename);

            e.Result = filename;
        }

        private void backgroundWorker1_RunWorkerCompleted_LoadFile(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar1.ProgressBar.Style = ProgressBarStyle.Blocks;

            isSongDataCorrectlyLoad = false;

            if (e.Cancelled)
            {
                // The user canceled the operation.
                MessageBox.Show("Operation was canceled");
            }
            else if (e.Error != null)
            {
                // There was an error during the operation.
                //string msg = String.Format("An error occurred: {0}", e.Error.Message);
                //MessageBox.Show(msg);
                HandleError(this, e.Error);
            }
            else
            {
                // The operation completed normally.
                inputFilename = (string)e.Result;
                //txtFileName.Text = Path.GetFileName( (string)e.Result );

                if (processInfo.hasErrors == false)
                {
                    isSongDataCorrectlyLoad = true;
                    // Carl Corcoran trick
                    numericUpDownTempo.Value = this.songData.InitialBPM;
                    numericUpDownTicks.Value = this.songData.TicksPerLine;
                }
                else
                {
                    MessageBox.Show(processInfo.Log, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            toolStripStatusLabel1.Text = "Ready";

        }

        #endregion

        #region BackgroundWorker Process Module

        private void backgroundWorker1_RunWorkerCompleted_ProcessModule(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar1.ProgressBar.Style = ProgressBarStyle.Blocks;

            if (e.Cancelled)
            {
                // The user canceled the operation.
                MessageBox.Show("Operation was canceled");
            }
            else if (e.Error != null)
            {
                // There was an error during the operation.
                //string msg = String.Format("An error occurred: {0}", e.Error.Message);
                //MessageBox.Show(msg);
                HandleError(this, e.Error);                
            }
            else
            {
                DialogResult dr;

                // The operation has completed normally.
                if (processInfo.hasErrors)
                {
                    dr = MessageBox.Show("JOB COMPLETED WITH SOME ERRORS\nSHOW LOG?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        string filePath = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) + "\\" + LogFileName;
                        File.WriteAllText(filePath, processInfo.Log);
                        Process.Start(filePath);
                    }
                }
                else
                {
                    MessageBox.Show("JOB COMPLETED", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (firstTimeOperation)
                    {
                        firstTimeOperation = false;
                        FormDonation frmDonation = new FormDonation();
                        frmDonation.Show();
                    }
                }
                this.openfileToolStripButton.Enabled = true;
            }

            toolStripStatusLabel1.Text = "Ready";

        }

        private void backgroundWorker1_DoWork_ProcessModule(object sender, DoWorkEventArgs e)
        {
            // Do not access the form's BackgroundWorker reference directly.
            // Instead, use the reference provided by the sender parameter.
            BackgroundWorker bw = sender as BackgroundWorker;

            // Extract the argument.
            ConvertXrnsArgs bwArgs = (ConvertXrnsArgs)e.Argument;

            bw.ReportProgress(0);

            e.Result = ParseModule(bwArgs.FileType, bw, e);

            bw.ReportProgress(99, "Writing output file..");

            Utility.Save2File(bwArgs.FileName, (byte[])e.Result);
        }

        private byte[] ParseModule(FileType fileType, BackgroundWorker worker, DoWorkEventArgs e)
        {
            byte[] output = null;

            IConverter converter = null;

            switch (fileType)
            {
                case FileType.MOD:
                    ModSettings modsettings = Shared.GetModSettingsFromIni();                    

                    converter = new ModConverter(inputFilename);
                    converter.EventProgress += converter_ReportProgress;

                    ((ModConverter)converter).Settings = modsettings;

                    output = converter.Convert(songData);

                    break;
                case FileType.XM:

                    XmSettings xmsettings = Shared.GetXmSettingsFromIni();

                    converter = new XMConverter(inputFilename);
                    converter.EventProgress += converter_ReportProgress;
                    //converter.EventProgress += converter_ReportProgress;

                    ((XMConverter)converter).Settings = xmsettings;

                    xmsettings.Tempo = (int)numericUpDownTempo.Value;
                    xmsettings.TicksRow = (int)numericUpDownTicks.Value;

                    output = converter.Convert(songData);

                    break;
            }

            //if (fileType == FileType.MOD)
            //    output = MainFactory.ConvertToMod(Shared.GetModSettingsFromConfig());
            //else
            //{
            //    XmSettings xmSettings = Shared.GetXmSettingsFromConfig();
            //    xmSettings.Tempo = (int)numericUpDownTempo.Value;
            //    xmSettings.TicksRow = (int)numericUpDownTicks.Value;
            //    output = MainFactory.ConvertToXM(xmSettings);
            //}

            return output;
        }


        #endregion


        private void converter_ReportProgress(object sender, EventReportProgressArgs e)
        {
            if (e.type == MsgType.ERROR)
            {
                processInfo.hasErrors = true;
            }
            else
            {
                BackgroundWorker1.ReportProgress(10, e.message);
            }

            processInfo.Log += string.Format("{0}: {1}\r\n", e.type.ToString(), e.message);
        }


        private void load_XrnsEvent(object sender, EventReportProgressArgs e)
        {
            if (e.type != MsgType.INFO)
                processInfo.hasErrors = true;

            processInfo.Log += e.message;
        }


        public static void HandleError(object sender, Exception e)
        {
            MessageBox.Show(e.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private void SaveAndProcessFile()
        {
            if (isConversionTypeChanged == false)
            {
                FileType fileType = cboTarget.SelectedItem.ToString().Equals("XM") ? FileType.XM : FileType.MOD;

                try
                {
                    ExecuteConversion(fileType);
                }
                catch (ConversionException e)
                {
                    HandleError(this, e);
                }
                catch (ApplicationException e)
                {
                    HandleError(this, e);
                }
            }
            else
            {
                OpenSaveDlgAndProcessFile();
            }

            
        }


        private void OpenSaveDlgAndProcessFile()
        {
            if (BackgroundWorker1.IsBusy) return;

            FileType fileType = cboTarget.SelectedItem.ToString().Equals("XM") ? FileType.XM : FileType.MOD;

            if (isSongDataCorrectlyLoad == false)
            {
                MessageBox.Show(MESSAGE_SELECT_VALID_SOURCE, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    outputFileName = this.openSaveDlg(fileType);

                    if (outputFileName != null)
                    {
                        ExecuteConversion(fileType);
                    }

                }
                catch (Exception e)
                {
                    HandleError(this, e);
                }
            }
        }


        private long GetInputFileLastTimeModified(string filename)
        {
            DateTime dtlw = File.GetLastWriteTime(filename);

            DateTime dtct = File.GetCreationTime(filename);

            long dt2compare = (DateTime.Compare(dtlw, dtct) > 0 ? dtlw : dtct).Ticks;

            return dt2compare;
        }

        private void ExecuteConversion(FileType fileType)
        {
            long dt2compare = GetInputFileLastTimeModified(inputFilename);

            if (dt2compare != inputfileLastModified)
            {
                inputfileLastModified = dt2compare;
                //MainFactory.LoadXrns(inputFileName);

                SongDataFactory songDataFactory = new SongDataFactory();

                songDataFactory.ReportProgress += load_XrnsEvent;

                RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(inputFilename);
                songData = songDataFactory.ExtractSongData(renoiseSong, inputFilename);
            }

            byte[] outputStream = new byte[0];

            processInfo = new ProcessInfo();

            //MainFactory.ReportProgress -= new ProgressHandler(_loadModule_ReportProgress);
            //MainFactory.ReportProgress -= new ProgressHandler(_converterFactory_ReportProgress);
            //MainFactory.ReportProgress += new ProgressHandler(_converterFactory_ReportProgress);

            RunBWorker4WritingModule(outputFileName, fileType);

            isConversionTypeChanged = false;
        }

        private void ReloadXrns()
        {
            if (!string.IsNullOrEmpty(inputFilename))
            {
                LoadXrnsFile(inputFilename);
            }
        }


        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            AboutXrns2Mod aboutXrns2Mod = new AboutXrns2Mod();
            aboutXrns2Mod.ShowDialog();
        }

        private void optionsToolStripButton_Click(object sender, EventArgs e)
        {
            FormSettings frmSettings = new FormSettings();
            frmSettings.ShowDialog();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenRenoiseSongFile();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            OpenSaveDlgAndProcessFile();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            // inputfileLastModified == 0 new file loaded
            if (inputfileLastModified == 0)                
                OpenSaveDlgAndProcessFile();
            else
                SaveAndProcessFile();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string bassEmail = Settings.Default.BassEmail;
            string bassCode = Settings.Default.BassCode;

            //MainFactory.InitResources(this.Handle, bassEmail, bassCode);

            BassWrapper.InitResources(this.Handle, bassEmail, bassCode);

            Shared.CheckForUpdates(false);

            cboTarget.SelectedItem = "XM";
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenRenoiseSongFile();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void infoStripButton_Click(object sender, EventArgs e)
        {
            if (isSongDataCorrectlyLoad)
            {
                FormQuerySong frmInfoSong = new FormQuerySong(inputFilename, songData);
                frmInfoSong.FormClosed += (sen, ev) => 
                {
                    ReloadXrns();
                };
                frmInfoSong.Show();
            }
            else
            {
                MessageBox.Show(MESSAGE_SELECT_VALID_SOURCE, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //MainFactory.FreeResources();

            BassWrapper.FreeResources();
        }

        private void reloadToolStripButton_Click(object sender, EventArgs e)
        {
            ReloadXrns();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            // make sure they're actually dropping files (not text or anything else)
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                // allow them to continue
                // (without this, the cursor stays a "NO" symbol
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length == 1)
                {
                    if (Path.GetExtension(files[0]).Equals(".xrns", StringComparison.InvariantCultureIgnoreCase))
                        e.Effect = DragDropEffects.All;
                }
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            // transfer the filenames to a string array
            // (yes, everything to the left of the "=" can be put in the 
            // foreach loop in place of "files", but this is easier to understand.)
            string file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];

            ChangeInputFilename(file);

            // load file
            this.LoadXrnsFile(file);
        }

        private void speedToolStripButton_Click(object sender, EventArgs e)
        {
            int bpm = 120;
            int lpb = 4;

            if (isSongDataCorrectlyLoad)
            {
                //bpm = songData.InitialBPM;
                //lpb = songData.LinesPerBeat;

                bpm = (int)numericUpDownTempo.Value;
                lpb = songData.LinesPerBeat;
            }

            FormCalculator frmCalculator = new FormCalculator(bpm, lpb);

            frmCalculator.Show();
        }

        //private void txtFileName_TextChanged(object sender, EventArgs e)
        //{
        //    ToolTip toolTip = new ToolTip();
        //    toolTip.SetToolTip(this.txtFileName, this.txtFileName.Text);
        //}

        private void cboTarget_SelectedValueChanged(object sender, EventArgs e)
        {
            isConversionTypeChanged = true;
            groupBoxInitialValues.Enabled = (((ComboBox)sender).SelectedItem.Equals(FileType.XM.ToString()));
        }

        private void volumeStripButton_Click(object sender, EventArgs e)
        {
            if (isSongDataCorrectlyLoad)
            {
                FormInstrumentSettings formSongSettings = new FormInstrumentSettings(songData, inputFilename);
                formSongSettings.FormClosed += new FormClosedEventHandler(formSongSettings_FormClosed);
                formSongSettings.ShowDialog();
            }
            else
            {
                MessageBox.Show(MESSAGE_SELECT_VALID_SOURCE, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        void formSongSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            ReloadXrns();
        }

        private void openfileToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileLocation();
        }
        
        private void OpenFileLocation()
        {
            if (!File.Exists(outputFileName) || Utility.IsWindowsOS() == false)
            {
                return;
            }

            // combine the arguments together
            // it doesn't matter if there is a space after ','
            string argument = @"/select, " + outputFileName;

            Process.Start("explorer.exe", argument);
        }

        private void txtFileName_MouseHover(object sender, EventArgs e)
        {
            if (isSongDataCorrectlyLoad)
            {
                ToolTip toolTip = new ToolTip();
                toolTip.SetToolTip(this.txtFileName, inputFilename);
            }            
        }

        private void downgradeToolStripButton_Click(object sender, EventArgs e)
        {
            if (isSongDataCorrectlyLoad && isDowngradeEnabled)
            {
                XrnsManager manager = new XrnsManager(inputFilename);

                
            }
        }

    }
}
