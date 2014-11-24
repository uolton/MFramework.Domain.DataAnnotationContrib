#region Imports



#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Commerce
{
    /// <summary>
    /// </summary>
    public class UpuPostalCodeAttribute : ValidationAlgorithmAttribute
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
        public UpuPostalCodeAttribute()
            : base(new UpuPostalCodeAlgorithm())
        { }
        #endregion

        #region Properties
        #endregion

        #region Private and Protected Methods
        #endregion

        #region Public Methods
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        #endregion
    }
}