namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The Language mapping
    /// </summary>
    public class LanguageMap : BaseClassMap<Language>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageMap"/> class.
        /// </summary>
        public LanguageMap()
        {
            this.Map(x => x.LanguageName).Length(100).Not.Nullable();
            this.Map(x => x.TwoLetterCode).Length(2).Not.Nullable();
        }

        #endregion
    }
}