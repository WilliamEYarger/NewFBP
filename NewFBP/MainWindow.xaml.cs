using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows;
using NewFBP.DataModels;
using NewFBP.HelperClasses;
using System.Printing;


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
                string currentSourceName = SourceName ;
                DataModels.AppProperties.SourceDirectory = currentSourceName;

                /*Create a path to a storage directory from the sourceRootDirectory and the SourceName+"Backup"
                 * check to see if it exists and if it doesn't create it.
                 * 
                 */
                string sourceBackupDirPath = sourceRootDirectory+SourceName + "Backup";

                // Check if the directory exists
                if (!Directory.Exists(sourceBackupDirPath))
                {
                    // Create the directory if it does not exist
                    Directory.CreateDirectory(sourceBackupDirPath);
                }
                else
                { // Get all of the old files

                    #region Get the DirCntr and FileCntr
                    // Get the DirCntr and FileCntr
                    string filePath = sourceBackupDirPath + '.' + "CurrentCntrValues.txt";

                    string[] currentCntrValues;

                    if (File.Exists(filePath))
                    {
                        currentCntrValues = File.ReadAllLines(filePath);

                        //get the string value of the DirCntr and convert into an int Dirvalue

                        string numberString = currentCntrValues[0];
                        if (int.TryParse(numberString, out int number))
                        {
                            DataModels.AppProperties.DirCntr = number;
                        }
                        else
                        {
                            MessageBox.Show("Invalid number format.");
                        }

                        //get the string value of the FileCntr and convert into an int FileCntr

                        numberString = currentCntrValues[1];
                        if (int.TryParse(numberString, out int fileNumber))
                        {
                            DataModels.AppProperties.DirCntr = fileNumber;
                        }
                        else
                        {
                            MessageBox.Show("Invalid number format.");
                        }

                        #endregion et the DirCntr and FileCntr

                    #region get OldDirNamesDict
                        // get OldDirNamesDict
                        filePath = sourceBackupDirPath + '.' + "CurrentDirNamesDict.txt";
                        //create a string array to hold the lines of the "CurrentDirNamesDict.txt file
                        string[] currentDirNamesDict;

                        //create a dictionary to hold the Keyvalue pairs of OldDirNamesDict
                        Dictionary<string, string> OldDirNamesDict = new Dictionary<string, string>();

                        // see if filePath file exists and if it does get it else create a new OldDirNamesDict
                        if (File.Exists(filePath))
                        {
                            string[] currentDirNamesDictArr = File.ReadAllLines(filePath);

                            //cycle thru currentDirNamesDictArr and create OldDirNamesDict
                            for (int i = 0; i < currentDirNamesDictArr.Length; i++)
                            {
                                string[] KeyValuePairArr = currentDirNamesDictArr[i].Split('~');
                                OldDirNamesDict.Add(KeyValuePairArr[0], KeyValuePairArr[1]);
                            }
                            DataModels.AppProperties.DirIDNamesDict = OldDirNamesDict;
                        }
                        #endregion get OldDirNamesDict

                        #region get OldFileNamesDict

                        // get OldDirNamesDict  CHANGED OldDirNamesDict TO CurrentFileNamesDict
                        filePath = sourceBackupDirPath + '.' + "CurrentFileNamesDict.txt";
                        //create a string array to hold the lines of the "CurrentFileNamesDict.txt file
                        //CHANGED currentFileNamesDict TO currentFileNamesDictArr
                        string[] currentFileNamesDictArr;

                        /*
                         * create a dictionary currentFileNamesDict, to hold XXXXXXXXXXXXXXXXt 
                         * Set it to the global FileNamesDict if it exists, if it doesn't create it 
                         * 
                         * CHECK TO SEE WHAT THE GLOBAL
                        */

                        //CHANGED OldFileNamesDict  TO currentFileNamesDict
                        Dictionary<string, string> currentFileNamesDict = new Dictionary<string, string>();
                     //   if(DataModels.AppProperties.)
                        // see if filePath file exists and if it does get it else create a new currentFileNamesDict
                        if (File.Exists(filePath))
                        {
                            currentFileNamesDictArr = File.ReadAllLines(filePath);

                            //cycle thru currentFileNamesDictArr and create currentFileNamesDict
                            for (int i = 0; i < currentFileNamesDictArr.Length; i++)
                            {
                                string[] KeyValuePairArr = currentFileNamesDictArr[i].Split('~');
                                currentFileNamesDict.Add(KeyValuePairArr[0], KeyValuePairArr[1]);
                            }
                            DataModels.AppProperties.FileNamesDict = currentFileNamesDict;
                        }
                        #endregion get OldFileNamesDict

                    #region get FileInfoDict
                        // get FileLengthDict
                        filePath = sourceBackupDirPath + '.' + "CurrentFileInfoDict.txt";
                        //create a string array to hold the lines of the "CurrentFileInfoDict.txt file
                        string[] currentFileInfoDict;

                        //create a dictionary to hold the Keyvalue pairs of FileLengthDict
                        Dictionary<string, string> OldFileInfoDict = new Dictionary<string, string>();

                        // see if filePath file exists and if it does get it else create a new FileLengthDict
                        if (File.Exists(filePath))
                        {
                            string[] currentFileInfoDictArr = File.ReadAllLines(filePath);

                            //cycle thru currentFileInfoDictArr and create FileLengthDict
                            for (int i = 0; i < currentFileInfoDictArr.Length; i++)
                            {
                                string[] KeyValuePairArr = currentFileInfoDictArr[i].Split('~');
                                OldFileInfoDict.Add(KeyValuePairArr[0], KeyValuePairArr[1]);
                            }
                            DataModels.AppProperties.FileLengthDict = OldFileInfoDict;
                        }

                        #endregion FileInfoDict

                    //#region get FileFetchDict

                    //    // get OldFileFetchDict
                    //    filePath = sourceBackupDirPath + '.' + "CurrentFileFetchDict.txt";
                    //    //create a string array to hold the lines of the "CurrentFileFetchDict.txt file
                    //    string[] currentFileFetchDict;

                    //    //create a dictionary to hold the Keyvalue pairs of OldFileFetchDict
                    //    Dictionary<string, string> OldFileFetchDict = new Dictionary<string, string>();

                    //    // see if filePath file exists and if it does get it else create a new OldFileFetchDict
                    //    if (File.Exists(filePath))
                    //    {
                    //        string[] currentFileFetchDictArr = File.ReadAllLines(filePath);

                    //        //cycle thru currentFileFetchDictArr and create OldFileFetchDict
                    //        for (int i = 0; i < currentFileFetchDictArr.Length; i++)
                    //        {
                    //            string[] KeyValuePairArr = currentFileFetchDictArr[i].Split('~');
                    //            OldFileFetchDict.Add(KeyValuePairArr[0], KeyValuePairArr[1]);
                    //        }
                    //        DataModels.AppProperties.OldFileFetchDict = OldFileFetchDict;
                    //    }

                    //    #endregion FileFetchDict

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