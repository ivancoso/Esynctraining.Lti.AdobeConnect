using System;

namespace EdugameCloud.Lti.Core
{
    /// <summary>
    /// TRICK: used to pass messages to user through application layers.
    /// </summary>
    public sealed class WarningMessageException : Exception
    {
        public WarningMessageException() : base() { }

        public WarningMessageException(string message) : base(message) { }

        public WarningMessageException(string message, Exception innerException) : base(message, innerException) { }

    }

}
