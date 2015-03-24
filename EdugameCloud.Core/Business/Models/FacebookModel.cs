//namespace EdugameCloud.Core.Business.Models
//{
//    using System.Collections.Generic;

//    using EdugameCloud.Core.Domain.DTO;
//    using EdugameCloud.Core.Domain.Entities;

//    using Esynctraining.Core.Providers;

//    using Facebook;

//    /// <summary>
//    ///     The facebook model.
//    /// </summary>
//    public class FacebookModel
//    {
//        #region Fields

//        /// <summary>
//        /// The settings.
//        /// </summary>
//        private readonly dynamic settings;

//        #endregion

//        #region Constructors and Destructors

//        /// <summary>
//        /// Initializes a new instance of the <see cref="FacebookModel"/> class.
//        /// </summary>
//        /// <param name="settings">
//        /// The settings.
//        /// </param>
//        public FacebookModel(ApplicationSettingsProvider settings)
//        {
//            this.settings = settings;
//        }

//        #endregion

//        #region Public Methods and Operators

//        public List<FacebookProfileDTO> SearchForUsers(string query)
//        {
//            var users = this.Search(query, FacebookSearchType.user);
//            return new List<FacebookProfileDTO>();
//        }


//        /// <summary>
//        /// The search.
//        /// </summary>
//        /// <param name="query">
//        /// The query.
//        /// </param>
//        /// <param name="searchType">
//        /// The search type.
//        /// </param>
//        /// <returns>
//        /// The <see cref="List{FacebookProfileDTO}"/>.
//        /// </returns>
//        private dynamic Search(string query, FacebookSearchType searchType)
//        {
//            var fb = new FacebookClient();
//            dynamic response = fb.Get(
//                "oauth/access_token", 
//                new
//                    {
//                        client_id = (string)settings.FBAppId, 
//                        client_secret = (string)settings.FBAppSecret, 
//                        grant_type = "client_credentials"
//                    });
//            fb.AccessToken = response.access_token;
//            var result = fb.Get("search", new { q = query, type = searchType.ToString() });
//            return result;
//        }

//        #endregion
//    }
//}