using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Esynctraining.Lti.Lms.Common.Dto
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