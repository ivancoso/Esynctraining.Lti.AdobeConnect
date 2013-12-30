namespace Esynctraining.Core.WCFExtension
{
    using System.Net.Security;
    using System.ServiceModel.Channels;

    /// <summary>
    /// The non SSL authentication security capabilities.
    /// </summary>
    public class NonSslAuthSecurityCapabilities : ISecurityCapabilities
    {
        #region Public Properties

        /// <summary>
        /// Gets the supported request protection level.
        /// </summary>
        public ProtectionLevel SupportedRequestProtectionLevel
        {
            get
            {
                return ProtectionLevel.EncryptAndSign;
            }
        }

        /// <summary>
        /// Gets the supported response protection level.
        /// </summary>
        public ProtectionLevel SupportedResponseProtectionLevel
        {
            get
            {
                return ProtectionLevel.EncryptAndSign;
            }
        }

        /// <summary>
        /// Gets a value indicating whether supports client authentication.
        /// </summary>
        public bool SupportsClientAuthentication
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether supports client windows identity.
        /// </summary>
        public bool SupportsClientWindowsIdentity
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether supports server authentication.
        /// </summary>
        public bool SupportsServerAuthentication
        {
            get
            {
                return true;
            }
        }

        #endregion
    }
}