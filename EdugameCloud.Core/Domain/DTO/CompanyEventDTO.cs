using System;
using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class CompanyEventDTO
    {

        [DataMember]
        public int companyId { get; set; }

        [DataMember]
        public string scoId { get; set; }

        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string desc { get; set; }
        [DataMember]
        public string urlPath { get; set; }
        [DataMember]
        public DateTime dateBegin { get; set; }
        public DateTime dateEnd { get; set; }
        public DateTime dateCreated { get; set; }
        public DateTime dateModified { get; set; }
        public bool isSeminar { get; set; }

    }
}