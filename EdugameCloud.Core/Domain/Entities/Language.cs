namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The language.
    /// </summary>
    public class Language : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the language name.
        /// </summary>
        public virtual string LanguageName { get; set; }

        #endregion
    }
}