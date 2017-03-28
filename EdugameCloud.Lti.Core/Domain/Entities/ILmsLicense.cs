using EdugameCloud.Lti.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.Domain.Entities
{
    public interface ILmsLicense
    {
        int Id { get; }

        string Title { get; }

        int LmsProviderId { get; }

        string ConsumerKey { get; }

        string SharedSecret { get; }

        string AcPassword { get; }

        string AcServer { get; }

        string AcUsername { get; }

        string LmsDomain { get; }

        bool IsActive { get; }

        int CompanyId { get; }

        bool? UseSSL { get; }

        bool? LoginUsingCookie { get; set; }

        bool UseMP4 { get; }

        bool? EnableProxyToolMode { get; }
        
        string ProxyToolSharedPassword { get; }

        bool AutoPublishRecordings { get; }

        bool UseSynchronizedUsers { get; }

        bool DenyACUserCreation { get; }

        bool? ACUsesEmailAsLogin { get; }

        bool EnableMeetingReuse { get; }

        bool? CanRemoveMeeting { get; }

        bool CanRemoveRecordings { get; }

        bool? EnableOfficeHours { get; }

        bool? ShowAnnouncements { get; }

        int MeetingNameFormatterId { get; }

        int LanguageId { get; }

        // TODO: lazy load????
        IList<LmsCourseMeeting> LmsCourseMeetings { get; }

        IList<LmsCompanyRoleMapping> RoleMappings { get; }

        IList<LmsCompanySetting> Settings { get; }

        IList<LmsUser> LmsUsers { get; }

        LmsUser AdminUser { get; }

        bool HasLmsDomain(string domainToCheck);

        T GetSetting<T>(string settingName);

        T GetSetting<T>(string settingName, T defaultValue);

        TelephonyProfileOption GetTelephonyOption(LmsMeetingType meetingType);

    }

}
