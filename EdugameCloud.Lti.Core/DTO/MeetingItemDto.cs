using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public sealed class MeetingItemDto
    {
        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "url")]
        public string url { get; set; }

        [DataMember(Name = "sco_id")]
        public string sco_id { get; set; }


        public static MeetingItemDto Build(MeetingItem arg)
        {
            return new MeetingItemDto
            {
                name = arg.MeetingName,
                url = arg.UrlPath,
                sco_id = arg.ScoId,
            };
        }

    }

}
