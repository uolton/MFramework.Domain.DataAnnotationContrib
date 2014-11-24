#region Imports

using System;
using System.Collections.Generic;
using MFramework.Domain.DataAnnotationsContrib.Interfaces;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib
{
    /// <summary>
    /// </summary>
    public abstract class Int32ValidationAlgorithm : IValidationAlgorithm<Int32>
    {
        #region Constants and Enums
        #endregion

        #region Inner Classes and Structures
        #endregion

        #region Delegates and Events
        #endregion

        #region Instance and Shared Fields
        private int _min;
        private int _max;
        #endregion

        #region Constructors
        protected Int32ValidationAlgorithm()
            : this(Int32.MinValue, Int32.MaxValue)
        { }
        protected Int32ValidationAlgorithm(Int32 min, Int32 max)
        {
            _min = min;
            _max = max;
        }
        #endregion

        #region Properties
        #endregion

        #region Private and Protected Methods
        protected virtual bool ValidateMin(int value)
        {
            return (value >= _min);
        }
        protected virtual bool ValidateMax(int value)
        {
            return (value <= _max);
        }
        #endregion

        #region Public Methods
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        #endregion

        #region IValidationAlgorithm<Int32> Members
        public bool Validate(int value)
        {
            //Delegate to richer overload.
            IList<string> messages;
            return Validate(value, out messages);
        }
        public virtual bool Validate(int value, out IList<string> validationMessages)
        {
            try
            {
                //1 Range checks.
                if (!ValidateMin(value))
                {
                    validationMessages = new string[] { SR.Validator_SizeMinFailure };
                    return false;
                }
                if (!ValidateMax(value))
                {
                    validationMessages = new string[] { SR.Validator_SizeMaxFailure };
                    return false;
                }

                //OK!
                //Set default error messages to empty array.
                validationMessages = new string[] { };
                return true;
            }
            catch (Exception ex)
            {
                validationMessages = new string[] { ex.Message };
                return false;
            }
        }
        #endregion

        #region IValidationAlgorithm Members
        bool IValidationAlgorithm.Validate(object value)
        {
            return Validate(Convert.ToInt32(value));
        }
        bool IValidationAlgorithm.Validate(object value, out IList<string> validationMessages)
        {
            return Validate(Convert.ToInt32(value), out validationMessages);
        }
        #endregion
    }
}