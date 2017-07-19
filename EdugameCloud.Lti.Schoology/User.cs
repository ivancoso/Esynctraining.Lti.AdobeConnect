namespace EdugameCloud.Lti.Schoology
{
    internal class User
    {
        public string uid { get; set; }
        public int id { get; set; }
        //public int school_id { get; set; }
        public int synced { get; set; }
        //public string school_uid { get; set; }
        //public string additional_buildings { get; set; }
        public string name_title { get; set; }
        public int name_title_show { get; set; }
        public string name_first { get; set; }
        public string name_first_preferred { get; set; }
        public string use_preferred_first_name { get; set; }
        public string name_middle { get; set; }
        public int name_middle_show { get; set; }
        public string name_last { get; set; }
        public string name_display { get; set; }
        public string username { get; set; }
        public string primary_email { get; set; }
        //public string picture_url { get; set; }
        //public object gender { get; set; }
        //public object position { get; set; }
        //public string grad_year { get; set; }
        //public string password { get; set; }
        //public int role_id { get; set; }
        //public int tz_offset { get; set; }
        //public string tz_name { get; set; }
        //public object parents { get; set; }
        //public object child_uids { get; set; }
        //public int send_message { get; set; }
        //public string language { get; set; }
        //public Permissions permissions { get; set; }


        // TRICK: enrollment
        public int admin { get; set; }
    }

}
