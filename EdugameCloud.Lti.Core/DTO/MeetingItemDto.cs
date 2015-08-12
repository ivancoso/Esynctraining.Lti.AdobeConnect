using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.DTO
{
    public sealed class MeetingItemDto
    {
        public string name { get; set; }

        public string url { get; set; }

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
