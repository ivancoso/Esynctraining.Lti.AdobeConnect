namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using Esynctraining.Core.Logging;

    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Newtonsoft.Json;

    /// <summary>
    ///     The geo model.
    /// </summary>
    public class GeoModel
    {
        #region Fields

        /// <summary>
        ///     The settings.
        /// </summary>
        private readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public GeoModel(ApplicationSettingsProvider settings)
        {
            this.settings = settings;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get nominatim result.
        /// </summary>
        /// <param name="geoDTO">
        /// The geo dto.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="GeoResultDTO"/>.
        /// </returns>
        public GeoResultDTO GetLocation(GeoDTO geoDTO, out string error)
        {
            GeoResultDTO result = null;
            error = null;
            NameValueCollection geoNameValue = geoDTO.GetNameValueCollection();
            if (geoNameValue != null)
            {
                result = this.ParseNominatimResult(geoNameValue, out error);
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The parse nominatim result.
        /// </summary>
        /// <param name="geoNameValue">
        /// The geo name value.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="GeoResultDTO"/>.
        /// </returns>
        private GeoResultDTO ParseNominatimResult(NameValueCollection geoNameValue, out string error)
        {
            error = null;
            string url = (string)this.settings.NominatimApiUrl + this.ToQueryString(geoNameValue);
            string strResponse = string.Empty;
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                WebResponse res = req.GetResponse();
                using (var stIn = new StreamReader(res.GetResponseStream()))
                {
                    strResponse = stIn.ReadToEnd();
                    dynamic result = JsonConvert.DeserializeObject(string.Format("{{ dataResult: {0}}}", strResponse));
                    stIn.Close();
                    var dataResult = new List<dynamic>(result.dataResult);
                    if (dataResult.Any())
                    {
                        dynamic firstEntry = dataResult.First();

                        return new GeoResultDTO
                        {
                            latitude = double.Parse((string)firstEntry.lat, NumberStyles.Any, CultureInfo.InvariantCulture),
                            longitude = double.Parse((string)firstEntry.lon, NumberStyles.Any, CultureInfo.InvariantCulture),
                        };
                    }

                    error = string.Format("Invalid response: url={0}; response: {1}", url, strResponse);
                    return null;
                }
            }
            catch (Exception ex)
            {
                IoC.Resolve<ILogger>().Error("ParseNominatimResult", ex);
                error = string.Format("Invalid response: url={0}; response: {1}", url, strResponse);
                return null;
            }
        }

        /// <summary>
        /// The to query string.
        /// </summary>
        /// <param name="nvc">
        /// The nvc.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ToQueryString(NameValueCollection nvc)
        {
            nvc.Add("format", "json");
            var array = new List<string>();
            foreach (var key in nvc.AllKeys.Where(key => !string.IsNullOrWhiteSpace(key)))
            {
                var value = nvc[key];
                if (!string.IsNullOrWhiteSpace(value))
                {
                    array.Add(string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)));
                }
            }

            return "?" + string.Join("&", array);
        }

        #endregion
    }
}