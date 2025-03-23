using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NewFBP.DataModels
{
    public static class AppProperties
    {

        //TOC LINE
        // strings = #region strings
        // dictionaries = #region Dictionaries
        // Integers = #region Integers
        // Lists = #region Lists

        #region strings
        public static string CurrentSourceName { get; set; }//The name of the directory selected i.e. "Religion"

        public static string CurrentSourcePath { get; set; }//the complete path to the directory selected for backup 
        // i.e. "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion"

        public static string FileNamesPath { get; set; }// the name of the file that holds the FileNames

        public static string FilePathsPath { get; set; }// the name of the file that holds the file paths

        public static string StoragePath { get; set; }//;//the folder where the Current File Data Should be store

        public static string SourceDirectory { get; set; }//the name of the source directory i.e. "Religion\\"
                                                          //so that the root can be extracted Religion\\
    
        /*
         * The SourcerootDirectory in the CurrentSourcePath - SourceDirectory
         * It is used to create the entries in the DirNamesDict
         * i.e. "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\"
         */
        public static string SourceRootDirectory{ get; set; }


        /* the root directory is that portion of CurrentSourcePath which is removed 
         * i.e. ("C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\) so that the directory
         * names in the source can be compared with the directory names in the output Dictionary of Directories
         */
        public static string  RootDirectory { get; set; }

        /* the SourceBackupDirPath is to path the folder where the source backup data will be stored
         * 
         */
        public static string  SourceBackupDirPath { get; set; }

        public static string CurrentFilesPath { get; set; }
        #endregion strings

        #region Dictionaries

        //Religion\,0
       // public static Dictionary<string, string> DirNamesDict { get; set; }

        //OldDirNamesDict
        // the OldDirNamesDict will be stored in the SpourceBackup folder with as Key+'~'+Value
        public static Dictionary<string,string> DirNamesDict { get; set; }

        //OldFileNamesDict
        public static Dictionary<string, string> OldFileNamesDict { get; set; }

        //OldFileInfoDict
        /*
         * FileFetchDict, a dictionary whose Key if the full path to a file and whose Value is Base26File# 
         * {FilePath,Base26File#} eg. {"C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx",AAA) 
         * this will be used to get the file to be copied to the Repository Backup folder and the Value (the Base26File#) 
         * will be used to search the CurrentVersionDict to the the current version number so it can be incemented and 
         * applied to the Base26File as the name of the current version of the file in the Repository Backup folder. 
         * eg. the file named AAA.0 will contain the first version of the file 
         * "C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx"
         */
        public static Dictionary<string, string> OldFileInfoDict { get; set; }

        /*
            OldFileFetchDict, a dictionary whose key if the full path to a file and whose value is Base26File# 
            [FilePath,Base26File#} eg. {"C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx",AAA) 
            this will be used to get the file to be copied to the Repository Backup folder and the Value (the Base26File#)
            will be used to search the CurrentVersionDict to the the current version number so it can be incemented and 
            applied to the Base26File as the name of the current version of the file in the Repository Backup folder. 
            eg. the file named AAA.0 will contain the first version of the file
            "C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx"
         */
        public static Dictionary<string, string> OldFileFetchDict { get; internal set; }


        public static Dictionary<string, string> OldCurrentVersionDict { get; internal set; }
        #endregion Dictionaries

        #region Integers
        //Integers

        public static int DirCntr { get; set; }

        public static int FileCntr { get; set; }

        #endregion integers

        #region Lists

        //creat a list that holds B26FileNames
        public static List<string> B26FileNames { get; set; }
        //create a list that contines DirName.FileName "0.Religion\"
        public static List<string> DirPlusFileNamesList { get; set; }

        // create a list that contains the simple file names
        public static List<string> FileNamesList { get; set; }

        //Create a list that contains the root and all of its subdirectories
        public static List<string> CombinedDirList { get; set; }

        //Create a list that contains the paths to all of the files in the root directory and its subdirectories
        public static List<string> ListOfAllFilePaths { get; set; }

        //create a list of short file name
        public static List<string> ListOfShortFileNames { get; set; }

        public static List<string> ListOfAll26Names { get; set; }

        #endregion Lists
    }//end 

}// End namespace
