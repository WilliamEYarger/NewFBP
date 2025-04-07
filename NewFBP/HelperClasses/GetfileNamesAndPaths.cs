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

  
            string STOPHERE = "";

        }// end ProcdessSourceFiles


 
        

 

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

                currentDirectoriesFilePathsList = Directory.GetFiles(currentDirectory).ToList();
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