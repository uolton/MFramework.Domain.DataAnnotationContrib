#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MFramework.Domain.DataAnnotationsContrib.Internal;
using MFramework.Domain.DataAnnotationsContrib.Resources;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Commerce
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// This was coded from wikipedia and authoritative source (UPU/USPS/BS7666...).
    /// See: http://www.geopostcodes.com
    /// See: http://www.upu.int/post_code/en/list_of_sites_by_country.html
    /// See: http://www.columbia.edu/kermit/postal.html
    /// </remarks>
    public class UpuPostalCodeAlgorithm : StringValidationAlgorithm
    {
        #region Constants and Enums
        #endregion

        #region Inner Classes and Structures
        [DebuggerDisplay("{CountryCode} ({CountryName}):{FullRegEx}")]
        public class UpuPostalCodeStructure
        {
            private UpuPostalCodeStructure()
            {
            }

            public string CountryCode { get; private set; }
            public string CountryName { get; private set; }

            public int TotalMaxLength { get; private set; }
            public bool IsFixedLength { get; private set; }
            public string FullRegEx { get; private set; }

            public string Comment { get; private set; }

            internal static UpuPostalCodeStructure Parse(string line)
            {
                try
                {
                    ArgumentValidation.CheckStringForEmpty(line, "line");
                    string[] parts = line.Split(ResourceHelper.ResourceLineItemSeparator);
                    ArgumentValidation.CheckArrayForSpecificLength(parts, 6, "line.Split('" + ResourceHelper.ResourceLineItemSeparator + "')");

                    return new UpuPostalCodeStructure()
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
        private static Dictionary<string, UpuPostalCodeStructure> __formatStructures;
        private static ReadOnlyCollection<UpuPostalCodeStructure> __readonlyFormatStructures;
        private List<UpuPostalCodeStructure> _validFormatStructures;
        #endregion

        #region Constructors
        static UpuPostalCodeAlgorithm()
        {
            __formatStructures = ResourceHelper.ReadFromResourceStream("UpuPostalCodeAlgorithmFormatData.csv",
                s => UpuPostalCodeStructure.Parse(s),
                s => s.Substring(0, 2),
                ResourceHelper.ResourceCommentLinePrefix);

            //Public readonly collection copy.
            __readonlyFormatStructures = AlgorithmHelper.CopyValuesToReadOnlyCollection<string, UpuPostalCodeStructure>(__formatStructures);
        }

        /// <summary>
        /// Accept postal codes from all known countries.
        /// </summary>
        public UpuPostalCodeAlgorithm()
            : base(1, 20)
        {
            _validFormatStructures = new List<UpuPostalCodeStructure>();
            //Add all known countries.
            foreach (var item in __formatStructures)
            {
                _validFormatStructures.Add(item.Value);
            }
        }
        public UpuPostalCodeAlgorithm(string acceptedCountry)
            : this(new string[] { acceptedCountry })
        {
        }
        public UpuPostalCodeAlgorithm(string[] acceptedCountries)
            : base(1, 20)
        {
            //Check params.
            ArgumentValidation.CheckArrayHasItems(acceptedCountries, "acceptedCountries");

            _validFormatStructures = new List<UpuPostalCodeStructure>();
            //Add specified countries.
            foreach (var country in acceptedCountries)
            {
                if (!__formatStructures.ContainsKey(country)) throw new ArgumentException(String.Format(SR.UpuPostalCodeAlgorithm_InvalidCountry, country));
                _validFormatStructures.Add(__formatStructures[country]);
            }
        }
        #endregion

        #region Properties
        public static ReadOnlyCollection<UpuPostalCodeStructure> Formats
        {
            get { return __readonlyFormatStructures; }
        }
        #endregion

        #region Private and Protected Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Determines the country from the structure of the postal code passed.
        /// Will almost always match more than one country, so this is likely to be of limited utility except to exclude
        /// countries.
        /// </summary>
        public static string[] InferCountryCode(string postalCode)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(postalCode, "postalCode");

            //Force uppercase.
            //TODO: 2009-11-11: MSJ Need to decide if this a good choice, all regexs are currently written assuming uppercase
            //and so this effectively makes the validation case insensitive, if all countries postal codes turn out to be case-insensitive this can be left as-is.
            string strippedNumber = postalCode.ToUpperInvariant();//TextHelper.StripWhiteSpace(rawValue).ToUpperInvariant();

            List<string> matchingCountries = new List<string>();
            //Check all known countries.
            foreach (var item in __formatStructures)
            {
                if (Regex.IsMatch(strippedNumber, item.Value.FullRegEx))
                {
                    matchingCountries.Add(item.Key);
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
            //Force uppercase.
            //TODO: 2009-11-11: MSJ Need to decide if this a good choice, all regexs are currently written assuming uppercase
            //and so this effectively makes the validation case insensitive, if all countries postal codes turn out to be case-insensitive this can be left as-is.
            string strippedNumber = rawValue.ToUpperInvariant();//TextHelper.StripWhiteSpace(rawValue).ToUpperInvariant();

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
