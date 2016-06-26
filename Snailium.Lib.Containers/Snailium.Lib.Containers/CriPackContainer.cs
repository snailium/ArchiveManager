#region File Description
//-----------------------------------------------------------------------------
// CriPack_Container.cs
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
using System.Security.Cryptography;
using System.Windows.Forms;
#endregion

namespace Snailium.Lib.Containers
{
    /// <summary>
    /// Location of value of current column.
    /// </summary>
    public enum CriPack_DataStorage : byte
    {
        UNDEFINED = 0x00,
        NONE = 0x10,
        STRING = 0x30,
        DATA = 0x50,
        MASK = 0xf0
    }

    /// <summary>
    /// Type of value stored in current column.
    /// </summary>
    public enum CriPack_DataType : byte
    {
        UNSIGNED_BYTE = 0x00,
        SIGNED_BYTE = 0x01,
        UNSIGNED_SHORT = 0x02,
        SIGNED_SHORT = 0x03,
        UNSIGNED_INT = 0x04,
        SIGNED_INT = 0x05,
        UNSIGNED_LONG = 0x06,
        SIGNED_LONG = 0x07,
        FLOAT = 0x08,
        DOUBLE = 0x09,
        STRING = 0x0a,
        DATA = 0x0b,
        MASK = 0x0f
    }

    /// <summary>
    /// Key item
    /// Stores property of UTF
    /// </summary>
    public class CriPack_UTFProperty{
        #region Public Members
        public CriPack_DataStorage storage = CriPack_DataStorage.UNDEFINED;
        public CriPack_DataType type = CriPack_DataType.DATA;
        public string name = "";
        public object value = null;
        #endregion

        public CriPack_UTFProperty() { }

    };

    /// <summary>
    /// UTF item.
    /// Represents a header
    /// =================================================================
    /// 
    /// UTF Header Structure
    /// +-------------+-------------------------------------------------+
    /// |     Byte    |                  Description                    |
    /// +-------------+-------------------------------------------------+
    /// |    0 - 3    | Identifier ("@UTF")                             |
    /// |             | If not "@UTF", then decryption is needed.       |
    /// +-------------+-------------------------------------------------+
    /// |    4 - 7    | Length of the rest of header.                   |
    /// +-------------+-------------------------------------------------+
    /// |    8 - 11   | Start offset of data bank  (SODB)               |
    /// |             | (Offset from Byte 8)                            |
    /// +-------------+-------------------------------------------------+
    /// |   12 - 15   | Start offset of string bank  (SOSB)             |
    /// |             | (Offset from Byte 8)                            |
    /// +-------------+-------------------------------------------------+
    /// |   16 - 19   | End Address (should match the length of UTF)    |
    /// |             | (Offset from Byte 8)                            |
    /// +-------------+-------------------------------------------------+
    /// |   20 - 23   | Start offset in string back for UTF description |
    /// +-------------+-------------------------------------------------+
    /// |   24 - 25   | Number of properties.                           |
    /// +-------------+-------------------------------------------------+
    /// |   26 - 27   | Size in data bank for an item.                  |
    /// +-------------+-------------------------------------------------+
    /// |   28 - 31   | Number of items.                                |
    /// +-------------+-------------------------------------------------+
    /// |  32 - SODB  | Information of values (1 + 4 Bytes per value)   |
    /// |             | Byte 0: Flag                                    |
    /// |             |         0xf0: Type of storage (encoded)         |
    /// |             |         0x0f: Type of data value (encoded)      |
    /// |             | Byte 1-4: Data / String (Offset in string bank) |
    /// +-------------+-------------------------------------------------+
    /// | SODB - SOSB | Data bank (any kind of integer values)          |
    /// +-------------+-------------------------------------------------+
    /// | SOSB - End  | String bank                                     |
    /// +-------------+-------------------------------------------------+
    /// </summary>
    public class CriPack_UTF
    {
        #region Protected Members
        /// <summary>
        /// Size of UTF header payload.
        /// </summary>
        protected uint payloadSize;

        /// <summary>
        /// Offset to data bank from beginning (Byte 0)
        /// </summary>
        protected uint dataBankOffset;

