namespace Esynctraining.Core.Wrappers
{
    using System.Globalization;

    /// <summary>
    /// The culture info wrapper.
    /// </summary>
    public class CultureInfoWrapper
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CultureInfoWrapper"/> class.
        /// </summary>
        public CultureInfoWrapper()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CultureInfoWrapper"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        public CultureInfoWrapper(CultureInfo info)
            : this(info.Name, info.EnglishName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CultureInfoWrapper"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="englishName">
        /// The english name.
        /// </param>
        public CultureInfoWrapper(string name, string englishName)
        {
            this.Name = name;
            this.EnglishName = englishName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the english name.
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}