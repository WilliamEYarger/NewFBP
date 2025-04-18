using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewFBP.HelperClasses
{
    public static class CreateAndRetrieveSourceTextFiles
    {

        //public static string  PathToSourceFolder { get; set; }

        private static string _sourceFolderPath;
        private static string _repositoryBackupPath;


        public static string SourceFolderPath
        {
            get => _sourceFolderPath;
            set
            {
                _sourceFolderPath = value;

                // Construct backup folder path
                string backupFolderPath = _sourceFolderPath + "Backup";

                // Create backup folder if it doesn't exist
                if (!Directory.Exists(backupFolderPath))
                {
                    //The Backup Directory has not been created so this is the Firstrun
                    DataModels.AppProperties.FirstRun = true;
                    Directory.CreateDirectory(backupFolderPath);

                    //20250417 START

                    DataModels.AppProperties.LocalBackupPath = backupFolderPath + '\\';

                    //20250417 END


                    // Prompt user to select an external SSD repository folder
                    string repositoryBasePath = SelectRepositoryFolder();
                    if (!string.IsNullOrEmpty(repositoryBasePath))
                    {
                        // Extract the source folder name
                        string sourceFolderName = new DirectoryInfo(_sourceFolderPath).Name;

                        // Construct the repository backup folder path
                        _repositoryBackupPath = Path.Combine(repositoryBasePath, sourceFolderName);

                        // Create repository backup folder if it doesn't exist
                        if (!Directory.Exists(_repositoryBackupPath))
                        {
                            Directory.CreateDirectory(_repositoryBackupPath);
                        }//end if (!Directory.Exists(_repositoryBackupPath))

                        // Create "FileVersions" subdirectory inside the repository
                        string fileVersionsPath = Path.Combine(_repositoryBackupPath, "FileVersions");
                        if (!Directory.Exists(fileVersionsPath))
                        {
                            Directory.CreateDirectory(fileVersionsPath);
                        }//end if (!Directory.Exists(fileVersionsPath))

                        // Create blank test files
                        CreateBlankTestFiles(backupFolderPath, _repositoryBackupPath);

                    }//end if (!string.IsNullOrEmpty(repositoryBasePath))

                }//end if (!Directory.Exists(backupFolderPath)) First Run
                else // this is Not the First Run
                {
                    DataModels.AppProperties.FirstRun = false;
                    return;
                }

            }//end set

        }//end public static string SourceFolderPath

        private static string SelectRepositoryFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Title = "Select Repository Base Folder on External SSD";

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            return string.Empty;
        }//end private static string SelectRepositoryFolder()

        public static void CreateBlankTestFiles(string backupFolderPath, string repositoryBackupPath)
        {
            string[] fileNames =
            {
                "B26FileNamesList.txt", "CurrentCntrValues.txt", "DirIDNamesDict.txt",
                "FileFetchDict.txt", "FileLengthDict.txt", "FileNamesList.txt",
                "FileVersionDict.txt", "PathToRepositoryLogFile.txt"
            };

            string[] repositoryFileNames =
            {
                "B26FileNamesList.txt", "CurrentCntrValues.txt", "DirIDNamesDict.txt",
                "FileFetchDict.txt", "FileLengthDict.txt", "FileNamesList.txt",
                "FileVersionDict.txt", "RepositoryLogFile.txt"
            };

            // Create text files in the local Source Backup folder
            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(backupFolderPath, fileName);
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                }
            }//end foreach (string fileName in fileNames)

            // Create text files in the Repository Backup folder
            foreach (string fileName in repositoryFileNames)
            {
                string filePath = Path.Combine(repositoryBackupPath, fileName);
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                }
            }//end foreach (string fileName in repositoryFileNames)

            // Store the path of RepositoryLogFile.txt in PathToRepositoryLogFile.txt
            string repositoryLogFilePath = Path.Combine(repositoryBackupPath, "RepositoryLogFile.txt");
            string pathToRepositoryLogFile = Path.Combine(backupFolderPath, "PathToRepositoryLogFile.txt");

            File.WriteAllText(pathToRepositoryLogFile, repositoryLogFilePath);
        }//end public static void CreateBlankTestFiles


        //DO NOT GO BEYOND HERE
    }//end public static class PathToSourceFolder

    //DO NOT GO BEYOND HERE
}//end public static class CreateAndRetrieveSourceTextFiles

