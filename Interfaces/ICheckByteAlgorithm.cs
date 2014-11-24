#region Imports



#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Interfaces
{
    /// <summary>
    /// </summary>
    public interface ICheckByteAlgorithm : IValidationAlgorithm
    {
        #region Delegates and Events
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        /// <summary> Add check digits to a string containing digits.
        /// </summary>
        byte[] AddCheckByte(byte[] data);
        /// <summary>Extracts the raw data without the check digits.
        /// </summary>
        byte[] RemoveCheckByte(byte[] protectedData);

        /// <summary> Verify a string that has been encoded with a check digit.
        /// </summary>
        bool Verify(byte[] protectedData);

        /// <summary> Computes the check digit value.
        /// </summary>
        byte ComputeCheckByte(byte[] data);
        /// <summary> Extract just the check digits from an encoded string.
        /// </summary>
        byte ExtractCheckByte(byte[] protectedData);
        #endregion
    }
}