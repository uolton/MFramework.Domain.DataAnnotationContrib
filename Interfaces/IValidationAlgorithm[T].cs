#region Imports

using System.Collections.Generic;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Interfaces
{
    /// <summary>
    /// </summary>
    public interface IValidationAlgorithm<T> : IValidationAlgorithm
    {
        #region Delegates and Events
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        bool Validate(T value);
        bool Validate(T value, out IList<string> validationMessages);
        #endregion
    }
}