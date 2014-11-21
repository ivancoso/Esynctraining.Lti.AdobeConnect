namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using EdugameCloud.Core.Constants;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    using Iesi.Collections.Generic;

    /// <summary>
    /// The company LMS.
    /// </summary>
    public class CompanyLms : Entity
    {
        /// <summary>
        /// The LMS users.
        /// </summary>
        private ISet<LmsUser> lmsUsers = new HashedSet<LmsUser>();

        /// <summary>
        /// The LMS course meetings.
        /// </summary>
        private ISet<LmsCourseMeeting> lmsCourseMeetings = new HashedSet<LmsCourseMeeting>();

        /// <summary>
        /// The LMS domain.
        /// </summary>
        private string lmsDomain;

        #region Public Properties

        /// <summary>
        /// Gets or sets the AC SCO id.
        /// </summary>
        public virtual string ACScoId { get; set; }

        /// <summary>
        /// Gets or sets the AC template SCO id.
        /// </summary>
        public virtual string ACTemplateScoId { get; set; }

        /// <summary>
        /// Gets or sets the AC password.
        /// </summary>
        public virtual string AcPassword { get; set; }

        /// <summary>
        /// Gets or sets the AC server.
        /// </summary>
        public virtual string AcServer { get; set; }

        /// <summary>
        /// Gets or sets the AC username.
        /// </summary>
        public virtual string AcUsername { get; set; }

        /// <summary>
        /// Gets or sets the admin user.
        /// </summary>
        public virtual LmsUser AdminUser { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        public virtual Company Company { get; set; }

        /// <summary>
        /// Gets or sets the consumer key.
        /// </summary>
        public virtual string ConsumerKey { get; set; }

        /// <summary>
        /// Gets or sets the use SSL.
        /// </summary>
        public virtual bool? UseSSL { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public virtual User CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the LMS domain.
        /// </summary>
        public virtual string LmsDomain
        {
            get
            {
                var domainUrl = this.lmsDomain.Return(x => x.ToLower(), string.Empty);
                if (domainUrl.StartsWith(HttpScheme.Http))
                {
                    return domainUrl.Substring(HttpScheme.Http.Length);
                }

                if (domainUrl.StartsWith(HttpScheme.Https))
                {
                    return domainUrl.Substring(HttpScheme.Https.Length);
                }

                return domainUrl;
            }

            set
            {
                if (!string.IsNullOrWhiteSpace(value) && this.UseSSL != true && value.StartsWith(HttpScheme.Https))
                {
                    this.UseSSL = true;
                }

                this.lmsDomain = value;
            }
        }

        /// <summary>
        /// Gets or sets the LMS provider.
        /// </summary>
        public virtual LmsProvider LmsProvider { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public virtual User ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the primary color.
        /// </summary>
        public virtual string PrimaryColor { get; set; }

        /// <summary>
        /// Gets or sets the shared secret.
        /// </summary>
        public virtual string SharedSecret { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show announcements.
        /// </summary>
        public virtual bool? ShowAnnouncements { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the LMS users.
        /// </summary>
        public virtual ISet<LmsUser> LmsUsers
        {
            get
            {
                return this.lmsUsers;
            }

            set
            {
                this.lmsUsers = value;
            }
        }

        /// <summary>
        /// Gets or sets the LMS course meetings.
        /// </summary>
        public virtual ISet<LmsCourseMeeting> LmsCourseMeetings
        {
            get
            {
                return this.lmsCourseMeetings;
            }

            set
            {
                this.lmsCourseMeetings = value;
            }
        }

        #endregion
    }
}