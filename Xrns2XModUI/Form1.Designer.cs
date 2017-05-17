namespace Xrns2XModUI
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.toolStripTop = new System.Windows.Forms.ToolStrip();
            this.optionsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.volumeStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolBoxToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.queryStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.reloadToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openfileToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.cboTarget = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.groupBoxInitialValues = new System.Windows.Forms.GroupBox();
            this.numericUpDownTicks = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownTempo = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStripTop.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBoxInitialValues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTicks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTempo)).BeginInit();
            this.SuspendLayout();
            // 
            // txtFileName
            // 
            this.txtFileName.BackColor = System.Drawing.SystemColors.Window;
            this.txtFileName.Location = new System.Drawing.Point(12, 83);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.ReadOnly = true;
            this.txtFileName.Size = new System.Drawing.Size(213, 20);
            this.txtFileName.TabIndex = 9;
            this.txtFileName.TabStop = false;
            this.txtFileName.MouseHover += new System.EventHandler(this.txtFileName_MouseHover);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(12, 119);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 29);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // toolStripTop
            // 
            this.toolStripTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripButton,
            this.volumeStripButton,
            this.toolBoxToolStripButton,
            this.queryStripButton,
            this.toolStripSeparator,
            this.openToolStripButton,
            this.reloadToolStripButton,
            this.saveToolStripButton,
            this.openfileToolStripButton,
            this.toolStripSeparator1,
            this.helpToolStripButton});
            this.toolStripTop.Location = new System.Drawing.Point(0, 0);
            this.toolStripTop.Name = "toolStripTop";
            this.toolStripTop.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripTop.Size = new System.Drawing.Size(359, 25);
            this.toolStripTop.TabIndex = 11;
            this.toolStripTop.Text = "toolStrip1";
            // 
            // optionsToolStripButton
            // 
            this.optionsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.optionsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("optionsToolStripButton.Image")));
            this.optionsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.optionsToolStripButton.Name = "optionsToolStripButton";
            this.optionsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.optionsToolStripButton.Text = "O&ptions";
            this.optionsToolStripButton.Click += new System.EventHandler(this.optionsToolStripButton_Click);
            // 
            // volumeStripButton
            // 
            this.volumeStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.volumeStripButton.Image = ((System.Drawing.Image)(resources.GetObject("volumeStripButton.Image")));
            this.volumeStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.volumeStripButton.Name = "volumeStripButton";
            this.volumeStripButton.Size = new System.Drawing.Size(23, 22);
            this.volumeStripButton.Text = "&Instrument Settings";
            this.volumeStripButton.Click += new System.EventHandler(this.volumeStripButton_Click);
            // 
            // toolBoxToolStripButton
            // 
            this.toolBoxToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBoxToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("toolBoxToolStripButton.Image")));
            this.toolBoxToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBoxToolStripButton.Name = "toolBoxToolStripButton";
            this.toolBoxToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.toolBoxToolStripButton.Text = "Conversion Tool";
            this.toolBoxToolStripButton.Click += new System.EventHandler(this.speedToolStripButton_Click);
            // 
            // queryStripButton
            // 
            this.queryStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.queryStripButton.Image = ((System.Drawing.Image)(resources.GetObject("queryStripButton.Image")));
            this.queryStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.queryStripButton.Name = "queryStripButton";
            this.queryStripButton.Size = new System.Drawing.Size(23, 22);
            this.queryStripButton.Text = "Info Song";
            this.queryStripButton.ToolTipText = "Renoise song info";
            this.queryStripButton.Click += new System.EventHandler(this.infoStripButton_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "&Open";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
            // 
            // reloadToolStripButton
            // 
            this.reloadToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reloadToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("reloadToolStripButton.Image")));
            this.reloadToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reloadToolStripButton.Name = "reloadToolStripButton";
            this.reloadToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.reloadToolStripButton.Text = "&Reload xrns";
            this.reloadToolStripButton.Click += new System.EventHandler(this.reloadToolStripButton_Click);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "&Save";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // openfileToolStripButton
            // 
            this.openfileToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openfileToolStripButton.Enabled = false;
            this.openfileToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openfileToolStripButton.Image")));
            this.openfileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openfileToolStripButton.Name = "openfileToolStripButton";
            this.openfileToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openfileToolStripButton.Text = "Open output folder";
            this.openfileToolStripButton.Click += new System.EventHandler(this.openfileToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // helpToolStripButton
            // 
            this.helpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripButton.Image")));
            this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpToolStripButton.Name = "helpToolStripButton";
            this.helpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.helpToolStripButton.Text = "He&lp";
            this.helpToolStripButton.Click += new System.EventHandler(this.helpToolStripButton_Click);
            // 
            // cboTarget
            // 
            this.cboTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTarget.FormattingEnabled = true;
            this.cboTarget.Items.AddRange(new object[] {
            "XM",
            "MOD"});
            this.cboTarget.Location = new System.Drawing.Point(268, 83);
            this.cboTarget.Name = "cboTarget";
            this.cboTarget.Size = new System.Drawing.Size(75, 21);
            this.cboTarget.TabIndex = 12;
            this.cboTarget.SelectedValueChanged += new System.EventHandler(this.cboTarget_SelectedValueChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(268, 119);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 29);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(231, 81);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(31, 23);
            this.btnOpen.TabIndex = 14;
            this.btnOpen.Text = "...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 158);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(359, 22);
            this.statusStrip1.TabIndex = 15;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // groupBoxInitialValues
            // 
            this.groupBoxInitialValues.Controls.Add(this.numericUpDownTicks);
            this.groupBoxInitialValues.Controls.Add(this.label2);
            this.groupBoxInitialValues.Controls.Add(this.numericUpDownTempo);
            this.groupBoxInitialValues.Controls.Add(this.label1);
            this.groupBoxInitialValues.Location = new System.Drawing.Point(12, 28);
            this.groupBoxInitialValues.Name = "groupBoxInitialValues";
            this.groupBoxInitialValues.Size = new System.Drawing.Size(331, 47);
            this.groupBoxInitialValues.TabIndex = 20;
            this.groupBoxInitialValues.TabStop = false;
            // 
            // numericUpDownTicks
            // 
            this.numericUpDownTicks.Location = new System.Drawing.Point(256, 19);
            this.numericUpDownTicks.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDownTicks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownTicks.Name = "numericUpDownTicks";
            this.numericUpDownTicks.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.numericUpDownTicks.Size = new System.Drawing.Size(43, 20);
            this.numericUpDownTicks.TabIndex = 23;
            this.numericUpDownTicks.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(190, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Ticks/Row";
            // 
            // numericUpDownTempo
            // 
            this.numericUpDownTempo.Location = new System.Drawing.Point(61, 19);
            this.numericUpDownTempo.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.numericUpDownTempo.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numericUpDownTempo.Name = "numericUpDownTempo";
            this.numericUpDownTempo.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.numericUpDownTempo.Size = new System.Drawing.Size(59, 20);
            this.numericUpDownTempo.TabIndex = 21;
            this.numericUpDownTempo.Value = new decimal(new int[] {
            125,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Tempo";
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(359, 180);
            this.Controls.Add(this.groupBoxInitialValues);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cboTarget);
            this.Controls.Add(this.toolStripTop);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtFileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Xrns2XMod";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.toolStripTop.ResumeLayout(false);
            this.toolStripTop.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBoxInitialValues.ResumeLayout(false);
            this.groupBoxInitialValues.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTicks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTempo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ToolStrip toolStripTop;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton optionsToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton helpToolStripButton;
        private System.Windows.Forms.ComboBox cboTarget;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.ToolStripButton queryStripButton;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripButton reloadToolStripButton;
        private System.Windows.Forms.ToolStripButton toolBoxToolStripButton;
        private System.Windows.Forms.ToolStripButton volumeStripButton;
        private System.Windows.Forms.ToolStripButton openfileToolStripButton;
        private System.Windows.Forms.GroupBox groupBoxInitialValues;
        private System.Windows.Forms.NumericUpDown numericUpDownTicks;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownTempo;
        private System.Windows.Forms.Label label1;

    }
}

