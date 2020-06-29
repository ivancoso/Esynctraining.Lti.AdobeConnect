namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The question mapping
    /// </summary>
    public class QuestionForLikertMap : BaseClassMap<QuestionForLikert>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionForLikertMap"/> class. 
        /// </summary>
        public QuestionForLikertMap()
        {
            this.Map(x => x.PageNumber).Nullable();
            this.Map(x => x.AllowOther).Nullable();
            this.Map(x => x.IsMandatory).Not.Nullable().Default("1");

            this.References(x => x.Question);
        }

        #endregion
    }
}