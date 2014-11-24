#region Imports

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MFramework.Domain.DataAnnotationsContrib.Interfaces;
using MFramework.Domain.DataAnnotationsContrib.Internal;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib
{
    /// <summary>
    /// </summary>
    public abstract class StringValidationAlgorithm : IValidationAlgorithm<String>
    {
        #region Constants and Enums
        protected const string NUMSEPCAP_REGEX = @"(?<separator>[-\s]?)";
        protected const string NUMSEPRPT_REGEX = @"\k<separator>";
        protected const string NUMSEP_REGEX = @"[-\s]?";
        protected const string SPACE_REGEX = @"[\s]?";
        protected const string DOTSPACE_REGEX = @"[\.\s]?";
        protected const string DASHSPACE_REGEX = @"[-\s]?";
        #endregion

        #region Inner Classes and Structures
        #endregion

        #region Delegates and Events
        #endregion

        #region Instance and Shared Fields
        private int? _minLength = null;
        private int? _maxLength = null;
        private Regex _formatRegex;

        private ICheckDigitAlgorithm _checkAlgorithm;
        #endregion

        #region Constructors
        protected StringValidationAlgorithm()
        { }
        protected StringValidationAlgorithm(Regex formatRegex)
        {
            //Check params.
            ArgumentValidation.CheckForNullReference(formatRegex, "formatRegex");

            _formatRegex = formatRegex;
        }
        protected StringValidationAlgorithm(int fixedLengthInclusive, Regex formatRegex)
            : this(fixedLengthInclusive, fixedLengthInclusive, formatRegex)
        { }
        protected StringValidationAlgorithm(int minLengthInclusive, int maxLengthInclusive)
        {
            _minLength = minLengthInclusive;
            _maxLength = maxLengthInclusive;
        }
        protected StringValidationAlgorithm(int minLengthInclusive, int maxLengthInclusive, Regex formatRegex)
        {
            //Check params.
            ArgumentValidation.CheckForNullReference(formatRegex, "formatRegex");

            _formatRegex = formatRegex;
            _minLength = minLengthInclusive;
            _maxLength = maxLengthInclusive;
        }

        protected StringValidationAlgorithm(
            int fixedLengthInclusive,
            Regex formatRegex,
            ICheckDigitAlgorithm checkAlgorithm)
            : this(fixedLengthInclusive, fixedLengthInclusive, formatRegex, checkAlgorithm)
        { }
        protected StringValidationAlgorithm(
            int minLengthInclusive, int maxLengthInclusive,
            Regex formatRegex,
            ICheckDigitAlgorithm checkAlgorithm)
            : this(minLengthInclusive, maxLengthInclusive, formatRegex)
        {
            //Check params.
            ArgumentValidation.CheckForNullReference(checkAlgorithm, "checkAlgorithm");

            _checkAlgorithm = checkAlgorithm;
        }
        protected StringValidationAlgorithm(
            Regex formatRegex,
            ICheckDigitAlgorithm checkAlgorithm)
            : this(formatRegex)
        {
            //Check params.
            ArgumentValidation.CheckForNullReference(checkAlgorithm, "checkAlgorithm");

            _checkAlgorithm = checkAlgorithm;
        }
        #endregion

        #region Properties
        protected Regex FormatRegex
        {
            get { return _formatRegex; }
            set { _formatRegex = value; }
        }
        protected ICheckDigitAlgorithm CheckAlgorithm
        {
            get { return _checkAlgorithm; }
            set { _checkAlgorithm = value; }
        }
        #endregion

        #region Private and Protected Methods
        /// <summary>
        /// By default this removes all non alphanumeric (not a-z A-Z 0-9) characters.
        /// </summary>
        protected virtual string Canononicalize(string value)
        {
            return TextHelper.StripNotAlphanumeric(value);
        }

        protected virtual bool ValidateMinLength(string canonicalValue)
        {
            if (_minLength == null) return true; //No min length validation required.

            int length = String.IsNullOrEmpty(canonicalValue) ? 0 : canonicalValue.Length;
            if (length < _minLength.Value) return false;
            else return true;
        }
        protected virtual bool ValidateMaxLength(string canonicalValue)
        {
            if (_maxLength == null) return true; //No max length validation required.

            int length = String.IsNullOrEmpty(canonicalValue) ? 0 : canonicalValue.Length;
            if (length > _maxLength.Value) return false;
            else return true;
        }
        protected virtual bool ValidateFormat(string rawValue)
        {
            if (_formatRegex == null) return true; //No regex validation required.

            //Match.                
            return _formatRegex.IsMatch(rawValue);
        }
        protected virtual bool ValidateChecksum(string canonicalValue)
        {
            if (_checkAlgorithm == null) return true; //No checksum validation required.

            //We can simply call verify.
            return _checkAlgorithm.Verify(canonicalValue);
        }
        protected virtual bool ValidateCustom(string rawValue, out string validationMessage)
        {
            validationMessage = "";
            return true;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        #endregion

        #region IValidationAlgorithm<String> Members
        public bool Validate(string value)
        {
            //Delegate to richer overload.
            IList<string> messages;
            return Validate(value, out messages);
        }
        public virtual bool Validate(string value, out IList<string> validationMessages)
        {
            try
            {
                ArgumentValidation.CheckStringForEmpty(value, "value");

                //Remove characters extraneous to validation.
                string canononicalValue = Canononicalize(value);
                string rawValue = value;

                //1 Length checks.
                if (!ValidateMinLength(canononicalValue))
                {
                    validationMessages = new string[] { SR.Validator_LengthMinFailure };
                    return false;
                }
                if (!ValidateMaxLength(canononicalValue))
                {
                    validationMessages = new string[] { SR.Validator_LengthMaxFailure };
                    return false;
                }

                //2 Format check.
                if (!ValidateFormat(rawValue))
                {
                    validationMessages = new string[] { SR.Validator_FormatFailure };
                    return false;
                }

                //3 Checksum.
                if (!ValidateChecksum(canononicalValue))
                {
                    validationMessages = new string[] { SR.Validator_ChecksumFailure };
                    return false;
                }

                //4 Custom.
                string message;
                if (!ValidateCustom(rawValue, out message))
                {
                    validationMessages = new string[] { message };
                    return false;
                }

                //OK!
                //Set default error messages to empty array.
                validationMessages = new string[] { };
                return true;
            }
            catch (Exception ex)
            {
                validationMessages = new string[] { ex.Message };
                return false;
            }
        }
        #endregion

        #region IValidationAlgorithm Members
        bool IValidationAlgorithm.Validate(object value)
        {
            return Validate(value as string);
        }
        bool IValidationAlgorithm.Validate(object value, out IList<string> validationMessages)
        {
            return Validate(value as string, out validationMessages);
        }
        #endregion
    }
}