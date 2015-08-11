using System.Linq;

namespace EdugameCloud.Lti.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using EdugameCloud.Lti.Core.Business.MeetingNameFormatting;
    using EdugameCloud.Lti.Extensions;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The company LMS.
    /// </summary>
    public class LmsCompany : Entity
    {
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
        /// Gets or sets the login using cookie.
        /// </summary>
        public virtual bool? LoginUsingCookie { get; set; }

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
        /// Gets or sets the search principal by email first.
        /// </summary>
        public virtual bool? ACUsesEmailAsLogin { get; set; }

        /// <summary>
        /// Gets or sets the add prefix to meeting name.
        /// </summary>
        public virtual bool? AddPrefixToMeetingName { get; set; }

        public virtual bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the LMS users.
        /// </summary>
        public virtual IList<LmsUser> LmsUsers { get; protected set; }

        /// <summary>
        /// Gets or sets the LMS course meetings.
        /// </summary>
        public virtual IList<LmsCourseMeeting> LmsCourseMeetings { get; protected set; }

        public virtual IList<LmsCompanySetting> Settings { get; set; }

        public virtual IList<LmsCompanyRoleMapping> RoleMappings { get; set; }

        /// <summary>
        /// Gets or sets the enable use flv.
        /// </summary>
        public virtual bool UseFLV
        {
            get
            {
                var useFLV = false;
                var setting = this.Settings.SingleOrDefault(x => string.Compare(x.Name, "UseFLV") == 0);
                return setting != null && bool.TryParse(setting.Value, out useFLV) && useFLV;
            }
            set
            {
                if ((Id == default(int)) && (Settings == null))
                    this.Settings = new List<LmsCompanySetting>();
                var setting = Settings.SingleOrDefault(x => string.Compare(x.Name, "UseFLV", true) == 0);
                if (setting == null)
                {
                    Settings.Add(new LmsCompanySetting
                    {
                        LmsCompany = this,
                        Name = "UseFLV",
                        Value = value.ToString(),
                    });
                }
                else
                {
                    setting.Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets the enable use mp4.
        /// </summary>
        public virtual bool UseMP4 {
            get
            {
                var useMP4 = false;
                var setting = this.Settings.SingleOrDefault(x => string.Compare(x.Name, "UseMP4") == 0);
                return setting != null && bool.TryParse(setting.Value, out useMP4) && useMP4;
            }
            set
            {
                if ((Id == default(int)) && (Settings == null))
                    this.Settings = new List<LmsCompanySetting>();
                var setting = Settings.SingleOrDefault(x => string.Compare(x.Name, "UseMP4", true) == 0);
                if (setting == null)
                {
                    Settings.Add(new LmsCompanySetting
                    {
                        LmsCompany = this,
                        Name = "UseMP4",
                        Value = value.ToString(),
                    });
                }
                else
                {
                    setting.Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets the condition of multiple meetings .
        /// </summary>
        public virtual bool EnableMultipleMeetings
        {
            get
            {
                var enableMultipleMeetings = false;
                var setting = this.Settings.SingleOrDefault(x => string.Compare(x.Name, "EnableMultipleMeetings") == 0);
                return setting != null && bool.TryParse(setting.Value, out enableMultipleMeetings) && enableMultipleMeetings;
            }
            set
            {
                if ((Id == default(int)) && (Settings == null))
                    this.Settings = new List<LmsCompanySetting>();
                var setting = Settings.SingleOrDefault(x => string.Compare(x.Name, "EnableMultipleMeetings", true) == 0);
                if (setting == null)
                {
                    Settings.Add(new LmsCompanySetting
                    {
                        LmsCompany = this,
                        Name = "EnableMultipleMeetings",
                        Value = value.ToString(),
                    });
                }
                else
                {
                    setting.Value = value.ToString();
                }
            }
        }

        public virtual bool DenyACUserCreation
        {
            get
            {
                bool denyACUserCreation = false;
                LmsCompanySetting setting = Settings.SingleOrDefault(x => String.Compare(x.Name, "DenyACUserCreation", true) == 0);
                return setting != null && bool.TryParse(setting.Value, out denyACUserCreation) && denyACUserCreation;
            }
            set
            {
                if ((Id == default(int)) && (Settings == null))
                    Settings = new List<LmsCompanySetting>();

                LmsCompanySetting setting = Settings.SingleOrDefault(x => string.Compare(x.Name, "DenyACUserCreation", true) == 0);
                if (setting == null)
                {
                    Settings.Add(new LmsCompanySetting
                    {
                        LmsCompany = this,
                        Name = "DenyACUserCreation",
                        Value = value.ToString(),
                    });
                }
                else
                {
                    setting.Value = value.ToString();
                }
            }
        }

        public virtual int MeetingNameFormatterId
        {
            get
            {
                LmsCompanySetting setting = Settings.SingleOrDefault(x => string.Compare(x.Name, "MeetingNameFormatterId", true) == 0);
                if (setting == null)
                    return MeetingNameFormatterFactory.DefaultFormatterId;

                return int.Parse(setting.Value);
            }
            set
            {
                if ((Id == default(int)) && (Settings == null))
                    Settings = new List<LmsCompanySetting>();

                LmsCompanySetting setting = Settings.SingleOrDefault(x => string.Compare(x.Name, "MeetingNameFormatterId", true) == 0);
                if (setting == null)
                {
                    Settings.Add(new LmsCompanySetting
                    {
                        LmsCompany = this,
                        Name = "MeetingNameFormatterId",
                        Value = value.ToString(),
                    });
                }
                else
                {
                    setting.Value = value.ToString();
                }
            }
        }

        public virtual bool UseSynchronizedUsers
        {
            get
            {
                bool useSynchronizedUsers = false;
                LmsCompanySetting setting = Settings.SingleOrDefault(x => String.Compare(x.Name, "UseSynchronizedUsers", true) == 0);
                return setting != null && bool.TryParse(setting.Value, out useSynchronizedUsers) && useSynchronizedUsers;
            }
        }

        #endregion

        public LmsCompany()
        {
            RoleMappings = new List<LmsCompanyRoleMapping>();
        }


        // TODO: !!! WWW section!!!
        public virtual bool HasLmsDomain(string domainToCheck)
        {
            string input = LmsDomain;
            int index = input.IndexOf("/");
            if (index > 0)
                input = input.Substring(0, index);

            // NOTE: sakai sends :8080 in our environment- check other LMS
            //index = input.IndexOf(":");
            //if (index > 0)
            //    input = input.Substring(0, index);

            return string.Equals(input, domainToCheck, StringComparison.OrdinalIgnoreCase);
        }

        public virtual T GetSetting<T>(string settingName)
        {
            LmsCompanySetting setting = Settings.SingleOrDefault(x => String.Compare(x.Name, settingName, true) == 0);
            return setting == null || String.IsNullOrWhiteSpace(setting.Value)
                ? default(T)
                : (T)Convert.ChangeType(setting.Value, typeof(T)); // assuming that we convert to primitive type
        }
    }

}