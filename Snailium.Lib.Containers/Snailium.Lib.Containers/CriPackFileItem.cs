#region File Description
//-----------------------------------------------------------------------------
// CriPackFileItem.cs
//
// Snailium Library (http://www.snailium.net)
// Copyright (C) Snailium. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
#endregion

namespace Snailium.Lib.Containers
{
    public class CriPackFileItem : FileItem
    {
        #region Fields
        /// <summary>
        /// Name of the directory
        /// </summary>
        protected string dirName;

        /// <summary>
        /// Local directory
        /// </summary>
        protected string localDir;

        /// <summary>
        /// Size of the extracted file in bytes.
        /// </summary>
        protected uint extractSize;

        /// <summary>
        /// The index of the file.
        /// </summary>
        protected uint fileId;

        /// <summary>
        /// CRC of the file.
        /// </summary>
        protected uint crc;

        /// <summary>
        /// Additional information.
        /// </summary>
        protected string userString;

        #endregion

        #region Properties
        /// <summary>
        /// Name of the directory
        /// </summary>
        public string DirName
        {
            set { this.dirName = value; }
            get { return this.dirName; }
        }

        /// <summary>
        /// Size of the extracted file in bytes.
        /// </summary>
        public uint ExtractSize
        {
            set { this.extractSize = value; }
            get { return this.extractSize; }
        }

        /// <summary>
        /// The index of the file.
        /// </summary>
        public uint FileId
        {
            set { this.fileId = value; }
            get { return this.fileId; }
        }

        /// <summary>
        /// Additional information.
        /// </summary>
        public string UserString
        {
            set { this.userString = value; }
            get { return this.userString; }
        }

