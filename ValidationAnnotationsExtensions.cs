#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib
{
    public static class ValidationAnnotationsExtensions
    {
        #region Constants and Enums
        #endregion

        #region Inner Classes and Structures
        public class ValidationAttributeSet
        {
            public Type Type { get; set; }
            public ICustomTypeDescriptor TypeDescriptor { get; set; }
            public Dictionary<string, ValidationAttribute> PropertyAttributes { get; set; }
        }
        #endregion

        #region Instance and Shared Fields
        private static Dictionary<ICustomTypeDescriptor, ValidationAttributeSet> __attributeSetCache = new Dictionary<ICustomTypeDescriptor, ValidationAttributeSet>();
        private static readonly ReaderWriterLock __readerWriterLock = new ReaderWriterLock();
        #endregion

        #region Private and Protected Methods
        private static ValidationAttributeSet GetAndCacheValidationAttributeSetFromType(Type type, TypeDescriptionProvider typeDescriptorProvider)
        {
            var typeDescriptor = typeDescriptorProvider.GetTypeDescriptor(type);

            //Cache hit?
            __readerWriterLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                ValidationAttributeSet cachedSet;
                if (__attributeSetCache.TryGetValue(typeDescriptor, out cachedSet)) return cachedSet;
            }
            finally
            {
                __readerWriterLock.ReleaseReaderLock();
            }

            //Reflect and cache.
            __readerWriterLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                //Double check.
                ValidationAttributeSet cachedSet;
                if (__attributeSetCache.TryGetValue(typeDescriptor, out cachedSet)) return cachedSet;

                //Reflect.
                ValidationAttributeSet newSet = GetValidationAttributeSetFromType(type, typeDescriptor);
                __attributeSetCache.Add(typeDescriptor, newSet);

                return newSet;
            }
            finally
            {
                __readerWriterLock.ReleaseWriterLock();
            }
        }

        private static ValidationAttributeSet GetValidationAttributeSetFromType(Type type, ICustomTypeDescriptor typeDescriptor)
        {
            var attributes = (from p in GetPropertiesFromType(typeDescriptor)
                              from a in GetAttributesFromProperty(p)
                              select new KeyValuePair<string, ValidationAttribute>(p.Name, a));

            var attributeDictionary = new Dictionary<string, ValidationAttribute>();
            foreach (var entry in attributes)
            {
                attributeDictionary.Add(entry.Key, entry.Value);
            }
            return new ValidationAttributeSet() { Type = type, TypeDescriptor = typeDescriptor, PropertyAttributes = attributeDictionary };
        }
        private static IEnumerable<PropertyDescriptor> GetPropertiesFromType(ICustomTypeDescriptor typeDescriptor)
        {
            return (from p in typeDescriptor.GetProperties().Cast<PropertyDescriptor>() select p).ToList();
        }
        private static IEnumerable<ValidationAttribute> GetAttributesFromProperty(PropertyDescriptor propertyDescriptor)
        {
            return (from a in propertyDescriptor.Attributes.OfType<ValidationAttribute>() select a).ToList();
        }
        #endregion

        #region Public Methods
        public static IList<ValidationAttribute> GetValidationAttributes(this Type target, string appliedToPropertyName)
             //where TAttribute : ValidationAttribute
        {
            return GetValidationAttributes(target, appliedToPropertyName, new AssociatedMetadataTypeTypeDescriptionProvider(target));
        }
        public static IList<ValidationAttribute> GetValidationAttributes(this Type target, string appliedToPropertyName, TypeDescriptionProvider metadataProvider)
             //where TAttribute : ValidationAttribute
        {
            ValidationAttributeSet set = GetValidationAttributes(target, metadataProvider);
            return (from item in set.PropertyAttributes
                    where item.Key == appliedToPropertyName
                    select item.Value).ToList();
        }

        public static ValidationAttributeSet GetValidationAttributes(this Type target)
        {
            return GetValidationAttributes(target, new AssociatedMetadataTypeTypeDescriptionProvider(target));
        }
        public static ValidationAttributeSet GetValidationAttributes(this Type target, TypeDescriptionProvider metadataProvider)
        {
            return GetAndCacheValidationAttributeSetFromType(target, metadataProvider);
        }

        public static object GetPropertyValue(this object target, string propertyName)
        {
            return target.GetType().GetProperty(propertyName).GetValue(target, null);
        }
        public static object GetPropertyValue<TPropertyType>(this object target, string propertyName)
        {
            return (TPropertyType)GetPropertyValue(target, propertyName);
        }
        #endregion
    }
}
