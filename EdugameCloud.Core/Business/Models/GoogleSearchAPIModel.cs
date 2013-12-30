namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web.Helpers;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Providers;

    /// <summary>
    /// The google search api model.
    /// </summary>
    public class GoogleSearchAPIModel
    {
        #region Static Fields

        /// <summary>
        /// The facebook id regex.
        /// </summary>
        private static readonly Regex FacebookIdRegex = new Regex(@"facebook\.com/(?<fbId>(\w|\.)+)/?", RegexOptions.Compiled);

        #endregion

        #region Fields

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSearchAPIModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public GoogleSearchAPIModel(ApplicationSettingsProvider settings)
        {
            this.settings = settings;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="List{GoogleSearchDTO}"/>.
        /// </returns>
        public List<GoogleSearchDTO> Search(string query)
        {
            var result = new List<GoogleSearchDTO>();

            int max = 30;
            int counter = 1;
            int step = 10;
            do
            {
                try
                {
                    var webclient = new WebClient();
                    dynamic jsonString =
                        webclient.DownloadString(
                            string.Format(
                                (string)this.settings.GoogleSearchApi_Query, 
                                this.settings.GoogleSearchApi_Key, 
                                this.settings.GoogleSearchEngine_Id, 
                                query, 
                                step, 
                                counter));
                    if (GetStatusCode(webclient) != 200)
                    {
                        if (result.Any())
                        {
                            return result;
                        }

                        return null;
                    }

                    dynamic json = Json.Decode(jsonString);
                    max = Math.Min(30, int.Parse(json.queries.request[0].totalResults));
                    foreach (dynamic item in json.items)
                    {
                        var itemDTO = new GoogleSearchDTO
                                          {
                                              link = item.link, 
                                              title = item.title, 
                                              type = this.GetTypeByLink((string)item.displayLink)
                                          };
                        if (itemDTO.type == 1)
                        {
                            itemDTO.facebookId = this.TryGetFacebookIdFromLink(itemDTO.link);
                        }

                        result.Add(itemDTO);
                    }
                }
                catch (Exception)
                {
                }

                counter += step;
            }
            while (counter < max);
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get status code.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int GetStatusCode(WebClient client)
        {
            FieldInfo responseField = client.GetType()
                .GetField("m_WebResponse", BindingFlags.Instance | BindingFlags.NonPublic);

            if (responseField != null)
            {
                var response = responseField.GetValue(client) as HttpWebResponse;

                if (response != null)
                {
                    return (int)response.StatusCode;
                }
            }

            return 200;
        }

        /// <summary>
        /// The get type by link.
        /// </summary>
        /// <param name="link">
        /// The link.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetTypeByLink(string link)
        {
            if (link.Contains("facebook.com"))
            {
                return 1;
            }

            if (link.Contains("linkedin.com"))
            {
                return 2;
            }

            if (link.Contains("twitter.com"))
            {
                return 3;
            }

            if (link.Contains("slideshare.net"))
            {
                return 4;
            }

            return 5;
        }

        /// <summary>
        /// The try get facebook id from link.
        /// </summary>
        /// <param name="link">
        /// The link.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string TryGetFacebookIdFromLink(string link)
        {
            Match match;
            if ((match = FacebookIdRegex.Match(link)).Success)
            {
                string id = match.Groups["fbId"].Value;
                if (id != "public" && id != "people" && !id.Contains("search"))
                {
                    return id;
                }
            }

            return null;
        }

        #endregion
    }
}