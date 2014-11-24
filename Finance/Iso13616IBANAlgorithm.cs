#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MFramework.Domain.DataAnnotationsContrib.Internal;
using MFramework.Domain.DataAnnotationsContrib.Resources;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Finance
{
    /// <summary>
    ///	The International Bank Account Number (IBAN) is an international standard for identifying bank accounts across national borders.
    ///	It was originally adopted by the European Committee for Banking Standards, and was later adopted as 
    ///	ISO 13616:1997 and now as ISO 13616:2003. 
    ///	The official IBAN registrar under ISO 13616:2003 is SWIFT and the IBAN registry is currently at SWIFT.
    ///
    /// The checksum is a basic ISO 7064 mod 97-10 calculation where the remainder must equal 1.
    /// To validate the checksum:
    ///   1. Check that the total IBAN length is correct as per the country. If not, the IBAN is invalid.
    ///   2. Move the four initial characters to the end of the string.
    ///   3. Replace the letters in the string with digits, expanding the string as necessary, such that A=10, B=11 and Z=35.
    ///   4. Convert the string to an integer and mod-97 the entire number.
    /// If the remainder is 1 you have a valid IBAN number.
    /// 
    /// To calculate the checksum, make the checksum digits 00 (e.g. GB00 for the UK), 
    /// move the four initial character to the end of the string, get the remainder (mod-97) and subtract from 98.
    /// </summary>
    /// <remarks>
    /// This was coded directly from authoritative source updated June 14, 2009.
    /// See: http://www.swift.com/solutions/messaging/information_products/directory_products/iban_format_registry/index.page
    /// See: http://www.swift.com/solutions/messaging/information_products/bic_downloads_documents/pdfs/IBAN_Registry.pdf 
    /// </remarks>
    public class Iso13616IBANAlgorithm : StringValidationAlgorithm
    {
        #region Constants and Enums
        private const string REGEX_FORMAT = @"^[a-z|A-Z]{2}" + NUMSEPCAP_REGEX + @"\d{2}" + NUMSEPRPT_REGEX + @"[a-z|A-Z|\d]{0,30}$";

        private const int CHECKSUM_MODULUS = 97;
        private const int CHECKSUM_ALPHAPOSOFFSET = 9;
        #endregion

        #region Inner Classes and Structures
        [DebuggerDisplay("{CountryCode} ({CountryName}):{FullRegEx}")]
        public class Iso13616IBANStructure
        {
            private Iso13616IBANStructure()
            {
            }

            public string CountryCode { get; private set; }
            public string CountryName { get; private set; }

            public int TotalMaxLength { get; private set; }
            public bool IsFixedLength { get; private set; }
            public string FullRegEx { get; private set; }
            public int BankIdStart { get; private set; }
            public int BankIdLength { get; private set; }
            public int BranchIdStart { get; private set; }
            public int BranchIdLength { get; private set; }
            public int BankAndBranchChecksumPosition { get; private set; }
            public int AccountStart { get; private set; }
            public int AccountLength { get; private set; }

            public bool DefinesBankId { get { return (this.BankIdStart != -1 && this.BankIdLength != -1); } }
            public bool DefinesBranchId { get { return (this.BranchIdStart != -1 && this.BranchIdLength != -1); } }
            public bool DefinesAccount { get { return (this.AccountStart != -1 && this.AccountLength != -1); } }

            internal static Iso13616IBANStructure Parse(string line)
            {
                try
                {
                    ArgumentValidation.CheckStringForEmpty(line, "line");
                    string[] parts = line.Split(ResourceHelper.ResourceLineItemSeparator);
                    ArgumentValidation.CheckArrayForSpecificLength(parts, 12, "line.Split('" + ResourceHelper.ResourceLineItemSeparator + "')");

                    return new Iso13616IBANStructure()
                    {
                        CountryCode = parts[0],
                        CountryName = parts[1],
                        TotalMaxLength = Int32.Parse(parts[2]),
                        IsFixedLength = Boolean.Parse(parts[3]),
                        FullRegEx = parts[4],
                        BankIdStart = Int32.Parse(parts[5]),
                        BankIdLength = Int32.Parse(parts[6]),
                        BranchIdStart = Int32.Parse(parts[7]),
                        BranchIdLength = Int32.Parse(parts[8]),
                        BankAndBranchChecksumPosition = Int32.Parse(parts[9]),
                        AccountStart = Int32.Parse(parts[10]),
                        AccountLength = Int32.Parse(parts[11]),
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
        private static Dictionary<string, Iso13616IBANStructure> __formatStructures;
        private static ReadOnlyCollection<Iso13616IBANStructure> __readonlyFormatStructures;
        #endregion

        #region Constructors
        static Iso13616IBANAlgorithm()
        {
            //Load and cache individual country formats.

            __formatStructures = ResourceHelper.ReadFromResourceStream("Iso13616IBANAlgorithmFormatData.csv",
                s => Iso13616IBANStructure.Parse(s),
                s => s.Substring(0, 2),
                ResourceHelper.ResourceCommentLinePrefix);

            //Public readonly collection copy.
            __readonlyFormatStructures = AlgorithmHelper.CopyValuesToReadOnlyCollection<string, Iso13616IBANStructure>(__formatStructures);
        }

        public Iso13616IBANAlgorithm()
            : base(4, 32, new Regex(REGEX_FORMAT))
        { }
        #endregion

        #region Properties
        public static ReadOnlyCollection<Iso13616IBANStructure> Formats
        {
            get { return __readonlyFormatStructures; }
        }
        #endregion

        #region Private and Protected Methods
        private static Iso13616IBANStructure GetStructure(string iban)
        {
            return __formatStructures[ExtractCountryCode(iban)];
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Determines the country from the first two characters of the IBAN passed.
        /// </summary>
        public static string ExtractCountryCode(string iban)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(iban, "iban");
            ArgumentValidation.CheckStringForRegexMatch(iban, REGEX_FORMAT, "iban");

            try
            {
                //Get prefix index.
                return iban.Substring(0, 2);
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "Country Code"));
            }
        }
        /// <summary>
        /// Extracts the BBAN (everything following the country code and checksum) from the IBAN passed.
        /// </summary>
        public static string ExtractFullBasicBankAccountNumber(string iban)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(iban, "iban");
            ArgumentValidation.CheckStringForRegexMatch(iban, REGEX_FORMAT, "iban");

            try
            {
                //Get prefix index.
                return iban.Substring(4);
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "BBAN"));
            }
        }

        /// <summary>
        /// Extracts the country specific bank identifier component of the IBAN passed.
        /// </summary>
        /// <returns>Empty string if this country does not define specific rules for bank identifier within their BBAN format.</returns>
        public static string ExtractBankIdentifier(string iban)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(iban, "iban");
            ArgumentValidation.CheckStringForRegexMatch(iban, REGEX_FORMAT, "iban");

            try
            {
                //Get structure.
                var structure = GetStructure(iban);
                //Extract.
                if (!structure.DefinesBankId) return "";
                return iban.Substring(structure.BankIdStart, structure.BankIdLength);
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "Bank Identifier"));
            }
        }
        /// <summary>
        /// Extracts the country specific branch identifier component of the IBAN passed.
        /// </summary>
        /// <returns>Empty string if this country does not define specific rules for branch identifier within their BBAN format.</returns>
        public static string ExtractBranchIdentifier(string iban)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(iban, "iban");
            ArgumentValidation.CheckStringForRegexMatch(iban, REGEX_FORMAT, "iban");

            try
            {
                //Get structure.
                var structure = GetStructure(iban);
                //Extract.
                if (!structure.DefinesBranchId) return "";
                return iban.Substring(structure.BranchIdStart, structure.BranchIdLength);
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "Branch Identifier"));
            }
        }
        /// <summary>
        /// Extracts the individual account number (valid only domestically in the context of the bank and branch identifiers) from the IBAN passed.
        /// </summary>
        /// <returns>Empty string if this country does not define specific rules for bank/branch/account identifiers within their BBAN format.</returns>
        public static string ExtractAccountIdentifier(string iban)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(iban, "iban");
            ArgumentValidation.CheckStringForRegexMatch(iban, REGEX_FORMAT, "iban");

            try
            {
                //Get structure.
                var structure = GetStructure(iban);
                //Extract.
                if (!structure.DefinesAccount) return "";
                return iban.Substring(structure.AccountStart, structure.AccountLength);
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "Account Identifier"));
            }
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        protected override bool ValidateChecksum(string canonicalValue)
        {
            //Append the first four charcaters to the end.
            string reformattedNumber = canonicalValue.Substring(4) + canonicalValue.Substring(0, 4);
            //Convert to 'numeric' string using position in alphabet + 9 for letters or as-is for numbers.
            string numericNumber = AlgorithmHelper.GetNumericEquivalentOfStringAsString(
                reformattedNumber,
                delegate(char c, int p) { return TextHelper.IsNumeric(c) ? TextHelper.GetNumericValueOfChar(c) : TextHelper.GetLatinAlphabetPosition(c) + CHECKSUM_ALPHAPOSOFFSET; });

            //Perform mod.
            return AlgorithmHelper.Mod(numericNumber, CHECKSUM_MODULUS) == 1;
        }
        protected override bool ValidateCustom(string rawValue, out string validationMessage)
        {
            //Remove all non alphanumeric characters.
            string strippedNumber = TextHelper.StripNotAlphanumeric(rawValue);

            //Check it is a valid country.
            string countryCode = strippedNumber.Substring(0, 2);
            if (!__formatStructures.ContainsKey(countryCode))
            {
                validationMessage = SR.Iso13616IBANAlgorithm_InvalidCountry;
                return false;
            }

            //Get structure.
            var structure = GetStructure(strippedNumber);
            if (
                (structure.IsFixedLength && strippedNumber.Length != structure.TotalMaxLength)
                || (strippedNumber.Length > structure.TotalMaxLength)
                )
            {
                validationMessage = SR.Iso13616IBANAlgorithm_InvalidBBANLength;
                return false;
            }

            if (!Regex.IsMatch(strippedNumber, structure.FullRegEx))
            {
                validationMessage = SR.Iso13616IBANAlgorithm_InvalidBBANFormat;
                return false;
            }

            validationMessage = "";
            return true;
        }
        #endregion
    }
}
