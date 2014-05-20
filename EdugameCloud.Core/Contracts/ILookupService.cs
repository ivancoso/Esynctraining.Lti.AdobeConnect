namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    using Weborb.Service;

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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<LookupAllDTO> GetAll();

        /// <summary>
        /// The get languages.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<EGCVersionsDTO> GetVersionInfo();

        /// <summary>
        /// The get languages.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<LanguageDTO> GetLanguages();

        /// <summary>
        /// The get build version types.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<BuildVersionTypeDTO> GetBuildVersionTypes();

        /// <summary>
        /// The get question types.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<QuestionTypeDTO> GetQuestionTypes();

        /// <summary>
        /// The get score types.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<ScoreTypeDTO> GetScoreTypes();

        /// <summary>
        /// The get time zones.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<TimeZoneDTO> GetTimeZones();

        /// <summary>
        /// The get user roles.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<UserRoleDTO> GetUserRoles();

        /// <summary>
        /// The get quiz formats.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<QuizFormatDTO> GetQuizFormats();

        /// <summary>
        /// The survey grouping types.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<SurveyGroupingTypeDTO> GetSurveyGroupingTypes();

        /// <summary>
        /// The get countries.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<CountryDTO> GetCountries();

        /// <summary>
        /// The get states.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<StateDTO> GetStates();

        /// <summary>
        /// The get states.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<SNServiceDTO> GetServices();

        /// <summary>
        /// The get states.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<SNMapProviderDTO> GetMapProviders();

        /// <summary>
        /// Gets the location of user by query.
        /// </summary>
        /// <param name="geoDTO">
        /// The geo DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebORBCache(CacheScope = CacheScope.Global)]
        ServiceResponse<GeoResultDTO> GetLocation(GeoDTO geoDTO);

        /// <summary>
        /// Gets the twitter search result by query.
        /// </summary>
        /// <param name="name">
        /// The name query.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TwitterProfileDTO> GetTwitterProfiles(string name);

        /// <summary>
        /// Gets the twitter search result by query.
        /// </summary>
        /// <param name="fullName">
        /// The full name query.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<GoogleSearchDTO> SearchSocialLinksUsingGoogleAPI(string fullName);

        /// <summary>
        /// Converts profile to XML
        /// </summary>
        /// <param name="vcfProfile">
        /// The VCF Profile.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<string> ConvertFromVCF(string vcfProfile);

        /// <summary>
        /// Converts profile to VCF
        /// </summary>
        /// <param name="xmlProfile">
        /// The VCF Profile.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<byte> ConvertToVCF(string xmlProfile);

        /// <summary>
        /// Gets the twitter search result by query.
        /// </summary>
        /// <param name="name">
        /// The name query.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TwitterStatusDTO> GetTweets(string name);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="countryDTO">
        /// The country DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<GeoCountryDTO> SaveCountry(GeoCountryDTO countryDTO);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="stateDTO">
        /// The state DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<GeoStateDTO> SaveState(GeoStateDTO stateDTO);

        #endregion
    }
}