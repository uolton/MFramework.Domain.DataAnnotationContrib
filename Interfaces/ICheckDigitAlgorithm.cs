#region Imports



#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Interfaces
{
    /// <summary>
    /// </summary>
    public interface ICheckDigitAlgorithm
    {
        #region Delegates and Events
        #endregion

        #region Properties
        Enums.CheckDigitPosition DigitPosition { get; }
        int DigitCount { get; }
        #endregion

        #region Public Methods
        /// <summary> Add check digits to a string containing digits.
        /// </summary>
        string AddCheckDigit(string data);
        /// <summary>Extracts the raw data without the check digits.
        /// </summary>
        string RemoveCheckDigit(string protectedData);

        /// <summary> Verify a string that has been encoded with a check digit.
        /// </summary>
        bool Verify(string protectedData);

        /// <summary> Computes the check digit value.
        /// </summary>
        string ComputeCheckDigit(string data);
        /// <summary> Extract just the check digits from an encoded string.
        /// </summary>
        string ExtractCheckDigit(string protectedData);
        #endregion
    }
}