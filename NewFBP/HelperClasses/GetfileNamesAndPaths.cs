using System.IO;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using System.Diagnostics.Eventing.Reader;


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
        private static bool oldDirIDNamesDictExists = false;

        private static string[] currentArrOfDirectories = new string[0];
        private static string[] AllDirectories = new string[0];
        private static string[] allOtherDirectories = new string[0];

        //integers
        private static int currentDirCntr = 0;
        private static int currentFileCntr = 0;

        //Dictionaries
        private static Dictionary<string, string> DirNamesDict = new Dictionary<string, string>();

        /*
         FileNamesDict a text file holding a compressed file path ([DirID.FileName]) and the file's Base26File# 
             *          eg {0.Articles List.docx,AAA}
         */
        private static Dictionary<string, string> FileNamesDict = new Dictionary<string, string>();

        /* currentFileInfoDict a text file holding its Base26File# unique file name as the Key and the file's current 
           Length as the value [Base26File#,fileLength]. eg. {ACR, 81351} [Base26File#,Length]
            I am creating it here as a local so that later I can test to see if a global dictionary exists
            before assiging values*/
        private static Dictionary<string, string> currentFileInfoDict = new Dictionary<string, string>();


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
        /* Create a Dictionary<stirng,string>   whose Key is the  DictIdB26Name and whose Value is the current
         VersionNumStr*/

        /* Create a List and arrray of strings FilePathsArr that will hold all of the file complete file paths*/

        private static List<string> FilePathsList = new List<string>();
        private static string[] FilePathsArr = new string [0];
        private static string currentFilesPath = "";
        private static Dictionary<string, string> currentDirIDNamesDict = new Dictionary<string, string>();


        #endregion properties


        public static void ProcessSourceFiles()
        {

            //get the current value of the currentDirCntr and currentFileCntr if they exist
            currentDirCntr = DataModels.AppProperties.DirCntr;
            currentFileCntr = DataModels.AppProperties.FileCntr;

            CreateCombinrDirList();//DONE

            CreateShortDirNamesList(); //DONE

            CreateListOfAllFiles();//DONE


            //Create private static void CreateDirNamesDict() {}
            CreateDirNamesDict();//DONE

            //Create private static voide Create B26NameList() {}
            CreateB26NameList();//DONE

            //Create DirDictNames
            CreateDirIDNamesDict();//DONE

            CreateFileNamesDict();
        }// end ProcdessSourceFiles
            


        /*
         * to create the B26NamesList I need to cycle through the DataModels.AppProperties.ListOfAllFilePaths = 
         * listOfAllFilePaths;   
         */
        private static void CreateB26NameList() 
        {
            /* The golbal properties I need at this point are
             * DataModels.AppProperties.FileCntr;
             * DataModels.AppProperties.DirIDNamesDict
             * DataModels.AppProperties.ListOfAllFilePaths
             * DataModels.AppProperties.DirPlusFileNamesList
             * DataModels.AppProperties.B26FileName            * 
             * 
             */

            //SET local variables to Global properties if they exist and if not set booleans
            //create a local currentFileCntr
            int currentFileCntr = DataModels.AppProperties.FileCntr;

            string dirIDplusFileName;
            string currentB26FileName =  string.Empty;

            //local booleans
            bool boolFileNamesListExists = true;
            bool boolB26FileNamesExists = true;
            bool boolDirPlusFileNamesListExists = true;


            //create local Dictionaries
            Dictionary<string, string> currneDirIDNamesDict = DataModels.AppProperties.DirIDNamesDict;          

            //create local Lists
            List<string> currentListOfAllFilePaths = DataModels.AppProperties.ListOfAllFilePaths;            

            //if B26FileNamesList exists create currentB26NamesList else Set boolB26FileNamesExists to false
            List<string> currentB26NamesList = new List<string>();
            if (DataModels.AppProperties.B26FileNamesList != null)
            { currentB26NamesList = DataModels.AppProperties.B26FileNamesList;}
            else 
            { boolB26FileNamesExists = false; }

            // if FileNamesList exists create currentFileNamesList else SET boolFileNamesListExists to false
            List<string> currentFileNamesList = new List<string>();
            currentFileNamesList = DataModels.AppProperties.FileNamesList;
            //check to see if  DataModels.AppProperties.FileNamesList is null
            if (DataModels.AppProperties.FileNamesList != null)
            { currentFileNamesList = DataModels.AppProperties.FileNamesList; }
            else { boolFileNamesListExists = false; }

            // if DirPlusFileNamesList exists create currentDirPlusFileNamesList else SET boolDirPlusFileNamesListExists to false
            List<string> currentDirPlusFileNamesList = new List<string>();
             if (DataModels.AppProperties.DirPlusFileNamesList!=null)
                { currentDirPlusFileNamesList = DataModels.AppProperties.DirPlusFileNamesList; }
             else { boolDirPlusFileNamesListExists = false; }
            
            //create local strings
            string shortFileName;//Articles.docx
            string shortDirName;// Relibion\
            string dirIntName; //0
            string root = DataModels.AppProperties.RootDirectory;
            string currntFilePath;
            string currentFileName;

            //create local string arrays
            // create a local varialbe to hold the simple file name
            string[] currentListOfAllFilePathsArr = DataModels.AppProperties.ListOfAllFilePaths.ToArray();

            /* to create a list of B26Names I need the abbreviated file name which is composed of its
             * folders int name + the individual file name eg. for I can get from DirIDNamesDict
             * I also need an abbreviated file name which I can get by using path in the ListFoldersAndFiles( 
             */
            //iterate thru currentListOfAllFilePathsArr to create currentB26NamesList
            for (int i = 0; i < currentListOfAllFilePathsArr.Length; i++)
            {                
                currntFilePath = currentListOfAllFilePathsArr[i];//"C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion\\Articles List.docx"
                currentFileName = Path.GetFileName(currntFilePath);//"Articles List.docx"


                //if currentFileNamesList doesn't contain currentFileName (which it wont on the
                //first use of the program) then add it 

                //I need to see if crrentFileNamesList is nullbefore testing to see if it contains a current file name
                if(currentFileNamesList == null) 
                {
                    currentFileNamesList = new List<string>();
                    currentFileNamesList.Add(currentFileName);
                    //create a new B26FileName
                    currentB26FileName = HelperClasses.StringHelper.ConvertToBase26(currentFileCntr);
                    currentFileCntr++;
                    //update currentB26NamesList
                    currentB26NamesList.Add(currentB26FileName);
                }
                else
                if ( !currentFileNamesList.Contains(currentFileName)) 
                {
                    //update currentFileNamesList
                    currentFileNamesList.Add(currentFileName);
                    //create a new B26FileName
                    currentB26FileName = HelperClasses.StringHelper.ConvertToBase26(currentFileCntr);
                    currentFileCntr++;
                    //update currentB26NamesList
                    currentB26NamesList.Add(currentB26FileName);
                }

               

            }// end iterate thru currentListOfAllFilePathsArr to create currentB26NamesList

            //CHECH TO SEE IF THE FOLLOWING NEED TO BE UPDATED

            //update FileNamesList
            DataModels.AppProperties.FileNamesList = currentFileNamesList;

            // Update FileNamesList
            //DataModels.AppProperties.FileNamesList = currentFileNamesList;

            //return the updated currentFileCntr
            DataModels.AppProperties.FileCntr = currentFileCntr;
        }// end CreateB26NamesList

        private static void CreateDirIDNamesDict()
        {
            //local strings
            string shortDirName = string.Empty;//"Religion\\"
            string dirIntName = string.Empty;//"0"
            string dirIDplusFileName = string.Empty;
            string currntFilePath = string.Empty;
            string currentFileName = string.Empty;
            string root = DataModels.AppProperties.RootDirectory;
            string currentB26FileName = string.Empty;
            string currentFilePath = string.Empty;

            //create local Dictionaries
            Dictionary<string, string> currnetDirIDNamesDict = new Dictionary<string, string>();
            if (DataModels.AppProperties.DirIDNamesDict != null)
            { currnetDirIDNamesDict = DataModels.AppProperties.DirIDNamesDict; }
                       

            //local arrays
            // create a local varialbe to hold the simple file name
            string[] currentListOfAllFilePathsArr = DataModels.AppProperties.ListOfAllFilePaths.ToArray();
            //currentListOfAllFilePathsArr[0] = "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion\\Articles List.docx"
 
            //local Lists
            List<string> currentB26FileNameList = DataModels.AppProperties.B26FileNamesList;

            //iterate thru currentListOfAllFilePathsArr to update currneDirIDNamesDict 
            for (int i = 0; i < currentListOfAllFilePathsArr.Length; i++)
            {
                currentFilePath = currentListOfAllFilePathsArr[i];//"C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion\\Articles List.docx"
                // replace root
                currentFilePath = currentFilePath.Replace(root, "");//"Religion\\Articles List.docx"
                currentFileName = Path.GetFileName(currentFilePath);//"Articles List.docx"
                shortDirName = currentFilePath.Replace(currentFileName, "");//"Religion\\"

                //currnetDirIDNamesDict[shortDirName] = {[Religion\, 0]}


                /*
                 * At this point I have the shortDirName "Religion\\" but I need to see if it is a;ready in the
                 * currnetDirIDNamesDic as a Key and if, get its value. If it isn't then I need to create a
                 * new entry into the currnetDirIDNamesDic and set its value to the currentDirCntr converted to a 
                 * string
                 */

                //first check to see if currnetDirIDNamesDict is not null, it it isn't see if it has a Key of shortDirName
                string newKey = String.Empty;
                string newValue = String.Empty;
                if (currnetDirIDNamesDict != null)
                {
                    //  if currnetDirIDNamesDict doesn't contains a Key of shortDirName then add a new entry
                    if (!currnetDirIDNamesDict.ContainsKey(shortDirName))
                    {
                        newKey = shortDirName;
                        newValue = currentDirCntr.ToString();
                        currnetDirIDNamesDict.Add(newKey, newValue);
                        currentDirCntr++;
                    }
                }
              
            }// end //iterate thru currentListOfAllFilePathsArr to update currneDirIDNamesDict

            //update DirDictNames
            DataModels.AppProperties.DirIDNamesDict = currnetDirIDNamesDict;
        }//end CreateDirIDNamesDict()

        //create a dictionary that holds (?,}
        private static void CreateFileNamesDict() 
        { 
        }

        // create a dictionary that holds {?,?]
        private static void CreateDirNamesDict()
        {
            //set currentDirIDNamesDict to old DirIDNamesDict if it exists
            Dictionary<string, string> currentDirNamesDict = DataModels.AppProperties.DirIDNamesDict;

            //Get the old DirIDNamesDict if it exists
            //ERROR ERROR currentDirIDNamesDict should be currentDirIDNamesDict
            GetfileNamesAndPaths.currentDirIDNamesDict = DataModels.AppProperties.DirIDNamesDict;

            //Set the boolean value oldDirIDNamesDictExists
            //ERROR ERROR oldDirIDNamesDictExists should be oldDirIDNamesDictExists
            if (GetfileNamesAndPaths.currentDirIDNamesDict != null) { oldDirIDNamesDictExists = true; }

            //Create the currentFileInfoDictionary from the global if it exists and as a local if it doesn't
            if (DataModels.AppProperties.FileInfoDict != null)
            {
                currentFileInfoDict = DataModels.AppProperties.FileInfoDict;
            }
            else
            {
                currentFileInfoDict = new Dictionary<string, string>();
            }
            //get the source root directour
            string root = DataModels.AppProperties.RootDirectory;


            //Local variables
            string currentFilesPath = "";
            string currentFileName = "";
            string currentFileAbbreviatdDirectoryName = "";//the directory left after removing the root and the FileNmae
            string currentDirectoryIntName = "";//the DirID from DirIDNamesDict eg. Religion\\ = 0
            string currentFileLengthStr = "";//the length of the file
            string currentFileBase26Name = "";
            string currentVersionNum = "";
            string rootDirectory = DataModels.AppProperties.RootDirectory;
            string FileNameDictKey = "";
            string FileNameDictValue = "";

            //iterate thru combinedDirArr and if a new directory esists get its name fron the
            //current value of currentDirCntr and add it to DirIDNamesDict
            string[] combinedDirArr = DataModels.AppProperties.CombinedDirList.ToArray();
            for (int i = 0; i < combinedDirArr.Length; i++)
            {
                string old = combinedDirArr[i];
                string newname = old.Replace(root, "");
                newname = newname + '\\';
                //see if oldDirIDNamesDictExists exists t
                if (oldDirIDNamesDictExists)
                {
                    //see if it contains a Key =newname
                    if (!GetfileNamesAndPaths.currentDirIDNamesDict.ContainsKey(newname))
                    {
                        //the old directory doesn't contain this directory name so add it
                        string newDirValueStr = currentDirCntr.ToString();

                        //ERROR ERROR DirNamesDict SHOULD BE DirIDNamesDict
                        DirNamesDict.Add(newname, newDirValueStr);
                        currentDirCntr++;
                    }
                }
                else
                {
                    combinedDirArr[i] = newname;
                    DirNamesDict.Add(newname, i.ToString());
                }
            }//end for (int i = 0; i<combinedDirArr.Length; i++)

            //ERROR ERROR DirNamesDict SHOULD BE currentDirIDNamesDict
            DataModels.AppProperties.DirIDNamesDict = currentDirIDNamesDict;
            DataModels.AppProperties.CurrentFilesPath = currentFilesPath;
        }

        //Create a list that contains the paths to all of the files in the root directory and its subdirectories
        private static void CreateListOfAllFiles() 
        {
            //create a string array of combinedDirList so that you can interate thru it to create DirIDNamesDict
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

            //Create lists needed to creast B26Names
            /* to create a list of B26Names I need the abbreviated file name which is composed of its
             * folders int name + the individual file name eg. for I can get from DirIDNamesDict
             * I also need an abbreviated file name which I can get by using path in the ListFoldersAndFiles( 
             */


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



        private static void CreateShortDirNamesList()
        {
            // local Lists
            List<string> combinedDirList = DataModels.AppProperties.CombinedDirList;
            List<string> combinedShortdDirNamesList = new List<string>();

            //local arrays
            string[] combinedDirNamesArr= combinedDirList.ToArray();

            // strings
            string currentDirName = String.Empty;
            string currentShortDirName = String.Empty;
            string shortDirName = String.Empty;
            string root = DataModels.AppProperties.RootDirectory;

            //iterate thru combinedDirArr getting currentShortDirName by removing root form
            //current value of combinedDirArr
            for (int i =0; i< combinedDirNamesArr.Length;i++)
            {
                currentDirName = combinedDirNamesArr[i];
                currentShortDirName = currentDirName.Replace(root, "");
                currentShortDirName = currentShortDirName + "\\";
                combinedShortdDirNamesList.Add(currentShortDirName);
            }

            // create a string array to hold combinedShortdDirNamesList
            string[] combinedShortdDirNamesArr = combinedShortdDirNamesList.ToArray();
            // save combinedDirList
            DataModels.AppProperties.ShortdDirNamesArr = combinedShortdDirNamesArr;
        }//end CreateShortDirNamesList()


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