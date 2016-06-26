#region File Description
//-----------------------------------------------------------------------------
// Uni2Container.cs
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
    /// The struct of a UNI2 file table item.
    /// </summary>
    public struct Uni2FileItem
    {
        #region Fields
        /// <summary>
        /// ID of the file.
        /// </summary>
        public uint fileId;

        /// <summary>
        /// Which cluster the file data starts.
        /// Note: this is an offset from the beginning of data section.
        /// </summary>
        public uint startCluster;

        /// <summary>
        /// How many clusters occupied for the file data.
        /// </summary>
        public uint lengthCluster;

        /// <summary>
        /// How many real bytes are consumed by the file data.
        /// </summary>
        public uint lengthByte;
        
        #endregion

        #region Utilities
        /// <summary>
        /// Check if this item is valid. An invalid item means all fiels are zero (0).
        /// </summary>
        /// <returns>If this item is valid.</returns>
        public bool isValid()
        {
            if (fileId == 0 && startCluster == 0 && lengthCluster == 0 && lengthByte == 0)
                return false;
            else
                return true;
        }
        #endregion
    }

    #endregion

    /// <summary>
    /// Class to access a UNI2 file.
    /// </summary>
    public class Uni2Container : FileContainer
    {
        #region Fields
        /// <summary>
        /// The number of files extracted from header.
        /// </summary>
        private uint numFiles;

        /// <summary>
        /// The cluster size used in UNI2 file.
        /// Default: 0x800 (2KB)
        /// </summary>
        private uint clusterSize = 0x800;

        /// <summary>
        /// The start location (in cluster) of file table.
        /// Default: 1 (Cluster 1)
        /// </summary>
        private uint tableCluster = 1;    // Default: File table is located from Cluster 1

        /// <summary>
        /// The start location (in cluster) of file data.
        /// Default: 2 (Cluster 2)
        /// </summary>
        private uint dataCluster = 2;     // Default: Data section is located from Cluster 2
        
        #endregion

        #region Properties
        /// <summary>
        /// The cluster size used in UNI2 file.
        /// Default: 0x800 (2KB)
        /// </summary>
        public uint ClusterSize
        {
            set { this.clusterSize = value; }
            get { return this.clusterSize; }
        }

        /// <summary>
        /// The start location (in cluster) of file table.
        /// Default: 1 (Cluster 1)
        /// </summary>
        public uint TableCluster
        {
            set { this.tableCluster = value; }
            get { return this.tableCluster; }
        }

        /// <summary>
        /// The start location (in cluster) of file data.
        /// Default: 2 (Cluster 2)
        /// </summary>
        public uint DataCluster
        {
            set { this.dataCluster = value; }
            get { return this.dataCluster; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Construct a UNI2 file.
        /// </summary>
        /// <param name="file">File stream.</param>
        public Uni2Container(FileStream file) : base(file)
        {
            long offset = 0;

            // Confirm 'UNI2' identifier
            uint identifier = StreamUtility.ReadUIntFromStream(this.container, offset);
            offset += 4;
            if (identifier != 0x554e4932) // If file identifier is not UNI2
                throw new FormatException(container.Name + ": File is not UNI2 format!");

            // Get the next value
            uint unknown = StreamUtility.ReadUIntFromStream(this.container, offset);
            offset += 4;

            // Get number of files
            this.numFiles = StreamUtility.ReadUIntFromStream(this.container, offset);
            offset += 4;

            // Get file table position
            this.tableCluster = StreamUtility.ReadUIntFromStream(this.container, offset);
            offset += 4;

            this.containerType = "Union 2";
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
            this.dataCluster = this.tableCluster +
                ((this.numFiles * 16) / this.clusterSize);
            if ((this.numFiles * 16) % this.clusterSize != 0)
                this.dataCluster++;

            long offset = this.clusterSize * this.tableCluster;
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
                Uni2FileItem item = new Uni2FileItem();
                item.fileId = StreamUtility.ReadUIntFromStream(this.container, offset);
                item.startCluster = StreamUtility.ReadUIntFromStream(this.container, offset + 4);
                item.lengthCluster = StreamUtility.ReadUIntFromStream(this.container, offset + 8);
                item.lengthByte = StreamUtility.ReadUIntFromStream(this.container, offset + 12);
                offset += 16;

                if (item.isValid())
                {
                    FileItem FileItem = ConvertUni2ToGeneral(item, fileNameBase + "-" + Convert.ToSingle(item.fileId));
                    uint fileIdentifier = StreamUtility.ReadUIntFromStream(this.container, FileItem.FileOffset);
                    string fileExt = StreamUtility.GetFileExtension(fileIdentifier);
                    FileItem.FileName += "." + fileExt;
                    fileTable.Add(FileItem);
                }
                else
                {
                    break;
                }
            }

            if (fileTable.Count != this.numFiles)
                throw new FormatException(this.container.Name + ": The real number of files doesn't match the header value!" + System.Environment.NewLine +
                    "The header indicated: " + Convert.ToString(this.numFiles) + System.Environment.NewLine +
                    "The real number of files: " + Convert.ToString(fileTable.Count));
            
            return fileTable.Count;
        }

        private FileItem ConvertUni2ToGeneral(Uni2FileItem item, string fileName)
        {
            FileItem FileItem = new FileItem(this.container);
            FileItem.FileName = fileName;
            FileItem.FileOffset = (item.startCluster + this.dataCluster) * this.clusterSize;
            FileItem.FileSize = item.lengthByte;
            return FileItem;
        }

        private Uni2FileItem ConvertGeneralToUni2(FileItem item, uint fileId)
        {
            Uni2FileItem FileItem = new Uni2FileItem();
            FileItem.fileId = fileId;
            FileItem.startCluster = (uint) (item.FileOffset / this.clusterSize - this.dataCluster);
            FileItem.lengthCluster = item.FileSize / this.clusterSize;
            if (item.FileSize % this.clusterSize != 0) FileItem.lengthCluster++;
            FileItem.lengthByte = item.FileSize;
            return FileItem;
        }
        #endregion
    }
}
