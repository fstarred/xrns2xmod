using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Un4seen.Bass;

namespace Xrns2XMod
{
    public class XrnsManager
    {

        const string READING_ERROR = "There was an error reading xrns";
        const string DOWNGRADE_ALREADY_DONE = "Song is already compatible with TIMING MODEL SPEED";

        private string filename = null;

        public XrnsManager(string filename)
        {
            this.filename = filename;
        }

        private class CustomStaticDataSource : IStaticDataSource
        {
            private Stream _stream;
            // Implement method from IStaticDataSource
            public Stream GetSource()
            {
                return _stream;
            }

            // Call this to provide the memorystream
            public void SetStream(Stream inputStream)
            {
                _stream = inputStream;
                _stream.Position = 0;
            }
        }

        /*
         * return flac sample stream, otherwise returns null
         * 
         * */
        public Stream GetSampleStream(int instrumentIndex, int sampleIndex)
        {
            Stream outputStream = null;
            string regExp = "SampleData/Instrument" + instrumentIndex.ToString("00", CultureInfo.InvariantCulture) +
                ".*/Sample" + sampleIndex.ToString("00", CultureInfo.InvariantCulture) + ".*\\.flac";

            Regex regPattern = new Regex(regExp);

            ZipFile zipFile = null;

            try
            {
                zipFile = new ZipFile(filename);

                string sampleFilename = null;

                for (int i = 0; i < zipFile.Count; i++)
                {
                    Match matchInst = regPattern.Match(zipFile[i].Name);

                    sampleFilename = matchInst.Value;

                    if (matchInst.Success) break;
                }

                if (!String.IsNullOrEmpty(sampleFilename))
                {
                    ZipEntry zipEntry = zipFile.GetEntry(sampleFilename);

                    outputStream = new MemoryStream();

                    using (StreamReader stream = new StreamReader(zipFile.GetInputStream(zipEntry)))
                    {
                        if (stream != null)
                        {
                            stream.BaseStream.Position = 0;

                            stream.BaseStream.CopyTo(outputStream);
                        }
                    }

                    
                }
            }
            finally
            {
                if (zipFile != null)
                    zipFile.Close();
            }

            return outputStream;
        }        

        public bool DowngradeSong(bool replaceZKCommand)
        {
            ZipFile zipFile = new ZipFile(filename);

            ZipEntry zipEntry = zipFile.GetEntry("Song.xml");

            try
            {
                if (zipEntry != null)
                {
                    XmlDocument doc = new XmlDocument();

                    Stream stream = zipFile.GetInputStream(zipEntry);

                    MemoryStream msEntry = new MemoryStream();
                    stream.CopyTo(msEntry);

                    msEntry.Position = 0;

                    doc.Load(msEntry);

                    XmlNode nodeversion = doc.SelectSingleNode("RenoiseSong/GlobalSongData/PlaybackEngineVersion");

                    if (nodeversion != null)
                    {
                        int version = Int16.Parse(nodeversion.InnerXml);
                        if (version != 1)
                        {
                            if (replaceZKCommand)
                            {
                                XmlNodeList list = doc.SelectNodes("//Lines/Line/EffectColumns/EffectColumn[Number='ZK']/Number");

                                list.Cast<XmlNode>().Select(o => o.InnerXml = "ZL").ToList();
                            }

                            nodeversion.InnerXml = "1";

                            zipFile.BeginUpdate();

                            CustomStaticDataSource sds = new CustomStaticDataSource();
                            sds.SetStream(msEntry);

                            doc.Save(sds.GetSource());

                            sds.GetSource().Position = 0;

                            zipFile.Add(sds, zipEntry.Name);
                            zipFile.CommitUpdate();
                        }
                        else
                            throw new XrnsException(DOWNGRADE_ALREADY_DONE);
                    }
                    else
                        throw new XrnsException(READING_ERROR);
                }
                else
                    throw new XrnsException(READING_ERROR);
            }
            finally
            {
                if (zipFile != null)
                    zipFile.Close();
            }

            return true;
        }
    }
}
