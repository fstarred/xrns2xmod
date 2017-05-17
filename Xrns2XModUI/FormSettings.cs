using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net;
using Xrns2XModUI.Properties;

namespace Xrns2XModUI
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();

            txtBassEMail.Text = Settings.Default.BassEmail;
            txtBassCode.Text = Settings.Default.BassCode;
            //cmbVolumeResampling.SelectedValue = Settings.Default.VolumeResampling;
            //chkVolumeResampling.Checked = Settings.Default.VolumeScalingMode; 
            //nudFPL.Value = Settings.Default.ExtraFinePitchValue;
            //nudFVL.Value = Settings.Default.ExtraFineVolumeValue;
            this.chkNotesLimit.Checked = Settings.Default.PTCompatibiliy;
            this.nudPortamentoLossThreshold.Value = Settings.Default.PortamentoLossTreshold;

            this.checkBoxProxy.Checked = Xrns2XModUI.Properties.Settings.Default.ProxyEnabled;
            this.checkBoxCredentials.Checked = Xrns2XModUI.Properties.Settings.Default.CredentialsEnabled;
            this.textBoxHost.Text = Xrns2XModUI.Properties.Settings.Default.ProxyHost;
            this.textBoxPort.Text = Convert.ToString(Xrns2XModUI.Properties.Settings.Default.ProxyPort);
            this.textBoxUser.Text = Xrns2XModUI.Properties.Settings.Default.ProxyUser;
            this.textBoxPassword.Text = Xrns2XModUI.Properties.Settings.Default.ProxyPassword;
            this.textBoxDomain.Text = Xrns2XModUI.Properties.Settings.Default.ProxyDomain;

            this.cbVolumeScaling.DataSource = Enum.GetNames(typeof( Xrns2XMod.VOLUME_SCALING_MODE));
            this.cbVolumeScaling.SelectedItem = Xrns2XModUI.Properties.Settings.Default.VolumeScalingMode.ToString();

            this.groupBoxProxy.Enabled = this.checkBoxProxy.Checked;
            this.groupBoxCredentials.Enabled = this.checkBoxCredentials.Checked;

            AddEnableSaveOnChanges(this);

            //KeyValuePair<bool, string> kvpOn = new KeyValuePair<bool, string>(true, "On");
            //KeyValuePair<bool, string> kvpOff = new KeyValuePair<bool, string>(false, "Off");
            //List<KeyValuePair<bool, string>> list = new List<KeyValuePair<bool, string>>();
            //list.Add(kvpOn);
            //list.Add(kvpOff);
            //chkVolumeResampling.DataSource = new BindingSource(list, null);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Settings.Default.BassEmail = txtBassEMail.Text;
            Settings.Default.BassCode = txtBassCode.Text;
            //Settings.Default.VolumeResampling = (bool)cmbVolumeResampling.SelectedValue;
            //Settings.Default.VolumeScalingMode = chkVolumeResampling.Checked;
            //Settings.Default.ExtraFinePitchValue = (int)nudFPL.Value;
            //Settings.Default.ExtraFineVolumeValue = (int)nudFVL.Value;
            Settings.Default.VolumeScalingMode = (Xrns2XMod.VOLUME_SCALING_MODE)Enum.Parse(typeof(Xrns2XMod.VOLUME_SCALING_MODE), cbVolumeScaling.SelectedItem.ToString());
            Settings.Default.PTCompatibiliy = chkNotesLimit.Checked;
            Settings.Default.PortamentoLossTreshold = (int)nudPortamentoLossThreshold.Value;

            Xrns2XMod.Properties.Settings.Default.Save();            

            if (IsProxyFormValid())
            {
                Xrns2XModUI.Properties.Settings.Default.ProxyEnabled = this.checkBoxProxy.Checked;
                Xrns2XModUI.Properties.Settings.Default.CredentialsEnabled = this.checkBoxCredentials.Checked;
                Xrns2XModUI.Properties.Settings.Default.ProxyHost = this.textBoxHost.Text;
                Xrns2XModUI.Properties.Settings.Default.ProxyPort = Convert.ToInt16(this.textBoxPort.Text);
                Xrns2XModUI.Properties.Settings.Default.ProxyUser = this.textBoxUser.Text;
                Xrns2XModUI.Properties.Settings.Default.ProxyPassword = this.textBoxPassword.Text;
                Xrns2XModUI.Properties.Settings.Default.ProxyDomain = this.textBoxDomain.Text;

                Xrns2XModUI.Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("Proxy settings are not valid", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.btnOk.Enabled = false;

            //this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {            
            this.Close();
        }


        private void linkBass_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://bass.radio42.com/bass_register.html");
        }

        private void buttonUpdates_Click(object sender, EventArgs e)
        {
            WebProxy webProxy = null;

            if (this.IsProxyFormValid())
            {
                try
                {
                    if (this.checkBoxProxy.Checked)
                    {
                        webProxy = new WebProxy(this.textBoxHost.Text, Convert.ToInt16(this.textBoxPort.Text));

                        if (this.checkBoxCredentials.Checked)
                            webProxy.Credentials = new NetworkCredential(this.textBoxUser.Text, this.textBoxPassword.Text, this.textBoxDomain.Text);
                    }

                    Shared.CheckForUpdates(true, webProxy);
                }
                catch (Exception exception)
                {
                    
                    MessageBox.Show(exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                MessageBox.Show("No valid settings specified", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public bool IsProxyFormValid()
        {
            if (this.checkBoxProxy.Checked)
            {
                int num = 0;

                if (string.IsNullOrEmpty(this.textBoxHost.Text))
                    return false;
                if (string.IsNullOrEmpty(this.textBoxPort.Text))
                    return false;
                else if (int.TryParse(this.textBoxPort.Text, out num) == false)
                    return false;

                if (this.checkBoxCredentials.Checked)
                {
                    if (string.IsNullOrEmpty(this.textBoxUser.Text))
                        return false;
                }
            }

            return true;
        }

        private void checkBoxProxy_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBoxProxy.Enabled = ((CheckBox)sender).Checked;        
        }

        private void checkBoxCredentials_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBoxCredentials.Enabled = ((CheckBox)sender).Checked;    
        }

        private void AddEnableSaveOnChanges(Control obj)
        {
            foreach (Control control in obj.Controls)
            {
                control.TextChanged += EnableSave;

                if (control.GetType() == typeof(CheckBox))
                {
                    ((CheckBox)control).CheckedChanged += EnableSave;
                }

                if (control.Controls.Count > 0)
                    AddEnableSaveOnChanges(control);
            }
        }

        private void EnableSave(object sender, EventArgs e)
        {
            this.btnOk.Enabled = true;
        }

    }
}
