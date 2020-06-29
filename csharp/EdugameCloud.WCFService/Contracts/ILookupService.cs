namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;
    
    [ServiceContract]
    public interface ILookupService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetAll", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LookupAllDTO GetAll();
        
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetVersionInfo", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        EGCVersionsDTO GetVersionInfo();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetLanguages", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LanguageDTO[] GetLanguages();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetBuildVersionTypes", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        BuildVersionTypeDTO[] GetBuildVersionTypes();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuestionTypes", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        QuestionTypeDTO[] GetQuestionTypes();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetScoreTypes", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ScoreTypeDTO[] GetScoreTypes();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetTimeZones", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        TimeZoneDTO[] GetTimeZones();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetUserRoles", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        UserRoleDTO[] GetUserRoles();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuizFormats", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        QuizFormatDTO[] GetQuizFormats();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSurveyGroupingTypes", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SurveyGroupingTypeDTO[] GetSurveyGroupingTypes();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetCountries", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CountryDTO[] GetCountries();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetStates", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        StateDTO[] GetStates();

        //[OperationContract]
        //[FaultContract(typeof(Error))]
        //[WebGet(UriTemplate = "GetSchools", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        //SchoolDTO[] GetSchools();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetServices", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SNServiceDTO[] GetServices();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetMapProviders", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
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

        ///// <summary>
        ///// Gets the twitter search result by query.
        ///// </summary>
        ///// <param name="fullName">
        ///// The full name query.
        ///// </param>
        ///// <returns>
        ///// The <see cref="GoogleSearchDTO"/>.
        ///// </returns>
        //[OperationContract]
        //[FaultContract(typeof(Error))]
        //[WebGet(UriTemplate = "SearchSocialLinksUsingGoogleAPI?fullName={fullName}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        //GoogleSearchDTO[] SearchSocialLinksUsingGoogleAPI(string fullName);

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

    }

}