using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using Nini.Config;

namespace Xrns2XMod
{
    public class IniWrapper
    {
        #region Fields

        private IConfigSource configSource;
        
        #endregion

        #region Properties

        public string IniPath { get; private set; }

        public bool IsIniLoad { get; private set; }
        
        #endregion

        #region Constructor

        public IniWrapper(string xrnsFile, bool forceCreateIni)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            string iniPath = Path.Combine(Path.GetDirectoryName(assembly.Location), "ini", Path.GetFileNameWithoutExtension(xrnsFile) + ".ini");
            this.IniPath = iniPath;
            bool iniExists = File.Exists(iniPath);
            if (forceCreateIni || iniExists)
            {                
                if (iniExists == false)                
                    File.Create(iniPath).Dispose();

                configSource = new IniConfigSource(iniPath);
                if (iniExists == false)
                {
                    configSource.AddConfig("volume");
                    configSource.AddConfig("frequency");                
                }
                configSource.AutoSave = true;

                this.IsIniLoad = true;
            }
        }
        
        #endregion

        #region Methods

        public void SaveDefaultVolumeSample(int instrument, int sample, int value)
        {
            //if (Utility.IsWindowsOS())				
            //    IniFile.IniWriteValue("volume", string.Format("{0}/{1}", instrument, sample), value.ToString(), IniPath);

            IConfig configSection = configSource.Configs["volume"];

            configSection.Set(string.Format("{0}/{1}", instrument, sample), value.ToString());
        }

        public void SaveNewFreqSample(int instrument, int sample, int value)
        {
            //if (Utility.IsWindowsOS())
            //    IniFile.IniWriteValue("frequency", string.Format("{0}/{1}", instrument, sample), value.ToString(), IniPath);

            IConfig configSection = configSource.Configs["frequency"];

            configSection.Set(string.Format("{0}/{1}", instrument, sample), value.ToString());
        }

        public int ReadDefaultVolumeSample(int instrument, int sample)
        {
            string value = null;

            //if (Utility.IsWindowsOS())			
            //    value = IniFile.IniReadValue("volume", string.Format("{0}/{1}", instrument, sample), IniPath);

            IConfig configSection = configSource.Configs["volume"];

            value = configSection.Get(string.Format("{0}/{1}", instrument, sample), "64");

            return int.Parse(value);
        }


        public int ReadFreqSample(int instrument, int sample)
        {
            string value = null;
            //if (Utility.IsWindowsOS())
            //    value = IniFile.IniReadValue("frequency", string.Format("{0}/{1}", instrument, sample), IniPath);

            IConfig configSection = configSource.Configs["frequency"];

            value = configSection.Get(string.Format("{0}/{1}", instrument, sample), "0");

            return int.Parse(value);
        }
        
        #endregion

    }

    //class IniFile
    //{        
    //    [DllImport("kernel32")]
    //    private static extern long WritePrivateProfileString(string section,
    //        string key, string val, string filePath);
    //    [DllImport("kernel32")]
    //    private static extern int GetPrivateProfileString(string section,
    //             string key, string def, StringBuilder retVal,
    //        int size, string filePath);

    //    /// <summary>

    //    /// INIFile Constructor.

    //    /// </summary>

    //    /// <PARAM name="INIPath"></PARAM>

    //    //public IniFile(string INIPath)
    //    //{
    //    //    path = INIPath;
    //    //}
    //    /// <summary>

    //    /// Write Data to the INI File

    //    /// </summary>

    //    /// <PARAM name="Section"></PARAM>

    //    /// Section name

    //    /// <PARAM name="Key"></PARAM>

    //    /// Key Name

    //    /// <PARAM name="Value"></PARAM>

    //    /// Value Name

    //    public static void IniWriteValue(string Section, string Key, string Value, string path)
    //    {
    //        WritePrivateProfileString(Section, Key, Value, path);
    //    }

    //    /// <summary>

    //    /// Read Data Value From the Ini File

    //    /// </summary>

    //    /// <PARAM name="Section"></PARAM>

    //    /// <PARAM name="Key"></PARAM>

    //    /// <PARAM name="Path"></PARAM>

    //    /// <returns></returns>

    //    public static string IniReadValue(string Section, string Key, string path)
    //    {
    //        StringBuilder temp = new StringBuilder(0xFF);
    //        int i = GetPrivateProfileString(Section, Key, "", temp,
    //                                        0xFF, path);
    //        return temp.ToString();

    //    }
    //}
}