        /// <summary>
        /// CRC of the file.
        /// </summary>
        public uint CRC
        {
            set { this.crc = value; }
            get { return this.crc; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public CriPackFileItem(FileStream file)
            : base(file)
        {
            this.dirName = "";
            this.localDir = "";
            this.extractSize = 0;
            this.fileId = 0;
            this.userString = null;
            this.crc = 0;
        }
        #endregion

        #region Actions
        /// <summary>
        /// Dump this file data to another file.
        /// </summary>
        /// <param name="toFile">The destination file stream.</param>
        /// <returns>Length copied.</returns>
        public override long DumpData(FileStream toFile)
        {
            if (this.file == null || !this.file.CanRead)
                throw new IOException("Source stream is not accessable!");

            if (this.fileSize < this.extractSize)
            {
                // Compressed
                return this.DecompressToFile(toFile);
            }
            else
            {
                // Not compressed
                byte[] content = StreamUtility.ReadBytesFromStream(this.file, this.fileOffset, (int)this.extractSize);
                Debug.WriteLine(String.Format("CRC for file {0} is {1:X4}", this.fileName, Crc32.CalcCrc32(content)));
                return StreamUtility.CopyBlock(this.file, toFile, this.fileOffset, (int)this.extractSize);
            }
        }

        /// <summary>
        /// Dump this file data to another file.
        /// </summary>
        /// <param name="path">The destination path (without file name).</param>
        /// <returns>Length copied.</returns>
        public override long DumpData(string baseDir)
        {
            if (this.file == null || !this.file.CanRead)
                throw new IOException("Source stream is not accessable!");

            string dir = baseDir + @"\" + this.dirName;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            FileStream toFile = new FileStream(dir + @"\" + this.fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            long length = this.DumpData(toFile);
            toFile.Close();
            return length;
        }
        #endregion

        #region Helper
        /// <summary>
        /// String representation of this file.
        /// </summary>
        /// <returns>The descriptive string.</returns>
        public override string ToString()
        {
            return this.dirName + "\\" + this.fileName + " (" + Convert.ToString(this.fileSize) + " bytes)" + 
                ((this.fileSize < this.extractSize) ? " [C]" : "");
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Assign properties from TOC.
        /// </summary>
        /// <param name="properties">Property bank in TOC.</param>
        public void AssignTocProperties(CriPack_UTFProperty[] properties, long tocOffset)
        {
            this.DirName = (string)CriPack_UTF.GetPropertyValue(properties, "DirName");
            this.FileName = (string)CriPack_UTF.GetPropertyValue(properties, "FileName");
            this.FileSize = (uint)CriPack_UTF.GetPropertyValue(properties, "FileSize");
            this.ExtractSize = (uint)CriPack_UTF.GetPropertyValue(properties, "ExtractSize");
            this.FileOffset = tocOffset + (long)(ulong)CriPack_UTF.GetPropertyValue(properties, "FileOffset");
            this.FileId = (uint)CriPack_UTF.GetPropertyValue(properties, "ID");
            this.UserString = (string)CriPack_UTF.GetPropertyValue(properties, "UserString");
            this.CRC = (uint)CriPack_UTF.GetPropertyValue(properties, "CRC");
        }

        /// <summary>
        /// Assign properties from E-TOC.
        /// </summary>
        /// <param name="properties">Property bank in E-TOC.</param>
        public void AssignEtocProperties(CriPack_UTFProperty[] properties)
        {
            this.localDir = (string)CriPack_UTF.GetPropertyValue(properties, "LocalDir");
            ulong timestamp = (ulong)CriPack_UTF.GetPropertyValue(properties, "UpdateDateTime");
            // Convert time
            int timeYear = (int)((timestamp >> 48) & 0xffff);
            int timeMonth = (int)((timestamp >> 40) & 0xff);
            int timeDay = (int)((timestamp >> 32) & 0xff);
            int timeHour = (int)((timestamp >> 24) & 0xff);
            int timeMinute = (int)((timestamp >> 16) & 0xff);
            int timeSecond = (int)((timestamp >> 8) & 0xff);
            this.fileDate = new DateTime(timeYear, timeMonth, timeDay, timeHour, timeMinute, timeSecond);
        }
        #endregion

        #region Compression
        public long DecompressToFile(FileStream toFile)
        {
            long offset = this.fileOffset;

            // Check identifier
            ulong identifier = StreamUtility.ReadULongFromStream(this.file, offset);
            if (identifier != 0 && identifier != 0x4352494c41594c41)
                throw new FormatException("File is not compressed by CRILAYLA!");

            int decompSize = StreamUtility.ReadIntFromStream(this.file, offset + 0x8);
            long decompHeaderOffset = StreamUtility.ReadUIntFromStream(this.file, offset + 0xC) + offset + 0x10;

            if (decompHeaderOffset + 0x100 != offset + this.fileSize)
                throw new FormatException("Size mismatch!");

            // Extract real header
            StreamUtility.CopyBlock(this.file, toFile, decompHeaderOffset, 0x100);

            //const long input_end = offset + input_size - 0x100 - 1;
            //long input_offset = input_end;
            //const long output_end = 0x100 + uncompressed_size - 1;
            //uint8_t bit_pool = 0;
            //int bits_left = 0;
            //long bytes_output = 0;

            //while ( bytes_output < uncompressed_size )
            //{
            //    if (GET_NEXT_BITS(1))
            //    {
            //        long backreference_offset =
            //            output_end-bytes_output+GET_NEXT_BITS(13)+3;
            //        long backreference_length = 3;

            //        // decode variable length coding for length
            //        enum { vle_levels = 4 };
            //        int vle_lens[vle_levels] = { 2, 3, 5, 8 };
            //        int vle_level;
            //        for (vle_level = 0; vle_level < vle_levels; vle_level++)
            //        {
            //            int this_level = GET_NEXT_BITS(vle_lens[vle_level]);
            //            backreference_length += this_level;
            //            if (this_level != ((1 << vle_lens[vle_level])-1)) break;
            //        }
            //        if (vle_level == vle_levels)
            //        {
            //            int this_level;
            //            do
            //            {
            //                this_level = GET_NEXT_BITS(8);
            //                backreference_length += this_level;
            //            } while (this_level == 255);
            //        }

            //        //printf("0x%08lx backreference to 0x%lx, length 0x%lx\n", output_end-bytes_output, backreference_offset, backreference_length);
            //        for (int i=0;i<backreference_length;i++)
            //        {
            //            output_buffer[output_end-bytes_output] = output_buffer[backreference_offset--];
            //            bytes_output++;
            //        }
            //    }
            //    else
            //    {
            //        // verbatim byte
            //        output_buffer[output_end-bytes_output] = GET_NEXT_BITS(8);
            //        //printf("0x%08lx verbatim byte\n", output_end-bytes_output);
            //        bytes_output++;
            //    }
            //}

            //put_bytes_seek(0, outfile, output_buffer, 0x100 + uncompressed_size);
            //free(output_buffer);

            return 0x100 + decompSize;
        }

        #endregion
    }

}
