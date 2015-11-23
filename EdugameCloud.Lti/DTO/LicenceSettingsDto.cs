using System;
using EdugameCloud.Core.Business;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Caching;

namespace EdugameCloud.Lti.DTO
{
    public sealed class LicenceSettingsDto
    {
        //public string RestoredACPassword { get; set; }

        public bool ACUsesEmailAsLogin { get; set; }

        public bool UseSynchronizedUsers { get; set; }

        public bool UseFLV { get; set; }

        public bool UseMP4 { get; set; }

        public bool EnableMultipleMeetings { get; set; }

        public bool EnableMeetingReuse { get; set; }

        public string SupportPageHtml { get; set; }

        public string LabelMeeting { get; set; }

        public string LabelOfficeHour { get; set; }

        public string LabelStudyGroup { get; set; }

        public bool ShowMeetingSummary { get; set; }
        
        public bool ShowMeetingTime { get; set; }
        
        public bool ShowMeetingDuration { get; set; }

        public bool CanRemoveRecordings { get; set; }


        public static LicenceSettingsDto Build(LmsCompany value, ICache cache)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (cache == null)
                throw new ArgumentNullException("cache");

            return CacheUtility.GetCachedItem<LicenceSettingsDto>(cache,
                CachePolicies.Keys.CompanyLmsSettings(value.Id),
                CachePolicies.Dependencies.CompanyLmsSettings(value.Id),
                () =>
            {
                return new LicenceSettingsDto
                {
                    //RestoredACPassword = session.LtiSession.RestoredACPassword,
                    ACUsesEmailAsLogin = value.ACUsesEmailAsLogin ?? false,
                    UseSynchronizedUsers = value.UseSynchronizedUsers,
                    UseFLV = value.UseFLV,
                    UseMP4 = value.UseMP4,
                    EnableMultipleMeetings = value.EnableMultipleMeetings,
                    SupportPageHtml = value.GetSetting<string>(LmsCompanySettingNames.SupportPageHtml),

                    LabelMeeting = value.GetSetting<string>(LmsCompanySettingNames.LabelMeeting) ?? "Course Meetings",
                    LabelOfficeHour = value.GetSetting<string>(LmsCompanySettingNames.LabelOfficeHour) ?? "Office Hours",
                    LabelStudyGroup = value.GetSetting<string>(LmsCompanySettingNames.LabelStudyGroup) ?? "Study Groups",
                    EnableMeetingReuse = value.EnableMeetingReuse,

                    ShowMeetingSummary =  value.GetSetting<bool>(LmsCompanySettingNames.ShowMeetingSummary),
                    ShowMeetingTime = value.GetSetting<bool>(LmsCompanySettingNames.ShowMeetingTime),
                    ShowMeetingDuration = value.GetSetting<bool>(LmsCompanySettingNames.ShowMeetingDuration),

                    CanRemoveRecordings = value.CanRemoveRecordings,
                };
            });            
        }

    }
}
