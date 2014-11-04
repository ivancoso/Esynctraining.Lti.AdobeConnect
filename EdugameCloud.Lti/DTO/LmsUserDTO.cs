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
        #region Public Properties

        /// <summary>
        /// Gets the email.
        /// </summary>
        public string Email
        {
            get
            {
                if (this.primary_email != null)
                {
                    return this.primary_email;
                }

                try
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new MailAddress(this.Login);
                    return this.Login;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        public string FirstName
        {
            get
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
        }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        public string LastName
        {
            get
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
        }

        /// <summary>
        /// Gets the login.
        /// </summary>
        public string Login
        {
            get
            {
                if (this.login_id != null)
                {
                    return this.login_id;
                }

                return this.name;
            }
        }

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
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the primary_email.
        /// </summary>
        [DataMember]
        public string primary_email { get; set; }

        #endregion
    }
}