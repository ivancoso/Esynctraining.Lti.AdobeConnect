namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The survey question result mapping
    /// </summary>
    public class SurveyQuestionResultAnswerMap : BaseClassMap<SurveyQuestionResultAnswer>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyQuestionResultAnswerMap"/> class. 
        /// </summary>
        public SurveyQuestionResultAnswerMap()
        {
            this.Map(x => x.Value).Not.Nullable();
            this.References(x => x.SurveyDistractor).Column("surveyDistractorId").Nullable();
            this.References(x => x.SurveyQuestionResultAnswerRef).Column("surveyQuestionResultAnswerRefId").Nullable();
            this.References(x => x.SurveyQuestionResult).Not.Nullable();
        }

        #endregion
    }
}