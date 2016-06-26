#region File Description
//-----------------------------------------------------------------------------
// CwabContainer.cs
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
    /// Class to access a Cwab file.
    /// </summary>
    public class CwabContainer : FileContainer
    {
        #region Fields
        /// <summary>
        /// The number of files extracted from header.
        /// </summary>
        private int numFiles;

        /// <summary>
        /// Size of a file table item.
        /// </summary>
        private int tableItemSize = 0x40;

        #endregion

        #region Initialization
        /// <summary>
        /// Construct a Cwab file.
        /// </summary>
        /// <param name="file">File stream.</param>
        public CwabContainer(FileStream file)
            : base(file)
        {
            long offset = 0;

            // Confirm 'CWAB' identifier
            uint identifier = StreamUtility.ReadUIntFromStream(this.container, offset);
            offset += 4;
            if (identifier != 0x43574142) // If file identifier is not CWAB
                throw new FormatException(container.Name + ": File is not CWAB format!");

            // Get number of files
            this.numFiles = StreamUtility.ReadIntFromStream(this.container, offset);
            offset += 4;

            // Set file table position
            this.tableOffset = offset;

            this.containerType = "CWAB Animation Package";
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
            int tableLength = this.numFiles * this.tableItemSize;
            long fileOffset = this.tableOffset + tableLength;
            this.fileTable = new List<FileItem>();

            for (int i = 0; i < this.numFiles; i++)
            {
                FileItem item = new FileItem(this.container);
                item.FileSize = StreamUtility.ReadUIntFromStream(this.container, offset);
                item.FileName = StreamUtility.ReadStringFromStream(this.container, offset + 4, -1);
                item.FileOffset = fileOffset;
                offset += this.tableItemSize;
                fileOffset += item.FileSize;

                fileTable.Add(item);
            }

            return fileTable.Count;
        }

        #endregion
    }
}
