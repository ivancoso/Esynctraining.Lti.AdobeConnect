using System;

namespace Esynctraining.Lti.Lms.Common
{
    public class WarningMessageException : Exception
    {
        public WarningMessageException()
        {
        }

        public WarningMessageException(string message)
            : base(message)
        {
        }

        public WarningMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
