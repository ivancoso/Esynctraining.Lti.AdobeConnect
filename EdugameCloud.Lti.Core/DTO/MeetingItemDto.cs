using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public sealed class MeetingItemDto
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string ScoId { get; set; }


        public static MeetingItemDto Build(MeetingItem arg)
        {
            return new MeetingItemDto
            {
                Name = arg.MeetingName,
                Url = arg.UrlPath,
                ScoId = arg.ScoId,
            };
        }

    }

}
