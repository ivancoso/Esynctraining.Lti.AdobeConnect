using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Canvas
{
    /// <summary>
    /// todo: temporary class, will be updated
    /// </summary>
    internal sealed class CanvasLmsUserDTO
    {
        public List<CanvasEnrollment> enrollments { get; set; }

        public string Id { get; set; }

        public string login_id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}