    using System.IO;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using System.Diagnostics.Eventing.Reader;
using System.Data;
using System.Windows.Forms;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;


namespace NewFBP.HelperClasses
{
        public static class GetfileNamesAndPaths
    {
        /*TOC
         *  CreateCombinrDirList();
         *  CreateShortDirNamesList(); 
         *  CreateListOfAllFiles();
         *  CreateDirIDNamesDict();
         *  CreateB26NameListAndFileLengthDict();
         *  CreateFileLengthDict();
         *  
         *  private static void CreateCombinrDirList() 
         *  private static void CreateShortDirNamesList()
         *  private static void CreateListOfAllFiles() 
         *  private static void CreateDirIDNamesDict()
         *  private static void CreateB26NameListAndFileLengthDict() 
         *  private static void CreateFileLengthDict() 
         */

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

        private static string currntFilePath = String.Empty;

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


            //Create private static void CreateDirIDNamesDict() {}
            CreateDirIDNamesDict();//DONE

            //Create private static voide Create B26NameList() {}
            CreateB26NameListAndFileLengthDict();//DONE


            // create FileFetchDict
            CreateFileFetchDict();

            HelperClasses.FileIOClass.SaveTextFilesToRepository();

            /*At this point it is time to save the actual renamed files 
             * but the method depends on whether this is the FirstRun or not
             * */

            if(DataModels.AppProperties.FirstRun)
            {
                // this is the first time the program has been run so save the 0 version of all files
                HelperClasses.FileIOClass.Save0VersionOfFiles();
                Environment.Exit(0);
            }//end  first Run
            else // this is not the first run  so compare the FileLengthDict.txt of the SourceBackup to the Repository Backup
            {

            }






        }// end ProcdessSourceFiles

        private static void CreateFileFetchDict()
        {
            /*
        * FileFetchDict, a dictionary whose Key if the full path to a file and whose Value is Base26File# 
        * {FilePath,Base26File#} eg. {"C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx",AAA) 
        * this will be used to get the file to be copied to the Repository Backup folder and the Value (the Base26File#) 
        * will be used to search the CurrentVersionDict to the the current version number so it can be incemented and 
        * applied to the Base26File as the name of the current version of the file in the Repository Backup folder. 
        * eg. the file named AAA.0 will contain the first version of the file 
        * "C:\Users\Owner\OneDrive\Documents\Learning\Religion\Articles List.docx"
        */

            //Create a local copy of DataModels.AppProperties.FileFetchDict
            Dictionary<string, string> currentFileFetchDict = new Dictionary<string, string>();
            if (DataModels.AppProperties.FileFetchDict != null)
            { currentFileFetchDict = DataModels.AppProperties.FileFetchDict; }


            //Create B26FileNameArr
            string[] B26FileNameArr = DataModels.AppProperties.B26FileNamesList.ToArray();

            //Create an arryal that holds all of the paths to the files in the root directory
            string[] filePathsArray = DataModels.AppProperties.ListOfAllFilePaths.ToArray();
            /*
             Iterate thur filePathsArray and get a full path to a file
            test to see if that file exists
            if it does get tis file length
            set the Key to the FileFetchDict to the filePath
            set the value to the FileFetchDict to the length converted to a string
             */
            string currentFilePath = String.Empty;
            string fileFetchDictKey = String.Empty;
            string fileFetchDictValue = String.Empty;

            for (int i = 0; i < filePathsArray.Length; i++)
            {
                currentFilePath = filePathsArray[i];
                if (!currentFileFetchDict.ContainsKey(currentFilePath))
                {
                    fileFetchDictKey = currentFilePath;

                    // the value is the B26FileName
                    fileFetchDictValue = B26FileNameArr[i];

                    //Add a new value to currentFileFetchDic
                    currentFileFetchDict.Add(fileFetchDictKey, fileFetchDictValue);

                }//end does the current fileFetchDict contain the current file path as a key

            }// end interate through filePathsArr and creat entries toFileFetchDict

            //Save FileFetchDict
            DataModels.AppProperties.FileFetchDict = currentFileFetchDict;
            //Save the FileFetchdict to the Disk
            HelperClasses.FileIOClass.saveDicrionary(currentFileFetchDict, "FileFetchDict");

            string stophere = "";

            /**/



        }//end private static void CreateFileFetchDict()




