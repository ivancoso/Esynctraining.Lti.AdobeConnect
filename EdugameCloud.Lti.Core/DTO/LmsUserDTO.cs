namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Net.Mail;
    using System.Runtime.Serialization;

    /// <summary>
    /// The user DTO.
    /// </summary>
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

        /// <summary>
        /// Gets or sets the ac_id.
        /// </summary>
        [DataMember]
        public string ac_id { get; set; }

        /// <summary>
        /// Gets or sets the ac_role.
        /// </summary>
        [DataMember]
        public string ac_role { get; set; }

        /// <summary>
        /// Gets or sets the LMS role.
        /// </summary>
        [DataMember]
        public string lms_role { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the login_id.
        /// </summary>
        [DataMember]
        public string login_id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the primary_email.
        /// </summary>
        [DataMember]
        public string primary_email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is_editable.
        /// </summary>
        [DataMember]
        public bool is_editable { get; set; }

        /// <summary>
        /// Gets or sets the lti_id.
        /// </summary>
        [DataMember]
        public string lti_id { get; set; }

        public string email { get; set; }

        // TODO:
        [DataMember]
        public int? guest_id { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the login.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetLogin()
        {
            return this.login_id ?? this.name;
        }

        /// <summary>
        /// Gets the email.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
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

        /// <summary>
        /// Gets the first name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
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

        /// <summary>
        /// Gets the last name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
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

            return this.name.Substring(index);
        }

        #endregion

    }

}