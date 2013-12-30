namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Utils;

    /// <summary>
    /// The question mapping
    /// </summary>
    public class QuestionForRateMap : BaseClassMap<QuestionForRate>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionForRateMap"/> class. 
        /// </summary>
        public QuestionForRateMap()
        {
            this.Map(x => x.Restrictions).Nullable();
            this.Map(x => x.PageNumber).Nullable();
            this.Map(x => x.AllowOther).Nullable();
            this.Map(x => x.IsMandatory).Not.Nullable().Default("1");

            this.References(x => x.Question);
        }

        #endregion
    }
}