        private static void CreateB26NameListAndFileLengthDict()
        {


            //SET local variables to Global properties if they exist and if not set booleans
            //create a local currentFileCntr  DILE CNTR IS NOT SET
            int currentFileCntr = DataModels.AppProperties.FileCntr;

            string dirIDplusFileName;
            string currentB26FileName = string.Empty;

            //local booleans

            bool boolB26FileNamesExists = true;
            bool boolDirPlusFileNamesListExists = true;

           
            //DataModels.AppProperties.DirIDNamesDict already exists
            Dictionary<string, string> currentDirIDNamesDict = DataModels.AppProperties.DirIDNamesDict;

            Dictionary<string, string> currentFileVersionDict = new Dictionary<string, string>();
            if (DataModels.AppProperties.FileVersionDict != null)
            {
                currentFileVersionDict = DataModels.AppProperties.FileVersionDict;
            }

            //create local Lists DataModels.AppProperties.ListOfAllFilePaths already exists
            List<string> currentListOfAllFilePaths = DataModels.AppProperties.ListOfAllFilePaths;


            //SECTION DEALING WITH THE CREATION OFcurrentB26NamesList
            List<string> currentB26NamesList = new List<string>();
            if (DataModels.AppProperties.B26FileNamesList != null)
            { currentB26NamesList = DataModels.AppProperties.B26FileNamesList; }

            //SECTION DEALING WITH THE CREATION OF currentFileNamesList
            List<string> currentFileNamesList = new List<string>();

            if (DataModels.AppProperties.FileNamesList != null)
            {
                currentFileNamesList = DataModels.AppProperties.FileNamesList;
            }

            //SECTION DEALING WITH FileLengthDict
            Dictionary<string, string> currentFileLengthDict = new Dictionary<string, string>();
            if (DataModels.AppProperties.FileLengthDict != null)
            { currentFileLengthDict = DataModels.AppProperties.FileLengthDict; }


            //create local strings
            string shortFileName = String.Empty; //Articles.docx
            string shortDirName = String.Empty; // Relibion\
            string dirIntName = String.Empty;  //0
            string root = DataModels.AppProperties.RootDirectory;
            //string currntFilePath = String.Empty;
            string currentFileName = String.Empty;
            string fileVersionDictKey = String.Empty;

            //create local string arrays DataModels.AppProperties.ListOfAllFilePaths already exists
            string[] currentListOfAllFilePathsArr = DataModels.AppProperties.ListOfAllFilePaths.ToArray();



            /* Iterate thru currentListOfAllFilePathsArr getting the currntFilePath
             * and the currentFileName
            */
            for (int i = 0; i < currentListOfAllFilePathsArr.Length; i++)
            {
                currntFilePath = currentListOfAllFilePathsArr[i];//"C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion\\Articles List.docx"

                shortDirName = currntFilePath.Replace(root, "");

                currentFileName = System.IO.Path.GetFileName(currntFilePath);//"Articles List.docx"

                shortDirName = shortDirName.Replace(currentFileName, "");

                dirIntName = currentDirIDNamesDict[shortDirName];
                //Get the didIDname and place it at the front of FileVersionDict

                currentFileName = dirIntName + '.' + currentFileName;
                if (!currentFileNamesList.Contains(currentFileName))
                {
                    //This is a new file

                    // add currentFileName to currentFileNamesList
                    currentFileNamesList.Add(currentFileName);

                    //create a new B26FileName
                    currentB26FileName = HelperClasses.StringHelper.ConvertToBase26(currentFileCntr);//"AAA"

                    currentFileCntr++;

                    currentB26NamesList.Add(currentB26FileName);

                    //get length of current file and create a new entry in currentFileLengthDict

                    //get all of the  info about the current file
                    FileInfo fileInfo = new FileInfo(currntFilePath);
                    long fileLength = fileInfo.Length;
                    currentFileLengthDict.Add(currentB26FileName, fileLength.ToString());

                    if (currentDirIDNamesDict.ContainsKey(shortDirName))
                    {
                        dirIntName = currentDirIDNamesDict[shortDirName];//"0"

                        //I need to change fileInfoKey to fileVersionDictKey 
                        fileVersionDictKey = currentFileName;// "0.Articles List.docx"

                        currentFileVersionDict.Add(fileVersionDictKey, "0");
                    }
                }//end if (!currentFileNamesList.Contains(currentFileName))                

            }//end for (int i = 0; i < currentListOfAllFilePathsArr.Length; i++)

            DataModels.AppProperties.FileCntr = currentFileCntr;

            ////FILE VERSIONS DICTIONARY

            // Save currentFileVersionDict DataModels.AppProperties.FileVersionDict 
            DataModels.AppProperties.FileVersionDict = currentFileVersionDict;

            // write the FileVersionDict to disk
            HelperClasses.FileIOClass.saveDicrionary(currentFileVersionDict, "FileVersionDict");


            //// FILE LENGTH DICTIONARY
            DataModels.AppProperties.FileLengthDict = currentFileLengthDict;
            //save FileLengthDict to disk 
            HelperClasses.FileIOClass.saveDicrionary(currentFileLengthDict, "FileLengthDict");


            //Save currentB26NamesList (DataModels.AppProperties.B26FileNamesList
            DataModels.AppProperties.B26FileNamesList = currentB26NamesList;
            HelperClasses.FileIOClass.SaveList(currentB26NamesList, "B26FileNamesList");

            // save currentFileNamesList DataModels.AppProperties.FileNamesList 
            DataModels.AppProperties.FileNamesList = currentFileNamesList;
            HelperClasses.FileIOClass.SaveList(currentFileNamesList, "FileNamesList");

            //Save current counter values
            string currentCntrValues =
                    DataModels.AppProperties.DirCntr.ToString() +'~' + DataModels.AppProperties.FileCntr.ToString();
            HelperClasses.FileIOClass.WriteString(currentCntrValues, "CurrentCntrValues");

            string stophere = "";

        }// end private static void CreateB26NameListAndFileLengthDict(

