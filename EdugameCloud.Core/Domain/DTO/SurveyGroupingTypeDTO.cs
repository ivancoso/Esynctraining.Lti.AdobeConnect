namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;


    /// <summary>
    ///     The survey grouping type DTO.
    /// </summary>
    [DataContract]
    public class SurveyGroupingTypeDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyGroupingTypeDTO"/> class.
        /// </summary>
        public SurveyGroupingTypeDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyGroupingTypeDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public SurveyGroupingTypeDTO(SurveyGroupingType result)
        {
            this.surveyGroupingTypeId = result.Id;
            this.surveyGroupingType = result.SurveyGroupingTypeName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public virtual string surveyGroupingType { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public virtual int surveyGroupingTypeId { get; set; }

        #endregion
    }
}