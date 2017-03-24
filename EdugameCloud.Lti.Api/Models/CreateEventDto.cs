using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class CreateEventDto
    {
        [DataMember]
        public int meetingId { get; set; }
    }
}