        // create a dictionary that holds {ShortDirName,DirIntName]
        private static void CreateDirIDNamesDict()
        {

            /*currentDirIDNamesDict 
             * 
             */

            // instantiate currentDirIDNamesDict
            Dictionary<string, string> currentDirIDNamesDict = new Dictionary<string, string>();

            if (DataModels.AppProperties.DirIDNamesDict != null)
            {
                currentDirIDNamesDict = DataModels.AppProperties.DirIDNamesDict;
            }

            //get the source root directour
            string root = DataModels.AppProperties.RootDirectory;


            //Local variables
            //string currentFilesPath = "";
            string currentFileName = "";
            string currentFileAbbreviatdDirectoryName = "";//the directory left after removing the root and the FileNmae
            string currentDirectoryIntName = "";//the DirID from DirIDNamesDict eg. Religion\\ = 0
            string currentFileLengthStr = "";//the length of the file
            string currentFileBase26Name = "";
            string currentVersionNum = "";
            string rootDirectory = DataModels.AppProperties.RootDirectory;
            string FileNameDictKey = "";
            string FileNameDictValue = "";

            //iterate thru combinedDirPathArr and if a new directory esists get its name fron the
            //current value of currentDirCntr and add it to DirIDNamesDict
            string[] combinedDirArr = DataModels.AppProperties.CombinedDirPathList.ToArray();


            for (int i = 0; i < combinedDirArr.Length; i++)
            {
                string oldDirName = combinedDirArr[i];
                string dirShortName = oldDirName.Replace(root, "");
                dirShortName = dirShortName + '\\';


                /* Although there is an existant DirIDNamesDict
                 * the program does not access it to determine if 
                 * there is a new directory found 2025040707132     
                 */
                //Update the currentDirIDNamesDict if a new dirShortName is found
                if (!currentDirIDNamesDict.ContainsKey(dirShortName))
                {
                    //this is a new directory so add it
                    string newDirValueStr = currentDirCntr.ToString();
                    //this should be currentDirIDNamesDict
                    currentDirIDNamesDict.Add(dirShortName, newDirValueStr);
                    currentDirCntr++;
                }// endUpdate the currentDirIDNamesDict if a new dirShortName is found

            }//end for (int i = 0; i<combinedDirPathArr.Length; i++)

            DataModels.AppProperties.DirIDNamesDict = currentDirIDNamesDict;

            // write the DirIDNamesDict to disk
            HelperClasses.FileIOClass.saveDicrionary(currentDirIDNamesDict, "DirIDNamesDict");
            DataModels.AppProperties.DirCntr = currentDirCntr;
        }//end private static void CreateDirIDNamesDict()



