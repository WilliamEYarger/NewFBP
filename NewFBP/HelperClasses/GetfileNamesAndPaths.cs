using System.IO;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;


namespace NewFBP.HelperClasses
{



    public static class GetfileNamesAndPaths


    {
        //Lists
        private static List<string> FolderPathList = new List<string>();
        private static List<string> FileNameList = new List<string>();
        private static List<string> FilePathList = new List<string>();

        //bools
        private static bool FirstDirectoryIsFileless = false;
        private static bool oldDirNamesDictExists = false;

        private static string[] currentArrOfDirectories = new string[0];
        private static string[] AllDirectories = new string[0];
        private static string[] allOtherDirectories = new string[0];

        //integers
        private static int dirCntr = 0;
        private static int fileCntr = 0;

        //Dictionaries
        private static Dictionary<string, string> DirNamesDict = new Dictionary<string, string>();

        /*
         FileNamesDict a text file holding a compressed file path ([DirID.FileName]) and the file's Base26File# 
             *          eg {0.Articles List.docx,AAA}
         */
        private static Dictionary<string, string> FileNamesDict = new Dictionary<string, string>();

        /* FileInfoDict a text file holding its Base26File# unique file name as the Key and the file's current 
           Length as the value [Base26File#,fileLength]. eg. {ACR, 81351} [Base26File#,Length]*/
        private static Dictionary<string, string> FileInfoDict = new Dictionary<string, string>();


        /* Create an arrray of strings DitIdB26Name that will hold all of the DirId.FileName(B26) in the
         * sane order as the files in the FileNamesDict*/
        private static string[] DictIdB26NameArr = new string [0];

        /*Create a List and arrray of strings B26NameArr that will hold all of the B26 file names in seriatem order */
        private static List<string> B26NameList = new List<string>();
        private static string[] B26NameArr = new string [0];

        /* Create a List and arrrayof strings FileLengthsArr that will hold all of the file lengths*/
        private static List<string> FileLengthsList = new List<string>();
        private static string[] FileLengthsArr = new string [0];

        /* Create a List and arrray of strings FilePathsArr that will hold all of the file complete file paths*/

        private static List<string> FilePathsList = new List<string>();
        private static string[] FilePathsArr = new string [0];


        private static Dictionary<string, string> oldDirNamesDict = new Dictionary<string, string>();
        private static Dictionary<string, string> oldFileNamesDict = new Dictionary<string, string>();

