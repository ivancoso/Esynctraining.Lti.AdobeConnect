using System.Collections.Generic;
using System.Runtime.Serialization;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Canvas
{
    /// <summary>
    /// todo: temporary class, will be updated
    /// </summary>
    internal sealed class CanvasLmsUserDTO : LmsUserDTO
    {
        [IgnoreDataMember]
        public List<CanvasEnrollment> enrollments { get; set; } 

    }

}