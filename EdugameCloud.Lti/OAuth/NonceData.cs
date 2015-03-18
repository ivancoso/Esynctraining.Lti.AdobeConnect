namespace EdugameCloud.Lti.OAuth
{
    using System;

    /// <summary>
    /// The nonce data.
    /// </summary>
    internal sealed class NonceData
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NonceData"/> class.
        /// </summary>
        /// <param name="nonce">
        /// The nonce.
        /// </param>
        /// <param name="timestamp">
        /// The timestamp.
        /// </param>
        public NonceData(string nonce, DateTime timestamp)
        {
            this.Nonce = nonce;
            this.Timestamp = timestamp;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the nonce.
        /// </summary>
        public string Nonce { get; private set; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        public DateTime Timestamp { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var otherNonce = obj as NonceData;
            if (otherNonce != null)
            {
                return string.Equals(otherNonce.Nonce, this.Nonce, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Nonce.GetHashCode();
        }

        #endregion
    }
}