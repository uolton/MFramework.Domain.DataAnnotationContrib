#region Imports



#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Interfaces
{
    /// <summary>
    /// </summary>
    public interface IMultipartValueAlgorithm
    {
        #region Delegates and Events
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        MultipartValueDetails ExtractComponents(string value);
        #endregion
    }
}