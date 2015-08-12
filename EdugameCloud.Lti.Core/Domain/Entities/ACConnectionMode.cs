namespace EdugameCloud.Lti.Domain.Entities
{
    /// <summary>
    /// The AC connection mode.
    /// </summary>
    public enum AcConnectionMode
    {
        /// <summary>
        /// The overwrite.
        /// </summary>
        Overwrite = 0, 

        /// <summary>
        /// The don't overwrite ac password.
        /// </summary>
        DontOverwriteACPassword = 1, 

        /// <summary>
        /// The don't overwrite local password.
        /// </summary>
        DontOverwriteLocalPassword = 2, 
    }
}