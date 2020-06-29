namespace Esynctraining.Lti.Lms.Canvas
{
    public class CanvasEnrollment
    {
        public static class EnrollmentState
        {
            public static readonly string Active = "active";

            public static readonly string Invited = "invited";

            public static readonly string Inactive = "inactive";

        }

        public string course_id { get; set; } //check

        public string role { get; set; }

        public int course_section_id { get; set; }

        /// <summary>
        /// https://canvas.instructure.com/doc/api/enrollments.html#Enrollment
        /// </summary>
        public string enrollment_state { get; set; }

    }

}