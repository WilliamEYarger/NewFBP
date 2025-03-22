using System;
using System.Collections.Generic;
using System.IO;
using NewFBP.HelperClasses;
using NewFBP.DataModels;

namespace NewFBP.HelperClasses
{
    public static class ProcessFilePathsFile
    {
        //public static Dictionary<string, string> DirNamesDict { get; private set; }
        //public static Dictionary<string, string> FileNamesDict { get; private set; }
        //public static Dictionary<string, string> FileInfoDict { get; private set; }
        private static string RootPath;
        private static int DirCntr;
        private static int FileCntr;
        private static Dictionary<string, string> DirNamesDict = new Dictionary<string, string>();
        private static Dictionary<string, string> FileNamesDict = new Dictionary<string, string>();
        private static Dictionary<string, string> FileInfoDict = new Dictionary<string, string>();
        private static void ProcessInputFile(string inputFilePath)
        {
           

            string[] lines = File.ReadAllLines(inputFilePath);

            string root = DataModels.AppProperties.SourceRootDirectory;
            string dirName;
            string compositFileName;
            string currentDirKey = "";
            string currentDirValue= "";
            string currentFileKey = "";
            string currentFileValue = "";
            string currentFilePath = "";
            string currentFileSizeStr = "";
            string lastFileModifiedDateStr = "";
            string currentFileInfoKey = "";
            string currentFileInfoValue = "";

            currentDirKey = DataModels.AppProperties.SourceDirectory + '\\';
            currentDirValue = "0";
            // Create the first DirNamesDict from the CurrentSourceName

            DirNamesDict.Add(currentDirKey, currentDirValue);
            DirCntr++;
            foreach (string line in lines)
            {
                currentFilePath = line;
                //step 1 remove the root
                string newline = line.Replace(root, "");

                //step 2 get the FileName and remove it from newlins
                string fileName = Path.GetFileName(line);

                //step 3 remove the filenmae from newline
                newline = newline.Replace(fileName, "");

                //step 4 determine if DirNamesDict does or does not has a Key = newline,
                //if it doesn't create one
                //set a new string currentDirKey = whatever the correct key is
                currentDirKey = newline;
                if (!DirNamesDict.ContainsKey(newline))
                {
                    //The current value of line contains a value that is not in the DirNamesDict
                    // add a new key value 
                    currentDirValue = DirCntr.ToString();
                    DirNamesDict.Add(currentDirKey, currentDirValue);
                    DirCntr++;
                }

                //Get the value associated with the currentDirKey
                currentDirValue = DirNamesDict[currentDirKey];

                /*The following section processes the File referenced in line whose original value =
                 * C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx
                 * The data will be entered into the FileNamesDict
                 * the Key will be the currentDirValue (ie 0)concatinated to the fileName ie Articles List.docx=
                 * 0.Articles List.docx
                 * To get the Value for the FileNamesDict send the current value of FileCntr
                 * to StringHeler.ConvertToBase to get a Base26 integer value ie AAA
                 * thus the entry for the FileNamesDict = (0.Articles List.docx,AAA)
                 */

                //Get the new value for currentFileKey
                currentFileKey = currentDirValue + "." + fileName;

                //Get the Value for the CurrentFileValue
                currentFileValue = StringHelper.ConvertToBase26(FileCntr);
                //Increment the FileCntr
                FileCntr++;
                //Add the new KeyValue pair to the FileNamesDict
                FileNamesDict.Add(currentFileKey, currentFileValue);

                /* The following section gets the needed update data about the file shown in line
                 * It uses the System.IO.FileInfo to get the data
                 * and stores the data in  the FileInfoDict dictionary
                 * the currentFileInfoKey =the currentFileValue
                 * the currentFileInfoValue is currentFileSizeStr+"~"+lastFileModifiedDateStr
                 */
                // Get the FileInfo
                FileInfo fileInfo = new FileInfo(currentFilePath);
                //Get currentFileSizeStr
                currentFileSizeStr= fileInfo.Length.ToString();
                //Get lastFileModifiedDateStr
                lastFileModifiedDateStr= fileInfo.LastWriteTime.ToString();
                //set the Key for the FileInfoDict dictionary
                currentFileInfoKey = currentFileValue;
                //set the Value  for the FileInfoDict dictionary
                currentFileInfoValue = currentFileSizeStr + "~" + lastFileModifiedDateStr;
                //Update the FileInfoDict dictionary
                FileInfoDict.Add(currentFileInfoKey, currentFileInfoValue);


            }// end foreach

            /*  At this point the following are defined
             *      DirNamesDict = new Dictionary<string, string>();
                    FileNamesDict = new Dictionary<string, string>();
                    FileInfoDict = new Dictionary<string,string>();
             * 
             */
            int i = 1;
        }//end public static void ProcessInputFil

        /* The purpose of the CreateOutputDirectories() is to:
         *  1. scan the Input text file AppProperties.FilePathsPath line by line
         *  2. from each line get the directory path 
         * 
         */


        private static void CreateOutputDirectories()
        {

        }// end CreateOutputDirectories()
    }//end class
}//end namespace
