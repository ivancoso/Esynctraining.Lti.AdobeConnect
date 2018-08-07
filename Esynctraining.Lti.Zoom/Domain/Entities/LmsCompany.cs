using System;
using System.Collections.Generic;
using System.Text;
using Esynctraining.Lti.Zoom.Constants;

namespace Esynctraining.Lti.Zoom.Domain.Entities
{
    public class LmsCompany : ILmsLicense
    {
        /// <summary>
        /// The LMS domain.
        /// </summary>
        private string lmsDomain;

        /// <summary>
        /// Gets or sets the use SSL.
        /// </summary>
        public virtual bool? UseSSL { get; set; }


        public int LanguageId { get; set; }

        public virtual string ConsumerKey { get; set; }

        public bool? ACUsesEmailAsLogin { get; set; }

        public virtual string SharedSecret { get; set; }

        public static LmsCompany GenerateCompany()
        {
            LmsCompany lmsCompany = new LmsCompany();
            lmsCompany.LanguageId = 5;
            lmsCompany.ACUsesEmailAsLogin = true;
            lmsCompany.ConsumerKey = "123";
            lmsCompany.CompanyId = 4;
            lmsCompany.LmsProviderId = 2;
            lmsCompany.IsActive = true;
            lmsCompany.SharedSecret = "f6f32cd0-70ca-4824-b74d-2abc59eac888";
            return lmsCompany;
        }

        /// <summary>
        /// Gets or sets the LMS domain.
        /// </summary>
        public virtual string LmsDomain
        {
            get
            {
                var domainUrl = this.lmsDomain == null ? string.Empty : lmsDomain.ToLower();

                return domainUrl;
            }

            set
            {

                if (this.UseSSL != true && (!string.IsNullOrWhiteSpace(value) && value.StartsWith(HttpScheme.Https, StringComparison.OrdinalIgnoreCase)))
                {
                    this.UseSSL = true;
                }

                value = value?.TrimEnd(@"/\".ToCharArray());

                this.lmsDomain = value;
            }
        }

        public int Id => throw new NotImplementedException();

        public string Title => throw new NotImplementedException();

        public int LmsProviderId { get; set; }

        public string AcPassword => throw new NotImplementedException();

        public string AcServer => throw new NotImplementedException();

        public string AcUsername => throw new NotImplementedException();

        public bool IsActive { get; set; }

        public int CompanyId { get; set; }

        public bool? LoginUsingCookie { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool UseMP4 => throw new NotImplementedException();

        public bool? EnableProxyToolMode => throw new NotImplementedException();

        public string ProxyToolSharedPassword => throw new NotImplementedException();

        public bool AutoPublishRecordings => throw new NotImplementedException();

        public bool UseSynchronizedUsers => throw new NotImplementedException();

        public bool DenyACUserCreation => throw new NotImplementedException();

        public bool EnableMeetingReuse => throw new NotImplementedException();

        public bool? CanRemoveMeeting => throw new NotImplementedException();

        public bool CanRemoveRecordings => throw new NotImplementedException();

        public bool? EnableOfficeHours => throw new NotImplementedException();

        public bool? ShowAnnouncements => throw new NotImplementedException();

        public int MeetingNameFormatterId => throw new NotImplementedException();
    }
}
