using System.IO;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;


namespace NewFBP.HelperClasses
{



    public static class GetfileNamesAndPaths
    {
        #region properties

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
        //  CHANGE INSTRUCTIONS creat a list 'DictIdB26NameList' of the above first so i can add to it as I iterate through the various files
        /*Create a List and arrray of strings B26NameArr that will hold all of the B26 file names in seriatem order */
        private static List<string> B26NameList = new List<string>();
        private static string[] B26NameArr = new string [0];

        /* Create a List and arrrayof strings FileLengthsArr that will hold all of the file lengths*/
        private static List<string> FileLengthsList = new List<string>();
        private static string[] FileLengthsArr = new string [0];

        /* Create a List and arrray of strings FilePathsArr that will hold all of the file complete file paths*/

        private static List<string> FilePathsList = new List<string>();
        private static string[] FilePathsArr = new string [0];
        private static string currentFilesPath = "";
        private static Dictionary<string, string> currentDirNamesDict = new Dictionary<string, string>();
        private static Dictionary<string, string> oldFileNamesDict = new Dictionary<string, string>();
        #endregion properties


        public static void ProcessSourceFiles()
        {

            //get the current value of the dirCntr and fileCntr if they exist
            dirCntr = DataModels.AppProperties.DirCntr;
            fileCntr = DataModels.AppProperties.FileCntr;

            CreateCombinrDirList();

            CreateListOfAllFiles();


            //Create private static void CreateDirNamesDict() {}
            CreateDirNamesDict();

            //Create private static voide Create B26NameList() {}
            CreateB26NameList();

            //Create private static void CreateFileNamesDict() {}
            CreateFileNamesDict();
        }// end ProcdessSourceFiles
            


        /*
         * to create the B26NamesList I need to cycle through the DataModels.AppProperties.ListOfAllFilePaths = listOfAllFilePaths;   
         */
        private static void CreateB26NameList() 
        {

            string dirIDplusFileName;
            string currentB26FileName =  string.Empty;
            //create a local fileCntr
            int currentFileCntr = DataModels.AppProperties.FileCntr;

           
            //create local dictionaries
            Dictionary<string, string> currnetDirDictNames = DataModels.AppProperties.DirNamesDict;
            
            //create local Lists
            List<string> currentListOfAllFilePaths = DataModels.AppProperties.ListOfAllFilePaths;

            List<string> currentFileNamesList = new List<string>();

            //check to see if  DataModels.AppProperties.FileNamesList is null
            if (DataModels.AppProperties.FileNamesList != null)
            {
                currentFileNamesList = DataModels.AppProperties.FileNamesList;
            }
            List<string> currentDirPlusFileNamesList = DataModels.AppProperties.DirPlusFileNamesList;

            List<string> currentB26NamesList = new List<string>();
            //check to see if  DataModels.AppProperties.currentB26NamesList is null

            if (DataModels.AppProperties.B26FileNames != null)
            {
                currentB26NamesList = DataModels.AppProperties.B26FileNames;
            }
            

            //create local strings
            string shortFileName;//Articles.docx
            string shortDirName;// Relibion\
            string dirIntName; //0
            string root = DataModels.AppProperties.RootDirectory;
            string currntFilePath;
            string currentFileName;
           //string test;

            //create local string arrays
            // create a local varialbe to hold the simple file name
            string[] currentListOfAllFilePathsArr = DataModels.AppProperties.ListOfAllFilePaths.ToArray();

            /* to create a list of B26Names I need the abbreviated file name which is composed of its
             * folders int name + the individual file name eg. for I can get from DirNamesDict
             * I also need an abbreviated file name which I can get by using path in the ListFoldersAndFiles( 
             */
            //iterate thru currentListOfAllFilePathsArr
            for (int i = 0; i < currentListOfAllFilePathsArr.Length; i++)
            {
                
                currntFilePath = currentListOfAllFilePathsArr[i];//"C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion\\Articles List.docx"
                currentFileName = Path.GetFileName(currntFilePath);//"Articles List.docx"


                //if currentFileNamesList doesn't contain currentFileName then add it



                if ( !currentFileNamesList.Contains(currentFileName)) 
                { 
                    currentFileNamesList.Add(currentFileName);
                    //create a new B26FileName
                    currentB26FileName = HelperClasses.StringHelper.ConvertToBase26(currentFileCntr);
                    currentFileCntr++;
                    currentB26NamesList.Add(currentB26FileName);
                }
                
                shortDirName = currntFilePath.Replace(currentFileName, "");
                shortDirName = shortDirName.Replace(root, "");
                dirIntName = currnetDirDictNames[shortDirName];
                dirIDplusFileName = dirIntName + '.' + currentFileName;
                //update currnetDirDictNames if it doesn not contain Key dirIDplusFileName

//STARTHERE
//                /*THE FOLLOWING IS IN ERROR BECAUSE I AM TRYING TO ADD A NEW ENTRY
//                 * currnetDirDictNames INSTEAD I SHOULD BE ADDING IT TO FileFetchDict, 
//                 * a dictionary whose key if the full path to a file and whose value is Base26File# 
//                 * {FilePath,Base26File#} WHICH I HAVENT CREATED YET */
//                if (!currnetDirDictNames.ContainsKey(dirIDplusFileName))

//                {
//                    //z = currnetDirDictNames.Count;
//                    //test = currentB26FileName;
//                    currnetDirDictNames.Add(dirIDplusFileName,  currentB26FileName);

//                }

            }

            //update FileNamesList
            DataModels.AppProperties.FileNamesList = currentFileNamesList;

            // Update FileNamesList
            DataModels.AppProperties.FileNamesList = currentFileNamesList;

            //return the updated fileCntr
            DataModels.AppProperties.FileCntr = currentFileCntr;
        }

