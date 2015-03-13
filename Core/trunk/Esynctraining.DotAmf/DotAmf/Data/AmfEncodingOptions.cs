// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfEncodingOptions.cs">
//   
// </copyright>
// <summary>
//   AMF serialization options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.Data
{
    /// <summary>
    ///     AMF serialization options.
    /// </summary>
    public struct AmfEncodingOptions
    {
        #region Fields

        /// <summary>
        ///     AMF version.
        /// </summary>
        public AmfVersion AmfVersion;

        /// <summary>
        ///     Use AMF context switch.
        /// </summary>
        public bool UseContextSwitch;

        #endregion
    }
}