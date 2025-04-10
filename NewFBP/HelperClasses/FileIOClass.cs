using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewFBP.HelperClasses
{
    public static class FileIOClass
    {
        /*FileIOClass
         * The purpose of this class is to handle all communicarions wirh the disk
         * The first task will be to create write to disk procedures
         */

        //LOCAL VARIABLES
        private static string currentRepostioryPath = DataModels.AppProperties.RepostioryPath;
        private static string currentFileVersionsPath = currentRepostioryPath + "FileVersions\\";
        private static string root = DataModels.AppProperties.RootDirectory;

        //private static Dictionary<string,string> currentDirIDNamesDict = DataModels.AppProperties.DirIDNamesDict;
        //private static Dictionary<string,string> currentFileVersionDict = DataModels.AppProperties.FileVersionDict;

        //Create a local value for the path to the local disk backup folder
        public static string sourceBackupDirPath { get; set; }

       
        public static void saveDicrionary( Dictionary<string,string> newDictionary, string dictName)
        {
            Dictionary<string, string> currentDictionary = newDictionary;
            string outputFilePath = sourceBackupDirPath + dictName + ".txt";

            //process each KeyValuePair in the currentDictionary to create a string Key~Value and store it in a 
            //List<string>
            List<string> OutputStringsList = new List<string>();


            string Key = String.Empty;
            string Value = String.Empty;
            string outputString = String.Empty;
            foreach(KeyValuePair<string, string> kvp in currentDictionary)
            {
                Key = kvp.Key;
                Value = kvp.Value;
                outputString = Key+  '~'+ Value;
                OutputStringsList.Add(outputString);
            }

            //convert the List to an array of strings
            string[] ousputArray = OutputStringsList .ToArray();

            //write ousputArray
            //
            //Write the file names to ReligionFileName
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (string line in ousputArray)
                {
                    writer.WriteLine(line);
                }
            }
            
        }// end  public static void saveDicrionary

        public static void SaveList(List<string> newList, string outputListName)
        {
            List<string> currentList = newList;


            string outputFilePath = sourceBackupDirPath + outputListName + ".txt";
            //convert List to straing array
            string[] currentListArr = currentList.ToArray();

            //write ousputArray
            //
            //Write the file names to ReligionFileName
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (string line in currentListArr)
                {
                    writer.WriteLine(line);
                }
            }

        }//end public static void SaveList(L

        public static void WriteString(string currentStr, string stringName)
        {
            string outputFilePath = sourceBackupDirPath + stringName + ".txt";

            File.WriteAllText(outputFilePath, currentStr);
            //string stophere = "";

        }
        //File.WriteAllText("example.txt", "This is a single line of text

        public static void SaveTextFilesToRepository()
        {
            string source = String.Empty;
            string destination = String.Empty;

            //Save B26FileNamesList
            source = sourceBackupDirPath + "B26FileNamesList.txt";
            destination = currentRepostioryPath + "\\B26FileNamesList.txt";
            SaveTextFilesToRepository(source, destination);

            //Save CurrentCntrValues
            source = sourceBackupDirPath + "CurrentCntrValues.txt";
            destination = currentRepostioryPath + "\\CurrentCntrValues.txt";
            SaveTextFilesToRepository(source, destination);

            //Save DirIDNamesDict
            source = sourceBackupDirPath + "DirIDNamesDict.txt";
            destination = currentRepostioryPath + "\\DirIDNamesDict.txt";
            SaveTextFilesToRepository(source, destination);

            //Save FileFetchDict
            source = sourceBackupDirPath + "FileFetchDict.txt";
            destination = currentRepostioryPath + "\\FileFetchDict.txt";
            SaveTextFilesToRepository(source, destination);

            //Save FileLengthDict
            source = sourceBackupDirPath + "FileLengthDict.txt";
            destination = currentRepostioryPath + "\\FileLengthDict.txt";
            SaveTextFilesToRepository(source, destination);


            //Save FileVersionDict
            source = sourceBackupDirPath + "FileVersionDict.txt";
            destination = currentRepostioryPath + "\\FileVersionDict.txt";
            SaveTextFilesToRepository(source, destination);



        }//end public static void SaveTextFilesToRepository()


        private static void SaveTextFilesToRepository(string source, string destination)
        {
            try
            {
                File.Copy(source, destination, true);
               
            }//end  try
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying {source}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }//end try catch


        }//end private static void SaveTextFilesToRepository(string source, string destination)


        public static void Save0VersionOfFiles()
        {
             

            //Get the FileFetchDict and convert it into a '~' delimited string array
            Dictionary<string, string> currentFileFetchDict = DataModels.AppProperties.FileFetchDict;
            string[] currentFileFetchDictArr = new string[currentFileFetchDict.Count];
            foreach (KeyValuePair<string, string> kvp in currentFileFetchDict)
            {
                string fullPath = kvp.Key;
                string B26Name = kvp.Value;
                string combinedFFDKVP = fullPath + '~' + B26Name;

                //Get the new repository version name for this file
                string newRepostionyName = GetFileVersionName(combinedFFDKVP);
                string repositoryPath = currentRepostioryPath + "\\FileVersions\\" + newRepostionyName;
                try
                {
                    File.Copy(fullPath, repositoryPath, true);
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            } //end foreach (KeyValuePair<string, string> kvp in currentFileFetchDict

        }// end public static void Save0VersionOfFiles()
        private static string GetFileVersionName(string FileFetchKVP)//‘~’ delimited KeyValue Pair
        {
            Dictionary<string, string> currentDirIDNamesDict = DataModels.AppProperties.DirIDNamesDict;
            Dictionary<string, string> currentFileVersionDict = DataModels.AppProperties.FileVersionDict;
            string returnStr = string.Empty;

            //Get full path and B26Name
            string[] FileFetchKVPArr = FileFetchKVP.Split('~');
            string fullPath = FileFetchKVPArr[0];
            string B26Name = FileFetchKVPArr[1];
            string fileName = Path.GetFileName(fullPath);
            string extension = Path.GetExtension(fullPath);
            string DirIDNamesDictKey = fullPath.Replace(root, "");
            DirIDNamesDictKey = DirIDNamesDictKey.Replace(fileName, "");
            string DirIDNumStr = currentDirIDNamesDict[DirIDNamesDictKey];
            string DirIDKey = DirIDNumStr + '.' + fileName;
            string currentVersionNumStr = currentFileVersionDict[DirIDKey];
            returnStr = DirIDNumStr + '.' + B26Name + '.' + currentVersionNumStr + extension;
            return returnStr;

        }//end private static string GetFileVersionName




















    //DO NOT GO PAST HERE
}//end public static class FileIOClass
}//end namespace NewFBP.HelperClasses
