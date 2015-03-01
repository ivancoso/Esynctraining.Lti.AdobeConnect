namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Extensions;

    /// <summary>
    ///     The survey item DTO.
    /// </summary>
    [DataContract]
    public class SurveyFromStoredProcedureDTO
    {
        public SurveyFromStoredProcedureDTO()
        {
        }

        public SurveyFromStoredProcedureDTO(SurveyFromStoredProcedureExDTO dto)
        {
            this.surveyId = dto.surveyId;
            this.surveyName = dto.surveyName;
            this.categoryName = dto.categoryName;
            this.createdBy = dto.createdBy;
            this.createdByLastName = dto.createdByLastName;
            this.createdByName = dto.createdByName;
            this.dateModified = dto.dateModified.ConvertToUnixTimestamp();
            this.firstName = dto.firstName;
            this.lastName = dto.lastName;
            this.description = dto.description;
            this.subModuleItemId = dto.subModuleItemId;
            this.subModuleCategoryId = dto.subModuleCategoryId;
            this.userId = dto.userId;
            this.surveyGroupingTypeId = dto.surveyGroupingTypeId;
            this.lmsSurveyId = dto.lmsSurveyId;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the survey id.
        /// </summary>
        [DataMember]
        public int surveyId { get; set; }

        /// <summary>
        /// Gets or sets the quiz name.
        /// </summary>
        [DataMember]
        public string surveyName { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int createdBy { get; set; }

        /// <summary>
        /// Gets or sets the created by last name.
        /// </summary>
        [DataMember]
        public string createdByLastName { get; set; }

        /// <summary>
        /// Gets or sets the created by name.
        /// </summary>
        [DataMember]
        public string createdByName { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [DataMember]
        public string firstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [DataMember]
        public string lastName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public string description { get; set; }

        /// <summary>
        ///     Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public int subModuleCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public int userId { get; set; }

        /// <summary>
        /// Gets or sets the survey Grouping Type id.
        /// </summary>
        [DataMember]
        public int surveyGroupingTypeId { get; set; }

        /// <summary>
        /// Gets or sets the LMS survey id.
        /// </summary>
        [DataMember]
        public int? lmsSurveyId { get; set; }

        #endregion
    }
}