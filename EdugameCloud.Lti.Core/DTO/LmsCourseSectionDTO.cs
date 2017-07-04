using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class LmsCourseSectionDTO
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<LmsUserDTO> Users { get; set; }
    }
}