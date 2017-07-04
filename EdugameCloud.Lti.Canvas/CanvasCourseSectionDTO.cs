using System;

namespace EdugameCloud.Lti.Canvas
{
    internal sealed class CanvasCourseSectionDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long Course_id { get; set; }
        public DateTime? Start_at { get; set; }
        public DateTime? End_at { get; set; }
        /*
        public string Sis_section_id { get; set; }
        public string Integration_id { get; set; }
        public long Sis_import_id { get; set; }
        public long Sis_course_id { get; set; }
        */
        public long? Nonxlist_course_id { get; set; }
        public long? Total_students { get; set; }
    }
}