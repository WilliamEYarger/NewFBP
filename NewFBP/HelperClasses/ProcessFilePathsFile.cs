using System;
using System.Collections.Generic;
using System.IO;
using NewFBP.HelperClasses;
namespace NewFBP.HelperClasses
{
    public static class ProcessFilePathsFile
    {
        public static Dictionary<string, string> DirNamesDict { get; private set; }
        public static Dictionary<string, string> FileNamesDict { get; private set; }
        private static string RootPath;
        private static int DirCntr;
        private static int FileCntr;
        
        public static void ProcessInputFile(string inputFilePath)
        {
            DirNamesDict = new Dictionary<string, string>();
            FileNamesDict = new Dictionary<string, string>();
            DirCntr = 0;
            FileCntr = 0;

            
            string[] lines = File.ReadAllLines(inputFilePath);
            string root = DataModels.AppProperties.SourceDirectory;// Religion\
            string firstLine = lines[0];//"C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion\\Articles List.docx"
            string fileName = Path.GetFileName(firstLine);// "Articles List.docx"
            string rootDirectoryName = DataModels.AppProperties.SourceDirectory;// "Religion\\"
            //first line contains both thefile nameand the root directory religion\

            // remove the fileName
            firstLine = firstLine.Replace(fileName, "");

            // remove the rootDirectoryName
            string rootDirectory = firstLine.Replace(rootDirectoryName, "");//Contains the path up to but excluding the root directory
            DataModels.AppProperties.RootDirectory = rootDirectory;

            //AT THIS POINT I SHOULD BE ABLE TO PROCESS lines TO CREATE THE DICTIONARYIES


        }//end public static void ProcessInputFil
    }//end class
}//end namespace
