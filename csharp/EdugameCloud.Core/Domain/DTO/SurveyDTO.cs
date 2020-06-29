namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The survey DTO.
    /// </summary>
    [DataContract]
    public class SurveyDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyDTO"/> class.
        /// </summary>
        public SurveyDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public SurveyDTO(Survey result)
        {
            this.surveyId = result.Id;
            this.subModuleItemId = result.SubModuleItem.Return(x => x.Id, (int?)null);
            this.surveyGroupingTypeId = result.SurveyGroupingType.With(x => x.Id);
            this.description = result.Description;
            this.surveyName = result.SurveyName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public virtual string description { get; set; }

        /// <summary>
        /// Gets or sets the quiz format.
        /// </summary>
        [DataMember]
        public virtual int surveyGroupingTypeId { get; set; }

        /// <summary>
        /// Gets or sets the quiz name.
        /// </summary>
        [DataMember]
        public virtual string surveyName { get; set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public virtual int? subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public virtual int surveyId { get; set; }

        /// <summary>
        /// Gets or sets the moodle id.
        /// </summary>
        [DataMember]
        public int lmsSurveyId { get; set; }

        #endregion
    }
}