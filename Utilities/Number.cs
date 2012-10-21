using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public class Number
    {
        private static string sTable = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static int numbase = 52;

        /// <summary>
        /// Number of characters long.
        /// </summary>
        private static int length = 4;

        /// <summary>
        /// Converts a long integer to a Base-52 string.
        /// </summary>
        public static string IntToBase52(long iDec)
        {
            string strBin = "";
            int[] result = new int[length];
            int MaxBit = result.Length;

            for (; iDec > 0; iDec /= numbase)
            {
                int rem = Convert.ToInt32(iDec % numbase);
                result[--MaxBit] = rem;
            }
            for (int i = 0; i < result.Length; i++)
                strBin += sTable[(int)result.GetValue(i)];
            strBin = strBin.TrimStart(new char[] { '0' });
            return strBin;
        }

        /// <summary>
        /// Converts a Base-52 string into a long integer.
        /// </summary>
        public static long Base52ToInt(string sBase)
        {
            long dec = 0;
            int b = 0;
            long iProduct = 1;

            for (int i = sBase.Length - 1; i >= 0; i--, iProduct *= numbase)
            {
                string sValue = sBase[i].ToString();
                b = sTable.IndexOf(sBase[i]);
                dec += (b * iProduct);
            }
            return dec;
        }
    }


}
