namespace Xrns2XModUI
{
    partial class FormCalculator
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtRenoiseRBPM = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.nudBPM = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nudLPB = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtDestinationRBPM = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.nudTPR = new System.Windows.Forms.NumericUpDown();
            this.nudTempo = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCalcBPM = new System.Windows.Forms.Button();
            this.btnDecHex = new System.Windows.Forms.Button();
            this.tabTools = new System.Windows.Forms.TabControl();
            this.tabTempo = new System.Windows.Forms.TabPage();
            this.btnCalcTPR = new System.Windows.Forms.Button();
            this.tabPitchSlide = new System.Windows.Forms.TabPage();
            this.lblLeftArrow = new System.Windows.Forms.Label();
            this.lblRightArrow = new System.Windows.Forms.Label();
            this.lblSlidePrecLoss = new System.Windows.Forms.Label();
            this.lblPitchLoss = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.nudDestPitchNote = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.nudDestSlide = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.nudTPR2 = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.nudRenSlide = new System.Windows.Forms.NumericUpDown();
            this.lblVolumeSlide = new System.Windows.Forms.Label();
            this.lblPitchNote = new System.Windows.Forms.Label();
            this.nudRenPitchNote = new System.Windows.Forms.NumericUpDown();
            this.tabNotes = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtMaxNote = new System.Windows.Forms.TextBox();
            this.txtMinNote = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.cmbFormat = new System.Windows.Forms.ComboBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cmbFrequency = new System.Windows.Forms.ComboBox();
            this.cmbNotes = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.nudFineTune = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnCalcRelNote = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBPM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLPB)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTPR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTempo)).BeginInit();
            this.tabTools.SuspendLayout();
            this.tabTempo.SuspendLayout();
            this.tabPitchSlide.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDestPitchNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDestSlide)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTPR2)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRenSlide)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRenPitchNote)).BeginInit();
            this.tabNotes.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFineTune)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtRenoiseRBPM);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.nudBPM);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.nudLPB);
            this.groupBox1.Location = new System.Drawing.Point(28, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(161, 124);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Renoise";
            // 
            // txtRenoiseRBPM
            // 
            this.txtRenoiseRBPM.BackColor = System.Drawing.SystemColors.Window;
            this.txtRenoiseRBPM.Location = new System.Drawing.Point(93, 97);
            this.txtRenoiseRBPM.Name = "txtRenoiseRBPM";
            this.txtRenoiseRBPM.ReadOnly = true;
            this.txtRenoiseRBPM.Size = new System.Drawing.Size(50, 20);
            this.txtRenoiseRBPM.TabIndex = 6;
            this.txtRenoiseRBPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Real BPM";
            // 
            // nudBPM
            // 
            this.nudBPM.Location = new System.Drawing.Point(93, 27);
            this.nudBPM.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudBPM.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudBPM.Name = "nudBPM";
            this.nudBPM.Size = new System.Drawing.Size(50, 20);
            this.nudBPM.TabIndex = 4;
            this.nudBPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudBPM.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudBPM.ValueChanged += new System.EventHandler(this.renNud_Changed);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Lines / Beat";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Beats / Min";
            // 
            // nudLPB
            // 
            this.nudLPB.Location = new System.Drawing.Point(93, 63);
            this.nudLPB.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.nudLPB.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLPB.Name = "nudLPB";
            this.nudLPB.Size = new System.Drawing.Size(50, 20);
            this.nudLPB.TabIndex = 1;
            this.nudLPB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudLPB.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLPB.ValueChanged += new System.EventHandler(this.renNud_Changed);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtDestinationRBPM);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.nudTPR);
            this.groupBox2.Controls.Add(this.nudTempo);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(246, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(161, 124);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Destination";
            // 
            // txtDestinationRBPM
            // 
            this.txtDestinationRBPM.BackColor = System.Drawing.SystemColors.Window;
            this.txtDestinationRBPM.Location = new System.Drawing.Point(93, 97);
            this.txtDestinationRBPM.Name = "txtDestinationRBPM";
            this.txtDestinationRBPM.ReadOnly = true;
            this.txtDestinationRBPM.Size = new System.Drawing.Size(50, 20);
            this.txtDestinationRBPM.TabIndex = 8;
            this.txtDestinationRBPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Real BPM";
            // 
            // nudTPR
            // 
            this.nudTPR.Location = new System.Drawing.Point(93, 63);
            this.nudTPR.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudTPR.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTPR.Name = "nudTPR";
            this.nudTPR.Size = new System.Drawing.Size(50, 20);
            this.nudTPR.TabIndex = 1;
            this.nudTPR.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudTPR.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTPR.ValueChanged += new System.EventHandler(this.destNud_Changed);
            // 
            // nudTempo
            // 
            this.nudTempo.Location = new System.Drawing.Point(93, 27);
            this.nudTempo.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.nudTempo.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudTempo.Name = "nudTempo";
            this.nudTempo.Size = new System.Drawing.Size(50, 20);
            this.nudTempo.TabIndex = 4;
            this.nudTempo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudTempo.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudTempo.ValueChanged += new System.EventHandler(this.destNud_Changed);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Ticks / Row";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Tempo";
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(387, 220);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCalcBPM
            // 
            this.btnCalcBPM.Location = new System.Drawing.Point(195, 118);
            this.btnCalcBPM.Name = "btnCalcBPM";
            this.btnCalcBPM.Size = new System.Drawing.Size(45, 23);
            this.btnCalcBPM.TabIndex = 7;
            this.btnCalcBPM.Text = ">>";
            this.toolTip.SetToolTip(this.btnCalcBPM, "Adjust Tempo by Real BPM");
            this.btnCalcBPM.UseVisualStyleBackColor = true;
            this.btnCalcBPM.Click += new System.EventHandler(this.btnCalc_Click);
            // 
            // btnDecHex
            // 
            this.btnDecHex.Location = new System.Drawing.Point(195, 47);
            this.btnDecHex.Name = "btnDecHex";
            this.btnDecHex.Size = new System.Drawing.Size(45, 23);
            this.btnDecHex.TabIndex = 8;
            this.btnDecHex.Text = "Hex";
            this.btnDecHex.UseVisualStyleBackColor = true;
            this.btnDecHex.Click += new System.EventHandler(this.btnDecHex_Click);
            // 
            // tabTools
            // 
            this.tabTools.Controls.Add(this.tabTempo);
            this.tabTools.Controls.Add(this.tabPitchSlide);
            this.tabTools.Controls.Add(this.tabNotes);
            this.tabTools.Location = new System.Drawing.Point(12, 12);
            this.tabTools.Name = "tabTools";
            this.tabTools.SelectedIndex = 0;
            this.tabTools.Size = new System.Drawing.Size(454, 202);
            this.tabTools.TabIndex = 9;
            // 
            // tabTempo
            // 
            this.tabTempo.BackColor = System.Drawing.Color.Transparent;
            this.tabTempo.Controls.Add(this.btnCalcTPR);
            this.tabTempo.Controls.Add(this.groupBox1);
            this.tabTempo.Controls.Add(this.btnDecHex);
            this.tabTempo.Controls.Add(this.groupBox2);
            this.tabTempo.Controls.Add(this.btnCalcBPM);
            this.tabTempo.Location = new System.Drawing.Point(4, 22);
            this.tabTempo.Name = "tabTempo";
            this.tabTempo.Padding = new System.Windows.Forms.Padding(3);
            this.tabTempo.Size = new System.Drawing.Size(446, 176);
            this.tabTempo.TabIndex = 0;
            this.tabTempo.Text = "Tempo";
            // 
            // btnCalcTPR
            // 
            this.btnCalcTPR.Location = new System.Drawing.Point(195, 83);
            this.btnCalcTPR.Name = "btnCalcTPR";
            this.btnCalcTPR.Size = new System.Drawing.Size(45, 23);
            this.btnCalcTPR.TabIndex = 9;
            this.btnCalcTPR.Text = ">>";
            this.toolTip.SetToolTip(this.btnCalcTPR, "Adjust TPR By TPL");
            this.btnCalcTPR.UseVisualStyleBackColor = true;
            this.btnCalcTPR.Click += new System.EventHandler(this.btnCalcTPR_Click);
            // 
            // tabPitchSlide
            // 
            this.tabPitchSlide.BackColor = System.Drawing.Color.Transparent;
            this.tabPitchSlide.Controls.Add(this.lblLeftArrow);
            this.tabPitchSlide.Controls.Add(this.lblRightArrow);
            this.tabPitchSlide.Controls.Add(this.lblSlidePrecLoss);
            this.tabPitchSlide.Controls.Add(this.lblPitchLoss);
            this.tabPitchSlide.Controls.Add(this.label12);
            this.tabPitchSlide.Controls.Add(this.groupBox4);
            this.tabPitchSlide.Controls.Add(this.groupBox3);
            this.tabPitchSlide.Location = new System.Drawing.Point(4, 22);
            this.tabPitchSlide.Name = "tabPitchSlide";
            this.tabPitchSlide.Padding = new System.Windows.Forms.Padding(3);
            this.tabPitchSlide.Size = new System.Drawing.Size(446, 176);
            this.tabPitchSlide.TabIndex = 1;
            this.tabPitchSlide.Text = "Pitch/Slide";
            // 
            // lblLeftArrow
            // 
            this.lblLeftArrow.AutoSize = true;
            this.lblLeftArrow.Location = new System.Drawing.Point(179, 125);
            this.lblLeftArrow.Name = "lblLeftArrow";
            this.lblLeftArrow.Size = new System.Drawing.Size(16, 13);
            this.lblLeftArrow.TabIndex = 10;
            this.lblLeftArrow.Text = "<-";
            // 
            // lblRightArrow
            // 
            this.lblRightArrow.AutoSize = true;
            this.lblRightArrow.Location = new System.Drawing.Point(239, 125);
            this.lblRightArrow.Name = "lblRightArrow";
            this.lblRightArrow.Size = new System.Drawing.Size(16, 13);
            this.lblRightArrow.TabIndex = 10;
            this.lblRightArrow.Text = "->";
            // 
            // lblSlidePrecLoss
            // 
            this.lblSlidePrecLoss.AutoSize = true;
            this.lblSlidePrecLoss.Location = new System.Drawing.Point(204, 125);
            this.lblSlidePrecLoss.Name = "lblSlidePrecLoss";
            this.lblSlidePrecLoss.Size = new System.Drawing.Size(16, 13);
            this.lblSlidePrecLoss.TabIndex = 10;
            this.lblSlidePrecLoss.Text = "   ";
            // 
            // lblPitchLoss
            // 
            this.lblPitchLoss.AutoSize = true;
            this.lblPitchLoss.Location = new System.Drawing.Point(204, 87);
            this.lblPitchLoss.Name = "lblPitchLoss";
            this.lblPitchLoss.Size = new System.Drawing.Size(19, 13);
            this.lblPitchLoss.TabIndex = 10;
            this.lblPitchLoss.Text = "    ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(188, 51);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 13);
            this.label12.TabIndex = 10;
            this.label12.Text = "Prec Loss:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.nudDestPitchNote);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.nudDestSlide);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.nudTPR2);
            this.groupBox4.Location = new System.Drawing.Point(261, 22);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(179, 128);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Destination";
            // 
            // nudDestPitchNote
            // 
            this.nudDestPitchNote.Hexadecimal = true;
            this.nudDestPitchNote.Location = new System.Drawing.Point(97, 63);
            this.nudDestPitchNote.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDestPitchNote.Name = "nudDestPitchNote";
            this.nudDestPitchNote.Size = new System.Drawing.Size(63, 20);
            this.nudDestPitchNote.TabIndex = 11;
            this.nudDestPitchNote.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDestPitchNote.ValueChanged += new System.EventHandler(this.nudDestPitchNote_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(53, 103);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Value";
            // 
            // nudDestSlide
            // 
            this.nudDestSlide.Hexadecimal = true;
            this.nudDestSlide.Location = new System.Drawing.Point(110, 101);
            this.nudDestSlide.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudDestSlide.Name = "nudDestSlide";
            this.nudDestSlide.Size = new System.Drawing.Size(50, 20);
            this.nudDestSlide.TabIndex = 8;
            this.nudDestSlide.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudDestSlide.ValueChanged += new System.EventHandler(this.nudDest_Changed);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 65);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(81, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Expected value";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Ticks / Row";
            // 
            // nudTPR2
            // 
            this.nudTPR2.Location = new System.Drawing.Point(110, 29);
            this.nudTPR2.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudTPR2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTPR2.Name = "nudTPR2";
            this.nudTPR2.Size = new System.Drawing.Size(50, 20);
            this.nudTPR2.TabIndex = 4;
            this.nudTPR2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudTPR2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTPR2.ValueChanged += new System.EventHandler(this.destNud2_Changed);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.nudRenSlide);
            this.groupBox3.Controls.Add(this.lblVolumeSlide);
            this.groupBox3.Controls.Add(this.lblPitchNote);
            this.groupBox3.Controls.Add(this.nudRenPitchNote);
            this.groupBox3.Location = new System.Drawing.Point(6, 22);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(167, 128);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Renoise";
            // 
            // nudRenSlide
            // 
            this.nudRenSlide.Hexadecimal = true;
            this.nudRenSlide.Location = new System.Drawing.Point(98, 101);
            this.nudRenSlide.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRenSlide.Name = "nudRenSlide";
            this.nudRenSlide.Size = new System.Drawing.Size(63, 20);
            this.nudRenSlide.TabIndex = 8;
            this.nudRenSlide.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudRenSlide.ValueChanged += new System.EventHandler(this.nudSlide_Changed);
            // 
            // lblVolumeSlide
            // 
            this.lblVolumeSlide.AutoSize = true;
            this.lblVolumeSlide.Location = new System.Drawing.Point(6, 103);
            this.lblVolumeSlide.Name = "lblVolumeSlide";
            this.lblVolumeSlide.Size = new System.Drawing.Size(68, 13);
            this.lblVolumeSlide.TabIndex = 10;
            this.lblVolumeSlide.Text = "Volume Slide";
            // 
            // lblPitchNote
            // 
            this.lblPitchNote.AutoSize = true;
            this.lblPitchNote.Location = new System.Drawing.Point(6, 65);
            this.lblPitchNote.Name = "lblPitchNote";
            this.lblPitchNote.Size = new System.Drawing.Size(57, 13);
            this.lblPitchNote.TabIndex = 7;
            this.lblPitchNote.Text = "Pitch Note";
            // 
            // nudRenPitchNote
            // 
            this.nudRenPitchNote.Hexadecimal = true;
            this.nudRenPitchNote.Location = new System.Drawing.Point(98, 63);
            this.nudRenPitchNote.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRenPitchNote.Name = "nudRenPitchNote";
            this.nudRenPitchNote.Size = new System.Drawing.Size(63, 20);
            this.nudRenPitchNote.TabIndex = 7;
            this.nudRenPitchNote.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudRenPitchNote.ValueChanged += new System.EventHandler(this.nudRenPitchChanged);
            // 
            // tabNotes
            // 
            this.tabNotes.Controls.Add(this.groupBox6);
            this.tabNotes.Controls.Add(this.groupBox5);
            this.tabNotes.Controls.Add(this.btnCalcRelNote);
            this.tabNotes.Location = new System.Drawing.Point(4, 22);
            this.tabNotes.Name = "tabNotes";
            this.tabNotes.Padding = new System.Windows.Forms.Padding(3);
            this.tabNotes.Size = new System.Drawing.Size(446, 176);
            this.tabNotes.TabIndex = 2;
            this.tabNotes.Text = "Notes";
            this.tabNotes.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label16);
            this.groupBox6.Controls.Add(this.txtMaxNote);
            this.groupBox6.Controls.Add(this.txtMinNote);
            this.groupBox6.Controls.Add(this.label15);
            this.groupBox6.Controls.Add(this.label14);
            this.groupBox6.Controls.Add(this.cmbFormat);
            this.groupBox6.Location = new System.Drawing.Point(257, 23);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(183, 126);
            this.groupBox6.TabIndex = 10;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Destination";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(32, 97);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(27, 13);
            this.label16.TabIndex = 5;
            this.label16.Text = "Max";
            // 
            // txtMaxNote
            // 
            this.txtMaxNote.BackColor = System.Drawing.SystemColors.Window;
            this.txtMaxNote.Location = new System.Drawing.Point(114, 95);
            this.txtMaxNote.Name = "txtMaxNote";
            this.txtMaxNote.ReadOnly = true;
            this.txtMaxNote.Size = new System.Drawing.Size(59, 20);
            this.txtMaxNote.TabIndex = 4;
            // 
            // txtMinNote
            // 
            this.txtMinNote.BackColor = System.Drawing.SystemColors.Window;
            this.txtMinNote.Location = new System.Drawing.Point(114, 64);
            this.txtMinNote.Name = "txtMinNote";
            this.txtMinNote.ReadOnly = true;
            this.txtMinNote.Size = new System.Drawing.Size(59, 20);
            this.txtMinNote.TabIndex = 3;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(32, 66);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(24, 13);
            this.label15.TabIndex = 2;
            this.label15.Text = "Min";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(17, 32);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(39, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Format";
            // 
            // cmbFormat
            // 
            this.cmbFormat.FormattingEnabled = true;
            this.cmbFormat.Items.AddRange(new object[] {
            "MOD (Amiga)",
            "MOD (Ext)",
            "XM"});
            this.cmbFormat.Location = new System.Drawing.Point(76, 29);
            this.cmbFormat.Name = "cmbFormat";
            this.cmbFormat.Size = new System.Drawing.Size(97, 21);
            this.cmbFormat.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cmbFrequency);
            this.groupBox5.Controls.Add(this.cmbNotes);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.nudFineTune);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Location = new System.Drawing.Point(6, 23);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(200, 126);
            this.groupBox5.TabIndex = 9;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Renoise";
            // 
            // cmbFrequency
            // 
            this.cmbFrequency.FormattingEnabled = true;
            this.cmbFrequency.Items.AddRange(new object[] {
            "11025",
            "22050",
            "32000",
            "44100",
            "48000",
            "88200",
            "96000"});
            this.cmbFrequency.Location = new System.Drawing.Point(92, 29);
            this.cmbFrequency.Name = "cmbFrequency";
            this.cmbFrequency.Size = new System.Drawing.Size(90, 21);
            this.cmbFrequency.TabIndex = 13;
            // 
            // cmbNotes
            // 
            this.cmbNotes.FormattingEnabled = true;
            this.cmbNotes.Location = new System.Drawing.Point(120, 94);
            this.cmbNotes.Name = "cmbNotes";
            this.cmbNotes.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmbNotes.Size = new System.Drawing.Size(62, 21);
            this.cmbNotes.TabIndex = 12;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(19, 102);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(57, 13);
            this.label13.TabIndex = 11;
            this.label13.Text = "Base Note";
            // 
            // nudFineTune
            // 
            this.nudFineTune.Location = new System.Drawing.Point(120, 64);
            this.nudFineTune.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.nudFineTune.Minimum = new decimal(new int[] {
            127,
            0,
            0,
            -2147483648});
            this.nudFineTune.Name = "nudFineTune";
            this.nudFineTune.Size = new System.Drawing.Size(62, 20);
            this.nudFineTune.TabIndex = 10;
            this.nudFineTune.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 66);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "FineTune";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Frequency";
            // 
            // btnCalcRelNote
            // 
            this.btnCalcRelNote.Location = new System.Drawing.Point(212, 84);
            this.btnCalcRelNote.Name = "btnCalcRelNote";
            this.btnCalcRelNote.Size = new System.Drawing.Size(39, 23);
            this.btnCalcRelNote.TabIndex = 2;
            this.btnCalcRelNote.Text = ">>";
            this.btnCalcRelNote.UseVisualStyleBackColor = true;
            this.btnCalcRelNote.Click += new System.EventHandler(this.btnCalcRelNote_Click);
            // 
            // FormCalculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(481, 255);
            this.Controls.Add(this.tabTools);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "FormCalculator";
            this.ShowIcon = false;
            this.Text = "Conversion Tool";
            this.Load += new System.EventHandler(this.FormSpeedCalculator_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBPM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLPB)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTPR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTempo)).EndInit();
            this.tabTools.ResumeLayout(false);
            this.tabTempo.ResumeLayout(false);
            this.tabPitchSlide.ResumeLayout(false);
            this.tabPitchSlide.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDestPitchNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDestSlide)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTPR2)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRenSlide)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRenPitchNote)).EndInit();
            this.tabNotes.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFineTune)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nudBPM;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudLPB;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown nudTempo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudTPR;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtRenoiseRBPM;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDestinationRBPM;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnCalcBPM;
        private System.Windows.Forms.Button btnDecHex;
        private System.Windows.Forms.TabControl tabTools;
        private System.Windows.Forms.TabPage tabTempo;
        private System.Windows.Forms.TabPage tabPitchSlide;
        private System.Windows.Forms.NumericUpDown nudTPR2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown nudRenPitchNote;
        private System.Windows.Forms.Label lblPitchNote;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown nudDestSlide;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nudRenSlide;
        private System.Windows.Forms.Label lblVolumeSlide;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblPitchLoss;
        private System.Windows.Forms.Label lblSlidePrecLoss;
        private System.Windows.Forms.Label lblLeftArrow;
        private System.Windows.Forms.Label lblRightArrow;
        private System.Windows.Forms.NumericUpDown nudDestPitchNote;
        private System.Windows.Forms.TabPage tabNotes;
        private System.Windows.Forms.Button btnCalcRelNote;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ComboBox cmbFormat;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox cmbFrequency;
        private System.Windows.Forms.ComboBox cmbNotes;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nudFineTune;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtMaxNote;
        private System.Windows.Forms.TextBox txtMinNote;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnCalcTPR;
        private System.Windows.Forms.ToolTip toolTip;

    }
}