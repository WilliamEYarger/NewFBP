using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewFBP.HelperClasses
{
    public static class StringHelper
    {

        #region GetItemNumberOfThisSubstring

        /// <summary>
        /// Receives a delimited string, a text string to search for and a delimiter characer 
        /// if the text string to search for is found its item number is returned, else -1 is returned
        /// </summary>
        /// <param name="delString"></param>
        /// <param name="itemText"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public static int GetItemNumberOfThisSubstring(string delString, string itemText, char del)
        {
            int itemNumber = -1;

            // create an array of all delimmited items
            string[] itemsArray = delString.Split(del);
            for (int itemNum = 0; itemNum < itemsArray.Length; itemNum++)
            {
                if (itemsArray[itemNum] == itemText)
                {
                    itemNumber = itemNum;
                    return itemNumber;
                }
            }
            // if no item matched the itemText then return -1
            return itemNumber;
        }

        #endregion GetItemNumberOfThisSubstring

        #region ReturnNumberOfDeliniters

        public static int ReturnNumberOfDeliniters(string line, char del)
        {
            char[] LineCharArray = line.ToCharArray();
            int count = 0;
            for (int i = 0; i < LineCharArray.Length; i++)
            {
                char thisChar = LineCharArray[i];
                if (thisChar == del) count++;
            }

            return count;
        }


        #endregion ReturnNumberOfDeliniters


        #region ReturnItemAtPos
        /// <summary>
        /// Received a delimited string, the delimiter and the number of the item desired in the string 
        /// and returns that item
        /// </summary>
        /// <param name="delString"></param>
        /// <param name="del"></param>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public static string ReturnItemAtPos(string delString, char del, int Pos)
        {
            string[] itemsArray = delString.Split(del);
            return itemsArray[Pos];
        }
        #endregion ReturnItemAtPos

        #region ReturnLastItem

        public static string ReturnLastItem(string line, char del)
        {
            var PosLastDel = line.LastIndexOf(del);
            var ReturnString = line.Substring(PosLastDel + 1);
            return ReturnString;

        }


        #endregion ReturnLastItem

        #region ReturnLastAndUpdate
        public static string[] ReturnLastAndUpdate(string delString, char del)
        {
            var returnArr = new string[2];
            int posLastDelimiter = delString.LastIndexOf(del);
            returnArr[0] = delString.Substring(posLastDelimiter);
            returnArr[1] = delString.Substring(0, posLastDelimiter - 1);

            return returnArr;

        }
        #endregion ReturnLastAndUpdate


        #region RemoveLastValue
        public static string RemoveLastValue(string delString, char del)
        {

            int posLastDelimiter = delString.LastIndexOf(del);
            if (posLastDelimiter < 0)
            {
                return "";
            }
            string updatedString = delString.Substring(0, posLastDelimiter);
            return updatedString;
        }
        #endregion RemoveLastValue


        #region CreateDisplayString

        /// <summary>
        /// Create a display string to show in a list box
        /// </summary>
        /// <param name="LeedingChar"> is a + or - </param>
        /// <param name="Text"></param>
        /// <param name="ID"> the Items AlphaNumber</param>
        /// <param name="NumberOfChildren"> The Items number of Children</param>
        /// <returns></returns>
        public static string CreateDisplayString(char LeedingChar, string Text, string ID, int NumberOfChildren)
        {
            string thisItemsListString;

            int LengthOFItemText = Text.Length;
            int addSpacesNumber = 100 - LengthOFItemText;
            string spacesString = new string(' ', addSpacesNumber);
            if (LeedingChar == '-')
            {
                thisItemsListString = "- " + Text + spacesString + '^' + ID + '^' + NumberOfChildren.ToString();
            }
            else
            {
                thisItemsListString = "+ " + Text + spacesString + '^' + ID + '^' + NumberOfChildren.ToString();
            }


            return thisItemsListString;
        }

        #endregion CreateDisplayString

        #region ReplaceItemAtPosition
        public static void ReplaceItemAtPosition(ref string line, char del, int postions, string item)
        {
            string[] LineArray = line.Split(del);
            LineArray[postions] = item;
            line = "";
            foreach (string Item in LineArray)
            {
                line = line + Item + del;
            }
            line = line.Substring(0, line.Length - 1);

        }
        #endregion ReplaceItemAtPosition


        #region GetAndRemoveNthItem

        public static string GetAndRemoveNthItem(ref string delimitedString, char del, int pos)
        {

            string[] itemsArray = delimitedString.Split(del);
            string returnItem = itemsArray[pos];
            List<string> ItemsList = itemsArray.ToList<string>();
            ItemsList.RemoveAt(pos);
            delimitedString = "";
            foreach (string item in ItemsList)
            {
                delimitedString = delimitedString + item + del;
            }
            delimitedString = delimitedString.Substring(0, delimitedString.Length - 1);

            return returnItem;

        }//End GetAndRemoveNthItem

        #endregion GetAndRemoveNthItem


        #region RemoveFirstItem

        public static string RemoveFirstItem(string delString, char del)
        {
            int posFirstDel = delString.IndexOf(del);
            delString = delString.Substring(posFirstDel + 1);
            return delString;
        }

        #endregion RemoveFirstItem


        #region ConvertToBase2
        public static string ConvertToBase26(int number)
        {
            if (number < 0 || number > 17575)
                throw new ArgumentException("Number must be between 0 and 17575 to fit in three characters.");

            StringBuilder result = new StringBuilder();
            while (number > 0 || result.Length < 3)
            {
                int remainder = number % 26;
                char digit = (char)('A' + remainder); // Convert remainder to letter
                result.Insert(0, digit);
                number /= 26;
            }

            return result.ToString();
        }// end ConvertToBase26 ConvertToBase2

        #endregion

    }// End Class


}//end namespace
