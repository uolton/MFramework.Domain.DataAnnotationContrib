#region Imports

using System;
using MFramework.Domain.DataAnnotationsContrib.Interfaces;
using MFramework.Domain.DataAnnotationsContrib.Internal;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib.CheckDigits
{
    /// <summary>
    /// Represents a generic numerical (string) checksum algorithm.
    /// </summary>
    public abstract class CheckDigitAlgorithm : ICheckDigitAlgorithm
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
        protected CheckDigitAlgorithm(Enums.CheckDigitPosition digitPosition, int digitCount)
        {
            this.DigitPosition = digitPosition;
            this.DigitCount = digitCount;
        }
        #endregion

        #region Properties
        public Enums.CheckDigitPosition DigitPosition { get; private set; }
        public int DigitCount { get; private set; }
        #endregion

        #region Private and Protected Methods
        protected abstract string ComputeCheckDigitOverride(string data);

        private string CombineDataAndChecksum(string data, string checksum)
        {
            if (this.DigitPosition == Enums.CheckDigitPosition.Start)
            {
                return checksum + data;
            }
            else
            {
                return data + checksum;
            }
        }
        private void ExtractDataAndChecksum(string value, out string data, out string checkDigits)
        {
            if (this.DigitPosition == Enums.CheckDigitPosition.Start)
            {
                data = value.Substring(this.DigitCount);
                checkDigits = value.Substring(0, this.DigitCount);
            }
            else
            {
                data = value.Substring(0, value.Length - this.DigitCount);
                checkDigits = value.Substring(value.Length - this.DigitCount);
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        #endregion

        #region ICheckDigitAlgorithm Members
        public string AddCheckDigit(string data)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(data, "data");

            return CombineDataAndChecksum(data, ComputeCheckDigitOverride(data));
        }
        public string RemoveCheckDigit(string protectedData)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(protectedData, "protectedData");

            string data;
            string checkDigits;
            ExtractDataAndChecksum(protectedData, out data, out checkDigits);
            return data;
        }
        public string ExtractCheckDigit(string protectedData)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(protectedData, "protectedData");

            string data;
            string checkDigits;
            ExtractDataAndChecksum(protectedData, out data, out checkDigits);
            return checkDigits;
        }

        public bool Verify(string protectedData)
        {
            try
            {
                //Check params.
                if (String.IsNullOrEmpty(protectedData)) return false;
                ArgumentValidation.CheckStringForEmpty(protectedData, "protectedData");

                string data;
                string checkDigits;
                ExtractDataAndChecksum(protectedData, out data, out checkDigits);
                string computedCheckDigits = ComputeCheckDigit(data);
                return (computedCheckDigits == checkDigits);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public string ComputeCheckDigit(string data)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(data, "data");

            //Call derived class.
            return ComputeCheckDigitOverride(data);
        }
        #endregion
    }
}
