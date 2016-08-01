namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The applet result DTO.
    /// </summary>
    [DataContract]
    public class AppletResultDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppletResultDTO"/> class.
        /// </summary>
        public AppletResultDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppletResultDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public AppletResultDTO(AppletResult result)
        {
            this.appletResultId = result.Id;
            this.acSessionId = result.ACSessionId;
            this.appletItemId = result.AppletItem.Id;
            this.dateCreated = result.DateCreated.With(x => x.ConvertToUnixTimestamp());
            this.endTime = result.EndTime.With(x => x.ConvertToUnixTimestamp());
            this.participantName = result.ParticipantName;
            this.score = result.Score;
            this.startTime = result.StartTime.With(x => x.ConvertToUnixTimestamp());
            this.isArchive = result.IsArchive ?? false;
            this.email = result.Email;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is archive.
        /// </summary>
        [DataMember]
        public string email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is archive.
        /// </summary>
        [DataMember]
        public bool isArchive { get; set; }

        /// <summary>
        /// Gets or sets the AdobeConnect session ID.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int acSessionId { get; set; }

        /// <summary>
        /// Gets or sets the document xml.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int appletItemId { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the end time. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double endTime { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int appletResultId { get; set; }

        /// <summary>
        /// Gets or sets the company Id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the participant name
        /// </summary>
        [DataMember]
        public string participantName { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        [DataMember]
        public int score { get; set; }

        /// <summary>
        /// Gets or sets the start time. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double startTime { get; set; }

        #endregion
    }
}