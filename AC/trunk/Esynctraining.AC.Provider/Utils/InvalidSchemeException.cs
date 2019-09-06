using System;

namespace Esynctraining.AC.Provider.Utils
{
    public sealed class InvalidSchemeException : Exception
    {
        public InvalidSchemeException() : base() { }

        public InvalidSchemeException(string message) : base(message) { }

        public InvalidSchemeException(string message, Exception innerException) : base(message, innerException) { }

    }

}
