using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewFBP.HelperClasses
{
    public static class FileIOClass
    {
        /*FileIOClass
         * The purpose of this class is to handle all communicarions wirh the disk
         * The first task will be to create write to disk procedures
         */

        //Create a local value for the path to the local disk backup folder
        public static string sourceBackupDirPath { get; set; }

       
        public static void saveDicrionary( Dictionary<string,string> newDictionary, string dictName)
        {
            Dictionary<string, string> currentDictionary = newDictionary;
            string outputFilePath = sourceBackupDirPath + dictName + ".txt";

            //process each KeyValuePair in the currentDictionary to create a string Key~Value and store it in a 
            //List<string>
            List<string> OutputStringsList = new List<string>();


            string Key = String.Empty;
            string Value = String.Empty;
            string outputString = String.Empty;
            foreach(KeyValuePair<string, string> kvp in currentDictionary)
            {
                Key = kvp.Key;
                Value = kvp.Value;
                outputString = Key+  '~'+ Value;
                OutputStringsList.Add(outputString);
            }

            //convert the List to an array of strings
            string[] ousputArray = OutputStringsList .ToArray();

            //write ousputArray
            //
            //Write the file names to ReligionFileName
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (string line in ousputArray)
                {
                    writer.WriteLine(line);
                }
            }
            
        }// end  public static void saveDicrionary

        public static void SaveList(List<string> newList, string outputListName)
        {
            List<string> currentList = newList;


            string outputFilePath = sourceBackupDirPath + outputListName + ".txt";
            //convert List to straing array
            string[] currentListArr = currentList.ToArray();

            //write ousputArray
            //
            //Write the file names to ReligionFileName
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (string line in currentListArr)
                {
                    writer.WriteLine(line);
                }
            }

        }//end public static void SaveList(L

        public static void WriteString(string currentStr, string stringName)
        {
            string outputFilePath = sourceBackupDirPath + stringName + ".txt";

            File.WriteAllText(outputFilePath, currentStr);
            //string stophere = "";

        }
        //File.WriteAllText("example.txt", "This is a single line of te




        //DO NOT GO PAST HERE
    }//end public static class FileIOClass
}//end namespace NewFBP.HelperClasses