        /// <summary>
        /// Offset to string bank from beginning (Byte 0)
        /// </summary>
        protected uint stringBankOffset;

        /// <summary>
        /// Offset to payload from beginning (Byte 0)
        /// </summary>
        protected uint endOfPayloadOffset;

        /// <summary>
        /// String bank.
        /// Offset => String
        /// </summary>
        protected Dictionary<int, string> stringBank;

        /// <summary>
        /// Number of item.
        /// </summary>
        protected int numItem;

        /// <summary>
        /// Length per item in data bank.
        /// </summary>
        protected int itemDataLength;
        #endregion
        
        #region Public Members
        /// <summary>
        /// UTF description
        /// </summary>
        public string name;

        /// <summary>
        /// All properties in UTF header.
        /// </summary>
        public CriPack_UTFProperty[][] properties;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public CriPack_UTF() { }
        #endregion

        #region Utilities
        /// <summary>
        /// Decrypt UTF block
        /// </summary>
        /// <param name="stream">The byte array of encrypted UTF block.</param>
        /// <returns>The byte array of decrypted UTF block.</returns>
        public static byte[] Decrypt(byte[] stream)
        {
            if (stream == null) return null;
            
            byte[] decrypted = new byte[stream.Length];
            
            // Setup decryption constants
            uint m = 0x0000655f;
            uint t = 0x00004115;

            // Do decryption
            for (int i = 0; i < stream.Length; i++)
            {
                decrypted[i] = (byte)(stream[i] ^ (m & 0xff));
                m = m * t;
            }

            return decrypted;
        }

        public object GetPropertyValue(int index, string name)
        {
            return CriPack_UTF.GetPropertyValue(this.properties[index], name);
        }

        public static object GetPropertyValue(CriPack_UTFProperty[] properties, string name)
        {
            foreach (CriPack_UTFProperty property in properties)
            {
                if (property.storage == CriPack_DataStorage.UNDEFINED)
                    continue;
                else if (property.name.Equals(name)) // Name match
                    return property.value;
            }
            throw new KeyNotFoundException("Property with name \"" + name + "\" not found.");
        }

        public string GetStringFromBank(int address)
        {
            if (this.stringBank.Keys.Contains(address))
                return this.stringBank[address];
            else
                throw new KeyNotFoundException(String.Format("String with address 0x{0:X4} does not exist in string bank.", address));
        }
        #endregion

        #region Pack and Unpack

