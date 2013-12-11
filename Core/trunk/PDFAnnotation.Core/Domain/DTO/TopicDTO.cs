namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using Esynctraining.Core.Extensions;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The Topic data transfer object.
    /// </summary>
    [DataContract]
    public class TopicDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TopicDTO" /> class.
        /// </summary>
        public TopicDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopicDTO"/> class.
        /// </summary>
        /// <param name="topic">
        /// The Topic.
        /// </param>
        public TopicDTO(Topic topic) 
        {
            if (topic != null)
            {
                this.topicId = topic.Id;
                this.categoryId = topic.Category.With(x => x.Id);
                this.firmName = topic.FirmName;
                this.firstName = topic.FirstName;
                this.lastName = topic.LastName;
                this.fullName = topic.FullName;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets ContactId
        /// </summary>
        [DataMember]
        public int categoryId { get; set; }

        /// <summary>
        ///     Gets or sets CompanyName
        /// </summary>
        [DataMember]
        public string firmName { get; set; }

        /// <summary>
        ///     Gets or sets FirstName
        /// </summary>
        [DataMember]
        public string firstName { get; set; }

        /// <summary>
        ///     Gets or sets LastName
        /// </summary>
        [DataMember]
        public string lastName { get; set; }

        /// <summary>
        ///     Gets or sets FullName
        /// </summary>
        [DataMember]
        public string fullName { get; set; }

        /// <summary>
        ///     Gets or sets ParticipantId
        /// </summary>
        [DataMember]
        public int topicId { get; set; }

        #endregion
    }
}