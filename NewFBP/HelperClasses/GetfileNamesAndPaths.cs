using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System;
using System.Collections.Generic;
using NewFBP.DataModels;
using NewFBP.HelperClasses;

namespace NewFBP.HelperClasses
{
    public static class GetfileNamesAndPaths


    {

        private static string[] FolderPathArray = new string[0];
        private static string[] FileNamesArray = new string[0];
        private static string[] FilePathArray = new string[0];
        private static List<string> FolderPathList = new List<string>();
        private static List<string> FileNameList = new List<string>();
        private static List<string> FilePathList = new List<string>();


        public static void ProcessSourceFiles()
        {//This is copied from ListFoldersAndFiles(string folderPath from  ns NewListFoldersAndFiles
            try
            {
                // Get all directories within the specified folder      
                FolderPathArray = Directory.GetDirectories(DataModels.AppProperties.CurrentSourcePath);
                // Get all files within the specified folder
                FilePathArray = Directory.GetFiles(DataModels.AppProperties.CurrentSourcePath);

                // Display directories
                foreach (string directory in FolderPathArray)
                {
                    FolderPathList.Add(directory);
                }

                // Display files
                foreach (string file in FilePathArray)
                {
                    string fileName = Path.GetFileName(file);
                    FilePathList.Add(file);
                    FileNameList.Add(fileName);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                System.Windows.MessageBox.Show("Error: " + ex.Message);
            }

            if (FolderPathList.Count != 0)
            {
                string firstFolderName = FolderPathList[0];
                FolderPathList.Remove(firstFolderName);
                ListFoldersAndFiles(firstFolderName);
            }
            else
            {
                string[] FileNamesArray = FileNameList.ToArray();
                string[] FilePathArray = FilePathList.ToArray();

                //string fileOutputPath = "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\ReligionFiles";
                string fileOutputPath = DataModels.AppProperties.StoragePath;//the folder where the Current File Data Should be stored

                string fileNamePath = DataModels.AppProperties.FileNamesPath;//the path of the FileNames
                string filePathsPath = DataModels.AppProperties.FilePathsPath;


                //Write the file names to ReligionFileName
                using (StreamWriter writer = new StreamWriter(fileNamePath))
                {
                    foreach (string line in FileNamesArray)
                    {
                        writer.WriteLine(line);
                    }
                }

                // Write the file paths to RelibionFilePaths
                using (StreamWriter writer = new StreamWriter(filePathsPath))
                {
                    foreach (string line in FilePathArray)
                    {
                        writer.WriteLine(line);
                    }
                }
                
                //Application.Exit;
            }

        }

        private static void ListFoldersAndFiles(string selectedFolder)
        {
            //List<string> FolderPathList = new List<string>();
            // List<string> FilePathList = new List<string>();
            //List<string> FileNameList = new List<string>();



            try
            {
                // Get all directories within the specified folder 
                string[] FolderPathArray = Directory.GetDirectories(selectedFolder);

                // Get all files within the specified folder
                string[] FilePathArray = Directory.GetFiles(selectedFolder);


                // Display directories
                //List<string> FolderPathList = new List<string>;
                foreach (string directory in FolderPathArray)
                {
                    FolderPathList.Add(directory);
                }

                //Display file names and paths
                foreach (string file in FilePathArray)
                {
                    string fileName = Path.GetFileName(file);
                    FilePathList.Add(file);
                    FileNameList.Add(fileName);
                }
            }
            catch (Exception ex)
            {
                //Handle any exceptions
                System.Windows.MessageBox.Show("Error: " + ex.Message);
            }


            if (FolderPathList.Count != 0)
            {
                string firstFolderName = FolderPathList[0];
                FolderPathList.Remove(firstFolderName);
                ListFoldersAndFiles(firstFolderName);
            }
            else
            {
                string[] FileNamesArray = FileNameList.ToArray();
                string[] FilePathArray = FilePathList.ToArray();
                //Get the path to the folder where the data will be stored
                string fileOutputPath = DataModels.AppProperties.StoragePath;
                //get tne name of the output file names
                string fileNamePath = DataModels.AppProperties.FileNamesPath;
                string filePathsPath = DataModels.AppProperties.FilePathsPath;

                //Write the file names to ReligionFileName
                using (StreamWriter writer = new StreamWriter(fileNamePath))
                {
                    foreach (string line in FileNamesArray)
                    {
                        writer.WriteLine(line);
                    }
                }

                // Write the file paths to RelibionFilePaths
                using (StreamWriter writer = new StreamWriter(filePathsPath))
                {
                    foreach (string line in FilePathArray)
                    {
                        writer.WriteLine(line);
                    }
                }
                //Environment.Exit(0);
                string pathsSourceFile = DataModels.AppProperties.FilePathsPath;

                HelperClasses.ProcessFilePathsFile.ProcessInputFile(pathsSourceFile);
            }

        }

    }//end cladss

}//end namespace