        public static CriPack_UTF ByteUnpack(byte[] stream)
        {
            if (stream == null || stream.Length < 4) return null;

            if (stream[0] != 0x40 || // @
                stream[1] != 0x55 || // U
                stream[2] != 0x54 || // T
                stream[3] != 0x46)   // F
                stream = CriPack_UTF.Decrypt(stream);
            
            using (MemoryStream buffer = new MemoryStream(stream))
            {
                CriPack_UTF utf = new CriPack_UTF();

                long offset = 4;

                utf.payloadSize = StreamUtility.ReadUIntFromStream(buffer, offset);
                offset += 4;

                utf.dataBankOffset = StreamUtility.ReadUIntFromStream(buffer, offset) + 8;
                offset += 4;

                utf.stringBankOffset = StreamUtility.ReadUIntFromStream(buffer, offset) + 8;
                offset += 4;

                utf.endOfPayloadOffset = StreamUtility.ReadUIntFromStream(buffer, offset) + 8;
                offset += 4;

                // Optional check
                if (utf.endOfPayloadOffset != (utf.payloadSize + 8))
                    MessageBox.Show("End of UTF Payload (" + String.Format("0x{0:X4}", utf.endOfPayloadOffset) + ") " +
                        "does not match UTF Payload Size (" + String.Format("0x{0:X4}", utf.payloadSize + 8) + ")", "Warning", MessageBoxButtons.OK);

                int nameOffset = StreamUtility.ReadIntFromStream(buffer, offset);
                offset += 4;

                uint temp = StreamUtility.ReadUIntFromStream(buffer, offset);
                offset += 4;
                int numProperties = (int)((temp >> 16) & 0xff);
                utf.itemDataLength = (int)(temp & 0xff);

                utf.numItem = StreamUtility.ReadIntFromStream(buffer, offset);
                offset += 4;

                // Optional check
                if ((utf.itemDataLength * utf.numItem) != (utf.stringBankOffset - utf.dataBankOffset))
                    MessageBox.Show(
                        String.Format("Calculated data bank size (0x{0:X4}) does not match specified size (0x{1:X4})",
                            utf.stringBankOffset - utf.dataBankOffset, utf.itemDataLength * utf.numItem),
                        "Warning", MessageBoxButtons.OK);

                { // Extract string bank
                    int address = (int)utf.stringBankOffset;
                    utf.stringBank = new Dictionary<int, string>();
                    while (address < utf.endOfPayloadOffset)
                    {
                        string value = StreamUtility.ReadStringFromStream(buffer, address, 0);
                        utf.stringBank.Add((address - (int)utf.stringBankOffset), value);
                        address += value.Length + 1;
                    }
                }

                // Get name of this UTF
                utf.name = utf.GetStringFromBank(nameOffset);

                // Extract items
                utf.properties = new CriPack_UTFProperty[utf.numItem][];
                for (int i = 0; i < utf.numItem; i++)
                {
                    // Extract properties
                    utf.properties[i] = new CriPack_UTFProperty[numProperties];
                    long itemOffset = offset;
                    int dataBankCursor = (int)utf.dataBankOffset + utf.itemDataLength * i;
                    for (int p = 0; p < numProperties; p++)
                    {
                        // Read flag
                        byte[] bytes = StreamUtility.ReadBytesFromStream(buffer, itemOffset, 1);
                        itemOffset += 1;
                        byte flag = bytes[0];

                        // Determine storage and type
                        CriPack_UTFProperty property = new CriPack_UTFProperty();
                        property.storage = (CriPack_DataStorage)(flag & (byte)CriPack_DataStorage.MASK);
                        property.type = (CriPack_DataType)(flag & (byte)CriPack_DataType.MASK);

                        if (property.storage == CriPack_DataStorage.UNDEFINED)
                            throw new NotSupportedException(String.Format("Unrecognized storage location: 0x{0:X4}", (byte)property.storage));

                        // Extract name of the property
                        {
                            int address = StreamUtility.ReadIntFromStream(buffer, itemOffset);
                            itemOffset += 4;
                            property.name = utf.GetStringFromBank(address);
                        }

                        // Extract value
                        if (property.storage == CriPack_DataStorage.NONE)
                        {
                            // No value associated
                        }
                        else if (property.storage == CriPack_DataStorage.STRING)
                        {
                            // String value
                            int address = StreamUtility.ReadIntFromStream(buffer, itemOffset);
                            itemOffset += 4;
                            property.value = utf.GetStringFromBank(address);
                        }
                        else if (property.storage == CriPack_DataStorage.DATA)
                        {
                            int address;
                            // Data value specified by 'type'
                            switch (property.type)
                            {
                                case CriPack_DataType.UNSIGNED_BYTE:
                                case CriPack_DataType.SIGNED_BYTE:
                                    bytes = StreamUtility.ReadBytesFromStream(buffer, dataBankCursor, 1);
                                    dataBankCursor += 1;
                                    property.value = bytes[0];
                                    break;

                                case CriPack_DataType.UNSIGNED_SHORT:
                                    bytes = StreamUtility.ReadBytesFromStream(buffer, dataBankCursor, 2);
                                    dataBankCursor += 2;
                                    Array.Reverse(bytes); // Change endian
                                    property.value = BitConverter.ToUInt16(bytes, 0);
                                    break;

                                case CriPack_DataType.SIGNED_SHORT:
                                    bytes = StreamUtility.ReadBytesFromStream(buffer, dataBankCursor, 2);
                                    dataBankCursor += 2;
                                    Array.Reverse(bytes); // Change endian
                                    property.value = BitConverter.ToInt16(bytes, 0);
                                    break;

                                case CriPack_DataType.UNSIGNED_INT:
                                    property.value = StreamUtility.ReadUIntFromStream(buffer, dataBankCursor);
                                    dataBankCursor += 4;
                                    break;

                                case CriPack_DataType.SIGNED_INT:
                                    property.value = StreamUtility.ReadIntFromStream(buffer, dataBankCursor);
                                    dataBankCursor += 4;
                                    break;

                                case CriPack_DataType.UNSIGNED_LONG:
                                    bytes = StreamUtility.ReadBytesFromStream(buffer, dataBankCursor, 8);
                                    dataBankCursor += 8;
                                    Array.Reverse(bytes); // Change endian
                                    property.value = BitConverter.ToUInt64(bytes, 0);
                                    break;

                                case CriPack_DataType.SIGNED_LONG:
                                    bytes = StreamUtility.ReadBytesFromStream(buffer, dataBankCursor, 8);
                                    dataBankCursor += 8;
                                    Array.Reverse(bytes); // Change endian
                                    property.value = BitConverter.ToInt64(bytes, 0);
                                    break;

                                case CriPack_DataType.FLOAT:
                                    bytes = StreamUtility.ReadBytesFromStream(buffer, dataBankCursor, 4);
                                    dataBankCursor += 4;
                                    Array.Reverse(bytes); // Change endian
                                    property.value = BitConverter.ToSingle(bytes, 0);
                                    break;

                                case CriPack_DataType.DOUBLE:
                                    bytes = StreamUtility.ReadBytesFromStream(buffer, dataBankCursor, 8);
                                    dataBankCursor += 8;
                                    Array.Reverse(bytes); // Change endian
                                    property.value = BitConverter.ToDouble(bytes, 0);
                                    break;

                                case CriPack_DataType.STRING:
                                    address = StreamUtility.ReadIntFromStream(buffer, dataBankCursor);
                                    dataBankCursor += 4;
                                    property.value = utf.GetStringFromBank(address);
                                    break;

                                case CriPack_DataType.DATA:
                                    address = StreamUtility.ReadIntFromStream(buffer, dataBankCursor);
                                    dataBankCursor += 4;
                                    int size = StreamUtility.ReadIntFromStream(buffer, dataBankCursor);
                                    dataBankCursor += 4;
                                    // Assume this is another table
                                    bytes = StreamUtility.ReadBytesFromStream(buffer, address, size);
                                    property.value = CriPack_UTF.ByteUnpack(bytes);
                                    break;

                                default:
                                    property.value = null;
                                    break;
                            }
                        }

                        utf.properties[i][p] = property;
                    }
                }

                return utf;
            }
        }
        #endregion
    };
    public class CriPack_Container : FileContainer
    {
        #region Fields
        private CriPack_UTF header = null;
        private CriPack_UTF toc = null;
        private CriPack_UTF etoc = null;
        #endregion

