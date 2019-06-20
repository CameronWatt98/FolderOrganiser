using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderOrganiser
{
    public partial class FolderOrganiserForm : Form
    {
        public FolderOrganiserForm()
        {
            InitializeComponent();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            try 
            {
                //initialise variable
                progressBar1.Value = 0;
                progressBar2.Value = 0;
                var rootPath = textBox1.Text;
                DateTime startTime = new DateTime();
                DateTime endTime = new DateTime();
                String[] monthNames = { "1-January", "2-February", "3-March", "4-April", "5-May", "6-June", "7-July", "8-August", "9-September", "10-October", "11-November", "12-December" };
                int[] monthLengths = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

                //Add files to an array and sort them
                DirectoryInfo dir = new DirectoryInfo(rootPath);
                FileInfo[] fileArray = dir.GetFiles();

                //this makes sure there are files and also prevents the user from spamming the button.
                if (fileArray.Length == 0)
                {
                    NoFiles NFform = new NoFiles();
                    NFform.StartPosition = FormStartPosition.CenterParent;
                    NFform.ShowDialog(this);
                }
                else
                {
                    Array.Sort(fileArray, (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.CreationTime, y.CreationTime));
                    progressBar1.Maximum = fileArray.Length;
                    //set time for last and first created file in directory
                    endTime = File.GetCreationTime(rootPath + "\\" + fileArray[fileArray.Length - 1].Name);
                    startTime = File.GetCreationTime(rootPath + "\\" + fileArray[0].Name);


                    //if you want subfolders split by YEARS
                    if (radioButton1.Checked)
                    {
                        createSubFolders("years", rootPath, startTime, endTime, monthNames, monthLengths, true);
                        moveFiles("years", rootPath, fileArray, monthNames);
                    }
                    //if you want subfolders split by MONTHS
                    else if (radioButton2.Checked)
                    {
                        createSubFolders("years", rootPath, startTime, endTime, monthNames, monthLengths, false);
                        createSubFolders("months", rootPath, startTime, endTime, monthNames, monthLengths, true);
                        moveFiles("months", rootPath, fileArray, monthNames);
                    }
                    //if you want subfolders split by DAYS
                    else if (radioButton4.Checked)
                    {
                        createSubFolders("years", rootPath, startTime, endTime, monthNames, monthLengths, false);
                        createSubFolders("months", rootPath, startTime, endTime, monthNames, monthLengths, false);
                        createSubFolders("days", rootPath, startTime, endTime, monthNames, monthLengths, true);
                        moveFiles("days", rootPath, fileArray, monthNames);
                    }
                    else
                    {
                        NoRadioButtonSelected NRBSform = new NoRadioButtonSelected();
                        NRBSform.StartPosition = FormStartPosition.CenterParent;
                        NRBSform.ShowDialog(this);
                    }
                    
                    //if you want just subfolders with data within them
                    if (radioButton3.Checked)
                    {
                        removeEmptyFolders(rootPath);
                    }
                }


            }
            catch 
            {
                InvalidPath form = new InvalidPath();
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog(this);
            }
        }



        /*-------CREATES SUBFOLDERS BASED ON THE SPECIFICATION IN VARIABLE "TIME"--------*/
        private void createSubFolders(String time, String rootPath, DateTime startTime, DateTime endTime, String[] months, int[] monthLength, bool updateProgressBar)
        {
            //if all files were created on the same day
            if (startTime.Date == endTime.Date)
            {
                if (time == "years")
                {
                    Directory.CreateDirectory(rootPath + "\\" + startTime.Year.ToString());
                    progressBar2.Value = progressBar2.Maximum;
                }
                else if (time == "months")
                {
                    Directory.CreateDirectory(rootPath + "\\" + startTime.Year.ToString() + "\\" + months[startTime.Month - 1]);
                    progressBar2.Value = progressBar2.Maximum;
                }
                else if (time == "days")
                {
                    Directory.CreateDirectory(rootPath + "\\" + startTime.Year.ToString() + "\\" + months[startTime.Month - 1] + "\\" + startTime.Day + "-" + startTime.DayOfWeek);
                    progressBar2.Value = progressBar2.Maximum;
                }
            }
            else
            {
                DateTime currentTime = new DateTime();
                currentTime = startTime;
                TimeSpan span = new TimeSpan();
                span = endTime.Subtract(startTime);
                progressBar2.Maximum = span.Days;
                //This creates all the subfolders
                if (time == "years")
                {
                    while (currentTime.Year <= endTime.Year)
                    {
                        //create folder
                        Directory.CreateDirectory(rootPath + "\\" + currentTime.Year.ToString());
                        currentTime = currentTime.AddYears(1);

                        //update progress bar
                        if (updateProgressBar)
                        {
                            if (progressBar2.Value + 364 < progressBar2.Maximum)
                            {
                                progressBar2.Value = progressBar2.Value + 364;
                            }
                            else
                            {
                                progressBar2.Value = progressBar2.Maximum;
                            }
                        }
                    }
                }
                else if (time == "months")
                {
                    while ((currentTime.Month <= endTime.Month || currentTime.Year < endTime.Year))
                    {
                        //create folder
                        Directory.CreateDirectory(rootPath + "\\" + currentTime.Year.ToString() + "\\" + months[currentTime.Month - 1]);
                        currentTime = currentTime.AddMonths(1);

                        //update progress bar
                        if (updateProgressBar)
                        {
                            if (progressBar2.Value + monthLength[currentTime.Month - 1] < progressBar2.Maximum)
                            {
                                progressBar2.Value = progressBar2.Value + monthLength[currentTime.Month - 1];
                            }
                            else
                            {
                                progressBar2.Value = progressBar2.Maximum;
                            }
                        }
                    }
                }
                else if (time == "days")
                {
                    while (currentTime.Day <= endTime.Day || currentTime.Month < endTime.Month || currentTime.Year < endTime.Year)
                    {
                        //create folder
                        Directory.CreateDirectory(rootPath + "\\" + currentTime.Year.ToString() + "\\" + months[currentTime.Month - 1] + "\\" + currentTime.Day + "-" + currentTime.DayOfWeek);
                        currentTime = currentTime.AddDays(1);

                        //update progress bar
                        if (updateProgressBar)
                        {
                            if (progressBar2.Value + 1 < progressBar2.Maximum)
                            {
                                progressBar2.Value = progressBar2.Value + 1;
                            }
                            else
                            {
                                progressBar2.Value = progressBar2.Maximum;
                            }
                        }
                    }
                }
            }
        }

        /*---------MOVES FILES INTO THE APPROPRIATE SUBFOLDERS---------*/
        private void moveFiles(String time, String rootPath, FileInfo[] fiArray, String[] months)
        {
            //add the files in the root folder to their correct subfolder
            DateTime creationTime = new DateTime();
            foreach (FileInfo fi in fiArray)
            {
                creationTime = File.GetCreationTime(rootPath + "\\" + fi.Name);
                if (time == "years")
                {
                    var targetFile = rootPath + "\\" + creationTime.Year + "\\" + fi.Name;
                    var rootFile = rootPath + "\\" + fi.Name;

                    File.Move(rootFile, targetFile);
                    File.SetCreationTime(targetFile, creationTime);
                    progressBar1.Value = progressBar1.Value + 1;
                }
                else if(time=="months")
                {
                    var targetFile = rootPath + "\\" + creationTime.Year + "\\" + months[creationTime.Month - 1] + "\\" + fi.Name;
                    var rootFile = rootPath + "\\" + fi.Name;

                    File.Move(rootFile, targetFile);
                    File.SetCreationTime(targetFile, creationTime);
                    progressBar1.Value = progressBar1.Value + 1;
                }
                else if(time=="days")
                {
                    var targetFile = rootPath + "\\" + creationTime.Year + "\\" + months[creationTime.Month - 1] +
                                     "\\" + creationTime.Day + "-" + creationTime.DayOfWeek + "\\" + fi.Name;
                    var rootFile = rootPath + "\\" + fi.Name;

                    File.Move(rootFile, targetFile);
                    File.SetCreationTime(targetFile, creationTime);
                    progressBar1.Value = progressBar1.Value + 1;
                }
            }
        }

        /*--------REMOVES ANY FOLDERS CREATED WHICH ARE EMPTY--------*/
        private static void removeEmptyFolders(string path)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                removeEmptyFolders(directory);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }

        /*-------SEARCH FOR FOLDERS MANUALLY--------*/
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
