#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib
{
    /// <summary>
    /// </summary>
    public abstract class MultipartValueDetails
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
        #endregion

        #region Properties
        public IDictionary<string, string> GetComponents()
        {
            return CopyPropertiesDictionary(this, false);
        }
        #endregion

        #region Private and Protected Methods
        private static IDictionary<string, string> CopyPropertiesDictionary(MultipartValueDetails target, bool prefixKeysWithTypeName)
        {
            var properties = new Dictionary<string, string>();
            if (target == null) return properties;

            var declaredProperties
                = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var typeName = target.GetType().Name;

            foreach (var propertyInfo in declaredProperties)
            {
                //Exclude 'Components' property.
                var key = prefixKeysWithTypeName ? typeName + "." + propertyInfo.Name : propertyInfo.Name;
                var value = propertyInfo.GetValue(target, null);
                properties.Add(key, value == null ? null : value.ToString());
            }
            return properties;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        public override string ToString()
        {
            return String.Join(" ", this.GetComponents().Values.ToArray());
        }
        #endregion
    }
}