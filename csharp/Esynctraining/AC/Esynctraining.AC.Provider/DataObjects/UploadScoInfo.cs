// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadSco.cs" company="">
//   
// </copyright>
// <summary>
//   The upload sco info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Esynctraining.AC.Provider.DataObjects
{
    /// <summary>
    /// The upload SCO info.
    /// </summary>
    public class UploadScoInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        public string scoId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        public string summary { get; set; }

        /// <summary>
        /// Gets or sets the file bytes.
        /// </summary>
        public byte[] fileBytes { get; set; }

        /// <summary>
        /// Gets or sets the file content type.
        /// </summary>
        public string fileContentType { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string fileName { get; set; }

        #endregion
    }
}