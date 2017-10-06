namespace AnonymousChat.Web.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The chat history email DTO.
    /// </summary>
    [DataContract]
    public class ChatHistoryEmailDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the BCC emails.
        /// </summary>
        [DataMember]
        public List<EmailAddressDTO> BccEmails { get; set; }

        /// <summary>
        /// Gets or sets the body html.
        /// </summary>
        [DataMember]
        public string BodyHTML { get; set; }

        /// <summary>
        /// Gets or sets the CC emails.
        /// </summary>
        [DataMember]
        public List<EmailAddressDTO> CcEmails { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [DataMember]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the to emails.
        /// </summary>
        [DataMember]
        public List<EmailAddressDTO> ToEmails { get; set; }

        #endregion
    }
}