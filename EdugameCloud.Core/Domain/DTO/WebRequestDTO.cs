namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    ///     The web request
    /// </summary>
    [DataContract]
    [KnownType(typeof(WebHeaderDTO))]
    [Serializable]
    public class WebRequestDTO
    {
        /// <summary>
        /// The headers field.
        /// </summary>
        private WebHeaderDTO[] headersField = { };

        #region Public Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        [DataMember]
        [XmlElement(ElementName = "Data")]
        public string data { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        [DataMember]
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        [DataMember]
        public WebHeaderDTO[] headers
        {
            get
            {
                return this.headersField ?? new WebHeaderDTO[] { };
            }

            set
            {
                this.headersField = value;
            }
        }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        [DataMember]
        public string contentType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is get method.
        /// </summary>
        [DataMember]
        public bool isGetMethod { get; set; }

        #endregion
    }
}
