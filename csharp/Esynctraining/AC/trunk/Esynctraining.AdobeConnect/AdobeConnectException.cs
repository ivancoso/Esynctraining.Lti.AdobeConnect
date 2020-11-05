using System;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public class AdobeConnectException : Exception
    {
        public StatusInfo Status { get; set; }


        public AdobeConnectException() : base() { }

        public AdobeConnectException(StatusInfo status) : base(string.Format("[AdobeConnectProxy Error] {0}.", status.GetErrorInfo()))
        {
            Status = status;
        }

        public AdobeConnectException(string message, Exception innerException) : base(message, innerException) { }
        
    }

}
