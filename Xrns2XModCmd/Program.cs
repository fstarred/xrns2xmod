using NDesk.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xrns2XMod;

namespace Xrns2XModCmd
{
    class Program
    {
        const int freeSpaceForMessaging = 50;

        const int cursorLeftPosition = freeSpaceForMessaging + 2;

        static readonly string[] VOLUME_SCALING_AVAILABLE_MODE = new string[] { "N", "S", "C" };

        private static void PrintUsage(OptionSet p)
        {
            Console.WriteLine("usage:");
            Console.WriteLine("Xrns2XmodCmd.exe <input-file> [options]");
            Console.WriteLine("options:");

            p.WriteOptionDescriptions(Console.Out);
        }

        // not used
        const int DEFAULT_TEMPO = 125;
        const int DEFAULT_TICKS = 6;

        static int Main(string[] args)
        {
            int retcode = 0;

            try
            {
                string destType = "xm";
                //bool convfreq = false;
                bool ptmode = true;
                bool ptNtsc = false;
                string volumeScalingMode = "S";
                bool downgrade = false;
                bool replaceZK = false;
                //int fpl = 0;
                //int fvl = 0;
                string outputFile = string.Empty;
                string outputLog = null;
                string bassEmail = null;
                string bassCode = null;
                int portamentoLossThreshold = 2;
                int tempo = 0;
                int ticks = 0;

                var p = new OptionSet() {
                    { "t|type=", "the destination format type (xm|mod).",
                      v => destType = v },
                    //{ "convfreq", "convert all sample frequency to 8363 Hz (affects only mod)",
                    //  v => convfreq = v != null },
                    { "ptmode=", "ProTracker compatibility (affects only mod)",
                      v => ptmode = v == null || Boolean.Parse(v) == true },
                    { "ntsc=", "ProTracker Region (affects only mod)",
                      v => ptNtsc = v == null || Boolean.Parse(v) == true },
                    { "volscal|volumescaling=", "volume scaling mode (N(one)|S(ample)|C(olumn))",
                      v => volumeScalingMode = v },
                    { "tempo=" , "the initial tempo value (affects only xm)", v => tempo = int.Parse(v) },
                    { "ticks=" , "the initial ticks/row value (affects only xm)", v => ticks = int.Parse(v) },
                    //{ "fpl=", "Apply Fine pitch not beyond x value (0 - 15)",
                    //  (int v) => fpl = v  },
                    //{ "fvl=", "Apply Fine volume not beyond x value (0 - 15)",
                    //  (int v) => fvl = v  },
                    { "portresh=", "Portamento treshold value (0-4, default: 2) (affects only mod)",
                      (int v) => portamentoLossThreshold = v  },
                    { "out=", "the <output-file>",
                      v => outputFile = v },
                    { "log=", "the <output-log>",
                      v => outputLog = v },
                    { "bass_email=", "bass registered email",
                      v => bassEmail = v },
                    { "bass_code=", "bass registered code",
                      v => bassCode = v },
                    //{ "downgrade", "downgrade playback engine to TIMING MODEL SPEED",
                    //  v => downgrade = v != null },
                    //{ "replaceZK", "(downgrade mode) Replace ZK with ZL command",
                    //  v => replaceZK = v != null },
                };

                List<string> extra;
                try
                {
                    extra = p.Parse(args);
                }
                catch (OptionException e)
                {
                    Console.WriteLine(e.Message);
                    return -1;
                }

                if (args.Length < 1)
                {
                    PrintUsage(p);
                }
                else
                {
                    string inputFile;

                    if (extra.Count == 1)
                    {
                        inputFile = extra[0];

                        if (!File.Exists(inputFile) || !Path.GetExtension(inputFile).Equals(".xrns", StringComparison.InvariantCultureIgnoreCase))
                        {
                            throw new ApplicationException("A valid xrns input file must be specified");
                        }
                    }
                    else
                        throw new ApplicationException("wrong argument number");

                    // output file
                    if (string.IsNullOrEmpty(outputFile))
                        outputFile = inputFile.Remove(inputFile.Length - 5); // default output file, same as input without .xrns extension

                    // destination format
                    destType = destType.ToLowerInvariant();

                    if ((destType.Equals("mod") || destType.Equals("xm")) == false)
                        throw new ApplicationException("-type must be xm or mod");

                    #region oldcode 2
                    // fine pitch value
                    //if ((fpl < 0 || fpl > 0x10) == false)
                    //    Settings.Default.ExtraFinePitchValue = fpl;
                    //else
                    //    throw new ApplicationException("fpl should be a number between 0 and 15");

                    // fine volume value
                    //if ((fvl < 0 || fvl > 0x10) == false)
                    //    Settings.Default.ExtraFineVolumeValue = fvl;
                    //else
                    //    throw new ApplicationException("fvl should be a number between 0 and 15");

                    // frequency conversion
                    //if (convfreq)
                    //    Settings.Default.MantainOriginalSampleFreq = false;

                    // pro tracker compatibility
                    //if (ptmode)
                    //    Settings.Default.PTCompatibiliy = true;

                    // volume adjust
                    //if (adjvol)
                    //    Settings.Default.VolumeResampling = true;

                    // bass email
                    //if (string.IsNullOrEmpty(bassEmail) == false)
                    //    Settings.Default.BassEmail = bassEmail;

                    // bass code
                    //if (string.IsNullOrEmpty(bassCode) == false)
                    //    Settings.Default.BassCode = bassCode;
                    #endregion

                    // log file
                    if (string.IsNullOrEmpty(outputLog) == false)
                    {
                        bool isValidLogPath = true;

                        if ((string.IsNullOrEmpty(Path.GetDirectoryName(outputLog)) == false) && Directory.Exists(outputLog) == false)
                            isValidLogPath = false;


                        //if (string.IsNullOrEmpty(Path.GetFileName(outputLog)) == false)
                        //    isValidLogPath = false;


                        if (isValidLogPath)
                        {
                            StreamWriter sw = new StreamWriter(outputLog);
                            sw.AutoFlush = true;
                            Console.SetOut(sw);
                        }
                        else
                            throw new ApplicationException("invalid log path");
                    }

                    #region old code
                    //for (int i = 1; i < args.Length; i++)
                    //{
                    //    switch (args[i])
                    //    {                            
                    //        case "-out":
                    //            i++;
                    //            if (args.Length >= i + 1)
                    //            {
                    //                outputFile = args[i];
                    //            }
                    //            else
                    //            {
                    //                throw new ApplicationException("-out requires a valid filename");
                    //            }
                    //            break;
                    //        case "-type":
                    //            i++;
                    //            if (args.Length >= i + 1)
                    //            {
                    //                if (args[i].ToLowerInvariant().Equals("xm"))
                    //                {
                    //                    destType = "xm";
                    //                }
                    //                else if (args[i].ToLowerInvariant().Equals("mod"))
                    //                {
                    //                    destType = "mod";
                    //                }
                    //                else
                    //                {
                    //                    throw new ApplicationException("invalid argument: " + args[i - 1] + "; type must be xm or mod");
                    //                }
                    //            }
                    //            else
                    //            {
                    //                throw new ApplicationException("-type must be xm or mod");
                    //            }
                    //            break;
                    //        case "-convfreq":
                    //            Settings.Default.MantainOriginalSampleFreq = false;
                    //            break;
                    //        case "-ptmode":
                    //            Settings.Default.PTCompatibiliy = true;
                    //            break;
                    //        case "-modvol":
                    //            Settings.Default.VolumeResampling = true;
                    //            break;
                    //        case "-fpl":
                    //            i++;
                    //            if (args.Length >= i + 1)
                    //            {
                    //                int argument = int.Parse(args[i]);
                    //                if (argument < 0x10)
                    //                {
                    //                    Settings.Default.ExtraFinePitchValue = argument;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                throw new ApplicationException("invalid argument: " + args[i - 1] + "; fpl should be a number between 0 and 15");
                    //            }
                    //            break;
                    //        case "-fvl":
                    //            i++;
                    //            if (args.Length >= i + 1)
                    //            {
                    //                int argument = int.Parse(args[i]);
                    //                if (argument < 0x10)
                    //                {
                    //                    Settings.Default.ExtraFineVolumeValue = argument;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                throw new ApplicationException("invalid argument: " + args[i - 1] + "; fvl should be a number between 0 and 15");
                    //            }
                    //            break;
                    //        default:
                    //            throw new ApplicationException("Error: some arguments might be not correct");
                    //    }
                    //}
                    #endregion

                    if (downgrade)
                    {
                        XrnsManager manager = new XrnsManager(inputFile);

                        Console.WriteLine("Loading file...");

                        //manager.Load(inputFile);

                        Console.Write("Starting downgrade ");

                        if (replaceZK)
                            Console.Write("and replacing ZK commands ");

                        Console.WriteLine("...");

                        try
                        {
                            manager.DowngradeSong(replaceZK);

                            Console.WriteLine("Downgrade completed");
                        }
                        catch (XrnsException e)
                        {
                            Console.WriteLine(e.Message);
                            retcode = -1;
                        }

                        return retcode;

                        //manager.Close();
                    }

                    BassWrapper.InitResources(IntPtr.Zero, bassEmail, bassCode);

                    //MainFactory.InitResources(IntPtr.Zero, bassEmail, bassCode);

                    /*
                    ** Slamy: Moving the cursor like this causes problems on Mono.
                    ** It says 'fixing "Value must be positive and below the buffer height."'
                    ** But it doesn't seems to be necessary anyway? I'll comment it out.
                    */
                    //Console.CursorTop++;

                    Console.WriteLine("Reading xrns data...");

                    //MainFactory.LoadXrns(inputFile);

                    SongDataFactory songDataFactory = new SongDataFactory();

                    songDataFactory.ReportProgress += ReportProgress;

                    RenoiseSong renoiseSong = songDataFactory.ExtractRenoiseSong(inputFile);

                    SongData songData = songDataFactory.ExtractSongData(renoiseSong, inputFile);

                    byte[] output;

                    IConverter converter = null;

                    Console.Write(Environment.NewLine);

                    Console.WriteLine("File is ok, now it's time to convert.");

                    Console.WriteLine("{0} conversion has started... please wait ", destType);

                    Thread t = new Thread(new ThreadStart(ThreadProc));

                    t.IsBackground = true;

                    t.Start();

                    DateTime dtStart = DateTime.Now;

                    //MainFactory.ReportProgress += new ReportProgressHandler(converter_ReportProgress);

                    if (destType.Equals("xm"))
                    {
                        XmSettings settings = new XmSettings();

                        if (tempo == 0)
                            tempo = songData.InitialBPM;
                        if (ticks == 0)
                            ticks = songData.TicksPerLine;

                        settings.VolumeScalingMode = GetVolumeScalingMode(volumeScalingMode);                        
                        settings.Tempo = tempo;
                        settings.TicksRow = ticks;

                        converter = new XMConverter(inputFile);
                        converter.EventProgress += ReportProgress;

                        ((XMConverter)converter).Settings = settings;
                    }
                    else
                    {
                        ModSettings settings = new ModSettings();

                        //settings.MantainOriginalSampleFreq = !convfreq;
                        settings.ForceProTrackerCompatibility = ptmode;
                        settings.NtscMode = ptNtsc;
                        settings.VolumeScalingMode = GetVolumeScalingMode(volumeScalingMode);
                        settings.PortamentoLossThreshold = portamentoLossThreshold;

						settings.printSettings();

                        if (portamentoLossThreshold < 0 || portamentoLossThreshold > 4)
                            throw new ApplicationException("invalid portamento loss threshold value (valid range: 0-4)");
                        if (settings.VolumeScalingMode == VOLUME_SCALING_MODE.COLUMN)
                            throw new ApplicationException("invalid volume scaling mode for MOD");

                        //settings.VolumeScaling = adjvol;

                        converter = new ModConverter(inputFile);
                        converter.EventProgress += ReportProgress;

                        ((ModConverter)converter).Settings = settings;
                    }

                    output = converter.Convert(songData);

                    //outputFile = Path.Combine(Path.GetDirectoryName(outputFile), Path.GetFileNameWithoutExtension(outputFile));

                    outputFile = System.Text.RegularExpressions.Regex.Match(outputFile, @"(?:(?!\.(mod|xm)$).)*").Value;

                    string outputFileExt = System.Text.RegularExpressions.Regex.Match(outputFile, @"\.(mod | xm)$").Value;

                    // add extension to output file in case user has not already specified it
                    if (!outputFileExt.Equals("." + destType, StringComparison.CurrentCultureIgnoreCase))
                    {
                        outputFile += '.' + destType;
                    }

                    Utility.SaveByteArrayToFile(outputFile, output);

                    TimeSpan ts = DateTime.Now.Subtract(dtStart);

                    Console.WriteLine("\nDone");
                    Console.WriteLine("Time elapsed: {0}", ts.TotalSeconds);
                    Console.WriteLine("Output file size: {0} bytes", new FileInfo(outputFile).Length);
                }
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
                retcode = -1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                retcode = -1;
            }
            finally
            {
                //MainFactory.FreeResources();
            }

            return retcode;
        }

