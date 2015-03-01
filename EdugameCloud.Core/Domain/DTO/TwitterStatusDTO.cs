namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Extensions;

    using TweetSharp;

    /// <summary>
    ///     The twitter profile DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(TwitterProfileDTO))]
    public class TwitterStatusDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TwitterStatusDTO" /> class.
        /// </summary>
        public TwitterStatusDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterStatusDTO"/> class.
        /// </summary>
        /// <param name="twitterStatus">
        /// The twitter user.
        /// </param>
        public TwitterStatusDTO(TwitterStatus twitterStatus)
        {
            this.statusId = twitterStatus.Id;
            this.author = new TwitterProfileDTO(twitterStatus.Author);
            this.createdDate = twitterStatus.CreatedDate.ConvertToUnixTimestamp();
            this.text = twitterStatus.Text;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the author.
        /// </summary>
        [DataMember]
        public TwitterProfileDTO author { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        [DataMember]
        public double createdDate { get; set; }

        /// <summary>
        ///     Gets or sets the twitter id.
        /// </summary>
        [DataMember]
        public long statusId { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        [DataMember]
        public string text { get; set; }

        #endregion
    }
}