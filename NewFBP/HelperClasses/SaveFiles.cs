using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewFBP.HelperClasses;

namespace NewFBP.HelperClasses
{
    public static class SaveFiles
    {
        private static Dictionary<string,string> currentDirIDNamesDict = DataModels.AppProperties.DirIDNamesDict;
        private static Dictionary<string, string> currentFileFetchDict = DataModels.AppProperties.FileFetchDict;
        private static Dictionary<string, string> currentFileLengthDict = DataModels.AppProperties.FileLengthDict;
        private static Dictionary<string, string> currentFileVersionDict = DataModels.AppProperties.FileVersionDict;
        private static List<string> currentB26FileNamesList = DataModels.AppProperties.B26FileNamesList;
        private static string currentCurrentCntrValues = DataModels.AppProperties.CurrentCntrValues;
        private static string currentRepositoryPath = DataModels.AppProperties.RepostioryPath;

        public static void  SaveAllFiles()
        {
            // test to see if this is the FirstRun
            string sourcePath = String.Empty;
            string destinationPath = String.Empty;

            if(DataModels.AppProperties.FirstRun)
            {
                //Save B26FileNamesList
                sourcePath = DataModels.AppProperties.SourceBackupDirPath + "B26FileNamesList.txt";

            }//end First run
            else // this is not the first run
            {

            }//end is not Firstrune


        }//end  public static void  SaveAllFiles()

        private static void SaveCurrentFile(string SourcePath, string DestinationPath)
        {

        }//end private static void SaveCurrentFile(string SourcePath, string DestinationPath)





        ////DO NOT DELETE PAST HERE ////






    }// end public static class SaveFiles
}// end namespace