        static VOLUME_SCALING_MODE GetVolumeScalingMode(string input)
        {
            VOLUME_SCALING_MODE mode = VOLUME_SCALING_MODE.SAMPLE;

            input = input.ToUpper();

            if (VOLUME_SCALING_AVAILABLE_MODE.Contains(input))
            {

                switch (input.ToCharArray()[0])
                {
                    case 'N':
                        mode = VOLUME_SCALING_MODE.NONE;
                        break;
                    case 'S':
                        mode = VOLUME_SCALING_MODE.SAMPLE;
                        break;
                    case 'C':
                        mode = VOLUME_SCALING_MODE.COLUMN;
                        break;
                }
            }
            else
                throw new ApplicationException("Invalid volume scaling specified");

            return mode;
        }

        static void ReportProgress(object sender, EventReportProgressArgs e)
        {
            Console.CursorLeft = 0;

            Console.CursorLeft = cursorLeftPosition;

            Console.WriteLine(" ");

            Console.CursorLeft = 0;

            Console.Write(e.message);

        }


        public static void ThreadProc()
        {
            char[] loopCharSeq = { '\\', '|', '/', '-' };
            int curChar = 0;

            while (true)
            {
                Console.CursorLeft = cursorLeftPosition;
                Console.Write(loopCharSeq[curChar++ % 4]);
                Thread.Sleep(500);
            }
        }


    }


}
