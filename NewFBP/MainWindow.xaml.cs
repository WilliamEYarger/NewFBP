using Microsoft.WindowsAPICodePack.Dialogs;
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
          /*
           Selecte the source folder
           */

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
                 * and I want to compare it to the directory whose path is "D:\\ReligionBackup\\DirIDNamesDict\\Religion" 
                 * they can only be compared if I strip off the roots.
                 * For the Religion source the sourceRootDirectory is "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\"
                 */
                string sourceRootDirectory = sourcePath.Replace(SourceName, "");
                 DataModels.AppProperties.RootDirectory = sourceRootDirectory;

                // Store the complete path to the source directory 
                //ie "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion"
                DataModels.AppProperties.CurrentSourcePath = sourcePath;

                //store the Name of the current source directory "Religion"
                DataModels.AppProperties.CurrentSourceName = SourceName;

                // Create the root directory name from SourceName+'\\' ="Religion\\"
                string currentSourceName = SourceName + '\\';
                DataModels.AppProperties.SourceDirectory = currentSourceName;


                /* Create a new directory to hold all of the files necessary to 
                 * determine if a new dirctory or file has been added, or 
                 * a file has changed its length since the last backup.
                 * 
                 * !!! the name of this folder will be the same as that of the Source with the
                 * word Backup and a file seperator appended, ie ReligionBackup\\
                 * These values will be saved to the disk during creation and retrieved when
                 * the source has been chosed !!! MAKE SURE THIS IS DONE!!!
                 * "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\ReligionBackup\\"
                 */
                string sourceBackupDirPath = sourceRootDirectory+SourceName + "Backup"+'\\';

                // Check if the directory exists
                if (!Directory.Exists(sourceBackupDirPath))
                {

                    /*
                     If the source directory doesn't exist then this is the First run so set
                    that to true and ask the user to select the path to the Repositoyr
                     */

                    DataModels.AppProperties.FirstRun = true;


                    //Set up the button to select the Repositoyr
                    SelectFolderButton.Content = $"Select the Drive and/or folder that will hold Repository";

                    //create a method to select the repository site
                    CreateRepository(SourceName);


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
                    // "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\ReligionBackup\\CurrentCntrValues.txt"
                    string filePath = sourceBackupDirPath  + "CurrentCntrValues.txt";

                    string[] currentCntrValues;
                    if (File.Exists(filePath))
                    {
                        currentCntrValues = File.ReadAllLines(filePath);
                        string delimitedString = currentCntrValues[0];
                        string[] currentCntrValuesArr = delimitedString.Split('~');


                        //Get the number of dirctories after the last run of the program "259"
                        string numberString = currentCntrValuesArr[0];
                        if (int.TryParse(numberString, out int number))
                        {
                            DataModels.AppProperties.DirCntr = number;
                        }
                        else
                        {
                            MessageBox.Show("Invalid number format.");
                        }

                        //get the numer of files detcted in the last run of the program
                        // ie, the FileCntr, and convert into an int FileCntr
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
                         /* The File Fetch Dictionary
                            FileFetchDict, a dictionary whose KEY if the full path to a file and whose value is Base26File# 
                            [FilePath,Base26File#} eg. {"C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles 
                            List.docx",AAA) this will be used to will be used to search the CurrentVersionDict to the the 
                            current version number so it can be incemented and applied to the Base26Figet the file to be 
                            copied to the Repository Backup folder and the Value (the Base26File#)  le as the name of the 
                            current version of the file in the Repository Backup folder. eg. the file named AAA.0 will 
                            contain the first version of the file 
                            "C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx"
                            */
                        // get the FileFetchDict path
                        // "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\ReligionBackup\\FileFetchDict.txt"
                        filePath = sourceBackupDirPath + "FileFetchDict.txt";

                        //create a string array to hold the lines of the "CurrentDirNamesDict.txt file
                        string[] FileFetchDictArr;

                        //create a dictionary to hold the Keyvalue pairs of FileFetchDict
                        Dictionary<string, string> currentFileFetchDict = new Dictionary<string, string>();

                        // see if filePath file exists and if it does get it else create a new OldDirNamesDict
                        if (File.Exists(filePath))
                        {
                            FileFetchDictArr = File.ReadAllLines(filePath);

                            //cycle thru FileFetchDictArr and create currentFileFetchDict
                            for (int i = 0; i < FileFetchDictArr.Length; i++)
                            {
                                string[] KeyValuePairArr = FileFetchDictArr[i].Split('~');
                                currentFileFetchDict.Add(KeyValuePairArr[0], KeyValuePairArr[1]);
                            }
                            //Save the currentFileFetchdict to the global property
                            DataModels.AppProperties.FileFetchDict = currentFileFetchDict;
                        }
                        #endregion get FileFetchDict

                        #region get FileLengthDict 
                        /*
                         The FileLength Dictionary has a Key of a file's B26 file name and a VALUE of 
                        its length at the last run of the program
                         */

                        //"C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\ReligionBackup\\FileLengthDict .txt"
                        filePath = sourceBackupDirPath   + "FileLengthDict .txt";

                        //create a string array to hold the lines of the currentFileLengthDictArr
                        string[] currentFileLengthDictArr;

                        /*
                         * create a dictionary currentFileNamesDict, to hold FileLengthDict
                            the Key is the B26Name and the value is the file length
                         * Set it to the global FileNamesDict if it exists, if it doesn't create it 
                        */

                        //Create currentFileLengthDict
                        Dictionary<string, string> currentFileLengthDict = new Dictionary<string, string>();

                        /*
                            the filePath file does not exist on the first run
                         */
                        // see if filePath file exists and if it does get it else create a new currentFileNamesDict
                        //"C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\ReligionBackup\\FileLengthDict .txt"
                        if (File.Exists(filePath))
                        {
                            currentFileLengthDictArr = File.ReadAllLines(filePath);

                            //cycle thru currentFileNamesDictArr and create currentFileNamesDict
                            for (int i = 0; i < currentFileLengthDictArr.Length; i++)
                            {
                                string[] KeyValuePairArr = currentFileLengthDictArr[i].Split('~');
                                currentFileLengthDict.Add(KeyValuePairArr[0], KeyValuePairArr[1]);
                            }

                            //Save the values of the file length dictionary at the last run of the program
                            // to the global property
                            DataModels.AppProperties.FileLengthDict = currentFileLengthDict;
                        }
                        #endregion get FileLengthDic

                        #region get FileVersionDict
                        /*
                         * The File Version Dictionary has a KEY is the drive independant name of the file 
                         * composed of it Directory's IdInt number +'.' plue its file name ie "0.Articles List.docx"
                         * and a VALUE of of the current version number (initially '0' for all files
                         */


                        // get FileLengthDict
                        // "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\ReligionBackup\\FileVersionDict.txt"
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

                            //Save the file version number from the last run of the program the count is = 6732
                            DataModels.AppProperties.FileVersionDict = currentFileVersionDict;
                        }

                        #endregion get FileVersionDict

                        #region get DirIDNamesDict


                        // get DirIDNamesDict
                        filePath = sourceBackupDirPath + "DirIDNamesDict.txt";
                        //create a string array to hold the lines of the "DirIDNamesDict.txt file
                        string[] currentDirIDNamesDictArr;

                        //create a dictionary to hold the Keyvalue pairs of OldCurrentVersionDict
                        Dictionary<string, string> currentDirIDNamesDict = new Dictionary<string, string>();

                        // see if filePath file exists and if it does get it else create a new OldCurrentVersionDict
                        if (File.Exists(filePath))
                        {
                            currentDirIDNamesDictArr = File.ReadAllLines(filePath);

                            //cycle thru currentCurrentVersionDictArr and create OldCurrentVersionDict
                            for (int i = 0; i < currentDirIDNamesDictArr.Length; i++)
                            {
                                string[] KeyValuePairArr = currentDirIDNamesDictArr[i].Split('~');
                                currentDirIDNamesDict.Add(KeyValuePairArr[0], KeyValuePairArr[1]);
                            }
                            DataModels.AppProperties.DirIDNamesDict = currentDirIDNamesDict;
                        }
                        #endregion  get DirIDNamesDict

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

                        /*
                         Create mechanis to get the path to the FRepository from
                         DataModels.AppProperties.RepostioryPath
                         */

                    }// end if file Exists sourceBackupDirPath + '.' + "CurrentCntrValues.txt"
                }// end if (!Directory.Exists(sourceBackupDirPath))
                              

                GetfileNamesAndPaths.ProcessSourceFiles();
            }// end show dialog

        }// end  private void SelectFolderButton_Click


        private void CreateRepository(string SourceName)
        {

            try
            {
                // Prompt the user to select the storage directory
                CommonOpenFileDialog dialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Title = "Select the folder where repository files should be stored"
                };

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string Repository  = Path.Combine(dialog.FileName, SourceName );

                    if (!Directory.Exists(Repository))
                    {
                        Directory.CreateDirectory(Repository);
                        string fileVersions = Path.Combine(Repository, "FileVersions\\");
                        Directory.CreateDirectory(fileVersions);
                        //MessageBox.Show($"Backup directory created: {backupDirectory}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    DataModels.AppProperties.RepostioryPath = Repository;
                }
                else
                {
                    MessageBox.Show("No repository directory was selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating backup directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        


        }//end private void CreateRepository

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