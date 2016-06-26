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
    public class FileItem
    {
        #region Fields
        /// <summary>
        /// The file stream.
        /// </summary>
        protected FileStream file;
        
        /// <summary>
        /// Name of the file
        /// </summary>
        protected string fileName;

        /// <summary>
        /// Size of the file in bytes.
        /// </summary>
        protected uint fileSize;

        /// <summary>
        /// True offset of the file.
        /// </summary>
        protected long fileOffset;

        /// <summary>
        /// Date of the file created.
        /// </summary>
        protected DateTime fileDate;

        /// <summary>
        /// Additional attribute array.
        /// </summary>
        protected byte[] fileAttibute;

        #endregion

        #region Properties
        /// <summary>
        /// Name of the file
        /// </summary>
        public string FileName
        {
            set { this.fileName = value; }
            get { return this.fileName; }
        }

        /// <summary>
        /// Size of the file in bytes.
        /// </summary>
        public uint FileSize
        {
            set { this.fileSize = value; }
            get { return this.fileSize; }
        }

        /// <summary>
        /// True offset of the file.
        /// </summary>
        public long FileOffset
        {
            set { this.fileOffset = value; }
            get { return this.fileOffset; }
        }

        /// <summary>
        /// Date of the file created.
        /// </summary>
        public DateTime FileDate
        {
            set { this.fileDate = value; }
            get { return this.fileDate; }
        }

        /// <summary>
        /// Additional attribute array.
        /// </summary>
        public byte[] FileAttibute
        {
            set { this.fileAttibute = value; }
            get { return this.fileAttibute; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public FileItem(FileStream file)
        {
            this.file = file;
            this.fileName = "";
            this.fileSize = 0;
            this.fileOffset = 0;
            this.fileDate = DateTime.Now;
            this.fileAttibute = new byte[0];
        }
        #endregion

        #region Actions
        /// <summary>
        /// Dump this file data to another file.
        /// </summary>
        /// <param name="toFile">The destination file stream.</param>
        /// <returns>Length copied. (If failed, return -1)</returns>
        public virtual long DumpData(FileStream toFile)
        {
            if (this.file == null || !this.file.CanRead)
                return -1;

            return StreamUtility.CopyBlock(this.file, toFile, this.fileOffset, (int)this.fileSize);
        }

        /// <summary>
        /// Dump this file data to another file.
        /// </summary>
        /// <param name="path">The destination path (without file name).</param>
        /// <returns>Length copied. (If failed, return -1)</returns>
        public virtual long DumpData(string path)
        {
            FileStream toFile = new FileStream(path + "\\" + this.fileName, FileMode.Create, FileAccess.Write, FileShare.None);
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
            return fileName + " (" + Convert.ToString(fileSize) + " bytes)";
        }
        #endregion
    }

}
