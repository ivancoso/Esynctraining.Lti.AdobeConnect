using System.Collections.Generic;

namespace EdugameCloud.Lti.Schoology
{
    internal class Enrollment
    {
        public string id { get; set; }
        public string uid { get; set; }
        //public string school_uid { get; set; }
        //public string name_title { get; set; }
        //public string name_title_show { get; set; }
        //public string name_first { get; set; }
        //public string name_first_preferred { get; set; }
        //public string use_preferred_first_name { get; set; }
        //public string name_middle { get; set; }
        //public string name_middle_show { get; set; }
        //public string name_last { get; set; }
        //public string name_display { get; set; }
        public int admin { get; set; }
        public string status { get; set; }
        //public string picture_url { get; set; }
        //public Links links { get; set; }
    }

    internal class RootObject2
    {
        public List<Enrollment> enrollment { get; set; }
        public string total { get; set; }
        public Links links { get; set; }
    }
}
