#region Imports

using System.Collections.Generic;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Interfaces
{
    /// <summary>
    /// </summary>
    public interface IValidationAlgorithm
    {
        #region Delegates and Events
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        bool Validate(object value);
        bool Validate(object value, out IList<string> validationMessages);
        #endregion
    }
}