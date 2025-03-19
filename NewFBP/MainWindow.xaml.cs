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
                 * and I want to compare it to the directory whose path is "D:\\ReligionBackup\\DirNamesDict\\Religion" they 
                 * can only be compared if I strip off the roots
                 */
                string currentSourcePath = sourcePath.Replace(SourceName, "");
                DataModels.AppProperties.SourceRootDirectory = currentSourcePath;

                // Store the complete path to the source directory
                DataModels.AppProperties.CurrentSourcePath = sourcePath;

                //store the Name of the current source directory
                DataModels.AppProperties.CurrentSourceName = SourceName;

                // Create the root directory name from SourceName+'\\'
                string currentSourceName = SourceName ;
                DataModels.AppProperties.SourceDirectory = currentSourceName;
                //string currentSourcePath = DataModels.AppProperties.CurrentSourcePath;
                //DataModels.AppProperties.SourceRootDirectory = currentSourceDirectorhy;



                SelectFolderButton.Content = $"Select the Drive or folder that will hold the backup of the {SourceName} Directory";

                // Call method to handle the backup directory creation
                CreateStorageDirectory(sourcePath);
            }

        }


        private void CreateStorageDirectory(string sourcePath)
        {
            try
            {
                // Prompt the user to select the storage directory
                CommonOpenFileDialog dialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Title = "Select the folder where backup files should be stored"
                };

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string backupDirectory = Path.Combine(dialog.FileName, SourceName + "FilesBackup");

                    if (!Directory.Exists(backupDirectory))
                    {
                        Directory.CreateDirectory(backupDirectory);
                        //MessageBox.Show($"Backup directory created: {backupDirectory}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    // Create required files
                    CreateFileIfNotExists(Path.Combine(backupDirectory, "DirNamesDict.txt"));
                    CreateFileIfNotExists(Path.Combine(backupDirectory, "FileNamesDict.txt"));
                    CreateFileIfNotExists(Path.Combine(backupDirectory, "FileInfoDict.txt"));

                    // Create required subdirectories
                    CreateDirectoryIfNotExists(Path.Combine(backupDirectory, "LastFileInfo"));
                    CreateDirectoryIfNotExists(Path.Combine(backupDirectory, "FileBackups"));

                    // Shift program control to 'Process Source Directory'
                    ProcessSourceDirectory(sourcePath, backupDirectory);
                }
                else
                {
                    MessageBox.Show("No storage directory was selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating backup directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ProcessSourceDirectory(string sourcePath, string backupDirectory)
        {
            // Change button content
            SelectFolderButton.Content = "Select the folder where the Current File Data Should be stored";

            // Open folder selection dialog for storing current file data
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select the folder where the Current File Data Should be stored"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string storagePath = dialog.FileName;
                DataModels.AppProperties.StoragePath = storagePath;//the folder where the Current File Data Should be stored


                string todayDate = DateTime.Now.ToString("yyyyMMdd");

                // Construct file names
                string fileNamesPath = Path.Combine(storagePath, $"{SourceName}FileNames{todayDate}.txt");
                DataModels.AppProperties.FileNamesPath = fileNamesPath;
                string filePathsPath = Path.Combine(storagePath, $"{SourceName}FilePaths{todayDate}.txt");
                DataModels.AppProperties.FilePathsPath = filePathsPath;


                // Create blank text files
                CreateFileIfNotExists(fileNamesPath);
                CreateFileIfNotExists(filePathsPath);

                // call HelperClasses.GetfileNamesAndPaths to process the files
                GetfileNamesAndPaths.ProcessSourceFiles();

            }
        }


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