using System.Windows.Forms;
namespace Xrns2XModUI
{
    partial class FormQuerySong : Form
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
            this.txtXQuery = new System.Windows.Forms.TextBox();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageQuery = new System.Windows.Forms.TabPage();
            this.tabPageProperties = new System.Windows.Forms.TabPage();
            this.groupBoxDowngrade = new System.Windows.Forms.GroupBox();
            this.labelModelTiming = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDowngrade = new System.Windows.Forms.Button();
            this.labelFilename = new System.Windows.Forms.Label();
            this.checkBoxReplaceZK = new System.Windows.Forms.CheckBox();
            this.tabControl.SuspendLayout();
            this.tabPageQuery.SuspendLayout();
            this.tabPageProperties.SuspendLayout();
            this.groupBoxDowngrade.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtXQuery
            // 
            this.txtXQuery.Location = new System.Drawing.Point(3, 6);
            this.txtXQuery.Multiline = true;
            this.txtXQuery.Name = "txtXQuery";
            this.txtXQuery.Size = new System.Drawing.Size(404, 55);
            this.txtXQuery.TabIndex = 0;
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(6, 96);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResult.Size = new System.Drawing.Size(404, 174);
            this.txtResult.TabIndex = 1;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(166, 67);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 2;
            this.btnExecute.Text = "&Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(166, 276);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click_1);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageQuery);
            this.tabControl.Controls.Add(this.tabPageProperties);
            this.tabControl.Location = new System.Drawing.Point(12, 44);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(424, 299);
            this.tabControl.TabIndex = 4;
            // 
            // tabPageQuery
            // 
            this.tabPageQuery.Controls.Add(this.btnExecute);
            this.tabPageQuery.Controls.Add(this.btnClose);
            this.tabPageQuery.Controls.Add(this.txtXQuery);
            this.tabPageQuery.Controls.Add(this.txtResult);
            this.tabPageQuery.Location = new System.Drawing.Point(4, 22);
            this.tabPageQuery.Name = "tabPageQuery";
            this.tabPageQuery.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageQuery.Size = new System.Drawing.Size(416, 273);
            this.tabPageQuery.TabIndex = 0;
            this.tabPageQuery.Text = "Query";
            this.tabPageQuery.UseVisualStyleBackColor = true;
            // 
            // tabPageProperties
            // 
            this.tabPageProperties.Controls.Add(this.groupBoxDowngrade);
            this.tabPageProperties.Controls.Add(this.buttonDowngrade);
            this.tabPageProperties.Location = new System.Drawing.Point(4, 22);
            this.tabPageProperties.Name = "tabPageProperties";
            this.tabPageProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProperties.Size = new System.Drawing.Size(416, 273);
            this.tabPageProperties.TabIndex = 1;
            this.tabPageProperties.Text = "Features";
            this.tabPageProperties.UseVisualStyleBackColor = true;
            // 
            // groupBoxDowngrade
            // 
            this.groupBoxDowngrade.Controls.Add(this.checkBoxReplaceZK);
            this.groupBoxDowngrade.Controls.Add(this.labelModelTiming);
            this.groupBoxDowngrade.Controls.Add(this.label1);
            this.groupBoxDowngrade.Location = new System.Drawing.Point(6, 6);
            this.groupBoxDowngrade.Name = "groupBoxDowngrade";
            this.groupBoxDowngrade.Size = new System.Drawing.Size(404, 95);
            this.groupBoxDowngrade.TabIndex = 2;
            this.groupBoxDowngrade.TabStop = false;
            // 
            // labelModelTiming
            // 
            this.labelModelTiming.Location = new System.Drawing.Point(6, 41);
            this.labelModelTiming.Name = "labelModelTiming";
            this.labelModelTiming.Size = new System.Drawing.Size(392, 23);
            this.labelModelTiming.TabIndex = 2;
            this.labelModelTiming.Text = "...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Playback mode";
            // 
            // buttonDowngrade
            // 
            this.buttonDowngrade.Enabled = false;
            this.buttonDowngrade.Location = new System.Drawing.Point(6, 107);
            this.buttonDowngrade.Name = "buttonDowngrade";
            this.buttonDowngrade.Size = new System.Drawing.Size(75, 32);
            this.buttonDowngrade.TabIndex = 0;
            this.buttonDowngrade.Text = "Downgrade";
            this.buttonDowngrade.UseVisualStyleBackColor = true;
            this.buttonDowngrade.Click += new System.EventHandler(this.buttonDowngrade_Click);
            // 
            // labelFilename
            // 
            this.labelFilename.BackColor = System.Drawing.SystemColors.Window;
            this.labelFilename.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelFilename.Location = new System.Drawing.Point(12, 9);
            this.labelFilename.Name = "labelFilename";
            this.labelFilename.Size = new System.Drawing.Size(420, 23);
            this.labelFilename.TabIndex = 5;
            this.labelFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBoxReplaceZK
            // 
            this.checkBoxReplaceZK.AutoSize = true;
            this.checkBoxReplaceZK.Enabled = false;
            this.checkBoxReplaceZK.Location = new System.Drawing.Point(9, 67);
            this.checkBoxReplaceZK.Name = "checkBoxReplaceZK";
            this.checkBoxReplaceZK.Size = new System.Drawing.Size(175, 17);
            this.checkBoxReplaceZK.TabIndex = 3;
            this.checkBoxReplaceZK.Text = "Replace ZK commands with ZL";
            this.checkBoxReplaceZK.UseVisualStyleBackColor = true;
            // 
            // FormQuerySong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(448, 355);
            this.Controls.Add(this.labelFilename);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormQuerySong";
            this.ShowIcon = false;
            this.Text = "Renoise song info";
            this.Load += new System.EventHandler(this.FormInfoSong_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageQuery.ResumeLayout(false);
            this.tabPageQuery.PerformLayout();
            this.tabPageProperties.ResumeLayout(false);
            this.groupBoxDowngrade.ResumeLayout(false);
            this.groupBoxDowngrade.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtXQuery;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnClose;
        private TabControl tabControl;
        private TabPage tabPageQuery;
        private TabPage tabPageProperties;
        private GroupBox groupBoxDowngrade;
        private Button buttonDowngrade;
        private Label label1;
        private Label labelModelTiming;
        private Label labelFilename;
        private CheckBox checkBoxReplaceZK;
    }
}