namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Net.Mail;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;
    
    [DataContract]
    public class LmsUserDTO
    {
        private string _name;


        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserDTO"/> class.
        /// </summary>
        public LmsUserDTO()
        {
            this.is_editable = true;
        }

        #region Public Properties
        
        [DataMember]
        public string ac_id { get; set; }
        
        [DataMember]
        public int? ac_role { get; set; }
        
        [DataMember]
        public string lms_role { get; set; }
        
        [DataMember]
        public string id { get; set; }

        [DataMember]
        [ScriptIgnore]
        public string login_id { get; set; }
        
        [DataMember]
        public string name 
        {
            get 
            {
                return _name; 
            } 
            set 
            {
                if (string.IsNullOrWhiteSpace(value))
                    _name = null;
                else
                    _name = value.Trim();
            } 
        }

        [DataMember]
        [ScriptIgnore]
        public string primary_email { get; set; }
        
        [DataMember]
        public bool is_editable { get; set; }
        
        [DataMember]
        [ScriptIgnore]
        public string lti_id { get; set; }

        [DataMember]
        public string email { get; set; }
        
        [DataMember]
        public int? guest_id { get; set; }

        #endregion

        #region Methods
        
        public string GetLogin()
        {
            return this.login_id ?? this.name;
        }
        
        public string GetEmail()
        {
            if (this.primary_email != null)
            {
                return this.primary_email;
            }

            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new MailAddress(this.GetLogin());
                return this.GetLogin();
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public string GetFirstName()
        {
            if (this.name == null)
            {
                return "no";
            }

            int index = this.name.IndexOf(" ", StringComparison.Ordinal);
            if (index < 0)
            {
                return this.name;
            }

            return this.name.Substring(0, index);
        }
        
        public string GetLastName()
        {
            if (this.name == null)
            {
                return "name";
            }

            int index = this.name.IndexOf(" ", StringComparison.Ordinal);
            if (index < 0)
            {
                return this.lms_role;
            }

            return this.name.Substring(index + 1);
        }

        #endregion

    }

}