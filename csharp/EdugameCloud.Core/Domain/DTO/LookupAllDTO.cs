namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;


    /// <summary>
    /// The lookup all DTO.
    /// </summary>
    [DataContract]
    public class LookupAllDTO
    {
        #region Properties

        /// <summary>
        /// Gets or sets the build version types.
        /// </summary>
        [DataMember]
        public BuildVersionTypeDTO[] buildVersionTypes { get; set; }

        /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        [DataMember]
        public CountryDTO[] countries { get; set; }

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        [DataMember]
        public LanguageDTO[] languages { get; set; }

        /// <summary>
        /// Gets or sets the map providers.
        /// </summary>
        [DataMember]
        public SNMapProviderDTO[] mapProviders { get; set; }

        /// <summary>
        /// Gets or sets the question types.
        /// </summary>
        [DataMember]
        public QuestionTypeDTO[] questionTypes { get; set; }

        /// <summary>
        /// Gets or sets the quiz formats.
        /// </summary>
        [DataMember]
        public QuizFormatDTO[] quizFormats { get; set; }

        /// <summary>
        /// Gets or sets the score types.
        /// </summary>
        [DataMember]
        public ScoreTypeDTO[] scoreTypes { get; set; }

        /// <summary>
        /// Gets or sets the services.
        /// </summary>
        [DataMember]
        public SNServiceDTO[] services { get; set; }

        /// <summary>
        /// Gets or sets the states.
        /// </summary>
        [DataMember]
        public StateDTO[] states { get; set; }

        //[DataMember]
        //public SchoolDTO[] schools { get; set; }

        /// <summary>
        /// Gets or sets the survey grouping types.
        /// </summary>
        [DataMember]
        public SurveyGroupingTypeDTO[] surveyGroupingTypes { get; set; }

        /// <summary>
        /// Gets or sets the time zones.
        /// </summary>
        [DataMember]
        public TimeZoneDTO[] timeZones { get; set; }

        /// <summary>
        /// Gets or sets the user roles.
        /// </summary>
        [DataMember]
        public UserRoleDTO[] userRoles { get; set; }

        #endregion
    }
}