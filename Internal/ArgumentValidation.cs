#region Imports

using System;
using System.Collections;
using System.Text.RegularExpressions;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Internal
{
    /// <summary>
    /// </summary>
    internal static class ArgumentValidation
    {
        #region Constants and Enums
        #endregion

        #region Inner Classes and Structures
        #endregion

        #region Delegates and Events
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
        public static void CheckForNullReference(object variable, string variableName)
        {
            CheckStringForEmpty(variableName, "variableName");

            if (variable == null)
            {
                throw new ArgumentNullException(variableName);
            }
        }

        public static void CheckStringForEmpty(string variable, string variableName)
        {
            if (String.IsNullOrEmpty(variableName))
            {
                throw new ArgumentException(SR.ArgumentException_EmptyString, variableName);
            }
            if (String.IsNullOrEmpty(variable))
            {
                throw new ArgumentException(SR.ArgumentException_EmptyString, variableName);
            }
        }
        public static void CheckStringForRegexMatch(string variable, string regEx, string variableName)
        {
            CheckStringForEmpty(variableName, "variableName");
            CheckStringForEmpty(variable, variableName);
            CheckStringForEmpty(regEx, "regEx");

            if (!Regex.IsMatch(variable, regEx))
            {
                throw new FormatException(String.Format(SR.FormatException_NotRegexMatch, regEx, variable));
            }
        }
        public static void CheckStringForRegexMatch(string variable, Regex regEx, string variableName)
        {
            CheckStringForEmpty(variableName, "variableName");
            CheckStringForEmpty(variable, variableName);
            CheckForNullReference(regEx, "regEx");

            if (!regEx.IsMatch(variable))
            {
                throw new FormatException(String.Format(SR.FormatException_NotRegexMatch, regEx, variable));
            }
        }

        public static void CheckNumberIsLessThan(int variable, int maximum, string variableName)
        {
            CheckNumberIsLessThan(variable, maximum, false, variableName);
        }
        public static void CheckNumberIsLessThan(int variable, int maximum, bool inclusive, string variableName)
        {
            CheckStringForEmpty(variableName, "variableName");

            if ((variable > maximum) || ((variable == maximum) && (!inclusive)))
            {
                throw new ArgumentOutOfRangeException(variableName,
                    String.Format(SR.ArgumentOutOfRangeException_NotLessThan, maximum, variable));
            }
        }

        public static void CheckArrayForSpecificLength(Array variable, int requiredLength, string variableName)
        {
            CheckStringForEmpty(variableName, "variableName");
            CheckForNullReference(variable, variableName);

            if (variable.Length != requiredLength)
            {
                throw new ArgumentException(
                    String.Format(SR.ArgumentException_InvalidArrayLength, requiredLength), variableName);
            }
        }
        public static void CheckArrayHasItems(Array variable, string variableName)
        {
            CheckStringForEmpty(variableName, "variableName");
            CheckForNullReference(variable, variableName);

            if (variable.Length == 0)
            {
                throw new ArgumentException(SR.ArgumentException_EmptyArray, variableName);
            }
        }

        public static void CheckNumberIsValidIndex(int variable, IList list, string listName, string variableName)
        {
            CheckStringForEmpty(listName, "listName");
            CheckStringForEmpty(variableName, "variableName");
            CheckForNullReference(list, "list");

            if ((variable < 0) || (variable > (list.Count - 1)))
            {
                throw new ArgumentOutOfRangeException(variableName,
                    String.Format(SR.ArgumentOutOfRangeException_NotValidIndex, listName, 0, list.Count, variable));
            }
        }
        public static void CheckNumberIsValidIndex(int variable, Array array, string arrayName, string variableName)
        {
            CheckStringForEmpty(arrayName, "arrayName");
            CheckStringForEmpty(variableName, "variableName");
            CheckForNullReference(array, "list");

            if ((variable < 0) || (variable > (array.Length - 1)))
            {
                throw new ArgumentOutOfRangeException(variableName,
                    String.Format(SR.ArgumentOutOfRangeException_NotValidIndex, arrayName, 0, (array.Length - 1), variable));
            }
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        #endregion
    }
}