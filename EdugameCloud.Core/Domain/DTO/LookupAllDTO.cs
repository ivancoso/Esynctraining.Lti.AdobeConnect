namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
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
        public List<BuildVersionTypeDTO> buildVersionTypes { get; set; }

        /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        [DataMember]
        public List<CountryDTO> countries { get; set; }

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        [DataMember]
        public List<LanguageDTO> languages { get; set; }

        /// <summary>
        /// Gets or sets the map providers.
        /// </summary>
        [DataMember]
        public List<SNMapProviderDTO> mapProviders { get; set; }

        /// <summary>
        /// Gets or sets the question types.
        /// </summary>
        [DataMember]
        public List<QuestionTypeDTO> questionTypes { get; set; }

        /// <summary>
        /// Gets or sets the quiz formats.
        /// </summary>
        [DataMember]
        public List<QuizFormatDTO> quizFormats { get; set; }

        /// <summary>
        /// Gets or sets the score types.
        /// </summary>
        [DataMember]
        public List<ScoreTypeDTO> scoreTypes { get; set; }

        /// <summary>
        /// Gets or sets the services.
        /// </summary>
        [DataMember]
        public List<SNServiceDTO> services { get; set; }

        /// <summary>
        /// Gets or sets the states.
        /// </summary>
        [DataMember]
        public List<StateDTO> states { get; set; }

        /// <summary>
        /// Gets or sets the survey grouping types.
        /// </summary>
        [DataMember]
        public List<SurveyGroupingTypeDTO> surveyGroupingTypes { get; set; }

        /// <summary>
        /// Gets or sets the time zones.
        /// </summary>
        [DataMember]
        public List<TimeZoneDTO> timeZones { get; set; }

        /// <summary>
        /// Gets or sets the user roles.
        /// </summary>
        [DataMember]
        public List<UserRoleDTO> userRoles { get; set; }

        #endregion
    }
}