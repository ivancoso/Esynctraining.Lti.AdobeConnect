using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Canvas
{
    /// <summary>
    /// todo: temporary class, will be updated
    /// </summary>
    internal sealed class CanvasLmsUserDTO : LmsUserDTO
    {
        [ScriptIgnore]
        [IgnoreDataMember]
        public List<CanvasEnrollment> enrollments { get; set; } 

    }

}