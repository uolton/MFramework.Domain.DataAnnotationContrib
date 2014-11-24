#region Imports

using System;
using System.Text;
using MFramework.Domain.DataAnnotationsContrib.Internal;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib
{
    /// <summary>
    /// </summary>
    public static class TextHelper
    {
        #region Constants and Enums
        public static readonly char SpaceChar = ' ';
        #endregion

        #region Inner Classes and Structures
        #endregion

        #region Delegates and Events
        #endregion

        #region Instance and Shared Fields
        private static readonly char[][] __charSets = CreateCharSets();
        #endregion

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Private and Protected Methods
        private static char[][] CreateCharSets()
        {
            StringBuilder alphaChars = new StringBuilder();
            StringBuilder numericChars = new StringBuilder();
            StringBuilder alphanumericChars = new StringBuilder();
            StringBuilder whiteSpaceChars = new StringBuilder();

            for (char c = Char.MinValue; c < Char.MaxValue; c++)
            {
                if (Char.IsLetter(c))
                {
                    alphaChars.Append(c);
                }
                if (Char.IsDigit(c))
                {
                    numericChars.Append(c);
                }
                if (Char.IsLetterOrDigit(c))
                {
                    alphanumericChars.Append(c);
                }
                if (Char.IsWhiteSpace(c))
                {
                    whiteSpaceChars.Append(c);
                }
            }

            char[][] charSets = new char[4][] { 
                                                  alphaChars.ToString().ToCharArray(),
                                                  numericChars.ToString().ToCharArray(),
                                                  alphanumericChars.ToString().ToCharArray(),
                                                  whiteSpaceChars.ToString().ToCharArray()
                                              };
            return charSets;
        }

        private static char[] AlphaCharSet
        {
            get { return __charSets[0]; }
        }
        private static char[] NumericCharSet
        {
            get { return __charSets[1]; }
        }
        private static char[] AlphanumericCharSet
        {
            get { return __charSets[2]; }
        }
        private static char[] WhiteSpaceCharSet
        {
            get { return __charSets[3]; }
        }
        #endregion

        #region Public Methods
        public static int GetLatinAlphabetPosition(char c)
        {
            int unicodePoint = (int)c;

            if (unicodePoint >= 65 && unicodePoint <= 90)
            {
                //Uppercase letter.
                return (unicodePoint - 64);
            }
            if (unicodePoint >= 97 && unicodePoint <= 122)
            {
                //Lowercase letter.
                return (unicodePoint - 96);
            }
            else
            {
                //Not a letter.
                return -1;
            }
        }

        public static bool IsAlpha(string value)
        {
            return IsAlpha(value, false);
        }
        public static bool IsAlpha(string value, bool ignoreWhiteSpace)
        {
            //Check params.
            ArgumentValidation.CheckForNullReference(value, "value");

            foreach (var c in value)
            {
                if (!Char.IsLetter(c) && (!ignoreWhiteSpace || !Char.IsWhiteSpace(c)))
                {
                    return false;
                }
            }
            return true;

            //// Managed pointer arithemetic is a faster way to iterate
            //// through a string. This can be several times faster than
            //// foreach (IEnumerable) iteration on very large strings.
            //// Although the 'unsafe' keyword is used, there is nothing
            //// unsafe about this code. The 'unsafe' keyword is often
            //// mistaken for 'unmanaged'. This is NOT the case.
            //unsafe
            //{
            //    fixed (char* pStart = value)
            //    {
            //        char* pCurrent = pStart;
            //        char* pEnd = (pStart + value.Length);
            //        for (; pCurrent < pEnd; pCurrent++)
            //        {
            //            if (!Char.IsLetter(*pCurrent) && (!ignoreWhiteSpace || !Char.IsWhiteSpace(*pCurrent)))
            //            {
            //                return false;
            //            }
            //        }
            //    }
            //}
            //return true;
        }

        public static int GetNumericValueOfChar(char c)
        {
            return Int32.Parse(new string(c, 1));
        }

        public static bool IsNumeric(char value)
        {
            return IsNumeric(value, false);
        }
        public static bool IsNumeric(char value, bool ignoreWhiteSpace)
        {
            return (Char.IsDigit(value)) || (ignoreWhiteSpace && Char.IsWhiteSpace(value));
        }
        public static bool IsNumeric(string value)
        {
            return IsNumeric(value, false);
        }
        public static bool IsNumeric(string value, bool ignoreWhiteSpace)
        {
            //Check params.
            ArgumentValidation.CheckForNullReference(value, "value");

            foreach (var c in value)
            {
                if (!Char.IsDigit(c) && (!ignoreWhiteSpace || !Char.IsWhiteSpace(c)))
                {
                    return false;
                }
            }
            return true;

            //// Managed pointer arithemetic is a faster way to iterate
            //// through a string. This can be several times faster than
            //// foreach (IEnumerable) iteration on very large strings.
            //// Although the 'unsafe' keyword is used, there is nothing
            //// unsafe about this code. The 'unsafe' keyword is often
            //// mistaken for 'unmanaged'. This is NOT the case.
            //unsafe
            //{
            //    fixed (char* pStart = value)
            //    {
            //        char* pCurrent = pStart;
            //        char* pEnd = (pStart + value.Length);
            //        for (; pCurrent < pEnd; pCurrent++)
            //        {
            //            if (!Char.IsDigit(*pCurrent) && (!ignoreWhiteSpace || !Char.IsWhiteSpace(*pCurrent)))
            //            {
            //                return false;
            //            }
            //        }
            //    }
            //}
            //return true;
        }

        public static bool IsAlphanumeric(string value)
        {
            return IsAlphanumeric(value, false);
        }
        public static bool IsAlphanumeric(string value, bool ignoreWhiteSpace)
        {
            //Check params.
            ArgumentValidation.CheckForNullReference(value, "value");

            foreach (var c in value)
            {
                if (!Char.IsLetterOrDigit(c) && (!ignoreWhiteSpace || !Char.IsWhiteSpace(c)))
                {
                    return false;
                }
            }
            return true;

            //// Managed pointer arithemetic is a faster way to iterate
            //// through a string. This can be several times faster than
            //// foreach (IEnumerable) iteration on very large strings.
            //// Although the 'unsafe' keyword is used, there is nothing
            //// unsafe about this code. The 'unsafe' keyword is often
            //// mistaken for 'unmanaged'. This is NOT the case.
            //unsafe
            //{
            //    fixed (char* pStart = value)
            //    {
            //        char* pCurrent = pStart;
            //        char* pEnd = (pStart + value.Length);
            //        for (; pCurrent < pEnd; pCurrent++)
            //        {
            //            if (!Char.IsLetterOrDigit(*pCurrent) && (!ignoreWhiteSpace || !Char.IsWhiteSpace(*pCurrent)))
            //            {
            //                return false;
            //            }
            //        }
            //    }
            //}
            //return true;
        }

        public static string Strip(string value, params char[] stripChars)
        {
            //Check params.
            ArgumentValidation.CheckForNullReference(value, "value");
            ArgumentValidation.CheckForNullReference(stripChars, "stripChars");

            string stripString = new string(stripChars);

            StringBuilder result = new StringBuilder(value.Length);
            foreach (var c in value)
            {
                if (stripString.IndexOf(c) == -1)
                {
                    result.Append(c);
                }
            }
            return result.ToString();

            //// Allocate a char[] rather than using a StringBuilder.
            //// This is potentially faster.
            ////
            //char[] newValue = new char[value.Length];
            //int index = 0;
            //// Managed pointer arithemetic is a faster way to iterate
            //// through a string. This can be several times faster than
            //// foreach (IEnumerable) iteration on very large strings.
            //// Although the 'unsafe' keyword is used, there is nothing
            //// unsafe about this code. The 'unsafe' keyword is often
            //// mistaken for 'unmanaged'. This is NOT the case.
            //unsafe
            //{
            //    fixed (char* pStart = value)
            //    {
            //        char* pCurrent = pStart;
            //        char* pEnd = (pStart + value.Length);
            //        for (; pCurrent < pEnd; pCurrent++)
            //        {
            //            if (searchString.IndexOf(*pCurrent) == -1)
            //            {
            //                newValue[index++] = *pCurrent;
            //            }
            //        }
            //    }
            //}
            //return new string(newValue, 0, index);
        }
        public static string StripNot(string value, params char[] retainChars)
        {
            //Check params.
            ArgumentValidation.CheckForNullReference(value, "value");
            ArgumentValidation.CheckForNullReference(retainChars, "retainChars");

            string retainString = new string(retainChars);

            StringBuilder result = new StringBuilder(value.Length);
            foreach (var c in value)
            {
                if (retainString.IndexOf(c) != -1)
                {
                    result.Append(c);
                }
            }
            return result.ToString();

            //// Allocate a char[] rather than using a StringBuilder.
            //// This is potentially faster.
            ////
            //char[] newValue = new char[value.Length];
            //int index = 0;
            //// Managed pointer arithemetic is a faster way to iterate
            //// through a string. This can be several times faster than
            //// foreach (IEnumerable) iteration on very large strings.
            //// Although the 'unsafe' keyword is used, there is nothing
            //// unsafe about this code. The 'unsafe' keyword is often
            //// mistaken for 'unmanaged'. This is NOT the case.
            //unsafe
            //{
            //    fixed (char* pStart = value)
            //    {
            //        char* pCurrent = pStart;
            //        char* pEnd = (pStart + value.Length);
            //        for (; pCurrent < pEnd; pCurrent++)
            //        {
            //            if (searchString.IndexOf(*pCurrent) != -1)
            //            {
            //                newValue[index++] = *pCurrent;
            //            }
            //        }
            //    }
            //}
            //return new string(newValue, 0, index);
        }
        public static string StripWhiteSpace(string value)
        {
            return Strip(value, WhiteSpaceCharSet);
        }

        /// <summary>
        /// Normalize duplicate characters to a single character.
        /// </summary>
        public static string Normalize(string value, char normalizeTo, params char[] normalizeChars)
        {
            //Check params.
            ArgumentValidation.CheckForNullReference(value, "value");
            ArgumentValidation.CheckForNullReference(normalizeChars, "normalizeChars");

            string searchString = new string(normalizeChars);

            StringBuilder result = new StringBuilder(value.Length);
            bool lastChar = false;
            foreach (var c in value)
            {
                if (searchString.IndexOf(c) == -1)
                {
                    result.Append(c);
                }
                else
                {
                    if (!lastChar)
                    {
                        result.Append(normalizeTo);
                        lastChar = true;
                    }
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// Normalize all duplicate occurances of c.
        /// </summary>
        public static string Normalize(string value, char c)
        {
            return Normalize(value, c, c);
        }
        /// <summary>
        /// Normalize all internal white-space to single spaces.
        /// Trim start and end of string.
        /// </summary>
        public static string NormalizeWhiteSpace(string value)
        {
            return Normalize(value, ' ', WhiteSpaceCharSet).Trim(WhiteSpaceCharSet);
        }

        /// <summary>
        /// Strip a string to numeric only.
        /// </summary>
        public static string StripNotNumeric(string value)
        {
            return StripNot(value, NumericCharSet);
        }
        /// <summary>
        /// Strip a string to alpha only.
        /// </summary>
        public static string StripNotAlpha(string value)
        {
            return StripNot(value, AlphaCharSet);
        }

        /// <summary>
        /// Strip a string to alpha-numeric only.
        /// </summary>
        public static string StripNotAlphanumeric(string value)
        {
            return StripNot(value, AlphanumericCharSet);
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        #endregion
    }
}
