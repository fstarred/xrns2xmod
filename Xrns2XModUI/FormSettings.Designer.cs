using System.Windows.Forms;
namespace Xrns2XModUI
{
    partial class FormSettings : Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bassPage = new System.Windows.Forms.TabControl();
            this.generalPage = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbVolumeScaling = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudPortamentoLossThreshold = new System.Windows.Forms.NumericUpDown();
            this.chkNotesLimit = new System.Windows.Forms.CheckBox();
            this.proxyPage = new System.Windows.Forms.TabPage();
            this.buttonUpdates = new System.Windows.Forms.Button();
            this.groupBoxProxy = new System.Windows.Forms.GroupBox();
            this.checkBoxCredentials = new System.Windows.Forms.CheckBox();
            this.groupBoxCredentials = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxDomain = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxHost = new System.Windows.Forms.TextBox();
            this.checkBoxProxy = new System.Windows.Forms.CheckBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.linkBass = new System.Windows.Forms.LinkLabel();
            this.txtBassCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBassEMail = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.bassPage.SuspendLayout();
            this.generalPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPortamentoLossThreshold)).BeginInit();
            this.proxyPage.SuspendLayout();
            this.groupBoxProxy.SuspendLayout();
            this.groupBoxCredentials.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(15, 281);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(62, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "&Save";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(201, 281);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "&Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bassPage
            // 
            this.bassPage.Controls.Add(this.generalPage);
            this.bassPage.Controls.Add(this.proxyPage);
            this.bassPage.Controls.Add(this.tabPage1);
            this.bassPage.Location = new System.Drawing.Point(15, 12);
            this.bassPage.Name = "bassPage";
            this.bassPage.SelectedIndex = 0;
            this.bassPage.Size = new System.Drawing.Size(248, 263);
            this.bassPage.TabIndex = 8;
            // 
            // generalPage
            // 
            this.generalPage.Controls.Add(this.groupBox2);
            this.generalPage.Controls.Add(this.groupBox1);
            this.generalPage.Location = new System.Drawing.Point(4, 22);
            this.generalPage.Name = "generalPage";
            this.generalPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalPage.Size = new System.Drawing.Size(240, 237);
            this.generalPage.TabIndex = 0;
            this.generalPage.Text = "General";
            this.generalPage.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.cbVolumeScaling);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(10, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(224, 107);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Common";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 72);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(181, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "* COLUMN mode applies only for XM";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(54, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Volume scaling mode";
            // 
            // cbVolumeScaling
            // 
            this.cbVolumeScaling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVolumeScaling.FormattingEnabled = true;
            this.cbVolumeScaling.Location = new System.Drawing.Point(45, 38);
            this.cbVolumeScaling.Name = "cbVolumeScaling";
            this.cbVolumeScaling.Size = new System.Drawing.Size(131, 21);
            this.cbVolumeScaling.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.nudPortamentoLossThreshold);
            this.groupBox1.Controls.Add(this.chkNotesLimit);
            this.groupBox1.Location = new System.Drawing.Point(10, 119);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(224, 87);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mod";
            // 
            // nudPortamentoLossThreshold
            // 
            this.nudPortamentoLossThreshold.Location = new System.Drawing.Point(166, 59);
            this.nudPortamentoLossThreshold.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudPortamentoLossThreshold.Name = "nudPortamentoLossThreshold";
            this.nudPortamentoLossThreshold.Size = new System.Drawing.Size(40, 20);
            this.nudPortamentoLossThreshold.TabIndex = 5;
            this.nudPortamentoLossThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chkNotesLimit
            // 
            this.chkNotesLimit.AutoSize = true;
            this.chkNotesLimit.Location = new System.Drawing.Point(45, 19);
            this.chkNotesLimit.Name = "chkNotesLimit";
            this.chkNotesLimit.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkNotesLimit.Size = new System.Drawing.Size(139, 17);
            this.chkNotesLimit.TabIndex = 4;
            this.chkNotesLimit.Text = "ProTracker compatibility";
            this.chkNotesLimit.UseVisualStyleBackColor = true;
            // 
            // proxyPage
            // 
            this.proxyPage.Controls.Add(this.buttonUpdates);
            this.proxyPage.Controls.Add(this.groupBoxProxy);
            this.proxyPage.Controls.Add(this.checkBoxProxy);
            this.proxyPage.Location = new System.Drawing.Point(4, 22);
            this.proxyPage.Name = "proxyPage";
            this.proxyPage.Padding = new System.Windows.Forms.Padding(3);
            this.proxyPage.Size = new System.Drawing.Size(240, 237);
            this.proxyPage.TabIndex = 3;
            this.proxyPage.Text = "Proxy";
            this.proxyPage.UseVisualStyleBackColor = true;
            // 
            // buttonUpdates
            // 
            this.buttonUpdates.Image = ((System.Drawing.Image)(resources.GetObject("buttonUpdates.Image")));
            this.buttonUpdates.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonUpdates.Location = new System.Drawing.Point(111, 3);
            this.buttonUpdates.Name = "buttonUpdates";
            this.buttonUpdates.Size = new System.Drawing.Size(123, 29);
            this.buttonUpdates.TabIndex = 10;
            this.buttonUpdates.Text = "Check updates";
            this.buttonUpdates.UseVisualStyleBackColor = true;
            this.buttonUpdates.Click += new System.EventHandler(this.buttonUpdates_Click);
            // 
            // groupBoxProxy
            // 
            this.groupBoxProxy.Controls.Add(this.checkBoxCredentials);
            this.groupBoxProxy.Controls.Add(this.groupBoxCredentials);
            this.groupBoxProxy.Controls.Add(this.label10);
            this.groupBoxProxy.Controls.Add(this.textBoxPort);
            this.groupBoxProxy.Controls.Add(this.label11);
            this.groupBoxProxy.Controls.Add(this.textBoxHost);
            this.groupBoxProxy.Location = new System.Drawing.Point(6, 29);
            this.groupBoxProxy.Name = "groupBoxProxy";
            this.groupBoxProxy.Size = new System.Drawing.Size(234, 205);
            this.groupBoxProxy.TabIndex = 9;
            this.groupBoxProxy.TabStop = false;
            this.groupBoxProxy.Text = "Proxy";
            // 
            // checkBoxCredentials
            // 
            this.checkBoxCredentials.AutoSize = true;
            this.checkBoxCredentials.Location = new System.Drawing.Point(127, 78);
            this.checkBoxCredentials.Name = "checkBoxCredentials";
            this.checkBoxCredentials.Size = new System.Drawing.Size(100, 17);
            this.checkBoxCredentials.TabIndex = 13;
            this.checkBoxCredentials.Text = "Use Credentials";
            this.checkBoxCredentials.UseVisualStyleBackColor = true;
            this.checkBoxCredentials.CheckedChanged += new System.EventHandler(this.checkBoxCredentials_CheckedChanged);
            // 
            // groupBoxCredentials
            // 
            this.groupBoxCredentials.Controls.Add(this.label1);
            this.groupBoxCredentials.Controls.Add(this.textBoxUser);
            this.groupBoxCredentials.Controls.Add(this.textBoxPassword);
            this.groupBoxCredentials.Controls.Add(this.label5);
            this.groupBoxCredentials.Controls.Add(this.label9);
            this.groupBoxCredentials.Controls.Add(this.textBoxDomain);
            this.groupBoxCredentials.Location = new System.Drawing.Point(6, 101);
            this.groupBoxCredentials.Name = "groupBoxCredentials";
            this.groupBoxCredentials.Size = new System.Drawing.Size(221, 101);
            this.groupBoxCredentials.TabIndex = 12;
            this.groupBoxCredentials.TabStop = false;
            this.groupBoxCredentials.Text = "Credentials";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "User";
            // 
            // textBoxUser
            // 
            this.textBoxUser.Location = new System.Drawing.Point(74, 22);
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.Size = new System.Drawing.Size(133, 20);
            this.textBoxUser.TabIndex = 1;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(74, 45);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(133, 20);
            this.textBoxPassword.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Domain";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Password";
            // 
            // textBoxDomain
            // 
            this.textBoxDomain.Location = new System.Drawing.Point(74, 71);
            this.textBoxDomain.Name = "textBoxDomain";
            this.textBoxDomain.Size = new System.Drawing.Size(133, 20);
            this.textBoxDomain.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 57);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(26, 13);
            this.label10.TabIndex = 10;
            this.label10.Text = "Port";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(41, 54);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(65, 20);
            this.textBoxPort.TabIndex = 9;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 31);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 8;
            this.label11.Text = "Host";
            // 
            // textBoxHost
            // 
            this.textBoxHost.Location = new System.Drawing.Point(41, 28);
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.Size = new System.Drawing.Size(162, 20);
            this.textBoxHost.TabIndex = 7;
            // 
            // checkBoxProxy
            // 
            this.checkBoxProxy.AutoSize = true;
            this.checkBoxProxy.Location = new System.Drawing.Point(6, 6);
            this.checkBoxProxy.Name = "checkBoxProxy";
            this.checkBoxProxy.Size = new System.Drawing.Size(88, 17);
            this.checkBoxProxy.TabIndex = 8;
            this.checkBoxProxy.Text = "Enable Proxy";
            this.checkBoxProxy.UseVisualStyleBackColor = true;
            this.checkBoxProxy.CheckedChanged += new System.EventHandler(this.checkBoxProxy_CheckedChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.linkBass);
            this.tabPage1.Controls.Add(this.txtBassCode);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtBassEMail);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(240, 237);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "BASS Audio";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Homepage:";
            // 
            // linkBass
            // 
            this.linkBass.AutoSize = true;
            this.linkBass.Location = new System.Drawing.Point(75, 87);
            this.linkBass.Name = "linkBass";
            this.linkBass.Size = new System.Drawing.Size(55, 13);
            this.linkBass.TabIndex = 4;
            this.linkBass.TabStop = true;
            this.linkBass.Text = "Bass.NET";
            this.linkBass.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkBass_LinkClicked);
            // 
            // txtBassCode
            // 
            this.txtBassCode.Location = new System.Drawing.Point(44, 42);
            this.txtBassCode.Name = "txtBassCode";
            this.txtBassCode.Size = new System.Drawing.Size(190, 20);
            this.txtBassCode.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Code";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "EMail";
            // 
            // txtBassEMail
            // 
            this.txtBassEMail.Location = new System.Drawing.Point(45, 16);
            this.txtBassEMail.Name = "txtBassEMail";
            this.txtBassEMail.Size = new System.Drawing.Size(189, 20);
            this.txtBassEMail.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(154, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Portamento accuracy threshold";
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(275, 316);
            this.Controls.Add(this.bassPage);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSettings";
            this.ShowIcon = false;
            this.Text = "Settings";
            this.bassPage.ResumeLayout(false);
            this.generalPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPortamentoLossThreshold)).EndInit();
            this.proxyPage.ResumeLayout(false);
            this.proxyPage.PerformLayout();
            this.groupBoxProxy.ResumeLayout(false);
            this.groupBoxProxy.PerformLayout();
            this.groupBoxCredentials.ResumeLayout(false);
            this.groupBoxCredentials.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private TabControl bassPage;
        private TabPage generalPage;
        private TabPage tabPage1;
        private Label label2;
        private TextBox txtBassEMail;
        private TextBox txtBassCode;
        private Label label3;
        private LinkLabel linkBass;
        private Label label4;
        private TabPage proxyPage;
        private GroupBox groupBoxProxy;
        private CheckBox checkBoxCredentials;
        private GroupBox groupBoxCredentials;
        private Label label1;
        private TextBox textBoxUser;
        private TextBox textBoxPassword;
        private Label label5;
        private Label label9;
        private TextBox textBoxDomain;
        private Label label10;
        private TextBox textBoxPort;
        private Label label11;
        private TextBox textBoxHost;
        private CheckBox checkBoxProxy;
        private Button buttonUpdates;
        private GroupBox groupBox2;
        private Label label7;
        private Label label6;
        private ComboBox cbVolumeScaling;
        private GroupBox groupBox1;
        private CheckBox chkNotesLimit;
        private NumericUpDown nudPortamentoLossThreshold;
        private Label label8;
    }
}