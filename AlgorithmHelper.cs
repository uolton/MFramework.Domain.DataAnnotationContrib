#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib
{
    /// <summary>
    /// </summary>
    public static class AlgorithmHelper
    {
        #region Constants and Enums
        #endregion

        #region Inner Classes and Structures
        #endregion

        #region Delegates and Events
        public delegate int CharToNumberMappingFunction(char character, int position);
        #endregion

        #region Instance and Shared Fields
        #endregion

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Private and Protected Methods
        #endregion

        #region Public Methods
        internal static ReadOnlyCollection<TValue> CopyValuesToReadOnlyCollection<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            var list = new List<TValue>();
            foreach (var keyvalue in dictionary)
            {
                list.Add(keyvalue.Value);
            }
            return new ReadOnlyCollection<TValue>(list);
        }

        public static void SetRegExCacheSize()
        {
            //{
            //    //Need to increase the framework regex cahce size as we will be using these extensively.
            //    int noOfValidators = ReflectionHelper.GetDerivedTypesInAssembly(
            //        typeof(StringValidator).Assembly, typeof(StringValidator),
            //        true, true).Length;
            //    //Most validators need atleast 2 regexs.
            //    Regex.CacheSize = (noOfValidators * 2);
            //}
        }

        public static bool IsOdd(int i)
        {
            return ((i % 2) != 0);
        }
        public static bool IsEven(int i)
        {
            return ((i % 2) == 0);
        }

        public static int Mod(string dividendText, int divisor)
        {
            int start, end, result, remainder, buffer;
            string remainderText = "", resultText = "";
            start = 0;
            end = 0;

            while (end <= dividendText.Length - 1)
            {
                buffer = Int32.Parse(remainderText + dividendText.Substring(start, end - start + 1));

                if (buffer >= divisor)
                {
                    result = buffer / divisor;
                    remainder = buffer - result * divisor;
                    remainderText = remainder.ToString();

                    resultText = resultText + result.ToString();

                    start = end + 1;
                    end = start;
                }
                else
                {
                    if (resultText != "") resultText = resultText + "0";
                    end = end + 1;
                }
            }

            if (start <= dividendText.Length) remainderText = remainderText + dividendText.Substring(start);

            return Int32.Parse(remainderText);
        }

        public static int GetNumericEquivalentOfChar(char c, CharToNumberMappingFunction map)
        {
            return map(c, -1);
        }
        public static int[] GetNumericEquivalentOfString(string s, CharToNumberMappingFunction map)
        {
            var result = new List<int>();
            for (int i = 0; i < s.Length; i++)
            {
                result.Add(map(s[i], i));
            }
            return result.ToArray();
        }
        public static string GetNumericEquivalentOfStringAsString(string s, CharToNumberMappingFunction map)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                result.Append(map(s[i], i));
            }
            return result.ToString();
        }

        public static int[] GetLatinAlphabeticPositionEquivalentOfString(string s)
        {
            return GetNumericEquivalentOfString(s,
                delegate(char c, int p) { return TextHelper.IsNumeric(c) ? TextHelper.GetNumericValueOfChar(c) : TextHelper.GetLatinAlphabetPosition(c); });
        }

        public static int CollapseDigits(int number)
        {
            //Check params.
            if (number < 0) throw new ArgumentException();

            int sumValue = SumDigits(number);
            while (sumValue > 9)
            {
                sumValue = SumDigits(sumValue);
            }
            return sumValue;
        }
        public static int CollapseDigits(string number)
        {
            //Check params.
            if (String.IsNullOrEmpty(number)) return 0;

            int sumValue = SumDigits(number);
            while (sumValue > 9)
            {
                sumValue = SumDigits(sumValue.ToString());
            }
            return sumValue;
        }
        public static int SumDigits(string number)
        {
            //Check params.
            if (String.IsNullOrEmpty(number)) return 0;

            int sumValue = 0;
            for (int i = 0; i < number.Length; i++)
            {
                int digit = Int32.Parse(number.Substring(i, 1));
                //Add digit to total.
                sumValue += digit;
            }
            return sumValue;
        }
        public static int SumDigits(int number)
        {
            //Check params.
            if (number < 0) throw new ArgumentException();

            int sumValue = 0;
            while (number != 0)
            {
                int c = number % 10;
                sumValue += c;
                number /= 10;
            }
            return sumValue;
        }

        public static bool StringEqualsIgnoreCase(string strA, string strB)
        {
            return String.Equals(strA, strB, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        #endregion
    }
}