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

        #region bools

        public static bool FirstRun { get; set; } = false;

        #endregion bools

        #region arrays
        /*
         ShortdDirNamesArr is a string array that holds all of the short directory names (the 
        value obtained after removing the root directory path from the path the the current directory 
        and includes its final '\\'
         */
        public static string[] ShortdDirNamesArr { get; set; }//holds the sohrt dir names eg Religion/
        #endregion ShortdDirNamesArr

        #region strings

        /*
         The Repository path holds the path to the repository files
         */
        public static string RepostioryPath { get; set; }


        public static string CurrentSourceName { get; set; }//The name of the directory selected i.e. "Religion"
        /*CurrentSourcePath 
         * /the complete path to the directory selected for backup 
         * i.e. "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion"
         */
        public static string CurrentSourcePath { get; set; }

        /*MAY NOT BE NEDED
         */
        public static string FileNamesPath { get; set; }// the name of the file that holds the FileNames

        public static string FilePathsPath { get; set; }// the name of the file that holds the file paths

        public static string StoragePath { get; set; }//;//the folder where the Current File Data Should be store

        public static string SourceDirectory { get; set; }//the name of the source directory i.e. "Religion\\"
                                                          //so that the root can be extracted Religion\\
    
        /*
         * The SourcerootDirectory in the CurrentSourcePath - SourceDirectory
         * It is used to create the entries in the DirIDNamesDict
         * i.e. "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\"
         */
        public static string SourceRootDirectory{ get; set; }


        /* the root directory is that portion of CurrentSourcePath which is removed 
         * i.e. ("C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\) so that the directory
         * names in the source can be compared with the directory names in the output Dictionary of Directories
         */
        public static string  RootDirectory { get; set; }

        /* the SourceBackupDirPath is to path the folder where the source backup data will be store
         * it includes the terminal \\
         * 
         */
        public static string  SourceBackupDirPath { get; set; }


        #endregion strings

        /*CurrentCntrValues
         * this is a single strin that contains the DirCntr to sting + '~' +FileCntr
         */
        public static string CurrentCntrValues { get; set; }


        #region Dictionaries

        //Religion\,0
        // public static Dictionary<string, string> DirIDNamesDict { get; set; }

        /* the DirNamesDict contians a series of Key Value pairs where 
         * the Key is a a Directory name ie Religion\ to
         * Religion\ReligiousStudies\Christianity\RR Reformation\RR Reformation Study Folder\
         * and the Va;lues are that Directories int ID num "0" ... "365" ect
         * */

        public static Dictionary<string,string> DirIDNamesDict { get; set; }


        //FileNamesDict
        public static Dictionary<string, string> FileNamesDict { get; set; }


        /*
         FileLengthDict
        the Key is the B26Name and the value is the file length
         */

        public static Dictionary<string, string> FileLengthDict { get; set; }

        /*
            FileFetchDict, a dictionary whose key if the full path to a file and whose value is Base26File# 
            [FilePath,Base26File#} eg. {"C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx",AAA) 
            this will be used to get the file to be copied to the Repository Backup folder and the Value (the Base26File#)
            will be used to search the CurrentVersionDict to the the current version number so it can be incemented and 
            applied to the Base26File as the name of the current version of the file in the Repository Backup folder. 
            eg. the file named AAA.0 will contain the first version of the file
            "C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx"
         */
        public static Dictionary<string, string> FileFetchDict { get; internal set; }


        public static Dictionary<string, string> OldCurrentVersionDict { get; internal set; }
        /*FileVersionDict
         * The FileVersionDict Key is the abbreviated file name which consists of a file’s DirID name 
         * found in the  DirIDNamesDict   + ‘.’ + its B26 file name found in the B26FileNamesList.  
         * Its Value is its current version number, which on startup will be ‘0’.         
         */
        public static Dictionary<string,string> FileVersionDict { get; set; }
        #endregion Dictionaries

        #region Integers
        //Integers

        public static int DirCntr { get; set; }

        public static int FileCntr { get; set; }

        #endregion integers

        #region Lists

        //CHANGES 20250412
        //Create a new List<string> LogOfDirsAndFilesList in AppProperties

        public static List<string> LogOfDirsAndFilesList { get; set; } = new List<string>();

        //END CHANGES 20250412

        //creat a list that holds B26FileNamesList
        public static List<string> B26FileNamesList { get; set; }
        //create a list that contines DirName.FileName "0.Religion\"
        public static List<string> DirPlusFileNamesList { get; set; } //NOT DEFINED

        // create a list that contains the simple file names
        public static List<string> FileNamesList { get; set; }

        
        /*CombinedDirPathList is obtained by concatinating the RootDirectory and the allDirectoriesList to
          Create a list (combinedDirList)that contains the root and all of its subdirectories
        "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion" to
        "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion\\Christianity\\Writings\\New Testament\\Books of the New Testament\\QA Files for books of the New Testament\\QAResults"
         */
        public static List<string> CombinedDirPathList { get; set; }

        /*ListOfAllFilePaths is a list that contains the paths to all of the files 
         * in the root directory and its subdirectories*/
        public static List<string> ListOfAllFilePaths { get; set; }

        //create a list of short file name
        public static List<string> ListOfShortFileNames { get; set; }

        public static List<string> ListOfAll26Names { get; set; }//NOT DEFINED

        #endregion Lists
    }//end 

}// End namespace
