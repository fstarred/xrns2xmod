using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Xrns2XMod.Properties;

namespace Xrns2XModUI
{
    public partial class FormCalculator : Form
    {

        private static string[] notesArray = { "C-", "C#", "D-", "D#", "E-", "F-", "F#", "G-", "G#", "A-", "A#", "B-" };
        private static string[] renoiseNotesArray = new string[120];                    

        private enum TempoVisMode
        {
            DECIMAL,
            HEX
        };
        private TempoVisMode currentMode;

        private int ticksPerRow;

        // prevent unwanted nud events to be called
        bool anotherEventIsAlreadyFired = false;

        public FormCalculator(int bpm, int lpb)
        {
            InitializeComponent();

            currentMode = TempoVisMode.DECIMAL;

            this.nudBPM.Value = bpm;            
            this.nudLPB.Value = lpb;
            this.nudTPR.Value = 6;

            MatchRenoiseSpeedByTPR();
            
        }

        private void MatchRenoiseSpeedByTPR()
        {
            int bpm = (int)this.nudBPM.Value;
            int lpb = (int)this.nudLPB.Value;

            int tpr = (int)this.nudTPR.Value;

            int renoiseRealBPM = bpm * lpb / 4;

            short tempo = (short)(renoiseRealBPM * tpr / 6);

            if (tempo < this.nudTempo.Minimum || tempo > this.nudTempo.Maximum)
            {
                MessageBox.Show("Module tempo is too low or too high", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                this.nudTempo.Value = tempo;
                //this.txtDestinationRBPM.Text = GetCurrentFormatValue( renoiseRealBPM.ToString(), 10 );
                this.txtDestinationRBPM.Text = renoiseRealBPM.ToString();
            }

        }
        

        private void CalculateRenoiseRealBPM()
        {
            int bpm = (int)this.nudBPM.Value;
            int lpb = (int)this.nudLPB.Value;

            int renoiseRealBPM = bpm * lpb / 4;

            //this.txtRenoiseRBPM.Text = GetCurrentFormatValue(renoiseRealBPM.ToString(), 10);
            this.txtRenoiseRBPM.Text = renoiseRealBPM.ToString();
        }

        private void CalculateDestModuleRealBPM()
        {
            int tempo = (int)this.nudTempo.Value;
            int tpr = (int)this.nudTPR.Value;

            int destinationRealBPM = tempo * 6 / tpr;

            //this.txtDestinationRBPM.Text = GetCurrentFormatValue(destinationRealBPM.ToString(), 10);
            this.txtDestinationRBPM.Text = destinationRealBPM.ToString();
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            MatchRenoiseSpeedByTPR();
        }

        private void renNud_Changed(object sender, EventArgs e)
        {
            CalculateRenoiseRealBPM();
        }

        private void destNud_Changed(object sender, EventArgs e)
        {
            ticksPerRow = (int)this.nudTPR.Value;
            CalculateDestModuleRealBPM();
            UpdateAllTPRNudValues();
        }

        private void btnDecHex_Click(object sender, EventArgs e)
        {
            if (currentMode == TempoVisMode.DECIMAL)
            {
                currentMode = TempoVisMode.HEX;
                this.btnDecHex.Text = "Dec";
            }
            else
            {
                currentMode = TempoVisMode.DECIMAL;
                this.btnDecHex.Text = "Hex";
            }
            //this.nudBPM.Value = GetCurrentFormatValue(this.nudBPM.Value, 16);
            //this.nudTempo.Text = GetCurrentFormatValue(this.nudTempo.Value, 16);                

            //this.nudBPM.Hexadecimal = (currentMode == TempoVisMode.HEX);
            //this.nudTempo.Hexadecimal = (currentMode == TempoVisMode.HEX);
            this.nudBPM.Hexadecimal = !this.nudBPM.Hexadecimal;
            this.nudTempo.Hexadecimal = !this.nudTempo.Hexadecimal;            

        }

        private void destNud2_Changed(object sender, EventArgs e)
        {
            ticksPerRow = (int)this.nudTPR2.Value;
            UpdateAllTPRNudValues();
            CalculateDestModuleRealBPM();
            CalculateDestPitch();
            CalculateDestSlide();
        }

        private void UpdateAllTPRNudValues()
        {
            this.nudTPR.Value = ticksPerRow;
            this.nudTPR2.Value = ticksPerRow;
        }

        private void nudRenPitchChanged(object sender, EventArgs e)
        {
            CalculateDestPitch();
        }

        private void CalculateDestPitch()
        {
            int divider = (ticksPerRow - 1);
            if (divider == 0) divider = 1;
            int destinationPitch = (int)this.nudRenPitchNote.Value / divider;
            int pitchPrecLoss = (int)this.nudRenPitchNote.Value % divider;
            //this.nudDestPitchNote.Value = (int)GetCurrentFormatValue(destinationPitch.ToString(), 16, TempoVisMode.HEX);
            this.nudDestPitchNote.Value = destinationPitch;
            this.lblPitchLoss.Text = pitchPrecLoss.ToString();
        }

        private void CalculateOriginalPitch()
        {
            int multiplier = (ticksPerRow - 1);
            if (multiplier == 0) multiplier = 1;
            int originalPitch = (int)this.nudDestPitchNote.Value * multiplier;
            int originalMaxValue = (int)this.nudRenPitchNote.Maximum; 
            int pitchPrecLoss = (int)this.nudRenPitchNote.Value % multiplier;
            if (originalPitch > originalMaxValue)
            {
                pitchPrecLoss = originalPitch - originalMaxValue;
                originalPitch = originalMaxValue;
            }
            this.nudRenPitchNote.Value = originalPitch;
            this.lblPitchLoss.Text = pitchPrecLoss.ToString();
        }

        private void CalculateDestSlide()
        {
            anotherEventIsAlreadyFired = true;
            int divisor = (ticksPerRow - 1);
            if (divisor == 0) divisor = 1;
            int originalValue = (int)this.nudRenSlide.Value;
            int destinationSlide = (originalValue / divisor) >> 2 ;            
            int maxValue = (int)this.nudDestSlide.Maximum;
            if (destinationSlide > maxValue) destinationSlide = maxValue;

            int divisorMod = (destinationSlide << 2) * divisor;
            /*
             * (((originalValue / divisorMod) - 1) * divisorMod) save high loss precision values to be lost, because 120 % 60 return 0;
             * 120 % 60 + (((120 / 60) - 1) * 60) = 0 + ((2 - 1) * 60) = 0 + 60; 60 is the right result
             * */
            int slidePrecLoss = destinationSlide > 0 ? originalValue % divisorMod + (((originalValue / divisorMod) - 1) * divisorMod) : originalValue;
            this.nudDestSlide.Value = destinationSlide;
            this.lblSlidePrecLoss.Text = slidePrecLoss.ToString();
            this.lblRightArrow.Visible = true;
            this.lblLeftArrow.Visible = false;
            anotherEventIsAlreadyFired = false;
        }

        private void CalculateOriginalSlide()
        {
            anotherEventIsAlreadyFired = true;
            int divider = (ticksPerRow - 1);
            if (divider == 0) divider = 1;
            int destinationValue = (int)this.nudDestSlide.Value;
            int originalValue = (destinationValue << 2) * divider;
            int maxValue = (int)this.nudRenSlide.Maximum;
            int precisionLoss = 0;
            if (originalValue > maxValue)
            {
                precisionLoss = originalValue - maxValue;
                originalValue = maxValue;                
            }
            this.nudRenSlide.Value = originalValue;
            this.lblSlidePrecLoss.Text = precisionLoss.ToString();
            this.lblRightArrow.Visible = false;
            this.lblLeftArrow.Visible = true;
            anotherEventIsAlreadyFired = false;
        }




        private string GetCurrentFormatValue(string text, int baseNum, TempoVisMode mode)
        {
            if (mode == TempoVisMode.DECIMAL)
            {
                return Convert.ToInt16(text, baseNum).ToString();
            }
            else
            {
                return String.Format("0x{0:X}", (Int16.Parse(text)));
            }
        }

        private void nudSlide_Changed(object sender, EventArgs e)
        {
            if (!anotherEventIsAlreadyFired)
                CalculateDestSlide();
        }

        private void nudDest_Changed(object sender, EventArgs e)
        {
            if (!anotherEventIsAlreadyFired)
                CalculateOriginalSlide();
        }

        private void FormSpeedCalculator_Load(object sender, EventArgs e)
        {
            ToolTip toolTip = new ToolTip();            
            toolTip.SetToolTip(lblPitchNote, "Renoise: 1xx, 2xx, 5xx\nMOD: 1xx, 2xx, 3xx");
            toolTip.SetToolTip(lblVolumeSlide, "Renoise: 6xx, 7xx\nMOD: Axy");

            for (int i = 0; i < 120; i++)
            {
                renoiseNotesArray[i] = notesArray[i % 12] + (i / 12).ToString();                
            }
            cmbNotes.Items.AddRange(renoiseNotesArray);                
            cmbNotes.SelectedIndex = 48;
            cmbFormat.SelectedIndex = 0;
            cmbFrequency.Text = "44100";
        }

        private void nudDestPitchNote_ValueChanged(object sender, EventArgs e)
        {
            CalculateOriginalPitch();
        }


        private static int GetRelNoteByFrequency(int renBaseNote, int renFineTuning, int sampleRate)
        {
            // range value in Renoise starts from 0 to 119

            // Thanks to Jojo of OpenMPT for giving me the right code

            const int defaultNote = 48; /* C-4 for Renoise */

            int relativeTone = 0;
            int fineTune = 0;

            int renoiseValue2Add = defaultNote - renBaseNote;

            int f2t = (int)(1536.0 * (Math.Log((double)sampleRate / 8363.0) / Math.Log(2.0)));
            int transp = f2t >> 7;
            int ftune = f2t & 0x7F; //0x7F == 111 1111 

            ftune += renFineTuning;
            if (ftune > 80)
            {
                transp++;
                ftune -= 128;
            }
            if (transp > 127) transp = 127;
            if (transp < -127) transp = -127;

            relativeTone = transp;
            fineTune = ftune;

            relativeTone += renoiseValue2Add;

            return relativeTone;
        }

        private void btnCalcRelNote_Click(object sender, EventArgs e)
        {
            const int modAmigaNoteRange = 36;
            const int modExtNoteRange = 72;
            const int xmNoteRange = 96;

            try
            {
                int relativeNote = cmbNotes.SelectedIndex;
                int fineTune = (int)nudFineTune.Value;
                int sampleRate = Convert.ToInt32(cmbFrequency.Text);

                int note = GetRelNoteByFrequency(relativeNote, fineTune, sampleRate);

                int minNote = 0;
                int maxNote = 0;

                switch (cmbFormat.SelectedIndex)
                {
                    case 0: // MOD (Amiga)
                        minNote = 36 - note;
                        maxNote = minNote + modAmigaNoteRange - 1;
                        break;
                    case 1: // MOD (Ext.)
                        minNote = 24 - note;
                        maxNote = minNote + modExtNoteRange - 1;
                        break;
                    case 2: // XM
                        minNote = 12;
                        maxNote = xmNoteRange - 1;
                        break;
                }

                if (minNote < 0) minNote = 0;
                if (maxNote > 120) maxNote = 120;

                txtMinNote.Text = renoiseNotesArray[minNote];
                txtMaxNote.Text = renoiseNotesArray[maxNote];
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void MatchRenoiseSpeedByBPM()
        {            
            int bpm = (int)this.nudBPM.Value;
            int lpb = (int)this.nudLPB.Value;

            int renoiseRealBPM = bpm * lpb / 4;
            
            short tpr = (short)(bpm * 6 / renoiseRealBPM);

            int moduleRealBPM = bpm * 6 / tpr;

            if (tpr < this.nudTPR.Minimum || tpr > this.nudTPR.Maximum)
            {
                MessageBox.Show("Module tempo is too low or too high", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (moduleRealBPM != renoiseRealBPM)
            {
                MessageBox.Show("Unable to find exact tpr value", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                this.nudTPR.Value = tpr;
                this.nudTempo.Value = bpm;
                this.txtDestinationRBPM.Text = moduleRealBPM.ToString();
            }

        }

        private void btnCalcTPR_Click(object sender, EventArgs e)
        {
            MatchRenoiseSpeedByBPM();
        }
    }
}