        //create a dictionary that holds (?,}
        private static void CreateFileNamesDict() 
        { 
        }

        // create a dictionary that holds {?,?]
        private static void CreateDirNamesDict()
        {
            //set currentDirNamesDict to old DirNamesDict if it exists
            Dictionary<string, string> currentDirNamesDict = DataModels.AppProperties.DirNamesDict;

            //Get the old DirNamesDict if it exists
            GetfileNamesAndPaths.currentDirNamesDict = DataModels.AppProperties.DirNamesDict;

            //Set the boolean value oldDirNamesDictExists
            if (GetfileNamesAndPaths.currentDirNamesDict != null) { oldDirNamesDictExists = true; }

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

            //iterate thru combinedDirArr and if a new directory esists get its name fron the
            //current value of dirCntr and add it to DirNamesDict
            string[] combinedDirArr = DataModels.AppProperties.CombinedDirList.ToArray();
            for (int i = 0; i < combinedDirArr.Length; i++)
            {
                string old = combinedDirArr[i];
                string newname = old.Replace(root, "");
                newname = newname + '\\';
                //see if oldDirNamesDictExists exists t
                if (oldDirNamesDictExists)
                {
                    //see if it contains a Key =newname
                    if (!GetfileNamesAndPaths.currentDirNamesDict.ContainsKey(newname))
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
            DataModels.AppProperties.DirNamesDict = DirNamesDict;
            DataModels.AppProperties.CurrentFilesPath = currentFilesPath;
        }

        //Create a list that contains the paths to all of the files in the root directory and its subdirectories
        private static void CreateListOfAllFiles() 
        {
            //create a string array of combinedDirList so that you can interate thru it to create DirNamesDict
            List<string> combinedDirList = DataModels.AppProperties.CombinedDirList;
            string[] combinedDirArr = combinedDirList.ToArray();

            //Create a list that contains all files using combinedDirArr
            List<string> listOfAllFilePaths = new List<string>();
            string currentDirectory;
            List<string> allFilePathsList = new List<string>();
            List<string> currentDirectoriesFilesList = new List<string>();

            int arrayLength = combinedDirArr.Length;

            for (int i = 0; i < arrayLength; i++)
            {
                currentDirectory = combinedDirArr[i];

                currentDirectoriesFilesList = Directory.GetFiles(currentDirectory).ToList();
                allFilePathsList.AddRange(currentDirectoriesFilesList);
            }

            listOfAllFilePaths = allFilePathsList;
            DataModels.AppProperties.ListOfAllFilePaths = listOfAllFilePaths;


            List<string> listOfShortFileNames = new List<string>();
            string shortFileName;
            //iterate thru listOfAllFilePaths and create a list of shortFileNames and if 
            foreach (string file in listOfAllFilePaths)
            {
                shortFileName = Path.GetFileName(file);
                listOfShortFileNames.Add(shortFileName);
            }

            DataModels.AppProperties.ListOfShortFileNames = listOfShortFileNames;
        }

        //CreateCombinrDirList contains the paths to all of the directoried in the root incouding the root
        private static void CreateCombinrDirList() 
        {
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

            DataModels.AppProperties.CombinedDirList = combinedDirList;
        }

        #region ListFoldersAndFiles
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
        }// end ListFoldersAnd Files
        #endregion ListFoldersAndFiles

        #region GetFileVariables
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
        }//end method private static string[] GetFileVariables(string filePath)
        #endregion GetFileVariables

    }//end public static class GetfileNamesAndPaths

}// end namespace