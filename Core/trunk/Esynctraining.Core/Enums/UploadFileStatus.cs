namespace Esynctraining.Core.Enums
{
    /// <summary>
    /// The image status.
    /// </summary>
    public enum UploadFileStatus 
    {
        /// <summary>
        /// The converting pages failed.
        /// </summary>
        ConvertingPagesFailed = 0,

        /// <summary>
        /// The converted.
        /// </summary>
        Converted = 1,

        /// <summary>
        /// The rendering.
        /// </summary>
        Rendering = 2,

        /// <summary>
        /// The converting.
        /// </summary>
        Converting = 3,

        /// <summary>
        /// The converting failed.
        /// </summary>
        ConvertingFailed = 4,

        /// <summary>
        /// The uploading initialized.
        /// </summary>
        Uploading = 5,
    }
}