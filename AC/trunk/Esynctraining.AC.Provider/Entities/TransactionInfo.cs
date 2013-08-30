namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// TransactionInfo structure
    /// </summary>
    [Serializable]
    [XmlRoot("row")]
    public class TransactionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionInfo"/> class.
        /// </summary>
        public TransactionInfo()
        {
            this.Type = ScoType.not_set;
        }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        [XmlAttribute("transaction-id")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [XmlAttribute("type")]
        public ScoType Type { get; set; }

        /// <summary>
        /// Gets or sets the principal id.
        /// </summary>
        [XmlAttribute("principal-id")]
        public string PrincipalId { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        [XmlAttribute("score")]
        public string Score { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        [XmlElement("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        [XmlElement("login")]
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [XmlElement("user-name")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [XmlElement("date-created")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date closed.
        /// </summary>
        [XmlElement("date-closed")]
        public DateTime DateClosed { get; set; }
    }
}
