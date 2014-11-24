#region

using System;
using System.ComponentModel.DataAnnotations;
using MFramework.Domain.DataAnnotationsContrib.Interfaces;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib
{
    public abstract class ValidationAlgorithmAttribute : ValidationAttribute
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
        public ValidationAlgorithmAttribute(IValidationAlgorithm algorithm)
        {
            if (algorithm == null) throw new ArgumentNullException();

            this.Algorithm = algorithm;
        }
        #endregion

        #region Properties
        public IValidationAlgorithm Algorithm { get; private set; }
        #endregion

        #region Private and Protected Methods
        #endregion

        #region Public Methods
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        public override bool IsValid(object value)
        {
            return this.Algorithm.Validate(value);
        }
        #endregion
    }
}
