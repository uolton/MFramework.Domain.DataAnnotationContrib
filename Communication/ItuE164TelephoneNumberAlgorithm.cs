#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MFramework.Domain.DataAnnotationsContrib.Internal;
using MFramework.Domain.DataAnnotationsContrib.Resources;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Communication
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class ItuE164TelephoneNumberAlgorithm : StringValidationAlgorithm
    {
        #region Constants and Enums
        internal const string PLUS = "+";
        #endregion

        #region Inner Classes and Structures
        [DebuggerDisplay("{CountryCode} ({CountryName}):{FullRegEx}")]
        public class ItuE164TelephoneNumberStructure
        {
            private ItuE164TelephoneNumberStructure()
            {
            }

            public string CountryCode { get; private set; }
            public string CountryName { get; private set; }

            public int TotalMaxLength { get; private set; }
            public bool IsFixedLength { get; private set; }
            public string FullRegEx { get; private set; }

            public string Comment { get; private set; }

            internal static ItuE164TelephoneNumberStructure Parse(string line)
            {
                try
                {
                    ArgumentValidation.CheckStringForEmpty(line, "line");
                    string[] parts = line.Split(ResourceHelper.ResourceLineItemSeparator);
                    ArgumentValidation.CheckArrayForSpecificLength(parts, 6, "line.Split('" + ResourceHelper.ResourceLineItemSeparator + "')");

                    return new ItuE164TelephoneNumberStructure()
                    {
                        CountryCode = parts[0],
                        CountryName = parts[1],
                        TotalMaxLength = Int32.Parse(parts[2]),
                        IsFixedLength = Boolean.Parse(parts[3]),
                        FullRegEx = parts[4],
                        Comment = parts[5]
                    };
                }
                catch (Exception ex)
                {
                    throw new FormatException(String.Format("Error parsing line {0}.", line), ex);
                }
            }
        }
        #endregion

        #region Delegates and Events
        #endregion

        #region Instance and Shared Fields
        private static Dictionary<string, ItuE164TelephoneNumberStructure> __formatStructures;
        private static ReadOnlyCollection<ItuE164TelephoneNumberStructure> __readonlyFormatStructures;
        private List<ItuE164TelephoneNumberStructure> _validFormatStructures;
        #endregion

        #region Constructors
        static ItuE164TelephoneNumberAlgorithm()
        {
            __formatStructures = ResourceHelper.ReadFromResourceStream("ItuE164TelephoneNumberAlgorithmFormatData.csv",
                s => ItuE164TelephoneNumberStructure.Parse(s),
                s => s.Substring(0, 2),
                ResourceHelper.ResourceCommentLinePrefix);

            //Public readonly collection copy.
            __readonlyFormatStructures = AlgorithmHelper.CopyValuesToReadOnlyCollection<string, ItuE164TelephoneNumberStructure>(__formatStructures);
        }

        /// <summary>
        /// Accept numbers from all known countries.
        /// </summary>
        public ItuE164TelephoneNumberAlgorithm()
            : base()
        {
            _validFormatStructures = new List<ItuE164TelephoneNumberStructure>();
            //Add all known countries.
            foreach (var item in __formatStructures)
            {
                _validFormatStructures.Add(item.Value);
            }
        }
        public ItuE164TelephoneNumberAlgorithm(string acceptedCountry)
            : this(new string[] { acceptedCountry })
        {
        }
        public ItuE164TelephoneNumberAlgorithm(string[] acceptedCountries)
            : base()
        {
            //Check params.
            ArgumentValidation.CheckArrayHasItems(acceptedCountries, "acceptedCountries");

            _validFormatStructures = new List<ItuE164TelephoneNumberStructure>();
            //Add specified countries.
            foreach (var country in acceptedCountries)
            {
                if (!__formatStructures.ContainsKey(country)) throw new ArgumentException(String.Format(SR.UpuPostalCodeAlgorithm_InvalidCountry, country));
                _validFormatStructures.Add(__formatStructures[country]);
            }
        }
        #endregion

        #region Properties
        public static ReadOnlyCollection<ItuE164TelephoneNumberStructure> Formats
        {
            get { return __readonlyFormatStructures; }
        }
        #endregion

        #region Private and Protected Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Determines the country from the first few (varying number) digits of the number passed.
        /// Will only ever match zero or one country.
        /// </summary>
        public static string[] InferCountryCode(string telephoneNumber)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(telephoneNumber, "telephoneNumber");

            bool hasPlus = telephoneNumber.StartsWith(PLUS);
            if (!hasPlus) return new string[] { };

            //Strip whitespace.
            string strippedNumber = TextHelper.StripNotNumeric(telephoneNumber);

            List<string> matchingCountries = new List<string>();
            //Check all known countries.
            foreach (var item in __formatStructures)
            {
                if (Regex.IsMatch(strippedNumber, item.Value.FullRegEx))
                {
                    matchingCountries.Add(item.Key);
                    break;
                }
            }
            return matchingCountries.ToArray();
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        protected override bool ValidateCustom(string rawValue, out string validationMessage)
        {
            bool hasPlus = rawValue.StartsWith(PLUS);
            string strippedNumber = TextHelper.StripNotNumeric(rawValue);

            //Get structures.
            foreach (var structure in _validFormatStructures)
            {
                if (
                    (structure.IsFixedLength && strippedNumber.Length == structure.TotalMaxLength)
                    || (strippedNumber.Length <= structure.TotalMaxLength)
                    )
                {
                    if (Regex.IsMatch(strippedNumber, structure.FullRegEx))
                    {
                        //We have matched against at least one country.
                        validationMessage = "";
                        return true;
                    }
                }
            }

            //We have failed to match against any country.
            validationMessage = SR.Validator_FormatFailure;
            return false;
        }
        #endregion
    }
}
