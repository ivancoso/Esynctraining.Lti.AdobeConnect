namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The recording DTO.
    /// </summary>
    [DataContract]
    public sealed class RecordingDTO
    {
        /// <summary>
        /// Gets or sets the begin date.
        /// </summary>
        [DataMember]
        public string begin_date { get; set; }

        //[DataMember]
        //public string description { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        [DataMember]
        public int duration { get; set; }

        //[DataMember]
        //public string end_date { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        [DataMember]
        public string url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is_public.
        /// </summary>
        [DataMember]
        public bool is_public { get; set; }

        //[DataMember]
        //public string password { get; set; }

    }

}