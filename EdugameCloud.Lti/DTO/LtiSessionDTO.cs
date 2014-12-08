namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Lti.Utils;

    /// <summary>
    /// The LTI session DTO.
    /// </summary>
    [DataContract]
    [Serializable]
    public class LtiSessionDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the AC password data.
        /// </summary>
        [DataMember]
        public virtual string ACPasswordData { get; set; }

        /// <summary>
        /// Gets or sets the LTI param.
        /// </summary>
        [DataMember]
        public virtual LtiParamDTO LtiParam { get; set; }

        /// <summary>
        /// Gets or sets the shared key.
        /// </summary>
        [DataMember]
        public virtual string SharedKey { get; set; }

        /// <summary>
        /// Gets the restored AC password.
        /// </summary>
        [DataMember]
        public virtual string RestoredACPassword
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.ACPasswordData) || string.IsNullOrWhiteSpace(this.SharedKey)
                           ? null
                           : AESGCM.SimpleDecrypt(this.ACPasswordData, Convert.FromBase64String(this.SharedKey));
            }
        }

        #endregion
    }
}