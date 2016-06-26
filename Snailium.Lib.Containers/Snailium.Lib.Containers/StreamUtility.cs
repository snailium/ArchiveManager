#region File Description
//-----------------------------------------------------------------------------
// StreamUtility.cs
//
// Snailium Library (http://www.snailium.net)
// Copyright (C) Snailium. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#endregion

namespace Snailium.Lib.Containers
{
    public class StreamUtility
    {
        /// <summary>
        /// Read an signed integer from stream.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <returns>The content in integer form.</returns>
        static public int ReadIntFromStream(Stream stream, long offsetFromBeginning)
        {
            return ReadIntFromStream(stream, offsetFromBeginning, true);
        }

        /// <summary>
        /// Read an signed integer from stream.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <param name="allowReverseOrder">If reversing order is allowed.</param>
        /// <returns>The content in integer form.</returns>
        static public int ReadIntFromStream(Stream stream, long offsetFromBeginning, bool allowReverseOrder)
        {
            byte[] tmpint = ReadBytesFromStream(stream, offsetFromBeginning, 4);
            if (allowReverseOrder && BitConverter.IsLittleEndian) Array.Reverse(tmpint);
            return BitConverter.ToInt32(tmpint, 0);
        }

        /// <summary>
        /// Read an unsigned integer from stream.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <returns>The content in integer form.</returns>
        static public uint ReadUIntFromStream(Stream stream, long offsetFromBeginning)
        {
            return ReadUIntFromStream(stream, offsetFromBeginning, true);
        }

        /// <summary>
        /// Read an unsigned integer from stream.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <param name="allowReverseOrder">If reversing order is allowed.</param>
        /// <returns>The content in integer form.</returns>
        static public uint ReadUIntFromStream(Stream stream, long offsetFromBeginning, bool allowReverseOrder)
        {
            byte[] tmpint = ReadBytesFromStream(stream, offsetFromBeginning, 4);
            if (allowReverseOrder && BitConverter.IsLittleEndian) Array.Reverse(tmpint);
            return BitConverter.ToUInt32(tmpint, 0);
        }

        /// <summary>
        /// Write an integer value to stream
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The integer to be written.</param>
        static public void WriteIntToStream(Stream stream, int value)
        {
            WriteIntToStream(stream, value, true);
        }

        /// <summary>
        /// Write an integer value to stream
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The integer to be written.</param>
        /// <param name="allowReverseOrder">If reversing order is allowed.</param>
        static public void WriteIntToStream(Stream stream, int value, bool allowReverseOrder)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (allowReverseOrder && BitConverter.IsLittleEndian) Array.Reverse(temp);
            WriteBytesToStream(stream, temp, 4);
        }

        /// <summary>
        /// Write an unsigned integer value to stream
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The unsigned integer to be written.</param>
        static public void WriteUIntToStream(Stream stream, uint value)
        {
            WriteUIntToStream(stream, value, true);
        }

