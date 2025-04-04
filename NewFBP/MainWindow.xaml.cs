﻿using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows;
using NewFBP.DataModels;
using NewFBP.HelperClasses;
using System.Printing;
using static System.Net.Mime.MediaTypeNames;


namespace NewFBP
{
    /// <summary>
    public partial class MainWindow : Window
    {
        // Define the static Dependency Property to store the Source Folder name
        public static readonly DependencyProperty SourceNameProperty =
            DependencyProperty.Register("SourceName", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));

        // create an object of AppProperties to hold system varialbes
        // DataModels.AppProperties appProperties = new DataModels.AppProperties();
        public string SourceName
        {
            get { return (string)GetValue(SourceNameProperty); }
            set { SetValue(SourceNameProperty, value); }
        }



        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
          

            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select the Folder to be Backed Up"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //Tne sourcePath is the full path of the selected folder
                //"C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion
                string sourcePath = dialog.FileName;

                // Store the selected folder name in the Dependency Property
                //"Religion"
                SourceName = new DirectoryInfo(dialog.FileName).Name;

                /*The SourceRootDirectory is the string up to, but not including the SourceName
                 * When the Dictionary of Directory names is created it will be used
                 * so that the properties of a directorr in the source can be compared to 
                 * a directory in the storage site.
                 * For example if the source directory path is "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion"
                 * and I want to compare it to the directory whose path is "D:\\ReligionBackup\\DirIDNamesDict\\Religion" they 
                 * can only be compared if I strip off the roots
                 */
                string sourceRootDirectory = sourcePath.Replace(SourceName, "");
                 DataModels.AppProperties.RootDirectory = sourceRootDirectory;

                // Store the complete path to the source directory
                DataModels.AppProperties.CurrentSourcePath = sourcePath;

                //store the Name of the current source directory
                DataModels.AppProperties.CurrentSourceName = SourceName;

                // Create the root directory name from SourceName+'\\'
                string currentSourceName = SourceName + '\\';// CORRECTED ERERRORROE DID NOT ADD \\
                DataModels.AppProperties.SourceDirectory = currentSourceName;
                /*Create a path to a storage directo\ry \oesn't create it.
                 * 
                 */
                string sourceBackupDirPath = sourceRootDirectory+SourceName + "Backup"+'\\';

