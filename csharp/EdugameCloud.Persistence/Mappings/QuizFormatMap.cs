namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The quiz format mapping
    /// </summary>
    public class QuizFormatMap : BaseClassMap<QuizFormat>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizFormatMap"/> class. 
        /// </summary>
        public QuizFormatMap()
        {
            this.Map(x => x.QuizFormatName).Length(50).Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.IsActive).Nullable();
        }

        #endregion
    }
}