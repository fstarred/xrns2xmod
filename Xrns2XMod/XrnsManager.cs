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

        /// <summary>
        /// return sample stream with audio format info
        /// </summary>
        /// <param name="instrumentIndex"></param>
        /// <param name="sampleIndex"></param>
        /// <returns></returns>
        public SampleStreamInfo GetSampleStreamInfo(int instrumentIndex, int sampleIndex)
        {
            SampleStreamInfo output = new SampleStreamInfo();

            Stream outputStream = null;

            string captureSampleRegExpr = String.Format(@"SampleData/Instrument{0}.*/Sample{1}.*\.(wav|aiff?|ogg|flac|mp3|aac)$",
                    instrumentIndex.ToString("00", CultureInfo.InvariantCulture),
                    sampleIndex.ToString("00", CultureInfo.InvariantCulture)
                );

            Regex regPattern = new Regex(captureSampleRegExpr, RegexOptions.IgnoreCase);

            ZipFile zipFile = null;

            try
            {
                zipFile = new ZipFile(filename);

                string sampleFilename = null;

                foreach (ZipEntry zip in zipFile)
                {
                    Match matchInst = regPattern.Match(zip.Name);

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
                            stream.BaseStream.CopyTo(outputStream);

                            output.Stream = outputStream;

                            string extension = Path.GetExtension(sampleFilename).Substring(1);

                            if (extension.Equals("aif", StringComparison.InvariantCultureIgnoreCase))
                                extension = "aiff";

                            output.Format = (FORMAT)Enum.Parse(typeof(FORMAT), extension.ToUpper());
                        }
                    }
                }
                else
                {
                    // Sample not found!
                    System.Diagnostics.Debug.WriteLine("No sample caught!");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (zipFile != null)
                    zipFile.Close();
            }            

            return output;
        }

        /// <summary>
        /// return stream sample, otherwise null
        /// </summary>
        /// <param name="instrumentIndex"></param>
        /// <param name="sampleIndex"></param>
        /// <returns></returns>
        public Stream GetSampleStream(int instrumentIndex, int sampleIndex)
        {
            return GetSampleStreamInfo(instrumentIndex, sampleIndex).Stream;
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
