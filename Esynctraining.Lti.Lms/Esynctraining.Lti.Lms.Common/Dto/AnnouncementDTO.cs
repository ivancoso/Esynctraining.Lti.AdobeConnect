using System.Runtime.Serialization;

namespace Esynctraining.Lti.Lms.Common.Dto
{
    /// <summary>
    /// The announcement DTO.
    /// </summary>
    [DataContract]
    public class AnnouncementDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [DataMember]
        public string message { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [DataMember]
        public string title { get; set; }

        #endregion
    }
}