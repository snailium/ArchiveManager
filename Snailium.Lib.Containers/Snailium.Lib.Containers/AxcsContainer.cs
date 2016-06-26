#region File Description
//-----------------------------------------------------------------------------
// AxcsContainer.cs
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
    /// Class to access a AXCS file.
    /// </summary>
    public class AxcsContainer : FileContainer
    {
        #region Fields
        /// <summary>
        /// The number of files extracted from header.
        /// </summary>
        private uint numFiles;

        #endregion

        #region Initialization
        /// <summary>
        /// Construct a AXCS file.
        /// </summary>
        /// <param name="file">File stream.</param>
        public AxcsContainer(FileStream file)
            : base(file)
        {
            
            long offset = 0;

            // Confirm 'AXCS' identifier
            uint identifier = StreamUtility.ReadUIntFromStream(this.container, offset);
            offset += 4;
            if (identifier != 0x53435841) // If file identifier is not AXCS
                throw new FormatException(container.Name + ": File is not AXCS format!");

            // Get the next value
            this.dataOffset = StreamUtility.ReadIntFromStream(this.container, offset, false);
            offset += 4;

            // Get number of files
            this.numFiles = StreamUtility.ReadUIntFromStream(this.container, offset, false);
            offset += 4;

            // Get file table position
            this.tableOffset = offset;

            this.containerType = "AXCS archive container";
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Parse the file table.
        /// </summary>
        /// <returns>Number of files found.</returns>
        public override int ParseFileTable()
        {
            // Set correct position for data section
            long offset = this.tableOffset;

            int[] tableItemStart = new int[this.numFiles];
            for (int i = 0; i < this.numFiles; i++)
            {
                tableItemStart[i] = StreamUtility.ReadIntFromStream(this.container, offset, false);
                offset += 4;
            }

            long listOffset = offset;

            this.fileTable = new List<FileItem>();
            for (int i = 0; i < this.numFiles; i++)
            {
                offset = listOffset + tableItemStart[i];
                FileItem item = new FileItem(this.container);
                item.FileOffset = StreamUtility.ReadUIntFromStream(this.container, offset, false) + this.dataOffset;
                item.FileSize = StreamUtility.ReadUIntFromStream(this.container, offset + 4, false);
                item.FileName = StreamUtility.ReadStringFromStream(this.container, offset + 8, -1, 0x00);
                item.FileName = item.FileName.Replace('\\', '+');
                this.fileTable.Add(item);
            }

            return fileTable.Count;
        }

        #endregion
    }
}
