using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect.Api.Seminar.Dto
{
    [DataContract]
    public class SeminarSessionDto// : AdobeConnect.SeminarSessionDto
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string SeminarRoomId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Summary { get; set; }
        
        [DataMember]
        public long StartTimeStamp { get; set; }

        [DataMember]
        public string AcRoomUrl { get; set; }

        [DataMember]
        public string Duration { get; set; }

        [DataMember]
        public bool IsEditable { get; set; }
        
        
    }

    [DataContract]
    public class SeminarSessionInputDto: SeminarSessionDto
    {
        public int ExpectedLoad { get; set; }

        [DataMember]
        public string StartDate { get; set; }

        [DataMember]
        public string StartTime { get; set; }

    }


}
