namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The survey item DTO.
    /// </summary>
    [DataContract]
    public class SurveyFromStoredProcedureDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the survey id.
        /// </summary>
        [DataMember]
        public virtual int surveyId { get; set; }

        /// <summary>
        /// Gets or sets the quiz name.
        /// </summary>
        [DataMember]
        public virtual string surveyName { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public virtual string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public virtual int createdBy { get; set; }

        /// <summary>
        /// Gets or sets the created by last name.
        /// </summary>
        [DataMember]
        public virtual string createdByLastName { get; set; }

        /// <summary>
        /// Gets or sets the created by name.
        /// </summary>
        [DataMember]
        public virtual string createdByName { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public virtual DateTime dateModified { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [DataMember]
        public virtual string firstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [DataMember]
        public virtual string lastName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public virtual string description { get; set; }

        /// <summary>
        ///     Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public virtual int subModuleCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public virtual int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public virtual int userId { get; set; }

        /// <summary>
        /// Gets or sets the survey Grouping Type id.
        /// </summary>
        [DataMember]
        public virtual int surveyGroupingTypeId { get; set; }

        #endregion
    }
}