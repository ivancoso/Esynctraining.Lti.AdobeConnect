using System;
using Esynctraining.Core;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    internal sealed class WarningMessageException : Exception, IUserMessageException
    {
        public WarningMessageException() : base() { }

        public WarningMessageException(string message) : base(message) { }

        public WarningMessageException(string message, Exception innerException) : base(message, innerException) { }

    }
}