        #region Initialization
        /// <summary>
        /// Construct a CriPack file.
        /// </summary>
        /// <param name="file">File stream.</param>
        public CriPack_Container(FileStream file)
            : base(file)
        {
            long offset = 0;

            // Confirm 'CriPack' identifier
            uint identifier = StreamUtility.ReadUIntFromStream(this.container, offset);
            offset += 4;
            if (identifier != 0x43504b20) // If file identifier is not CriPack
                throw new FormatException(container.Name + ": File is not CriPack format!");

            // Get UTF length
            offset += 4;
            int headerLength = StreamUtility.ReadIntFromStream(this.container, offset, false);
            offset += 4;

            // Zero padding, ignore
            offset += 4;


            this.containerType = "CRI Pack Container";

            //byte[] utfHeader = StreamUtility.ReadBytesFromStream(this.container, offset, (int)this.utfLength);
            //MessageBox.Show("CRC32 of UTF: " + String.Format("0x{0:X4}", Crc32.Compute(0xedb88320, 0xffffffff, utfHeader)), "Info", MessageBoxButtons.OK);
            //MessageBox.Show("CRC32 of UTF: " + String.Format("0x{0:X4}", Crc32.Compute(0x04C11DB7, 0xffffffff, utfHeader)), "Info", MessageBoxButtons.OK);
            //utfHeader = StreamUtility.ReadBytesFromStream(this.container, offset + 8, (int)this.utfLength - 8);
            //MessageBox.Show("CRC32 of UTF: " + String.Format("0x{0:X4}", Crc32.Compute(0xedb88320, 0xffffffff, utfHeader)), "Info", MessageBoxButtons.OK);
            //MessageBox.Show("CRC32 of UTF: " + String.Format("0x{0:X4}", Crc32.Compute(0x04C11DB7, 0xffffffff, utfHeader)), "Info", MessageBoxButtons.OK);
            //utfHeader = StreamUtility.ReadBytesFromStream(this.container, offset - 8, (int)this.utfLength + 8);
            //MessageBox.Show("CRC32 of UTF: " + String.Format("0x{0:X4}", Crc32.Compute(0xedb88320, 0xffffffff, utfHeader)), "Info", MessageBoxButtons.OK);
            //MessageBox.Show("CRC32 of UTF: " + String.Format("0x{0:X4}", Crc32.Compute(0x04C11DB7, 0xffffffff, utfHeader)), "Info", MessageBoxButtons.OK);
            //utfHeader = StreamUtility.ReadBytesFromStream(this.container, offset - 16, (int)this.utfLength + 16);
            //MessageBox.Show("CRC32 of UTF: " + String.Format("0x{0:X4}", Crc32.Compute(0xedb88320, 0xffffffff, utfHeader)), "Info", MessageBoxButtons.OK);
            //MessageBox.Show("CRC32 of UTF: " + String.Format("0x{0:X4}", Crc32.Compute(0x04C11DB7, 0xffffffff, utfHeader)), "Info", MessageBoxButtons.OK);
            //utfHeader = StreamUtility.ReadBytesFromStream(this.container, offset + 4, (int)this.utfLength - 4);
            //MessageBox.Show("CRC32 of UTF: " + String.Format("0x{0:X4}", Crc32.Compute(0xedb88320, 0xffffffff, utfHeader)), "Info", MessageBoxButtons.OK);
            //MessageBox.Show("CRC32 of UTF: " + String.Format("0x{0:X4}", Crc32.Compute(0x04C11DB7, 0xffffffff, utfHeader)), "Info", MessageBoxButtons.OK);



            byte[] headerBytes = StreamUtility.ReadBytesFromStream(this.container, offset, headerLength);
            this.header = CriPack_UTF.ByteUnpack(headerBytes);
            this.tableOffset = (long)(ulong)this.header.GetPropertyValue(0, "TocOffset");
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Parse the file table.
        /// </summary>
        /// <returns>Number of files found.</returns>
        public override int ParseFileTable()
        {
            this.fileTable = new List<FileItem>();

            // Extract TOC
            {
                uint identifier = StreamUtility.ReadUIntFromStream(this.container, this.tableOffset);
                if (identifier != 0x544f4320) // TOC 
                    throw new FormatException(container.Name + ": Checking TOC identified failed!");

                // Get UTF length
                int tocLength = StreamUtility.ReadIntFromStream(this.container, this.tableOffset + 0x8, false);

                byte[] tocBytes = StreamUtility.ReadBytesFromStream(this.container, this.tableOffset + 0x10, tocLength);
                this.toc = CriPack_UTF.ByteUnpack(tocBytes);
            }

            // Extract E-TOC (Extended TOC)
            try
            {
                long etocOffset = (long)(ulong)this.header.GetPropertyValue(0, "EtocOffset");
                uint identifier = StreamUtility.ReadUIntFromStream(this.container, etocOffset);
                if (identifier != 0x45544f43) // ETOC 
                    throw new FormatException(container.Name + ": Checking E-TOC identified failed!");

                // Get UTF length
                int etocLength = StreamUtility.ReadIntFromStream(this.container, etocOffset + 0x8, false);

                byte[] etocBytes = StreamUtility.ReadBytesFromStream(this.container, etocOffset + 0x10, etocLength);
                this.etoc = CriPack_UTF.ByteUnpack(etocBytes);
            }
            catch (KeyNotFoundException e) { } // No E-TOC

            for (int i = 0; i < toc.properties.Length; i++)
            {
                CriPackFileItem item = new CriPackFileItem(this.container);
                item.AssignTocProperties(toc.properties[i], this.tableOffset);
                if (this.etoc != null) item.AssignEtocProperties(etoc.properties[i]);
                fileTable.Add(item);
            }

            return fileTable.Count;
        }

        #endregion
    }
}
