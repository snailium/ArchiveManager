#region File Description
//-----------------------------------------------------------------------------
// AfsContainer.cs
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
    /// The struct of a Afs file table item.
    /// </summary>
    public struct AfsFileItem
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
    /// Class to access a Afs file.
    /// </summary>
    public class AfsContainer : FileContainer
    {
        #region Fields
        /// <summary>
        /// The number of files extracted from header.
        /// </summary>
        private uint numFiles;

        #endregion

        #region Initialization
        /// <summary>
        /// Construct a Afs file.
        /// </summary>
        /// <param name="file">File stream.</param>
        public AfsContainer(FileStream file)
            : base(file)
        {
            long offset = 0;

            // Confirm 'Afs' identifier
            uint identifier = StreamUtility.ReadUIntFromStream(this.container, offset);
            offset += 4;
            if (!StreamUtility.GetFileExtension(identifier).Equals("afs")) // If file identifier is not Afs
                throw new FormatException(container.Name + ": File is not Afs format!");

            // Get file address
            this.numFiles = StreamUtility.ReadUIntFromStream(this.container, offset, false);
            offset += 4;

            this.containerType = "Sega AFS";
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
                AfsFileItem item = new AfsFileItem();
                item.fileId = fid;
                item.startOffset = StreamUtility.ReadUIntFromStream(this.container, offset, false);
                offset += 4;
                item.length = StreamUtility.ReadUIntFromStream(this.container, offset, false);
                offset += 4;

                if (item.isValid())
                {
                    FileItem fileItem = ConvertAfsToGeneral(item, fileNameBase + "-" + Convert.ToSingle(item.fileId));
                    uint fileIdentifier = StreamUtility.ReadUIntFromStream(this.container, fileItem.FileOffset);
                    string fileExt = StreamUtility.GetFileExtension(fileIdentifier);
                    fileItem.FileName += "." + fileExt;
                    fileTable.Add(fileItem);
                }
                else
                {
                    break;
                }

                fid++;
            }

            if (fileTable.Count == this.numFiles + 1)
            {
                // The last one is the file property table.
                FileItem fileItem = fileTable[fileTable.Count - 1];
                offset = fileItem.FileOffset;
                // Retrieve file names
                for (int i = 0; i < this.numFiles; i++)
                {
                    fileTable[i].FileName = StreamUtility.ReadStringFromStream(this.container, offset, -1);
                    offset += 0x30;
                }
                // Remove the file property table
                fileTable.RemoveAt(fileTable.Count - 1);
            }

            if (fileTable.Count != this.numFiles)
                throw new FormatException(this.container.Name + ": The real number of files doesn't match the header value!" + System.Environment.NewLine +
                    "The header indicated: " + Convert.ToString(this.numFiles) + System.Environment.NewLine +
                    "The real number of files: " + Convert.ToString(fileTable.Count));

            return fileTable.Count;
        }

        private FileItem ConvertAfsToGeneral(AfsFileItem item, string fileName)
        {
            FileItem FileItem = new FileItem(this.container);
            FileItem.FileName = fileName;
            FileItem.FileOffset = item.startOffset;
            FileItem.FileSize = item.length;
            return FileItem;
        }

        private AfsFileItem ConvertGeneralToAfs(FileItem item, uint fileId)
        {
            AfsFileItem FileItem = new AfsFileItem();
            FileItem.fileId = fileId;
            FileItem.startOffset = (uint)(item.FileOffset - this.dataOffset);
            FileItem.length = item.FileSize;
            return FileItem;
        }
        #endregion
    }
}
