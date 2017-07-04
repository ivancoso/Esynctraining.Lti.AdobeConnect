using System.Collections.Generic;

namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Net.Mail;
    using System.Runtime.Serialization;
    
    [DataContract]
    public class LmsUserDTO
    {
        private string _name;


        public LmsUserDTO()
        {
            this.IsEditable = true;
        }

        #region Public Properties
        
        [DataMember]
        public string AcId { get; set; }
        
        [DataMember]
        public int? AcRole { get; set; }
        
        [DataMember]
        public string LmsRole { get; set; }
        
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public List<string> SectionIds { get; set; }

        /// <summary>
        /// User's login in LMS system.
        /// If AC uses Email-as-Login, this value represents user's login in AC.
        /// NOTE: login used by External API calls only.
        /// </summary>
        [DataMember]
        public string Login { get; set; }
        
        [DataMember]
        public string Name
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
        
        [IgnoreDataMember]
        public string PrimaryEmail { get; set; }
        
        [DataMember]
        public bool IsEditable { get; set; }

        [IgnoreDataMember]
        public string LtiId { get; set; }

        [DataMember]
        public string Email { get; set; }
        
        [DataMember]
        public int? GuestId { get; set; }

        #endregion

        #region Methods

        public string GetLogin()
        {
            return this.Login ?? this.Name;
        }
        
        public string GetEmail()
        {
            if (this.PrimaryEmail != null)
            {
                return this.PrimaryEmail;
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
            if (this.Name == null)
            {
                return "no";
            }

            int index = this.Name.IndexOf(" ", StringComparison.Ordinal);
            if (index < 0)
            {
                return this.Name;
            }

            return this.Name.Substring(0, index);
        }
        
        public string GetLastName()
        {
            if (this.Name == null)
            {
                return "name";
            }

            int index = this.Name.IndexOf(" ", StringComparison.Ordinal);
            if (index < 0)
            {
                return this.LmsRole;
            }

            return this.Name.Substring(index + 1);
        }

        #endregion

    }

}