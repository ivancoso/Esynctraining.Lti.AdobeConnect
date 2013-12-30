namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Providers;

    using TweetSharp;

    using Weborb.Util.Logging;

    /// <summary>
    ///     The twitter model.
    /// </summary>
    public class TwitterModel
    {
        #region Fields

        /// <summary>
        ///     The settings.
        /// </summary>
        private readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public TwitterModel(ApplicationSettingsProvider settings)
        {
            this.settings = settings;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The search for tweets.
        /// </summary>
        /// <param name="screenName">
        /// The screen name.
        /// </param>
        /// <returns>
        /// The <see cref="List{TwitterStatusDTO}"/>.
        /// </returns>
        public List<TwitterStatusDTO> SearchForTweets(string screenName)
        {
            try
            {
                var options = new ListTweetsOnUserTimelineOptions { ScreenName = screenName };
                var service = new TwitterService((string)this.settings.TWConsumerKey, (string)this.settings.TWConsumerSecret);
                service.AuthenticateWith((string)this.settings.TWAccessToken, (string)this.settings.TWAccessSecret);
                return service.ListTweetsOnUserTimeline(options).Select(x => new TwitterStatusDTO(x)).ToList();
            }
            catch (Exception ex)
            {
                Log.log(1, ex);
                return new List<TwitterStatusDTO>();
            }
        }

        /// <summary>
        /// The search for users.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="List{TwitterProfileDTO}"/>.
        /// </returns>
        public List<TwitterProfileDTO> SearchForUsers(string query)
        {
            var service = new TwitterService(
                (string)this.settings.TWConsumerKey, (string)this.settings.TWConsumerSecret);
            service.AuthenticateWith((string)this.settings.TWAccessToken, (string)this.settings.TWAccessSecret);
            IEnumerable<TwitterUser> users = service.SearchForUser(new SearchForUserOptions { Q = query });
            return users.Select(x => new TwitterProfileDTO(x)).ToList();
        }

        #endregion
    }
}