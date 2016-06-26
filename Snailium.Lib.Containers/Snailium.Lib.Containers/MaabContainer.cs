#region File Description
//-----------------------------------------------------------------------------
// MaabContainer.cs
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
    /// <summary>
    /// Class to access a MAAB file.
    /// </summary>
    public class MaabContainer : FileContainer
    {
        #region Fields
        /// <summary>
        /// The number of files extracted from header.
        /// </summary>
        private uint numFiles;

        #endregion

        #region Initialization
        /// <summary>
        /// Construct a MAAB file.
        /// </summary>
        /// <param name="file">File stream.</param>
        public MaabContainer(FileStream file)
            : base(file)
        {
            long offset = 0;

            // Confirm 'MAAB' identifier
            uint identifier = StreamUtility.ReadUIntFromStream(this.container, offset);
            offset += 4;
            if (identifier != 0x4d414142) // If file identifier is not MAAB
                throw new FormatException(container.Name + ": File is not MAAB format!");

            // Get number of files
            this.numFiles = StreamUtility.ReadUIntFromStream(this.container, offset);
            offset += 4;

            this.tableOffset = offset;
            this.containerType = "MAAB Container";
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Parse the file table.
        /// </summary>
        /// <returns>Number of files found.</returns>
        public override int ParseFileTable()
        {
            long offset = this.tableOffset;
            long fileOffset = this.tableOffset + this.numFiles * 4;

            this.fileTable = new List<FileItem>();

            string fileNameBase;
            { // Get file name base
                int fileNameStart = this.container.Name.LastIndexOf('\\') + 1;
                int fileNameEnd = this.container.Name.LastIndexOf('.');
                if (fileNameEnd == -1) fileNameEnd = this.container.Name.Length;
                fileNameBase = this.container.Name.Substring(fileNameStart, fileNameEnd - fileNameStart);
            }

            for (int i = 0; i < this.numFiles; i++)
            {
                FileItem item = new FileItem(this.container);
                item.FileName = fileNameBase + "-" + Convert.ToSingle(i);
                item.FileSize = StreamUtility.ReadUIntFromStream(this.container, offset);
                item.FileOffset = fileOffset;
                offset += 4;
                fileOffset += item.FileSize;

                uint fileIdentifier = StreamUtility.ReadUIntFromStream(this.container, item.FileOffset);
                string fileExt = StreamUtility.GetFileExtension(fileIdentifier);
                item.FileName += "." + fileExt;
                fileTable.Add(item);
            }

            return fileTable.Count;
        }

        #endregion
    }
}
