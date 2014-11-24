#region Imports

using System;
using System.Text.RegularExpressions;
using MFramework.Domain.DataAnnotationsContrib.Internal;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Finance
{
    /// <summary>
    /// ISO 6166 defines the structure of an International Securities Identifying Number (ISIN).
    /// An ISIN uniquely identifies a fungible security. Securities with which ISINs can be used are Equities, Fixed income & ETF's only.
    /// ISINs consist of two alphabetic characters, which are the ISO 3166-1 alpha-2 code for the issuing country,
    /// nine alpha-numeric digits (the National Securities Identifying Number, or NSIN, which identifies the security),
    /// and one numeric check digit. 
    /// 
    /// The ISIN is issued by a national numbering agency (NNA) for that country. 
    /// Regional substitute NNAs have been allocated the task of functioning as NNAs in those countries where NNAs have not yet been established.
    /// </summary>
    /// <remarks>
    /// See http://en.wikipedia.org/wiki/ISO_6166
    /// </remarks>
    public class Iso6166ISINAlgorithm : StringValidationAlgorithm
    {
        #region Constants and Enums
        private const string REGEX_FORMAT = @"^([a-z|A-Z]{2})" + NUMSEP_REGEX + @"([a-z|A-Z|0-9]{9})\d{1}$";

        private const int CHECKSUM_MODULUS = 10;
        private const int CHECKSUM_ALPHAPOSOFFSET = 9;
        #endregion

        #region Inner Classes and Structures
        #endregion

        #region Delegates and Events
        #endregion

        #region Instance and Shared Fields
        #endregion

        #region Constructors
        public Iso6166ISINAlgorithm()
            : base(12, new Regex(REGEX_FORMAT))
        { }
        #endregion

        #region Properties
        #endregion

        #region Private and Protected Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Determines the country of issuance from the first two cahracters of the ISIN passed.
        /// </summary>
        public static string ExtractCountryCode(string isin)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(isin, "isin");
            ArgumentValidation.CheckStringForRegexMatch(isin, REGEX_FORMAT, "isin");

            try
            {
                //Get prefix index.
                return isin.Substring(0, 2);
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "Country Code"));
            }
        }

        /// <summary>
        /// Extracts the 9 digit National Security Identifiying Number (ie CUSIP for US, CEDOL for UK etc...)
        /// </summary>
        public static string ExtractNSIN(string isin, bool trimLeadingZeros)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(isin, "isin");
            ArgumentValidation.CheckStringForRegexMatch(isin, REGEX_FORMAT, "isin");

            try
            {
                //Get NSIN.
                string nsin = isin.Substring(2, 9);
                //Trim.
                return (trimLeadingZeros) ? nsin.TrimStart('0') : nsin;
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "NSIN"));
            }
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        protected override bool ValidateChecksum(string canonicalValue)
        {
            //Extract applied checksum (final number).
            int appliedChecksum = Int32.Parse(canonicalValue.Substring(canonicalValue.Length - 1));
            //Convert to 'numeric' string using position in alphabet + 9 for letters or as-is for numbers.
            string numericNumber = AlgorithmHelper.GetNumericEquivalentOfStringAsString(
                canonicalValue.Substring(0, canonicalValue.Length - 1),
                delegate(char c, int p) { return TextHelper.IsNumeric(c) ? TextHelper.GetNumericValueOfChar(c) : TextHelper.GetLatinAlphabetPosition(c) + CHECKSUM_ALPHAPOSOFFSET; });

            int sum = 0;
            for (int i = numericNumber.Length - 1; i >= 0; i--)
            {
                string loopDigit = numericNumber.Substring(i, 1);
                int loopDigitValue = Int32.Parse(loopDigit);
                if (AlgorithmHelper.IsEven(numericNumber.Length - 1 - i)) loopDigitValue *= 2;
                loopDigitValue = AlgorithmHelper.CollapseDigits(loopDigitValue);
                sum += loopDigitValue;
            }

            //Perform mod.
            int mod = sum % CHECKSUM_MODULUS;
            //if (mod == 10) return false;
            mod = CHECKSUM_MODULUS - mod;
            int calculatedChecksum = mod % CHECKSUM_MODULUS;

            return (calculatedChecksum == appliedChecksum);
        }
        #endregion
    }
}