namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The Company Service interface.
    /// </summary>
    [ServiceContract]
    public interface ILookupService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get languages.
        /// </summary>
        /// <returns>
        /// The <see cref="LookupAllDTO"/>.
        /// </returns>
        [OperationContract]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetAll", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LookupAllDTO GetAll();

        /// <summary>
        /// The get languages.
        /// </summary>
        /// <returns>
        /// The <see cref="EGCVersionsDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetVersionInfo", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        EGCVersionsDTO GetVersionInfo();

        /// <summary>
        /// The get languages.
        /// </summary>
        /// <returns>
        /// The <see cref="LanguageDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetLanguages", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        LanguageDTO[] GetLanguages();

        /// <summary>
        /// The get build version types.
        /// </summary>
        /// <returns>
        /// The <see cref="BuildVersionTypeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetBuildVersionTypes", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        BuildVersionTypeDTO[] GetBuildVersionTypes();

        /// <summary>
        /// The get question types.
        /// </summary>
        /// <returns>
        /// The <see cref="QuestionTypeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuestionTypes", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        QuestionTypeDTO[] GetQuestionTypes();

        /// <summary>
        /// The get score types.
        /// </summary>
        /// <returns>
        /// The <see cref="ScoreTypeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetScoreTypes", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        ScoreTypeDTO[] GetScoreTypes();

        /// <summary>
        /// The get time zones.
        /// </summary>
        /// <returns>
        /// The <see cref="TimeZoneDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetTimeZones", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        TimeZoneDTO[] GetTimeZones();

        /// <summary>
        /// The get user roles.
        /// </summary>
        /// <returns>
        /// The <see cref="UserRoleDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetUserRoles", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        UserRoleDTO[] GetUserRoles();

        /// <summary>
        /// The get quiz formats.
        /// </summary>
        /// <returns>
        /// The <see cref="QuizFormatDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuizFormats", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        QuizFormatDTO[] GetQuizFormats();

        /// <summary>
        /// The survey grouping types.
        /// </summary>
        /// <returns>
        /// The <see cref="SurveyGroupingTypeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSurveyGroupingTypes", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        SurveyGroupingTypeDTO[] GetSurveyGroupingTypes();

        /// <summary>
        /// The get countries.
        /// </summary>
        /// <returns>
        /// The <see cref="CountryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetCountries", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        CountryDTO[] GetCountries();

        /// <summary>
        /// The get states.
        /// </summary>
        /// <returns>
        /// The <see cref="StateDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetStates", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        StateDTO[] GetStates();

        /// <summary>
        /// The get states.
        /// </summary>
        /// <returns>
        /// The <see cref="SNServiceDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetServices", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        SNServiceDTO[] GetServices();

        /// <summary>
        /// The get states.
        /// </summary>
        /// <returns>
        /// The <see cref="SNMapProviderDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetMapProviders", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        SNMapProviderDTO[] GetMapProviders();

        /// <summary>
        /// Gets the location of user by query.
        /// </summary>
        /// <param name="geoDTO">
        /// The geo DTO.
        /// </param>
        /// <returns>
        /// The <see cref="GeoResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "GetLocation", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
////    [WebORBCache(CacheScope = CacheScope.Global)]
        GeoResultDTO GetLocation(GeoDTO geoDTO);

        ///// <summary>
        ///// Gets the twitter search result by query.
        ///// </summary>
        ///// <param name="name">
        ///// The name query.
        ///// </param>
        ///// <returns>
        ///// The <see cref="TwitterProfileDTO"/>.
        ///// </returns>
        //[OperationContract]
        //[FaultContract(typeof(Error))]
        //[WebGet(UriTemplate = "GetTwitterProfiles?name={name}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        //TwitterProfileDTO[] GetTwitterProfiles(string name);

        /// <summary>
        /// Gets the twitter search result by query.
        /// </summary>
        /// <param name="fullName">
        /// The full name query.
        /// </param>
        /// <returns>
        /// The <see cref="GoogleSearchDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "SearchSocialLinksUsingGoogleAPI?fullName={fullName}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        GoogleSearchDTO[] SearchSocialLinksUsingGoogleAPI(string fullName);

        /// <summary>
        /// Converts profile to XML
        /// </summary>
        /// <param name="vcfProfile">
        /// The VCF Profile.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "ConvertFromVCF", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        string ConvertFromVCF(string vcfProfile);

        /// <summary>
        /// Converts profile to VCF
        /// </summary>
        /// <param name="xmlProfile">
        /// The VCF Profile.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "ConvertToVCF", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        byte[] ConvertToVCF(string xmlProfile);

        ///// <summary>
        ///// Gets the twitter search result by query.
        ///// </summary>
        ///// <param name="name">
        ///// The name query.
        ///// </param>
        ///// <returns>
        ///// The <see cref="TwitterStatusDTO"/>.
        ///// </returns>
        //[OperationContract]
        //[FaultContract(typeof(Error))]
        //[WebGet(UriTemplate = "GetTweets?name={name}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        //TwitterStatusDTO[] GetTweets(string name);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="countryDTO">
        /// The country DTO.
        /// </param>
        /// <returns>
        /// The <see cref="GeoCountryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "SaveCountry", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        GeoCountryDTO SaveCountry(GeoCountryDTO countryDTO);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="stateDTO">
        /// The state DTO.
        /// </param>
        /// <returns>
        /// The <see cref="GeoStateDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "SaveState", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        GeoStateDTO SaveState(GeoStateDTO stateDTO);

        #endregion
    }
}