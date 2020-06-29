namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The language.
    /// </summary>
    public class Language : Entity
    {
        /// <summary>
        /// Gets or sets the language name.
        /// </summary>
        public virtual string LanguageName { get; set; }


        /// <summary>
        /// ISO 639-1 two-letter language code.
        /// </summary>
        public virtual string TwoLetterCode { get; set; }
 
    }

}