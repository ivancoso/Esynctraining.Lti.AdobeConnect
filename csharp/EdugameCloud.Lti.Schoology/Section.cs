using System.Collections.Generic;

namespace EdugameCloud.Lti.Schoology
{
    public class Section
    {
        public string id { get; set; }
        public string course_title { get; set; }
        public string course_code { get; set; }
        public string course_id { get; set; }
        public string school_id { get; set; }
        public string access_code { get; set; }
        public string section_title { get; set; }
        public string section_code { get; set; }
        public string section_school_code { get; set; }
        public string synced { get; set; }
        public int active { get; set; }
        //public string description { get; set; }
        //public object parent_id { get; set; }
        //public List<int> grading_periods { get; set; }
        //public string profile_url { get; set; }
        //public string location { get; set; }
        //public List<string> meeting_days { get; set; }
        //public string start_time { get; set; }
        //public string end_time { get; set; }
        //public string weight { get; set; }
        //public Options options { get; set; }
        //public Links links { get; set; }
        //public int admin { get; set; }
    }

    public class Links
    {
        public string self { get; set; }
    }

    public class RootObject
    {
        public List<Section> section { get; set; }
        public int total { get; set; }
        public Links links { get; set; }
    }

}
