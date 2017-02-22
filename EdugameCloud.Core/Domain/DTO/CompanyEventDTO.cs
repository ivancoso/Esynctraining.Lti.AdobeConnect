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
        [DataMember]
        public DateTime dateEnd { get; set; }
        [DataMember]
        public DateTime dateCreated { get; set; }
        [DataMember]
        public DateTime dateModified { get; set; }
        [DataMember]
        public bool isSeminar { get; set; }


        [DataMember]
        public bool isMappedToQuizzes { get; set; }

    }
}