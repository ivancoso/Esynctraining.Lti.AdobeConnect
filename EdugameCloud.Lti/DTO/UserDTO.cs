namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Net.Mail;
    using System.Runtime.Serialization;

    [DataContract]
    public class UserDTO
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string ac_id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string login_id { get; set; }

        [DataMember]
        public string canvas_role { get; set; }

        [DataMember]
        public string ac_role { get; set; }

        [DataMember]
        public string primary_email { get; set; }

        public string Email
        {
            get
            {
                if (primary_email != null) return primary_email;
                try
                {
                    var address = new MailAddress(Login);
                    return Login;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string FirstName
        {
            get
            {
                if (name == null) return "no";
                var index = name.IndexOf(" ");
                if (index < 0) return name;
                return name.Substring(0, index);
            }
        }

        public string LastName
        {
            get
            {
                if (name == null) return "name";
                var index = name.IndexOf(" ");
                if (index < 0) return canvas_role;
                return name.Substring(index);
            }
        }

        public string Login
        {
            get
            {
                if (login_id != null) return login_id;
                return name;
            }
        }
    }
}