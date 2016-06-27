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
    public class PakContainer : FileContainer
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
        public PakContainer(FileStream file)
            : base(file)
        {
            long offset = 0;

            // Get number of files
            this.numFiles = StreamUtility.ReadUIntFromStream(this.container, offset);
            offset += 4;

            this.tableOffset = offset;
            this.containerType = "Pak Container";
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
            uint[] fileOffset = new uint[numFiles + 1];

            this.fileTable = new List<FileItem>();

            // Read file offsets
            for (int i = 0; i < numFiles; i++)
            {
                fileOffset[i] = StreamUtility.ReadUIntFromStream(this.container, offset);
                offset += 4;
            }
            fileOffset[numFiles] = (uint)this.container.Length;

            string fileNameBase;
            { // Get file name base
                int fileNameStart = this.container.Name.LastIndexOf('\\') + 1;
                int fileNameEnd = this.container.Name.LastIndexOf('.');
                if (fileNameEnd == -1) fileNameEnd = this.container.Name.Length;
                fileNameBase = this.container.Name.Substring(fileNameStart, fileNameEnd - fileNameStart);
            }

            for (int i = 0; i < numFiles; i++)
            {
                FileItem item = new FileItem(this.container);
                item.FileName = fileNameBase + "-" + Convert.ToSingle(i);
                item.FileSize = fileOffset[i+1] - fileOffset[i];
                item.FileOffset = fileOffset[i];

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
