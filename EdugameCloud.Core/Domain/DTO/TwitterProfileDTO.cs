//namespace EdugameCloud.Core.Domain.DTO
//{
//    using System.Runtime.Serialization;

//    using TweetSharp;

//    /// <summary>
//    ///     The twitter profile dto.
//    /// </summary>
//    [DataContract]
//    public class TwitterProfileDTO
//    {
//        #region Constructors and Destructors

//        /// <summary>
//        /// Initializes a new instance of the <see cref="TwitterProfileDTO"/> class.
//        /// </summary>
//        public TwitterProfileDTO()
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="TwitterProfileDTO"/> class.
//        /// </summary>
//        /// <param name="twitterUser">
//        /// The twitter user.
//        /// </param>
//        public TwitterProfileDTO(TwitterUser twitterUser)
//        {
//            this.twitterId = twitterUser.Id;
//            this.imageUrl = twitterUser.ProfileImageUrl;
//            this.imageUrlHttps = twitterUser.ProfileImageUrlHttps;
//            this.screenName = twitterUser.ScreenName;
//            this.profileUrl = twitterUser.Url;
//            this.name = twitterUser.Name;
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="TwitterProfileDTO"/> class.
//        /// </summary>
//        /// <param name="twitterUser">
//        /// The twitter user.
//        /// </param>
//        public TwitterProfileDTO(ITweeter twitterUser)
//        {
//            this.imageUrl = twitterUser.ProfileImageUrl;
//            this.screenName = twitterUser.ScreenName;
//        }

//        #endregion

//        #region Public Properties

//        /// <summary>
//        /// Gets or sets the twitter id.
//        /// </summary>
//        [DataMember]
//        public long twitterId { get; set; }

//        /// <summary>
//        /// Gets or sets the File url.
//        /// </summary>
//        [DataMember]
//        public string imageUrl { get; set; }

//        /// <summary>
//        /// Gets or sets the File url https.
//        /// </summary>
//        [DataMember]
//        public string imageUrlHttps { get; set; }

//        /// <summary>
//        /// Gets or sets the name.
//        /// </summary>
//        [DataMember]
//        public string name { get; set; }

//        /// <summary>
//        /// Gets or sets the profile url.
//        /// </summary>
//        [DataMember]
//        public string profileUrl { get; set; }

//        /// <summary>
//        /// Gets or sets the screen name.
//        /// </summary>
//        [DataMember]
//        public string screenName { get; set; }

//        #endregion
//    }
//}