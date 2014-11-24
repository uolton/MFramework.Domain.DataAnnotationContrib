#region Imports



#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Finance
{
    /// <summary>
    /// </summary>
    public class PaymentCardNumberAttribute : ValidationAlgorithmAttribute
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
        public PaymentCardNumberAttribute(PaymentCardNumberAlgorithm.IssuerIdentifier acceptedIssuers)
            : base(new PaymentCardNumberAlgorithm(acceptedIssuers))
        { }
        public PaymentCardNumberAttribute()
            : base(new PaymentCardNumberAlgorithm())
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