        public static void ProcessSourceFiles()
        {
            #region Set up local variables
            #region general variables
            //get the current value of the dirCntr and fileCntr if they exist
            dirCntr = DataModels.AppProperties.DirCntr;
            fileCntr = DataModels.AppProperties.FileCntr;

            //Get the path of the current source folder
            string rootFolder = DataModels.AppProperties.CurrentSourcePath;

            // create a temp list so that it can be concatinated into a combined list add the root folder to it
            List<string> RootDirectory = new List<string>();
            RootDirectory.Add(rootFolder);

            //Get all of the subdirectories of the root
            string[] allOtherDirectories = Directory.GetDirectories(rootFolder, "*", SearchOption.AllDirectories);

            //create a list containing all of the subdirectories of the root so it can be concatinated with RootDirectory
            List<string> AllDirectoriesList = new List<string>();
            AllDirectoriesList = allOtherDirectories.ToList();

            //Create a list that contains the root and all of its subdirectories
            List<string> combinedDirList = RootDirectory.Concat(AllDirectoriesList).ToList();

            //create a string array of combinedDirList so that you can interate thru it to create DirNamesDict
            string[] combinedDirArr = combinedDirList.ToArray();

            //Get the old DirNamesDict if it exists
            oldDirNamesDict = DataModels.AppProperties.OldDirNamesDict;

            //Set the boolean value oldDirNamesDictExists
            if (oldDirNamesDict != null) { oldDirNamesDictExists = true; }

            //Set the FileInfoDictionary
            if (DataModels.AppProperties.OldFileInfoDict != null)
            {
                FileInfoDict = DataModels.AppProperties.OldFileInfoDict;
            }
            else
            {
                FileInfoDict = new Dictionary<string, string>();
            }
            //get the source root directour
            string root = DataModels.AppProperties.RootDirectory;
            #endregion general variables

            #region local variables related to files

            //Local variables
            string currentFilesPath = "";
            string currentFileName = "";
            string currentFileAbbreviatdDirectoryName = "";//the directory left after removing the root and the FileNmae
            string currentDirectoryIntName = "";//the DirID from DirNamesDict eg. Religion\\ = 0
            string currentFileLengthStr = "";//the length of the file
            string currentFileBase26Name = "";
            string currentVersionNum = "";
            string rootDirectory = DataModels.AppProperties.RootDirectory;
            string FileNameDictKey = "";
            string FileNameDictValue = "";


            #endregion general variables

            #endregion  Set up local variables

            #region Create DirNamesDict

            //iterate thru combinedDirArr and if a new directory esists get its name fron the
            //current value of dirCntr and add it to DirNamesDict
            for (int i = 0; i < combinedDirArr.Length; i++)
            {
                string old = combinedDirArr[i];
                string newname = old.Replace(root, "");
                newname = newname + '\\';
                //see if oldDirNamesDictExists exists t
                if (oldDirNamesDictExists)
                {
                    //see if it contains a Key =newname
                    if (!oldDirNamesDict.ContainsKey(newname))
                    {
                        //the old directory doesn't contain this directory name so add it
                        string newDirValueStr = dirCntr.ToString();
                        DirNamesDict.Add(newname, newDirValueStr);
                        dirCntr++;
                    }
                }
                else
                {
                    combinedDirArr[i] = newname;
                    DirNamesDict.Add(newname, i.ToString());
                }
            }//end for (int i = 0; i<combinedDirArr.Length; i++)

           // string Stop = "";
            #endregion Create DirNamesDict


            
            #region Create File related Dictionaries

            /* to process the files in the selected directory
             * 1.   I will need the following dictionaries
             *      a.  FileNamesDict a text file holding a compressed file path (composed of its directory's 
             *          Key Value+the Nameof the file [DirID.FileName]) and the file's Base26File# 
             *          eg {0.Articles List.docx,AAA}
             *      b.  FileInfoDict a text file holding itsBase26File# unique file name as the Key and the file's current 
             *          Length as the value [Base26File#,fileLength]. eg. {ACR, 81351} [Base26File#,Length]
             *      c.  FileFetchDict, a dictionary whose key if the full path to a file and whose value is Base26File# 
             *          {FilePath,Base26File#} eg. {"C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx",
             *          AAA) this will be used to get the file to be copied to the Repository Backup folder and the Value 
             *          (the Base26File#) will be used to search the CurrentVersionDict to the the current version number 
             *          so it can be incemented and applied to the Base26File as the name of the current version
             *          of the file in the Repository Backup folder. eg. the file named AAA.0 will contain the first version 
             *          of the file "C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx"
             *      d.  CurrentVersionDict, a dictionary whose Key is its Base26File and whose value if the most current 
             *          version number, with all starting at 0. eg. the original value of file
             *          "C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx" will be {AAA,0}
             * 2.   I will need to create properties for all of these files as well a methods for retrieving their
             *      current values form the SourceBackup folder;
             *      a.  FileNamesDict       DONE
             *      b.  FileInfoDict        DONE
             *      c.  FileFetchDict       NOT DONE
             *      d.  CurrentVersionDict  NOT DONE
             */


            // Get all the files in these folders

            #region Create FileNamesDict 

            if (DataModels.AppProperties.OldFileNamesDict != null)
            { FileNamesDict = DataModels.AppProperties.OldFileNamesDict; }
            else
            { FileNamesDict = new Dictionary<string, string>(); }


            foreach (string s in combinedDirList)
            {

                if (!string.IsNullOrEmpty(s))
                {
                    //Get  the file paths of all the files in the current 's' directory
                    //   and palce them in a string array filePathsArr
                    string[] filePathsArr = Directory.GetFiles(s);

                    //convert filePathsArr to  filePathsList
                    List<string> filePathsList = filePathsArr.ToList();

                    //iterate through the list of all of the file paths in the source folder
                    foreach (string currentFilePath in filePathsList)
                    {                        
                        //save the path to the current file so it can be fetched for processing
                        currentFilesPath = currentFilePath;
                        //get the file variables for this file path
                        string[] fileVariables = GetFileVariables(currentFilePath);
                        currentFileName = fileVariables[0];
                        currentFileAbbreviatdDirectoryName = fileVariables[1];

                        // get currentDirectoryIntName
                        //test to see if the  DirNamesDict contains currentFileAbbreviatdDirectoryName
                        //if it does then get if value, its file number converted to a string
                        if (DirNamesDict.ContainsKey(currentFileAbbreviatdDirectoryName))
                        {
                            currentDirectoryIntName = DirNamesDict[currentFileAbbreviatdDirectoryName];
                        }
                        else
                        {
                            MessageBox.Show("The directory " + currentFileAbbreviatdDirectoryName + " coesn't exist");
                        }//end test to see if the  DirNamesDict contains currentFileAbbreviatdDirectoryName

                        // get currentFileLengthStr
                        FileInfo fileInfo = new FileInfo(currentFilePath);

                        //long fileSize = fileInfo.Length; // Size in bytes 
                        currentFileLengthStr = fileInfo.Length.ToString();

                        // create the Key value of the FileNameDict, DirNameInt.currentFileName
                        FileNameDictKey = currentDirectoryIntName + '.' + currentFileName;


                        //I AM TEMPORARLY COMMENTING THIS SECTION TO SEE IF IT IS NEEDED WITH REVISIONS WHEN I TRY TO PROCESS A NEW
                        //SOURCE DIRECTORY THAT HAS A FILE ADDED IN AN OLD DIRECTORY THAT WAS NOT PRESENT WHEN THE PROGRAM WAS LAST RUN
                        // if the FileNameDict  contains this key then get its value else creat a new entry
                        //with with key and the current value of FileCntr converted to Base26
                        if (FileNamesDict.ContainsKey(FileNameDictKey))
                        {
                            FileNameDictValue = FileNamesDict[FileNameDictKey];
                        }
                        else
                        {
                            //get FileCntr and covert it to Base26 to createFileNameDictKey
                            FileNameDictValue = HelperClasses.StringHelper.ConvertToBase26(fileCntr);

                            // Add the Key Value pair FileNameDictKey and FileNameDictValue to FileNamesDict
                            FileNamesDict.Add(FileNameDictKey, FileNameDictValue);

                            //add the FileNameDictValue to the B26NameList
                            B26NameList.Add(FileNameDictValue);
                            fileCntr++;
                        }

                    }//end  foreach(string currentFilePath in filePathsList)

                }//end if (!string.IsNullOrEmpty

            }//end foreadch (string s


           // string Stop = "";

            //The items in FileNamesDict have FolderId (an int to string)+'.'+FileName as a Key
            //The items in FileNamesDict have FileBase26 name as a Value

            //CREATE DirB26NameArr from the values in 
            string[] DirB26NameArr = FileNamesDict.Keys.ToArray();

            //CREATE B26NameArr
            string[] B26NameArr = FileNamesDict.Values.ToArray();

            string Stop = "";
            #endregion Create FileNamesDict

            //string stop = "Stop here";


            #region Create FileInfoDict
            /*FileInfoDict a dictoionary whose Key is its its Base26File# unique file name 
             * and whose Value is its file length 
             Length as the value [Base26File#,fileLength]. eg. {ACR, 81351} [Base26File#,Length]*/

            // Create index string arrays of combinedDirList and FileNamesDict
            string[] combinedDirListArray = combinedDirList.ToArray();


           // var keyValueList = new List<KeyValuePair<string, int>>(FileNamesDict);

            /*I NEED SOMEWAY TO HAVE ARRAYS OF STIRNGS THAT CONTAIN THE FILEPATHS OF ALL FILES , THIR BASE26 NAME 
             * AND THEIR DIRECTORY'S ABBREVIATED NAME*/




            foreach (string filePath in combinedDirList)
            {
                ////get this file's Base26File#
                //Create a private Method to get the current file Paths various variabes
                //string fileNameB26 = HelperClasses.StringHelper.ConvertToBase26(fileCntr);
                //fileCntr++;


            }


            //string z = "Stop Here";
            #endregion FileInfoDict

            //Create FileNamesDict


        }//end processfiles

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
                //only process if FolderPathArray in not null or empty
                if (!(FolderPathArray == null || FolderPathArray.Length == 0))
                    foreach (string directory in FolderPathArray)
                    {
                        FolderPathList.Add(directory);
                    }
                else { FirstDirectoryIsFileless = true; }

                //Display file names and paths
                foreach (string file in FilePathArray)
                {
                    string fileName = Path.GetFileName(file);
                    FilePathList.Add(file);
                    FileNameList.Add(fileName);//delet
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

                //HelperClasses.ProcessFilePathsFile.ProcessInputFile(pathsSourceFile);
            }
            #endregion Create File related Dictionaries
        }// end ListFoldersAnd Files


        private static string[] GetFileVariables(string filePath)
        {
            string[] FileVariables = new string[2];

            //Get the name of the current file
            string currentFileName = Path.GetFileName(filePath);
            FileVariables[0] = currentFileName;

            // get the abbreviated directory name
            string currentFilePath;
            filePath.Replace(currentFileName, "");
            string currentFileAbbreviatdDirectoryName;
            currentFileAbbreviatdDirectoryName = filePath.Replace(DataModels.AppProperties.RootDirectory, "");
            currentFileAbbreviatdDirectoryName = currentFileAbbreviatdDirectoryName.Replace(currentFileName, "");
            FileVariables[1] = currentFileAbbreviatdDirectoryName;


            return FileVariables;
        }//end namespace


    }//end public static class GetfileNamesAndPaths



}// end namespace