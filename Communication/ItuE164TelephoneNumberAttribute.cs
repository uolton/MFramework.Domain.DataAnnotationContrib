#region Imports



#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Communication
{
    /// <summary>
    /// </summary>
    public class ItuE164TelephoneNumberAttribute : ValidationAlgorithmAttribute
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
        public ItuE164TelephoneNumberAttribute()
            : base(new ItuE164TelephoneNumberAlgorithm())
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