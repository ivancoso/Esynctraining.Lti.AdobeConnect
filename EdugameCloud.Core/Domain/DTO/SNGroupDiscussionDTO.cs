namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The SN Group discussion DTO.
    /// </summary>
    [DataContract]
    public class SNGroupDiscussionDTO 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNGroupDiscussionDTO"/> class.
        /// </summary>
        public SNGroupDiscussionDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SNGroupDiscussionDTO"/> class.
        /// </summary>
        /// <param name="groupDiscussion">
        /// The group discussion.
        /// </param>
        public SNGroupDiscussionDTO(SNGroupDiscussion groupDiscussion)
        {
            if (groupDiscussion != null)
            {
                this.snGroupDiscussionId = groupDiscussion.Id;
                this.dateCreated = groupDiscussion.DateCreated.ConvertToUnixTimestamp();
                this.dateModified =
                    (groupDiscussion.DateModified ?? groupDiscussion.DateCreated)
                    .With(x => x.ConvertToUnixTimestamp());
                this.groupDiscussionTitle = groupDiscussion.GroupDiscussionTitle;
                this.groupDiscussionData = groupDiscussion.GroupDiscussionData;
                this.isActive = groupDiscussion.IsActive;
                this.acSessionId = groupDiscussion.ACSessionId;
            }
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the SN group discussion id.
        /// </summary>
        [DataMember]
        public int snGroupDiscussionId { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the date created. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [DataMember]
        public string groupDiscussionTitle { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        [DataMember]
        public string groupDiscussionData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public bool isActive { get; set; }

        /// <summary>
        /// Gets or sets the AC session.
        /// </summary>
        [DataMember]
        public int acSessionId { get; set; }

        #endregion
    }
}