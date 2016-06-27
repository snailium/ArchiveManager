using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CoalescedCooker
{
    public partial class CoalescedCooker : Form
    {
        public CoalescedCooker()
        {
            InitializeComponent();
        }

        private void CoalescedCooker_Load(object sender, EventArgs e)
        {

        }

        private int ReadIntFromFile(FileStream TheFile, long offset_from_beginning)
        {
            byte[] tmpint = ReadBytesFromFile(TheFile, offset_from_beginning, 4);
            if (BitConverter.IsLittleEndian) Array.Reverse(tmpint);
            return BitConverter.ToInt32(tmpint, 0);
        }

        private void WriteIntToFile(FileStream TheFile, int value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(temp);
            WriteBytesToFile(TheFile, temp, 4);
        }

        private float ReadFloatFromFile(FileStream TheFile, long offset_from_beginning)
        {
            byte[] tmpint = ReadBytesFromFile(TheFile, offset_from_beginning, 4);
            if (BitConverter.IsLittleEndian) Array.Reverse(tmpint);
            return BitConverter.ToSingle(tmpint, 0);
        }

        private void WriteFloatToFile(FileStream TheFile, float value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(temp);
            WriteBytesToFile(TheFile, temp, 4);
        }

        private byte[] ReadBytesFromFile(FileStream TheFile, long offset_from_beginning, int num_bytes)
        {
            byte[] tmpint = new byte[num_bytes];
            long ret = TheFile.Seek(offset_from_beginning, SeekOrigin.Begin);
            TheFile.Read(tmpint, 0, num_bytes);
            return tmpint;
        }

        private void WriteBytesToFile(FileStream TheFile, byte[] value, int num_bytes = 0)
        {
            byte[] temp = value;
            int length = num_bytes;
            if (length == 0) length = temp.Length;
            //long ret = NewFile.Seek(offset_from_beginning, SeekOrigin.Begin);
            TheFile.Write(temp, 0, length);
        }

        private string ReadStringFromFile(FileStream TheFile, long offset_from_beginning, int num_bytes, byte delimiter = 0x00)
        {
            byte[] temp = ReadBytesFromFile(TheFile, offset_from_beginning, num_bytes);
            if (temp[temp.Length - 1] == delimiter) Array.Resize(ref temp, temp.Length - 1);
            return ASCIIEncoding.ASCII.GetString(temp);
        }

        private void WriteStringToFile(FileStream TheFile, string value, byte delimiter = 0x00)
        {
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(value);
            WriteBytesToFile(TheFile, temp, temp.Length);
            if (temp[temp.Length - 1] != delimiter)
            {
                byte[] tmp = { delimiter };
                WriteBytesToFile(TheFile, tmp, tmp.Length);
            }
        }
        
        private long CopyBlock(FileStream FromFile, FileStream ToFile, long StartOffset, int CopyLength)
        {
            byte[] temp = ReadBytesFromFile(FromFile, StartOffset, CopyLength);
            WriteBytesToFile(ToFile, temp);
            return CopyLength;
        }

        private void UncookButton_Click(object sender, EventArgs e)
        {
            string FileName, FolderName;
            FileStream INIFile;
            long Offset;

            openFileDialog.Title = "Open Coalesced.ini";
            openFileDialog.Filter = "Coalesced.ini|Coalesced.ini";
            DialogResult FileResult = openFileDialog.ShowDialog();
            
            folderBrowserDialog.Description = "Choose folder for unpacked files";
            DialogResult FolderResult = folderBrowserDialog.ShowDialog();

            // OK button was pressed.
            if (FileResult == DialogResult.OK && FolderResult == DialogResult.OK)
            {
                FileName = openFileDialog.FileName;
                FolderName = folderBrowserDialog.SelectedPath;
                if(FolderName.LastIndexOf('\\') != FolderName.Length - 1)
                    FolderName = FolderName + "\\";
            
                try
                {
                    FileStream idxFile = new FileStream(FolderName + "files.idx", FileMode.Create, FileAccess.Write);
                    INIFile = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                    if (INIFile != null && INIFile.CanRead &&
                        idxFile != null && idxFile.CanWrite)
                    {
                        // Read number of files
                        Offset = 0;
                        int numFiles = ReadIntFromFile(INIFile, Offset);
                        Offset += 4;
                        numFiles = numFiles / 2; // Actual value is half
                        WriteIntToFile(idxFile, numFiles);
                        
                        for (int i = 0; i < numFiles; i++)
                        {
                            FileStream subFile;
                            // Read File Name
                            int lenString = ReadIntFromFile(INIFile, Offset);
                            Offset += 4;
                            string subPath = ReadStringFromFile(INIFile, Offset, lenString);
                            Offset += lenString;
                            WriteIntToFile(idxFile, lenString);
                            WriteStringToFile(idxFile, subPath);
                            string subName = subPath.Substring(subPath.LastIndexOf('\\') + 1);
                            //MessageBox.Show(String.Format("{0:X}", i) + " SubFileName: " + FolderName + subName, "Debug", MessageBoxButtons.OK);
                            subFile = new FileStream(FolderName + subName, FileMode.Create, FileAccess.Write);
                            if (subFile != null && subFile.CanWrite)
                            {
                                int lenFile = ReadIntFromFile(INIFile, Offset);
                                Offset += 4;
                                CopyBlock(INIFile, subFile, Offset, lenFile-1);
                                byte[] temp = ReadBytesFromFile(INIFile, Offset, 1); // Disposal the last byte (0x00)
                                Offset += lenFile;
                                subFile.Close();
                            }
                            else
                            {
                                MessageBox.Show(subName + ": File is not writeable!?", "Error", MessageBoxButtons.OK);
                                INIFile.Close();
                                idxFile.Close();
                                return;
                            }
                        }
                        INIFile.Close();
                        idxFile.Close();
                        MessageBox.Show("Unpacked files are saved at \n" + FolderName, "Unpack", MessageBoxButtons.OK);
                    }
                    else
                    {
                        if (INIFile == null || !INIFile.CanRead) MessageBox.Show(FileName + ": File is not readable!?", "Error", MessageBoxButtons.OK);
                        if (idxFile == null || !idxFile.CanWrite) MessageBox.Show(FolderName + "files.idx" + ": File is not writeable!?", "Error", MessageBoxButtons.OK);
                        if (idxFile != null) idxFile.Close();
                        if (INIFile != null) INIFile.Close();
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show("An error occurred while attempting to load the file. The error is: "
                                    + System.Environment.NewLine + exp.ToString() + System.Environment.NewLine);
                }
            }

            // Cancel button was pressed.
            else
            {
                return;
            }

        }

        private void CookButton_Click(object sender, EventArgs e)
        {
            string FileName, FolderName;
            FileStream INIFile;
            long Offset;

            folderBrowserDialog.Description = "Choose folder containing ini files";
            DialogResult FolderResult = folderBrowserDialog.ShowDialog();

            saveFileDialog.Title = "Save Coalesced.ini";
            saveFileDialog.Filter = "Coalesced.ini|Coalesced.ini";
            DialogResult FileResult = saveFileDialog.ShowDialog();
            
            // OK button was pressed.
            if (FileResult == DialogResult.OK && FolderResult == DialogResult.OK)
            {
                FileName = saveFileDialog.FileName;
                FolderName = folderBrowserDialog.SelectedPath;
                if (FolderName.LastIndexOf('\\') != FolderName.Length - 1)
                    FolderName = FolderName + "\\";

                try
                {
                    FileStream idxFile = new FileStream(FolderName + "files.idx", FileMode.Open, FileAccess.Read);
                    INIFile = new FileStream(FileName, FileMode.Create, FileAccess.Write);
                    if (INIFile != null && INIFile.CanWrite &&
                        idxFile != null && idxFile.CanRead)
                    {
                        // Read number of files
                        Offset = 0;
                        int numFiles = ReadIntFromFile(idxFile, Offset);
                        Offset += 4;
                        WriteIntToFile(INIFile, (numFiles * 2));

                        for (int i = 0; i < numFiles; i++)
                        {
                            FileStream subFile;
                            // Read File Name
                            int lenString = ReadIntFromFile(idxFile, Offset);
                            Offset += 4;
                            string subPath = ReadStringFromFile(idxFile, Offset, lenString);
                            Offset += lenString;
                            WriteIntToFile(INIFile, lenString);
                            WriteStringToFile(INIFile, subPath);
                            string subName = subPath.Substring(subPath.LastIndexOf('\\') + 1);
                            //MessageBox.Show(String.Format("{0:X}", i) + " SubFileName: " + FolderName + subName, "Debug", MessageBoxButtons.OK);
                            subFile = new FileStream(FolderName + subName, FileMode.Open, FileAccess.Read);
                            if (subFile != null && subFile.CanRead)
                            {
                                int lenFile = (int)(subFile.Length + 1);
                                WriteIntToFile(INIFile, lenFile);
                                CopyBlock(subFile, INIFile, 0, lenFile - 1);
                                byte[] delimiter = { 0x00 };
                                WriteBytesToFile(INIFile, delimiter); // Disposal the last byte (0x00)
                                subFile.Close();
                            }
                            else
                            {
                                MessageBox.Show(subName + ": File is not readable!?", "Error", MessageBoxButtons.OK);
                                INIFile.Close();
                                idxFile.Close();
                                return;
                            }
                        }
                        INIFile.Close();
                        idxFile.Close();
                        MessageBox.Show("Coalesced.ini is saved at \n" + FileName, "Unpack", MessageBoxButtons.OK);
                    }
                    else
                    {
                        if (INIFile == null || !INIFile.CanWrite) MessageBox.Show(FileName + ": File is not writeable!?", "Error", MessageBoxButtons.OK);
                        if (idxFile == null || !idxFile.CanRead) MessageBox.Show(FolderName + "files.idx" + ": File is not readable!?", "Error", MessageBoxButtons.OK);
                        if (idxFile != null) idxFile.Close();
                        if (INIFile != null) INIFile.Close();
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show("An error occurred while attempting to load the file. The error is: "
                                    + System.Environment.NewLine + exp.ToString() + System.Environment.NewLine);
                }
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
