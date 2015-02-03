namespace EdugameCloud.Lti.Domain.Entities
{
    using System;
    using EdugameCloud.Lti.Extensions;
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
        public virtual int CompanyId { get; set; }

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
        public virtual int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the enable proxy tool mode.
        /// </summary>
        public virtual bool? EnableProxyToolMode { get; set; }

        /// <summary>
        /// Gets or sets the proxy tool shared password.
        /// </summary>
        public virtual string ProxyToolSharedPassword { get; set; }

        /// <summary>
        /// Gets or sets the LMS domain.
        /// </summary>
        public virtual string LmsDomain
        {
            get
            {
                var domainUrl = this.lmsDomain.Return(x => x.ToLower(), string.Empty);
                
                return domainUrl.RemoveHttpProtocolAndTrailingSlash();
            }

            set
            {
                if (this.UseSSL != true && value.IsSSL())
                {
                    this.UseSSL = true;
                }

                this.lmsDomain = value.Return(x => x.TrimEnd(@"/\".ToCharArray()), null).RemoveHttpProtocolAndTrailingSlash();
            }
        }

        /// <summary>
        /// Gets or sets the LMS provider.
        /// </summary>
        public virtual LmsProvider LmsProvider { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public virtual int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the primary color.
        /// </summary>
        public virtual string PrimaryColor { get; set; }

        /// <summary>
        /// Gets or sets the last signal id.
        /// </summary>
        public virtual long LastSignalId { get; set; }

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
        /// Gets or sets the use user folder.
        /// </summary>
        public virtual bool? UseUserFolder { get; set; }

        /// <summary>
        /// Gets or sets the user folder name.
        /// </summary>
        public virtual string UserFolderName { get; set; }

        /// <summary>
        /// Gets or sets the can remove meeting.
        /// </summary>
        public virtual bool? CanRemoveMeeting { get; set; }

        /// <summary>
        /// Gets or sets the can edit meeting.
        /// </summary>
        public virtual bool? CanEditMeeting { get; set; }

        /// <summary>
        /// Gets or sets the is settings visible.
        /// </summary>
        public virtual bool? IsSettingsVisible { get; set; }

        /// <summary>
        /// Gets or sets the enable office hours.
        /// </summary>
        public virtual bool? EnableOfficeHours { get; set; }

        /// <summary>
        /// Gets or sets the enable study groups.
        /// </summary>
        public virtual bool? EnableStudyGroups { get; set; }

        /// <summary>
        /// Gets or sets the enable course meetings.
        /// </summary>
        public virtual bool? EnableCourseMeetings { get; set; }

        /// <summary>
        /// Gets or sets the show lms help.
        /// </summary>
        public virtual bool? ShowLmsHelp { get; set; }

        /// <summary>
        /// Gets or sets the show egc help.
        /// </summary>
        public virtual bool? ShowEGCHelp { get; set; }

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