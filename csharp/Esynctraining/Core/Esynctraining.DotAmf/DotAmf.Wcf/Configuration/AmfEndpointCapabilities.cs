// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmfEndpointCapabilities.cs" company="">
//   
// </copyright>
// <summary>
//   AMF endpoint capabilities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DotAmf.ServiceModel.Configuration
{
    /// <summary>
    ///     AMF endpoint capabilities.
    /// </summary>
    public struct AmfEndpointCapabilities
    {
        #region Fields

        /// <summary>
        ///     Include exception details in faults.
        /// </summary>
        public bool ExceptionDetailInFaults;

        /// <summary>
        ///     Messaging version.
        /// </summary>
        public uint MessagingVersion;

        #endregion
    }
}