#region File Description
//-----------------------------------------------------------------------------
// FileContainer.cs
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
    public class FileContainer
    {
        #region Fields
        /// <summary>
        /// File stream which represents the container file.
        /// </summary>
        protected FileStream container;

        /// <summary>
        /// The file table.
        /// </summary>
        protected List<FileItem> fileTable;

        /// <summary>
        /// The description of container.
        /// </summary>
        protected string containerType;

        /// <summary>
        /// The start location (in bytes) of file table.
        /// </summary>
        protected long tableOffset;

        /// <summary>
        /// The start location (in bytes) of data section.
        /// </summary>
        protected long dataOffset;

        #endregion
        
        #region Properties
        /// <summary>
        /// The file table.
        /// Call GetFileTable() function to get the file table.
        /// </summary>
        public List<FileItem> FileTable
        {
            get { return this.fileTable; }
        }

        /// <summary>
        /// The description of container.
        /// </summary>
        public string ContainerType
        {
            get { return this.containerType; }
        }
        
        /// <summary>
        /// The start location (in bytes) of file table.
        /// </summary>
        public long TableOffset
        {
            set { this.tableOffset = value; }
            get { return this.tableOffset; }
        }

        /// <summary>
        /// The start location (in bytes) of data section.
        /// </summary>
        public long DataOffset
        {
            set { this.dataOffset = value; }
            get { return this.dataOffset; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Construct a container file.
        /// </summary>
        /// <param name="file">File stream.</param>
        public FileContainer(FileStream file)
        {
            this.container = file;
            this.containerType = "Unknown";
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Parse the file table.
        /// </summary>
        /// <returns>Number of files found.</returns>
        public virtual int ParseFileTable()
        {
            throw new NotImplementedException("FileContainer::ParseFileTable() should not be called." + System.Environment.NewLine +
                "Please implement ParseFileTable() in sub-classes.");
        }

        public virtual FileItem GetFileInfo(string fileName)
        {
            if (this.fileTable == null || this.fileTable.Count == 0)
                throw new FileNotFoundException("File table is not initialized.", fileName);

            foreach (FileItem file in this.fileTable)
            {
                if (file.FileName.Equals(fileName))
                    return file;
            }

            throw new FileNotFoundException("File is not found in file table.", fileName);
        }
        #endregion
    }
}
