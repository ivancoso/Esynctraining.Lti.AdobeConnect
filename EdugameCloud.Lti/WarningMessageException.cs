using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdugameCloud.Lti
{
    /// <summary>
    /// TRICK: used to pass messages to user through application layers.
    /// </summary>
    internal sealed class WarningMessageException : Exception
    {
        public WarningMessageException() : base() { }

        public WarningMessageException(string message) : base(message) { }

    }

}
