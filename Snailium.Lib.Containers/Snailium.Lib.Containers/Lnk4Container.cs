#region File Description
//-----------------------------------------------------------------------------
// Lnk4Container.cs
//
// Snailium Library (http://www.snailium.net)
// Copyright (C) Snailium. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
#endregion

namespace Snailium.Lib.Containers
{
    #region Structs
    /// <summary>
    /// The struct of a Lnk4 file table item.
    /// </summary>
    public struct Lnk4FileItem
    {
        #region Fields
        /// <summary>
        /// ID of the file.
        /// </summary>
        public uint fileId;

        /// <summary>
        /// Which address the file data starts.
        /// Note: this is an offset from the beginning of data section.
        /// </summary>
        public uint startOffset;

        /// <summary>
        /// How many real bytes are consumed by the file data.
        /// </summary>
        public uint length;

        #endregion

        #region Utilities
        /// <summary>
        /// Check if this item is valid. An invalid item means all fiels are zero (0).
        /// </summary>
        /// <returns>If this item is valid.</returns>
        public bool isValid()
        {
            if (startOffset == 0 && length == 0)
                return false;
            else
                return true;
        }
        #endregion
    }

    #endregion

    /// <summary>
    /// Class to access a Lnk4 file.
    /// </summary>
    public class Lnk4Container : FileContainer
    {
        #region Fields
        /// <summary>
        /// The number of files extracted from header.
        /// </summary>
        private uint numFiles;

        /// <summary>
        /// The start address of file data.
        /// </summary>
        private uint dataOffset;     // Default: Data section is located from Cluster 2

        #endregion

        #region Properties
        /// <summary>
        /// The start address of file data.
        /// </summary>
        public uint DataOffset
        {
            set { this.dataOffset = value; }
            get { return this.dataOffset; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Construct a Lnk4 file.
        /// </summary>
        /// <param name="file">File stream.</param>
        public Lnk4Container(FileStream file)
            : base(file)
        {
            long offset = 0;
            
            // Confirm 'Lnk4' identifier
            uint identifier = StreamUtility.ReadUIntFromStream(this.container, offset);
            offset += 4;
            if (identifier != 0x4c4e4b34) // If file identifier is not Lnk4
                throw new FormatException(container.Name + ": File is not Lnk4 format!");

            // Get file address
            this.dataOffset = StreamUtility.ReadUIntFromStream(this.container, offset, false);
            offset += 4;

            this.containerType = "Link 4";
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Parse the file table.
        /// </summary>
        /// <returns>Number of files found.</returns>
        public override int ParseFileTable()
        {
            long offset = 8;
            uint fid = 0;
            
            this.fileTable = new List<FileItem>();

            string fileNameBase;
            { // Get file name base
                int fileNameStart = this.container.Name.LastIndexOf('\\') + 1;
                int fileNameEnd = this.container.Name.LastIndexOf('.');
                if (fileNameEnd == -1) fileNameEnd = this.container.Name.Length;
                fileNameBase = this.container.Name.Substring(fileNameStart, fileNameEnd - fileNameStart);
            }

            while (true)
            {
                Lnk4FileItem item = new Lnk4FileItem();
                item.fileId = fid;
                item.startOffset = StreamUtility.ReadUIntFromStream(this.container, offset, false) << 11;
                offset += 4;
                item.length = StreamUtility.ReadUIntFromStream(this.container, offset, false) << 10;
                offset += 4;

                if (item.isValid())
                {
                    FileItem FileItem = ConvertLnk4ToGeneral(item, fileNameBase + "-" + Convert.ToSingle(item.fileId));
                    uint fileIdentifier = StreamUtility.ReadUIntFromStream(this.container, FileItem.FileOffset);
                    string fileExt = StreamUtility.GetFileExtension(fileIdentifier);
                    FileItem.FileName += "." + fileExt;
                    fileTable.Add(FileItem);
                }
                else
                {
                    break;
                }

                fid++;
            }

            return fileTable.Count;
        }

        private FileItem ConvertLnk4ToGeneral(Lnk4FileItem item, string fileName)
        {
            FileItem FileItem = new FileItem(this.container);
            FileItem.FileName = fileName;
            FileItem.FileOffset = item.startOffset + this.dataOffset;
            FileItem.FileSize = item.length;
            return FileItem;
        }

        private Lnk4FileItem ConvertGeneralToLnk4(FileItem item, uint fileId)
        {
            Lnk4FileItem FileItem = new Lnk4FileItem();
            FileItem.fileId = fileId;
            FileItem.startOffset = (uint)(item.FileOffset - this.dataOffset);
            FileItem.length = item.FileSize;
            return FileItem;
        }
        #endregion
    }
}
