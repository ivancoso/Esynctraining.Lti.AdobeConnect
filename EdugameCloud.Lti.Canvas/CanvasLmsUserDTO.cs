using System.Collections.Generic;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Canvas
{
    /// <summary>
    /// todo: temporary class, will be updated
    /// </summary>
    public class CanvasLmsUserDTO : LmsUserDTO
    {
        public List<CanvasEnrollment> enrollments { get; set; } 
    }
}