        /// <summary>
        /// Write an integer unsigned value to stream
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The unsigned integer to be written.</param>
        /// <param name="allowReverseOrder">If reversing order is allowed.</param>
        static public void WriteUIntToStream(Stream stream, uint value, bool allowReverseOrder)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (allowReverseOrder && BitConverter.IsLittleEndian) Array.Reverse(temp);
            WriteBytesToStream(stream, temp, 4);
        }

        /// <summary>
        /// Read an signed long integer (64-bit) from stream.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <returns>The content in long integer (64-bit) form.</returns>
        static public long ReadLongFromStream(Stream stream, long offsetFromBeginning)
        {
            return ReadLongFromStream(stream, offsetFromBeginning, true);
        }

        /// <summary>
        /// Read an signed long integer (64-bit) from stream.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <param name="allowReverseOrder">If reversing order is allowed.</param>
        /// <returns>The content in long integer (64-bit) form.</returns>
        static public long ReadLongFromStream(Stream stream, long offsetFromBeginning, bool allowReverseOrder)
        {
            byte[] tmplong = ReadBytesFromStream(stream, offsetFromBeginning, 8);
            if (allowReverseOrder && BitConverter.IsLittleEndian) Array.Reverse(tmplong);
            return BitConverter.ToInt64(tmplong, 0);
        }

        /// <summary>
        /// Read an unsigned long integer (64-bit) from stream.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <returns>The content in long integer (64-bit) form.</returns>
        static public ulong ReadULongFromStream(Stream stream, long offsetFromBeginning)
        {
            return ReadULongFromStream(stream, offsetFromBeginning, true);
        }

        /// <summary>
        /// Read an unsigned long integer (64-bit) from stream.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <param name="allowReverseOrder">If reversing order is allowed.</param>
        /// <returns>The content in long integer (64-bit) form.</returns>
        static public ulong ReadULongFromStream(Stream stream, long offsetFromBeginning, bool allowReverseOrder)
        {
            byte[] tmplong = ReadBytesFromStream(stream, offsetFromBeginning, 8);
            if (allowReverseOrder && BitConverter.IsLittleEndian) Array.Reverse(tmplong);
            return BitConverter.ToUInt64(tmplong, 0);
        }

        /// <summary>
        /// Write an long integer (64-bit) value to stream
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The long integer (64-bit) to be written.</param>
        static public void WriteLongToStream(Stream stream, long value)
        {
            WriteLongToStream(stream, value, true);
        }

        /// <summary>
        /// Write an long integer (64-bit) value to stream
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The long integer (64-bit) to be written.</param>
        /// <param name="allowReverseOrder">If reversing order is allowed.</param>
        static public void WriteLongToStream(Stream stream, long value, bool allowReverseOrder)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (allowReverseOrder && BitConverter.IsLittleEndian) Array.Reverse(temp);
            WriteBytesToStream(stream, temp, 8);
        }

        /// <summary>
        /// Write an unsigned long integer (64-bit) value to stream
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The unsigned long integer (64-bit) to be written.</param>
        static public void WriteULongToStream(Stream stream, ulong value)
        {
            WriteULongToStream(stream, value, true);
        }

        /// <summary>
        /// Write an long integer (64-bit) unsigned value to stream
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The unsigned long integer (64-bit) to be written.</param>
        /// <param name="allowReverseOrder">If reversing order is allowed.</param>
        static public void WriteULongToStream(Stream stream, ulong value, bool allowReverseOrder)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (allowReverseOrder && BitConverter.IsLittleEndian) Array.Reverse(temp);
            WriteBytesToStream(stream, temp, 8);
        }

        /// <summary>
        /// Read a float value from stream. (IEEE 754 32-bit format)
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <returns>The content in float form.</returns>
        static public float ReadFloatFromStream(Stream stream, long offsetFromBeginning)
        {
            return ReadFloatFromStream(stream, offsetFromBeginning, true);
        }

        /// <summary>
        /// Read a float value from stream. (IEEE 754 32-bit format)
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <param name="allowReverseOrder">If reversing order is allowed.</param>
        /// <returns>The content in float form.</returns>
        static public float ReadFloatFromStream(Stream stream, long offsetFromBeginning, bool allowReverseOrder)
        {
            byte[] tmpint = ReadBytesFromStream(stream, offsetFromBeginning, 4);
            if (allowReverseOrder && BitConverter.IsLittleEndian) Array.Reverse(tmpint);
            return BitConverter.ToSingle(tmpint, 0);
        }

        /// <summary>
        /// Write a float value to stream. (IEEE 754 32-bit format)
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The float value to be written</param>
        static public void WriteFloatToStream(Stream stream, float value)
        {
            WriteFloatToStream(stream, value, true);
        }

        /// <summary>
        /// Write a float value to stream. (IEEE 754 32-bit format)
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The float value to be written</param>
        /// <param name="allowReverseOrder">If reversing order is allowed.</param>
        static public void WriteFloatToStream(Stream stream, float value, bool allowReverseOrder)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (allowReverseOrder && BitConverter.IsLittleEndian) Array.Reverse(temp);
            WriteBytesToStream(stream, temp, 4);
        }

        /// <summary>
        /// Read a double-precision float value from stream. (IEEE 754 64-bit format)
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <returns>The content in double-precision float form.</returns>
        static public double ReadDoubleFromStream(Stream stream, long offsetFromBeginning)
        {
            return ReadDoubleFromStream(stream, offsetFromBeginning, true);
        }

        /// <summary>
        /// Read a double-precision float value from stream. (IEEE 754 64-bit format)
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <param name="allowReverseOrder">If reversing order is allowed.</param>
        /// <returns>The content in double-precision float form.</returns>
        static public double ReadDoubleFromStream(Stream stream, long offsetFromBeginning, bool allowReverseOrder)
        {
            byte[] tmpint = ReadBytesFromStream(stream, offsetFromBeginning, 8);
            if (allowReverseOrder && BitConverter.IsLittleEndian) Array.Reverse(tmpint);
            return BitConverter.ToDouble(tmpint, 0);
        }

        /// <summary>
        /// Write a double-precision float value to stream. (IEEE 754 64-bit format)
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The double-precision float value to be written</param>
        static public void WriteDoubleToStream(Stream stream, double value)
        {
            WriteDoubleToStream(stream, value, true);
        }

        /// <summary>
        /// Write a double-precision float value to stream. (IEEE 754 64-bit format)
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The float value to be written</param>
        /// <param name="allowReverseOrder">If reversing order is allowed.</param>
        static public void WriteDoubleToStream(Stream stream, double value, bool allowReverseOrder)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (allowReverseOrder && BitConverter.IsLittleEndian) Array.Reverse(temp);
            WriteBytesToStream(stream, temp, 8);
        }

        /// <summary>
        /// Read a byte array from stream.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <param name="length">The length to be read.</param>
        /// <returns>The content in byte array form.</returns>
        static public byte[] ReadBytesFromStream(Stream stream, long offsetFromBeginning, int length)
        {
            byte[] tmpint = new byte[length];
            long ret = stream.Seek(offsetFromBeginning, SeekOrigin.Begin);
            stream.Read(tmpint, 0, length);
            return tmpint;
        }

        /// <summary>
        /// Write a whole byte array to stream.
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The byte array to be written.</param>
        static public void WriteBytesToStream(Stream stream, byte[] value)
        {
            WriteBytesToStream(stream, value, value.Length);
        }
        
        /// <summary>
        /// Write a byte array to stream.
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The byte array to be written.</param>
        /// <param name="length">The length to be written. If length is larger than the input array, or less than or equal to 0, the length of array will be used.</param>
        static public void WriteBytesToStream(Stream stream, byte[] value, int length)
        {
            int writeLength = length;
            if (length > value.Length || length <= 0) writeLength = value.Length;
            stream.Write(value, 0, writeLength);
        }

        /// <summary>
        /// Read a string from stream, with end delimiter set to "0x00".
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream.</param>
        /// <param name="length">The length to be read. If length less than or equal to 0, this function will keep reading until reach the end delimiter.</param>
        /// <returns></returns>
        static public string ReadStringFromStream(Stream stream, long offsetFromBeginning, int length)
        {
            return ReadStringFromStream(stream, offsetFromBeginning, length, 0x00);
        }
        
        /// <summary>
        /// Read a string from stream.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="offsetFromBeginning">The starting offset from beginning of source stream</param>
        /// <param name="length">The length to be read. If length less than or equal to 0, this function will keep reading until reach the end delimiter.</param>
        /// <param name="delimiter">The end delimiter of the string. This delimiter will be removed from final result string.</param>
        /// <returns></returns>
        static public string ReadStringFromStream(Stream stream, long offsetFromBeginning, int length, byte delimiter)
        {
            byte[] temp = { };
            
            if (length > 0)
            {
                temp = ReadBytesFromStream(stream, offsetFromBeginning, length);
                if (temp[temp.Length - 1] == delimiter) Array.Resize(ref temp, temp.Length - 1);
            }
            else
            {
                long currentOffset = offsetFromBeginning;
                while (true)
                {
                    byte[] nextOne = ReadBytesFromStream(stream, currentOffset, 1);
                    if (nextOne[0] == delimiter)
                    {
                        break;
                    }
                    else
                    {
                        Array.Resize(ref temp, temp.Length + 1);
                        temp[temp.Length - 1] = nextOne[0];
                    }
                    currentOffset++;
                }
            }

            return ConvertBytesToString(temp);
        }

        /// <summary>
        /// Write a string to stream, with end delimiter set to "0x00".
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The string to be written.</param>
        static public void WriteStringToStream(Stream stream, string value)
        {
            WriteStringToStream(stream, value, 0x00);
        }
        
        /// <summary>
        /// Write a string to stream
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <param name="value">The string to be written.</param>
        /// <param name="delimiter">The end delimiter of the string. If the input string have not been properly delimited, the delimiter will be inserted.</param>
        static public void WriteStringToStream(Stream stream, string value, byte delimiter)
        {
            byte[] temp = ConvertStringToBytes(value);
            WriteBytesToStream(stream, temp, temp.Length);
            if (temp[temp.Length - 1] != delimiter)
            {
                byte[] tmp = { delimiter };
                WriteBytesToStream(stream, tmp, tmp.Length);
            }
        }

        /// <summary>
        /// Copy a block of data from source stream, and append to destination stream.
        /// </summary>
        /// <param name="fromFile">The source stream.</param>
        /// <param name="toFile">The destination stream.</param>
        /// <param name="startOffset">The starting offset from beginning of source stream.</param>
        /// <param name="length">The length to be copied.</param>
        /// <returns></returns>
        static public long CopyBlock(Stream fromFile, Stream toFile, long startOffset, int length)
        {
            byte[] temp = ReadBytesFromStream(fromFile, startOffset, length);
            WriteBytesToStream(toFile, temp);
            return temp.Length;
        }

        /// <summary>
        /// Convert a string to byte array.
        /// </summary>
        /// <param name="value">The original string.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ConvertStringToBytes(string value)
        {
            MemoryStream stream = new MemoryStream();

            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(value);
                writer.Flush();
            }

            return stream.ToArray();
        }

        /// <summary>
        /// Convert a byte array to string
        /// </summary>
        /// <param name="bytes">The original byte array.</param>
        /// <returns>The string.</returns>
        public static string ConvertBytesToString(byte[] bytes)
        {
            string output = String.Empty;

            MemoryStream stream = new MemoryStream(bytes);
            stream.Position = 0;

            using (StreamReader reader = new StreamReader(stream))
            {
                output = reader.ReadToEnd();
            }

            return output;
        }

        /// <summary>
        /// Get the stream extension based on identifier value.
        /// </summary>
        /// <param name="identifier">32-bit (4-char) identifier.</param>
        /// <returns>File extension</returns>
        public static string GetFileExtension(uint identifier)
        {
            switch(identifier)
            {
                case 0x0ff512ee: { return "xbc"; }   //     : Xbox Compression stream
                case 0x43574142: { return "cwab"; }  // CWAB: Animation container (found in "Ore no Yome")
                case 0x44445320: { return "dds"; }   // DDS : Direct Draw Surface texture
                case 0x444e4257: { return "xwb"; }   // DNBW: Xbox Wave Bank Audio [http://wiki.xentax.com/index.php/XBOX_XWB3]
                case 0x4d414142: { return "maab"; }  // MAAB: Animation container (found in "Ore no Yome")
                case 0x4d414230: { return "mab"; }   // MABO: Animation index stream (found in "Ore no Yome")
                case 0x52494646: { return "wav"; }   // RIFF: Generic WAVE stream (could be any of the subset, e.g. xWMA)
                case 0x554e4932: { return "uni"; }   // UNI2: Union container
                case 0x53435841: { return "axcs"; }  // SCXA: AXCS archive container (found in "Pia Carrot 4")
                case 0x89504e47: { return "png"; }   //  PNG: Portable Network Graphics
                case 0x43504b20: { return "cpk"; }   // CPK : CRI-ware Package
                case 0x4c4e4b34: { return "lnk4"; }  // LNK4: Link-4 Package
                case 0x41465300: { return "afs"; }   // AFS : Sega AFS Package
                default: { return "stream"; }
            }
        }
    }
}
