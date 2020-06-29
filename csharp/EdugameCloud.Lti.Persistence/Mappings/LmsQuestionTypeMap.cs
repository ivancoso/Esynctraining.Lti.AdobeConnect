using Esynctraining.Persistence.Mappings;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    /// <summary>
    /// The LMS question type map.
    /// </summary>
    public sealed class LmsQuestionTypeMap : BaseClassMap<LmsQuestionType>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsQuestionTypeMap"/> class.
        /// </summary>
        public LmsQuestionTypeMap()
        {
            this.Map(x => x.LmsQuestionTypeName).Column("lmsQuestionType").Not.Nullable();
            this.Map(x => x.QuestionTypeId).Not.Nullable();
            this.Map(x => x.SubModuleId).Nullable();
            this.References(x => x.LmsProvider).Not.Nullable();
        }

        #endregion
    }
}
