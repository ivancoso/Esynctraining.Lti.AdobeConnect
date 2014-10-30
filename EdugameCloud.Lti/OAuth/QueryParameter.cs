namespace EdugameCloud.Lti.OAuth
{
    /// <summary>
    /// The query parameter.
    /// </summary>
    public class QueryParameter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public QueryParameter(string name, string value)
        {
            this.Name = name.OAuthUrlEncode();
            this.Value = value.OAuthUrlEncode();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; private set; }

        #endregion
    }
}