                // Check if the directory exists
                if (!Directory.Exists(sourceBackupDirPath))
                {
                    // Create the directory if it does not exist
                    Directory.CreateDirectory(sourceBackupDirPath);

                    // save the name of the backup folder
                    DataModels.AppProperties.SourceBackupDirPath = sourceBackupDirPath;
                    HelperClasses.FileIOClass.sourceBackupDirPath = sourceBackupDirPath ;
                }                else
                { // Get all of the old files


                    // save the name of the backup folder
                    DataModels.AppProperties.SourceBackupDirPath = sourceBackupDirPath ;
                    HelperClasses.FileIOClass.sourceBackupDirPath = sourceBackupDirPath ;

                    #region Get the DirCntr and FileCntr
                    // Get the DirCntr and FileCntr
                    string filePath = sourceBackupDirPath  + "CurrentCntrValues.txt";

                    string[] currentCntrValues;
                    if (File.Exists(filePath))
                    {
                        currentCntrValues = File.ReadAllLines(filePath);
                        string delimitedString = currentCntrValues[0];
                        string[] currentCntrValuesArr = delimitedString.Split('~');

                        //get the string value of the DirCntr and convert into an int Dirvalue
                        
                        string numberString = currentCntrValuesArr[0];
                        if (int.TryParse(numberString, out int number))
                        {
                            DataModels.AppProperties.DirCntr = number;
                        }
                        else
                        {
                            MessageBox.Show("Invalid number format.");
                        }

                        //get the string value of the FileCntr and convert into an int FileCntr

                        numberString = currentCntrValuesArr[1];
                        if (int.TryParse(numberString, out int fileNumber))
                        {
                            DataModels.AppProperties.FileCntr = fileNumber;
                        }
                        else
                        {
                            MessageBox.Show("Invalid number format.");
                        }

                        #endregion et the DirCntr and FileCntr

                        #region get FileFetchDict 
                        // get OldDirNamesDict
                        filePath = sourceBackupDirPath + "FileFetchDict.txt";
                        //create a string array to hold the lines of the "CurrentDirNamesDict.txt file
                        string[] FileFetchDictArr;

                        //create a dictionary to hold the Keyvalue pairs of OldDirNamesDict
                        Dictionary<string, string> currentFileFetchDict = new Dictionary<string, string>();

                        // see if filePath file exists and if it does get it else create a new OldDirNamesDict
                        if (File.Exists(filePath))
                        {
                            FileFetchDictArr = File.ReadAllLines(filePath);

                            //cycle thru currentDirNamesDictArr and create OldDirNamesDict
                            for (int i = 0; i < FileFetchDictArr.Length; i++)
                            {
                                string[] KeyValuePairArr = FileFetchDictArr[i].Split('~');
                                currentFileFetchDict.Add(KeyValuePairArr[0], KeyValuePairArr[1]);
                            }
                            DataModels.AppProperties.FileFetchDict = currentFileFetchDict;
                        }
                        #endregion get OldDirNamesDict

                        #region get FileLengthDict 

                        // get OldDirNamesDict  CHANGED OldDirNamesDict TO CurrentFileNamesDict
                        filePath = sourceBackupDirPath   + "FileLengthDict .txt";
                        //create a string array to hold the lines of the "CurrentFileNamesDict.txt file
                        //CHANGED currentFileNamesDict TO currentFileNamesDictArr
                        string[] currentFileLengthDictArr;

                        /*
                         * create a dictionary currentFileNamesDict, to hold XXXXXXXXXXXXXXXXt 
                         * Set it to the global FileNamesDict if it exists, if it doesn't create it 
                         * 
                         * CHECK TO SEE WHAT THE GLOBAL
                        */

                        //CHANGED OldFileNamesDict  TO currentFileNamesDict
                        Dictionary<string, string> currentFileLengthDict = new Dictionary<string, string>();

                        /*
                            the filePath file does not exist on the first run
                         */
                        // see if filePath file exists and if it does get it else create a new currentFileNamesDict
                        if (File.Exists(filePath))
                        {
                            currentFileLengthDictArr = File.ReadAllLines(filePath);

                            //cycle thru currentFileNamesDictArr and create currentFileNamesDict
                            for (int i = 0; i < currentFileLengthDictArr.Length; i++)
                            {
                                string[] KeyValuePairArr = currentFileLengthDictArr[i].Split('~');
                                currentFileLengthDict.Add(KeyValuePairArr[0], KeyValuePairArr[1]);
                            }
                            DataModels.AppProperties.FileLengthDict = currentFileLengthDict;
                        }
                        #endregion get OldFileNamesDict

                        #region get FileVersionDict
                        // get FileLengthDict
                        filePath = sourceBackupDirPath  + "FileVersionDict.txt";
                        //create a string array to hold the lines of the "CurrentFileInfoDict.txt file
                        string[] currentFileVersionDictArr;

                        //create a dictionary to hold the Keyvalue pairs of FileLengthDict
                        Dictionary<string, string> currentFileVersionDict = new Dictionary<string, string>();

                        // see if filePath file exists and if it does get it else create a new FileLengthDict
                        if (File.Exists(filePath))
                        {
                           currentFileVersionDictArr = File.ReadAllLines(filePath);

                            //cycle thru currentFileInfoDictArr and create FileLengthDict
                            for (int i = 0; i < currentFileVersionDictArr.Length; i++)
                            {
                                string[] KeyValuePairArr = currentFileVersionDictArr[i].Split('~');
                                currentFileVersionDict.Add(KeyValuePairArr[0], KeyValuePairArr[1]);
                            }
                            DataModels.AppProperties.FileVersionDict = currentFileVersionDict;
                        }

                        #endregion FileInfoDict

                        #region get CurrentVersionDict


                        // get OldCurrentVersionDict
                        filePath = sourceBackupDirPath + '.' + "CurrentCurrentVersionDict.txt";
                        //create a string array to hold the lines of the "CurrentCurrentVersionDict.txt file
                        string[] currentCurrentVersionDict;

                        //create a dictionary to hold the Keyvalue pairs of OldCurrentVersionDict
                        Dictionary<string, string> OldCurrentVersionDict = new Dictionary<string, string>();

                        // see if filePath file exists and if it does get it else create a new OldCurrentVersionDict
                        if (File.Exists(filePath))
                        {
                            string[] currentCurrentVersionDictArr = File.ReadAllLines(filePath);

                            //cycle thru currentCurrentVersionDictArr and create OldCurrentVersionDict
                            for (int i = 0; i < currentCurrentVersionDictArr.Length; i++)
                            {
                                string[] KeyValuePairArr = currentCurrentVersionDictArr[i].Split('~');
                                OldCurrentVersionDict.Add(KeyValuePairArr[0], KeyValuePairArr[1]);
                            }
                            DataModels.AppProperties.OldCurrentVersionDict = OldCurrentVersionDict;
                        }
                        #endregion  CurrentVersionDict

                        #region Get B26FileNamesList.txt


                        // get OldCurrentVersionDict
                        filePath = sourceBackupDirPath   + "B26FileNamesList.txt";

                        // if file exists read it ito array
                        if (File.Exists(filePath))
                        {
                            string[] B26FileNamesArr =  File.ReadAllLines(filePath);

                            // convert the array to a list
                            List<string> currentB26FileNamesList = B26FileNamesArr.ToList();

                            //save the list to appProperties
                            DataModels.AppProperties.B26FileNamesList = currentB26FileNamesList;
                        }





                        #endregion Get B26FileNamesList.txt



                    }// end if file Exists sourceBackupDirPath + '.' + "CurrentCntrValues.txt"
                }// end if (!Directory.Exists(sourceBackupDirPath))
                              

                GetfileNamesAndPaths.ProcessSourceFiles();
            }// end show dialog

        }// end SelectfolderButton_Click


      

        // Helper method to create a file if it doesn't exist
        private void CreateFileIfNotExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, ""); // Create an empty file
            }
        }

        // Helper method to create a directory if it doesn't exist
        private void CreateDirectoryIfNotExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

    }//end class

}//end namespace