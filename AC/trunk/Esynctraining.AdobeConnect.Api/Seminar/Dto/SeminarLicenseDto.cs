using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect.Api.Seminar.Dto
{
    [DataContract]
    public class SeminarLicenseDto<TRoom>
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public TRoom[] Rooms { get; set; }

    }

}