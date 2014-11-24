#region Imports



#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Finance
{
    /// <summary>
    /// </summary>
    public class Iso9362SWIFTBICAttribute : ValidationAlgorithmAttribute
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
        public Iso9362SWIFTBICAttribute()
            : base(new Iso9362SWIFTBICAlgorithm())
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