using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Esynctraining.Mp4Service.Tasks.Client.Dto
{
    [DataContract]
    public class DataPage<T>
    {
        [DataMember(Name = "results")]
        public IEnumerable<T> Results { get; set; }

        [DataMember(Name = "totalCount")]
        public int TotalCount { get; set; }

    }

}
