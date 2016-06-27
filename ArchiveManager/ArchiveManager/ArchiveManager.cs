using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Snailium.Lib.Containers;

namespace ArchiveManager
{
    public partial class ArchiveManager : Form
    {
        private FileStream archiveFile;
        private FileContainer archive;

        public ArchiveManager()
        {
            InitializeComponent();
        }

        private void ArchiveManager_Load(object sender, EventArgs e)
        {
            ResetButtonStates();
        }

        private void ResetButtonStates()
        {
            openButton.Enabled = true;
            saveButton.Enabled = false;
            closeButton.Enabled = false;
            selectAllButton.Enabled = false;
            deselectAllButton.Enabled = false;
            extractButton.Enabled = false;
            removeButton.Enabled = false;
            insertButton.Enabled = false;
        }

        private void SetButtonsAfterOpenFile()
        {
            openButton.Enabled = false;
            saveButton.Enabled = true;
            closeButton.Enabled = true;
            selectAllButton.Enabled = true;
            deselectAllButton.Enabled = true;
            extractButton.Enabled = true;
            removeButton.Enabled = true;
            insertButton.Enabled = true;
        }

        private void selectAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < fileList.Items.Count; i++)
                fileList.SetItemChecked(i, true);
        }

        private void deselectAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < fileList.Items.Count; i++)
                fileList.SetItemChecked(i, false);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            openButton.Enabled = false;

            // Popup Open File dialog
            openFileDialog.Title = "Open Archive File";
            openFileDialog.Filter = "Union File (*.uni)|*.uni|" + 
                                    "CWAB Animation Package (*.cwab)|*.cwab|" +
                                    "MAAB Animation Package (*.maab)|*.maab|" +
                                    "Pak File (*.pak)|*.pak|" +
                                    "CPack File (*.cpk)|*.cpk|" +
                                    "LNK4 File (*.lnk4)|*.lnk4|" +
                                    "Sega AFS (*.afs)|*.afs|" +
                                    "All Files|*.*";
            openFileDialog.Multiselect = false;
            DialogResult FileResult = openFileDialog.ShowDialog();

            // OK button was pressed.
            if (FileResult == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                try
                {
                    string fileNameBase;
                    { // Get file name base
                        int fileNameStart = fileName.LastIndexOf('\\') + 1;
                        int fileNameEnd = fileName.LastIndexOf('.');
                        if (fileNameEnd == -1) fileNameEnd = fileName.Length;
                        fileNameBase = fileName.Substring(fileNameStart, fileNameEnd - fileNameStart);
                    }

                    archiveFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    if (archiveFile != null && archiveFile.CanRead)
                    {
                        // Determin file type
                        uint identifier = StreamUtility.ReadUIntFromStream(archiveFile, 0);
                        string fileType = StreamUtility.GetFileExtension(identifier);

                        // Initiate parser based on file type
                        switch (fileType)
                        {
                            case "uni":       // Union 2 Archive
                                archive = new Uni2Container(archiveFile);
                                break;
                            case "cwab":      // CWAB Animation Package
                                archive = new CwabContainer(archiveFile);
                                break;
                            case "maab":      // MAAB Animation Package
                                archive = new MaabContainer(archiveFile);
                                break;
                            case "pak":       // Time Leap Pak Archive
                                archive = new PakContainer(archiveFile);
                                break;
                            case "axcs":      // AXCS Archive container
                                archive = new AxcsContainer(archiveFile);
                                break;
                            case "cpk":       // CRI Archive container
                                archive = new CriPack_Container(archiveFile);
                                break;
                            case "lnk4":      // LNK4 Archive container
                            archive = new Lnk4Container(archiveFile);
                                break;
                            case "afs":       // AFS Archive container
                                archive = new AfsContainer(archiveFile);
                                break;
                            default:
                                throw new FileLoadException("Unknown file type", fileName);
                        }

                    archive.ParseFileTable();

                        // Get file list
                        foreach (FileItem file in archive.FileTable)
                            fileList.Items.Add(file, true);

                        // Update screen prompts
                        archiveType.Text = archive.ContainerType;
                        numFiles.Text = Convert.ToString(archive.FileTable.Count);

                        SetButtonsAfterOpenFile();
                    }
                    else
                    {
                        if (archiveFile == null || !archiveFile.CanRead) MessageBox.Show(fileName + ": File is not readable!?", "Error", MessageBoxButtons.OK);
                        if (archiveFile != null) archiveFile.Close();
                        ResetButtonStates();
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show("An error occurred while attempting to load the file. The error is: "
                                    + System.Environment.NewLine + exp.ToString() + System.Environment.NewLine);
                    ResetButtonStates();
                }
            }

            // Cancel button was pressed.
            else
            {
                ResetButtonStates();
                return;
            }
        }

        private void extractButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = "Choose folder for unpacked files";
            DialogResult FolderResult = folderBrowserDialog.ShowDialog();

            // OK button was pressed.
            if (FolderResult == DialogResult.OK)
            {
                try
                {
                    string folderName = folderBrowserDialog.SelectedPath;
                    if (folderName.LastIndexOf('\\') != folderName.Length - 1)
                        folderName = folderName + "\\";

                    // Extract files
                    foreach (object itemChecked in fileList.CheckedItems)
                    {
                        FileItem item = (FileItem)itemChecked;
                        item.DumpData(folderName);
                        //FileStream subFile = new FileStream(folderName + item.FileName, FileMode.Create, FileAccess.Write);
                        //if (subFile != null && subFile.CanWrite)
                        //{
                        //    item.DumpData(subFile);
                        //    subFile.Close();
                        //}
                        //else
                        //{
                        //    MessageBox.Show(item.FileName + ": File is not writeable!?", "Error", MessageBoxButtons.OK);
                        //    return;
                        //}
                    }

                    MessageBox.Show("All files are successfully extracted.", "Info", MessageBoxButtons.OK);
                }
                catch (Exception exp)
                {
                    MessageBox.Show("An error occurred while attempting to load the file. The error is: "
                                    + System.Environment.NewLine + exp.ToString() + System.Environment.NewLine);
                }
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            if (this.archiveFile != null) this.archiveFile.Close();
            if (this.archive != null) this.archive = null;
            fileList.Items.Clear();
            archiveType.Text = "<N/A>";
            numFiles.Text = "<N/A>";
            ResetButtonStates();
        }
    }
}
