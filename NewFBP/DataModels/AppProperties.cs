using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NewFBP.DataModels
{
    public static class AppProperties
    {
        public static string CurrentSourceName { get; set; }//The name of the directory selected i.e. "Religion"

        public static string CurrentSourcePath { get; set; }//the complete path to the directory selected for backup 
        // i.e. "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\Religion"

        public static string FileNamesPath { get; set; }// the name of the file that holds the FileNames

        public static string FilePathsPath { get; set; }// the name of the file that holds the file paths

        public static string StoragePath { get; set; }//;//the folder where the Current File Data Should be store

        public static string SourceDirectory { get; set; }//the name of the source directory i.e. "Religion\\"
                                                          //so that the root can be extracted Religion\\

        //NOTE ON 0315 I CHANGES THIS FROM ROOTDIRECTORY TO SOURCEROOTDIRECTORY AND MOVED ITS DEFINITION TO MAINWINDOW.XAML.CS

        /*
         * The SourcerootDirectory in the CurrentSourcePath - SourceDirectory
         * It is used to create the entries in the DirNamesDict
         * i.e. "C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\"
         */
        public static string SourceRootDirectory{ get; set; }



        public static string  RootDirectory { get; set; }
        /* the root directory is that portion of CurrentSourcePath which is removed 
         * i.e. ("C:\\Users\\Owner\\OneDrive\\Documents\\Learning\\) so that the directory
         * names in the source can be compared with the directory names in the output Dictionary of Directories
         */
    }//end 

}// End namespace
