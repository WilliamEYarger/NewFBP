using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NewFBP.DataModels
{
    public static class AppProperties
    {
        public static string CurrentSourceName { get; set; }

        public static string CurrentSourcePath { get; set; }

        public static string FileNamesPath { get; set; }// the name of the file that holds the FileNames

        public static string FilePathsPath { get; set; }// the name of the file that holds the file paths

        public static string StoragePath { get; set; }//;//the folder where the Current File Data Should be store

        public static string SourceDirectory { get; set; }//the name of the source directory so that the root can be extracted Re;ligion\\

        public static string  RootDirectory { get; set; }
    }//end 

}// End namespace
