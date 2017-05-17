using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Xrns2XMod.Properties;

namespace Xrns2XMod
{
    class AudioEncUtil
    {
        public static Stream EncodeDelta8BitMonoSample(byte[] buffer)
        {
            MemoryStream inputSample = new MemoryStream(buffer);

            MemoryStream outputStream = new MemoryStream();

            BinaryReader reader = new BinaryReader(inputSample);

            BinaryWriter writer = new BinaryWriter(outputStream);

            //int s_old = reader.ReadByte();

            int s_old = 128;

            inputSample.Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < inputSample.Length; i++)
            {
                int s_new = reader.ReadByte();
                int s_diff = (int)(s_new - s_old);

                writer.Write((sbyte)s_diff);

                s_old = s_new;
            }

            return outputStream;
        }

        public static Stream EncodeDelta8BitStereoSample(byte[] buffer)
        {
            MemoryStream inputSample = new MemoryStream(buffer);

            BinaryReader reader = new BinaryReader(inputSample);

            MemoryStream leftChannel = new MemoryStream();
            MemoryStream rightChannel = new MemoryStream();
            MemoryStream outputStream = new MemoryStream();

            BinaryWriter leftChannelWriter = new BinaryWriter(leftChannel);
            BinaryWriter rightChannelWriter = new BinaryWriter(rightChannel);

            //int s_oldLeft = reader.ReadByte();
            //int s_oldRight = reader.ReadByte();

            int s_oldLeft = 128;
            int s_oldRight = 128;

            inputSample.Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < (inputSample.Length); i++)
            {
                if (i % 2 == 0)
                {
                    int s_newLeft = reader.ReadByte();
                    int s_diffLeft = (int)(s_newLeft - s_oldLeft); // should be unsigned; if possible, use uint8 or uint8_t or whatever it's called in c# for more clarity. 

                    leftChannelWriter.Write((sbyte)s_diffLeft);

                    s_oldLeft = s_newLeft;
                }
                else
                {
                    int s_newRight = reader.ReadByte();
                    int s_diffRight = (int)(s_newRight - s_oldRight); // should be unsigned; if possible, use uint8 or uint8_t or whatever it's called in c# for more clarity. 

                    rightChannelWriter.Write((sbyte)s_diffRight);

                    s_oldRight = s_newRight;
                }
            }

            leftChannel.Seek(0, SeekOrigin.Begin);
            rightChannel.Seek(0, SeekOrigin.Begin);

            buffer = new byte[leftChannel.Length];
            leftChannel.Read(buffer, 0, buffer.Length);

            outputStream.Write(buffer, 0, buffer.Length);

            buffer = new byte[rightChannel.Length];
            rightChannel.Read(buffer, 0, buffer.Length);

            outputStream.Write(buffer, 0, buffer.Length);

            return outputStream;
        }

        public static Stream EncodeDelta16BitMonoSample(byte[] buffer)
        {
            MemoryStream inputSample = new MemoryStream(buffer);

            BinaryReader reader = new BinaryReader(inputSample);

            MemoryStream stream = new MemoryStream();

            BinaryWriter writer = new BinaryWriter(stream);

            int s_old = 0; //reader.ReadInt16();

            inputSample.Seek(0, SeekOrigin.Begin);

            short[] values = new short[inputSample.Length / 2];

            for (int i = 0; i < (inputSample.Length / 2); i++)
            {
                int s_new = reader.ReadInt16();
                int s_diff = (int)(s_new - s_old);

                writer.Write((short)s_diff);

                s_old = s_new;

                values[i] = (short)s_diff;
            }

            return stream;
        }


        public static Stream EncodeDelta16BitStereoSample(byte[] buffer)
        {
            MemoryStream inputSample = new MemoryStream(buffer);

            BinaryReader reader = new BinaryReader(inputSample);

            MemoryStream leftChannel = new MemoryStream();
            MemoryStream rightChannel = new MemoryStream();
            MemoryStream outputStream = new MemoryStream();

            BinaryWriter leftChannelWriter = new BinaryWriter(leftChannel);
            BinaryWriter rightChannelWriter = new BinaryWriter(rightChannel);

            int s_oldLeft = 0;// reader.ReadInt16();
            int s_oldRight = 0;// reader.ReadInt16();

            inputSample.Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < (inputSample.Length / 2); i++)
            {
                if (i % 2 == 0)
                {
                    int s_newLeft = reader.ReadInt16();
                    int s_diffLeft = (int)(s_newLeft - s_oldLeft); // should be unsigned; if possible, use uint8 or uint8_t or whatever it's called in c# for more clarity. 

                    leftChannelWriter.Write((short)s_diffLeft);

                    s_oldLeft = s_newLeft;
                }
                else
                {
                    int s_newRight = reader.ReadInt16();
                    int s_diffRight = (int)(s_newRight - s_oldRight); // should be unsigned; if possible, use uint8 or uint8_t or whatever it's called in c# for more clarity. 

                    rightChannelWriter.Write((short)s_diffRight);

                    s_oldRight = s_newRight;
                }
            }

            leftChannel.Seek(0, SeekOrigin.Begin);
            rightChannel.Seek(0, SeekOrigin.Begin);

            buffer = new byte[leftChannel.Length];
            leftChannel.Read(buffer, 0, buffer.Length);

            outputStream.Write(buffer, 0, buffer.Length);

            buffer = new byte[rightChannel.Length];
            rightChannel.Read(buffer, 0, buffer.Length);

            outputStream.Write(buffer, 0, buffer.Length);

            return outputStream;
        }


    }


}
