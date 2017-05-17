using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    public static class Utility
    {
        public static string NullToString(string value)
        {
            //return value == null ? String.Empty : value;
            return value ?? string.Empty;
        }


        /// <summary>
        /// Converts an integer into a byte array
        /// with the size of 4.
        /// source: http://codesnips.co.funpic.de/index.php?option=com_content&task=view&id=43&Itemid=34
        /// </summary>
        /// <param name="i">An unsigned integer value</param>
        /// <returns>Byte array with the size of 4</returns>
        public static byte[] MakeByte4FromInt(long i)
        {
            //return new byte[] { (byte)(i >> 24), (byte)(i >> 16), (byte)(i >> 8), (byte)i };
            //okay, after having a corrupted wave file, I noticed that we need the data in LSB:
            return new byte[] { (byte)i, (byte)(i >> 8), (byte)(i >> 16), (byte)(i >> 24) };
        }

        /// <summary>
        /// Converts an integer into a byte array
        /// with the size of 2.
        /// </summary>
        /// <param name="i">Integer value from 0 to 65536</param>
        /// <returns>Byte array with the size of 2</returns>
        public static byte[] MakeByte2FromInt(long i)
        {
            //return new byte[] { (byte)(i >> 8), (byte)i };
            //yes, everything but text is little endian:
            return new byte[] { (byte)i, (byte)(i >> 8) };
        }


        public static byte[] MakeByte2FromInt(long i, bool bigEndian)
        {
            if (!bigEndian)
                return MakeByte2FromInt(i);

            return new byte[] { (byte)(i >> 8), (byte)(i) };
        }


        public static void PutInt4InByteArray(long value, Byte[] array, int offset)
        {
            byte[] ret = MakeByte4FromInt(value);

            array[offset++] = ret[0];
            array[offset++] = ret[1];
            array[offset++] = ret[2];
            array[offset++] = ret[3];
        }

        public static void PutInt2InByteArray(long value, Byte[] array, int offset)
        {
            byte[] ret = MakeByte2FromInt(value);

            array[offset++] = ret[0];
            array[offset++] = ret[1];
        }

        public static void PutInt2InByteArray(long value, bool bigEndian, Byte[] array, int offset)
        {
            byte[] ret = MakeByte2FromInt(value, bigEndian);

            array[offset++] = ret[0];
            array[offset++] = ret[1];
        }

        public static void PutNameInByteArray(string value, byte[] output, int offset, int maxLen)
        {
            if (!String.IsNullOrEmpty(value))
            {
                System.Text.ASCIIEncoding _encoding = new System.Text.ASCIIEncoding();

                Array.Copy(_encoding.GetBytes(value), 0, output, offset, value.Length > maxLen ? maxLen : value.Length);
            }
        }

        public static byte[] GetBytesFromString(string value, int maxLen)
        {
            byte[] buffer = new byte[maxLen];

            if (!String.IsNullOrEmpty(value))
            {
                System.Text.ASCIIEncoding _encoding = new System.Text.ASCIIEncoding();

                Array.Copy(_encoding.GetBytes(value), 0, buffer, 0, value.Length > maxLen ? maxLen : value.Length);
            }

            return buffer;
        }



        // taken from http://www.yoda.arachsys.com/csharp/readbinary.html
        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        public static byte[] GetBytesFromStream(Stream stream, long initialLength)
        {
            // If we've been passed an unhelpful initial length, just
            // use 32K.
            if (initialLength <= 0)
            {
                initialLength = 32767;
            }

            byte[] buffer = new byte[initialLength];
            int read = 0;

            int chunk;
            while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
            {
                read += chunk;

                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    // End of stream? If so, we're done
                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    // Nope. Resize the buffer, put in the byte we've just
                    // read, and continue
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[read];
            Array.Copy(buffer, ret, read);
            return ret;
        }

        public static bool IsWindowsOS()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNearlyEqual(double a, double b, double epsilon)
        {
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a == b)
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || diff < Double.MinValue)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * Double.MinValue);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        public static void Save2File(string fileName, byte[] inputStream)
        {
            FileStream fileStreamToWrite = File.OpenWrite(fileName);
            fileStreamToWrite.Write(inputStream, 0, inputStream.Length);
            fileStreamToWrite.Close();
        }        

    }
}
