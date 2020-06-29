namespace EdugameCloud.Persistence.Mappings
{
    using System.Globalization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Utils;

    /// <summary>
    /// The question mapping
    /// </summary>
    public class QuestionMap : BaseClassMap<Question>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionMap"/> class. 
        /// </summary>
        public QuestionMap()
        {
            this.Map(x => x.QuestionName).Length(2000).Not.Nullable();
            this.Map(x => x.HtmlText).Length(2000).Nullable();
            this.Map(x => x.QuestionOrder).Not.Nullable();
            this.Map(x => x.Instruction).Nullable();
            this.Map(x => x.CorrectMessage).Nullable();
            this.Map(x => x.CorrectReference).Length(2000).Nullable();
            this.Map(x => x.IncorrectMessage).Nullable();
            this.Map(x => x.Hint).Nullable();
            this.Map(x => x.IsActive).Nullable();
            this.Map(x => x.ScoreValue).Not.Nullable().Default(0.ToString(CultureInfo.InvariantCulture));
            this.Map(x => x.DateModified).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.LmsQuestionId).Nullable();
            this.Map(x => x.RandomizeAnswers).Nullable();
            this.Map(x => x.Rows).Nullable();
            this.Map(x => x.LmsProviderId).Nullable().Column("lmsProviderId");

            this.HasMany(x => x.QuizQuestionResults).ExtraLazyLoad().Cascade.DeleteOrphan().Inverse();
            this.HasMany(x => x.SurveyQuestionResults).ExtraLazyLoad().Cascade.DeleteOrphan().Inverse();
            this.HasMany(x => x.TestQuestionResults).ExtraLazyLoad().Cascade.DeleteOrphan().Inverse();
            this.HasMany(x => x.Distractors).ExtraLazyLoad().Cascade.DeleteOrphan().Inverse();
            this.HasMany(x => x.LikertQuestions).ExtraLazyLoad().Cascade.DeleteOrphan().Inverse();
            this.HasMany(x => x.RateQuestions).ExtraLazyLoad().Cascade.DeleteOrphan().Inverse();
            this.HasMany(x => x.TrueFalseQuestions).ExtraLazyLoad().Cascade.DeleteOrphan().Inverse();
            this.HasMany(x => x.WeightBucketQuestions).ExtraLazyLoad().Cascade.DeleteOrphan().Inverse();
            this.HasMany(x => x.OpenAnswerQuestions).ExtraLazyLoad().Cascade.DeleteOrphan().Inverse();
            this.HasMany(x => x.SingleMultipleChoiceQuestions).ExtraLazyLoad().Cascade.DeleteOrphan().Inverse();

            this.References(x => x.QuestionType);
            this.References(x => x.SubModuleItem);
            this.References(x => x.Image).Column("imageId");
            this.References(x => x.ModifiedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<Question>(x => x.ModifiedBy)));
            this.References(x => x.CreatedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<Question>(x => x.CreatedBy)));
        }

        #endregion
    }
}