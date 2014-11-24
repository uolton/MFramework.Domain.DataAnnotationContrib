#region Imports

using System;
using System.Text.RegularExpressions;
using MFramework.Domain.DataAnnotationsContrib.Internal;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Finance
{
    /// <summary>
    /// ISO 9362 (also known as SWIFT-BIC, BIC code, SWIFT ID or SWIFT code) is a standard format of Bank Identifier Codes
    /// approved by the International Organization for Standardization (ISO).
    /// It is the unique identification code of a particular bank. 
    /// These codes are used when transferring money between banks, particularly for international wire transfers, 
    /// and also for the exchange of other messages between banks. The codes can sometimes be found on account statements.
    /// The SWIFT code is 8 or 11 characters, made up of:
    /// * 4 characters - bank code (only letters)
    /// * 2 characters - ISO 3166-1 alpha-2 country code (only letters)
    /// * 2 characters - location code (letters and digits) (if the second character is '1', then it denotes a passive participant in the SWIFT network)
    /// * 3 characters - branch code, optional ('XXX' for primary office) (letters and digits)
    /// Where an 8-digit code is given, it may be assumed that it refers to the primary office.
    /// </summary>
    /// <remarks>
    /// This was coded directly from authoritative source (SWIFT).
    /// See: http://www.swift.com/biconline/
    /// See: http://en.wikipedia.org/wiki/ISO_9362
    /// </remarks>
    public class Iso9362SWIFTBICAlgorithm : StringValidationAlgorithm
    {
        #region Constants and Enums
        public static readonly string PrimaryBranchIdentifier = "XXX";
        private const string REGEX_FORMAT = @"^[a-z|A-Z]{4}[a-z|A-Z]{2}[a-z|A-Z|\d]{2}([a-z|A-Z|\d]{3}){0,1}$";
        #endregion

        #region Inner Classes and Structures
        #endregion

        #region Delegates and Events
        #endregion

        #region Instance and Shared Fields
        #endregion

        #region Constructors
        public Iso9362SWIFTBICAlgorithm()
            : this(false)
        { }
        public Iso9362SWIFTBICAlgorithm(bool requireBranchCode)
            : base(8, 11, new Regex(REGEX_FORMAT))
        { }
        #endregion

        #region Properties
        #endregion

        #region Private and Protected Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Determines the country from the 5th and 6th characters of the BIC passed.
        /// </summary>
        public static string ExtractCountryCode(string bic)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(bic, "bic");
            ArgumentValidation.CheckStringForRegexMatch(bic, REGEX_FORMAT, "bic");

            try
            {
                return bic.Substring(4, 2);
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "Country Code"));
            }
        }
        /// <summary>
        /// Extracts the 4 digit bank identifier component of the BIC passed.
        /// </summary>
        public static string ExtractBankIdentifier(string bic)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(bic, "bic");
            ArgumentValidation.CheckStringForRegexMatch(bic, REGEX_FORMAT, "bic");

            try
            {
                return bic.Substring(0, 4);
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "Bank Identifier"));
            }
        }
        /// <summary>
        /// Extracts the location identifier component of the BIC passed.
        /// </summary>
        public static string ExtractLocationIdentifier(string bic)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(bic, "bic");
            ArgumentValidation.CheckStringForRegexMatch(bic, REGEX_FORMAT, "bic");

            try
            {
                return bic.Substring(6, 2);
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "Location Identifier"));
            }
        }
        /// <summary>
        /// Extracts the branch identifier component of the BIC passed.
        /// </summary>
        /// <returns>Empty string if this optional part of the BIC is not present.</returns>
        public static string ExtractBranchIdentifier(string bic)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(bic, "bic");
            ArgumentValidation.CheckStringForRegexMatch(bic, REGEX_FORMAT, "bic");

            try
            {
                if (bic.Length > 8) return bic.Substring(6, 3);
                return "";
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "Branch Identifier"));
            }
        }

        /// <summary>
        /// Extracts whether the branch identifier component of the BIC passed represents the primary branch.
        /// </summary>
        public static bool ExtractPrimaryBranchFlag(string bic)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(bic, "bic");
            ArgumentValidation.CheckStringForRegexMatch(bic, REGEX_FORMAT, "bic");

            try
            {
                string branchIdentifier = ExtractBranchIdentifier(bic);
                return (branchIdentifier == "" || AlgorithmHelper.StringEqualsIgnoreCase(branchIdentifier, PrimaryBranchIdentifier));
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "Branch Identifier"));
            }
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        #endregion
    }
}