        //Create a list that contains the paths to all of the files in the root directory and its subdirectories
        private static void CreateListOfAllFiles() 
        {
            //create a string array of combinedDirPathList so that you can interate thru it to create DirIDNamesDict
            List<string> combinedDirPathList = DataModels.AppProperties.CombinedDirPathList;
            string[] combinedDirPathArr = combinedDirPathList.ToArray();

            /*Create a list that contains the complete paths to all of the files
             in the root and all of its subdirectories*/
            List<string> listOfAllFilePaths = new List<string>();

            /*
             currentDirectory is the path to the current directory minus the terminal '\\'
             */
            string currentDirectory;

            /*
             List of the paths to all files in the root directory and all of its
            subdirectories
             */

            List<string> allFilePathsList = new List<string>();

            /*
             currentDirectoriesFilePathsList is a list of all of the files in whatever the current directory i
            for example in the Religion\ directory these currently go from
            "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion\\Articles List.docx" to
            "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion\\Timeline Template.docx"
             */
            List<string> currentDirectoriesFilePathsList = new List<string>();

            int arrayLength = combinedDirPathArr.Length;

            for (int i = 0; i < arrayLength; i++)
            {
                currentDirectory = combinedDirPathArr[i];

                //Get all of the files in the currentDirectory
                currentDirectoriesFilePathsList = Directory.GetFiles(currentDirectory).ToList();

                //add the files in the current directory to the list of all files paths
                allFilePathsList.AddRange(currentDirectoriesFilePathsList);
            }

            listOfAllFilePaths = allFilePathsList;
            DataModels.AppProperties.ListOfAllFilePaths = listOfAllFilePaths;

            // DETERMINE A DirNamesDict EXISTS AT THIS POINT NO IT IS NULL SO I NEED TO CREATE IT FIRST
            Dictionary<string, string> currentDirNamesDict = DataModels.AppProperties.DirIDNamesDict;

            List<string> listOfShortFileNames = new List<string>();
            string shortFileName;
            //iterate thru listOfAllFilePaths and create a list of shortFileNames and if 
            foreach (string file in listOfAllFilePaths)
            {
                shortFileName = System.IO.Path.GetFileName(file);
                listOfShortFileNames.Add(shortFileName);
            }

            DataModels.AppProperties.ListOfShortFileNames = listOfShortFileNames;

            //Create lists needed to creast B26Names
            /* to create a list of B26Names I need the abbreviated file name which is composed of its
             * folders int name + the individual file name eg. for I can get from DirIDNamesDict
             * I also need an abbreviated file name which I can get by using path in the ListFoldersAndFiles( 
             */


        }// end private static void CreateListOfAllFiles(

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

            //convert the array allOtherDirectories ro a list
            AllDirectoriesList = allOtherDirectories.ToList();

            //Concatenate the RootDirectory and the allDirectoriesList to
            //Create a list (combinedDirPathList)that contains the root and all of its subdirectories
            List<string> combinedDirList = RootDirectory.Concat(AllDirectoriesList).ToList();


            DataModels.AppProperties.CombinedDirPathList = combinedDirList;
        }



        private static void CreateShortDirNamesList()
        {
            // local Lists
            List<string> combinedDirList = DataModels.AppProperties.CombinedDirPathList;

            /*
             combinedShortdDirNamesList is a list of the short directory names
            Religion\\  to
            Religion\\Christianity\\Writings\\New Testament\\Books of the New Testament\\
            QA Files for books of the New Testament\\QAResults\\"
             */
            List<string> combinedShortdDirNamesList = new List<string>();

            /*combinedDirNamesArr     */
            string[] combinedDirNamesArr= combinedDirList.ToArray();

            // LOCAL STRINGS

            /*currentDirName is the directory being extracted from 
            thecurrent value of combinedDirNamesArr*/
            string currentDirName = String.Empty;

            /*
             currentShortDirName is the name of the current directory including '\\'
            "Religion\\"
             */
            string currentShortDirName = String.Empty;

            // DEFINITION shortDirName is the Path to the directory after removing the root directory path
            string shortDirName = String.Empty;
            string root = DataModels.AppProperties.RootDirectory;

            /*iterate thru combinedDirPathArr getting currentShortDirName by removing root from
            //current value of combinedDirPathArr*/
            for (int i =0; i< combinedDirNamesArr.Length;i++)
            {
                currentDirName = combinedDirNamesArr[i];
                currentShortDirName = currentDirName.Replace(root, "");
                currentShortDirName = currentShortDirName + "\\";
                combinedShortdDirNamesList.Add(currentShortDirName);
            }

            // create a string array to hold combinedShortdDirNamesList
            string[] combinedShortdDirNamesArr = combinedShortdDirNamesList.ToArray();
            // save combinedDirPathList
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
                    string fileName = System.IO.Path.GetFileName(file);
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
                string fileOutputPath = DataModels.AppProperties.StoragePath;//??? may not be defined
                //get tne name of the output file names
                string fileNamePath = DataModels.AppProperties.FileNamesPath;//??? may not be defined
                string filePathsPath = DataModels.AppProperties.FilePathsPath;//??? may not be defined

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





    }//end public static class GetfileNamesAndPaths

}// end namespace