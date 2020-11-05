using System.Runtime.Serialization;

namespace Esynctraining.Wcf.ErrorHandling
{
    [DataContract]
    public class WcfErrorWrapper
    {
        [DataMember]
        public int ErrorCode { get; set; }

        [DataMember]
        public string FullException { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string Title { get; set; }

    }

}