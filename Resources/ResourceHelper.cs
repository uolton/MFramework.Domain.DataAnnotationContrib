#region Imports

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Resources
{
    /// <summary>
    /// </summary>
    internal static class ResourceHelper
    {
        #region Constants and Enums
        public static readonly string ResourceCommentLinePrefix = "--";
        public static readonly char ResourceLineItemSeparator = ';';
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
        #endregion

        #region Private and Protected Methods
        #endregion

        #region Public Methods
        public static StreamReader GetFileResourceStreamReader(string resourceFileName)
        {
            Assembly thisAssembly = typeof(ResourceHelper).Assembly;
            Stream stream = thisAssembly.GetManifestResourceStream("DataAnnotationsContrib.Resources." + resourceFileName);
            //Check params (there is no way to find out if the resourceName exists in this assembly without getting it first
            //(sadly this get also doesn't throw if the resource isn't present).
            if (stream == null) throw new ArgumentException(String.Format("Resource {0} not present in assembly.", resourceFileName), "resourceFileName");
            return new StreamReader(stream);
        }

        public static Dictionary<string, T> ReadFromResourceStream<T>(string resourceFileName,
            Func<string, T> itemConstructor,
            Func<string, string> keyConstructor,
            string skipLinePrefix)
        {
            var items = new Dictionary<string, T>();
            using (var reader = ResourceHelper.GetFileResourceStreamReader(resourceFileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!String.IsNullOrEmpty(skipLinePrefix) && !line.StartsWith(skipLinePrefix))
                    {
                        var item = itemConstructor(line);
                        var key = keyConstructor(line);
                        items.Add(key, item);
                    }
                }
            }
            return items;
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        #endregion
    }
}