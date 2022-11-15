using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Xrns2XModUI.Properties;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using Service.Updater;
using System.Windows.Forms;
using System.IO;
using Xrns2XMod;

namespace Xrns2XModUI
{
    class Shared
    {

        public static ModSettings GetModSettingsFromIni()
        {
            ModSettings settings = new ModSettings();

            settings.ForceProTrackerCompatibility = Settings.Default.PTCompatibiliy ? PROTRACKER_COMPATIBILITY_MODE.A3MAX : PROTRACKER_COMPATIBILITY_MODE.NONE;
            //settings.MantainOriginalSampleFreq = Settings.Default.MantainOriginalSampleFreq;E
            settings.VolumeScalingMode = Settings.Default.VolumeScalingMode;
            settings.PortamentoLossThreshold = Settings.Default.PortamentoLossTreshold;

            return settings;
        }


        public static XmSettings GetXmSettingsFromIni()
        {
            XmSettings settings = new XmSettings();

            settings.VolumeScalingMode = Settings.Default.VolumeScalingMode;            

            return settings;
        }

        public static WebProxy GetWebProxyFromResources()
        {
            WebProxy webProxy = null;

            bool isProxyEnabled = Settings.Default.ProxyEnabled;
            bool isCredentialsEnabled = Settings.Default.CredentialsEnabled;

            if (isProxyEnabled)
            {
                try
                {
                    string host = Settings.Default.ProxyHost;
                    int port = Settings.Default.ProxyPort;
                    webProxy = new WebProxy(host, port);

                    if (isCredentialsEnabled)
                    {
                        string user = Settings.Default.ProxyUser;
                        string pwd = Settings.Default.ProxyPassword;
                        string domain = Settings.Default.ProxyDomain;

                        webProxy.Credentials = new NetworkCredential(user, pwd, domain);
                    }
                }
                catch (Exception)
                {

                }

            }

            return webProxy;
        }


        public static Version GetProductVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Path.GetDirectoryName(assembly.Location) + "/Xrns2XMod.dll");

            return new Version(fileVersionInfo.ProductVersion);
        }

        public static void CheckForUpdates(bool showMessageAnyway)
        {
            WebProxy webProxy = Shared.GetWebProxyFromResources();

            CheckForUpdates(showMessageAnyway, webProxy);
        }

        public static void CheckForUpdates(bool showMessageAnyway, WebProxy webProxy)
        {

            Task.Factory.StartNew(() => ServiceUpdater.CheckForUpdates(Globals.URI_UPDATER, webProxy)).ContinueWith(task =>
            {
                if (task.Result != null)
                {
                    ServiceUpdater.VersionInfo lastVersionInfo = task.Result;

                    Version productVersion = Shared.GetProductVersion();

                    if (lastVersionInfo.LatestVersion > productVersion)
                    {
                        DialogResult result = MessageBox.Show(String.Format("A new version ({0}) is avaible, do you want to go to the homepage?", lastVersionInfo.LatestVersion), "NEW VERSION", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                        if (result == DialogResult.Yes)
                        {
                            Process.Start(lastVersionInfo.LatestVersionUrl);
                        }
                    }
                    else if (showMessageAnyway)
                    {
                        MessageBox.Show(String.Format("This version is up to date", lastVersionInfo.LatestVersion), "No updates avaible", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (showMessageAnyway)
                        MessageBox.Show("Network error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

    }
}
