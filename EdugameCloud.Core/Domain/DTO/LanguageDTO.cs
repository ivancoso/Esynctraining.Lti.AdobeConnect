namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The language dto.
    /// </summary>
    [DataContract]
    public class LanguageDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageDTO"/> class.
        /// </summary>
        public LanguageDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageDTO"/> class.
        /// </summary>
        /// <param name="lng">
        /// The role.
        /// </param>
        public LanguageDTO(Language lng)
        {
            this.languageId = lng.Id;
            this.language = lng.LanguageName;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int languageId { get; set; }

        /// <summary>
        /// Gets or sets the language name.
        /// </summary>
        [DataMember]
        public string language { get; set; }

        #endregion
    }
}