using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class CompanyLms
    {
        public CompanyLms()
        {
            AcCachePrincipal = new HashSet<AcCachePrincipal>();
            LmsCompanyRoleMapping = new HashSet<LmsCompanyRoleMapping>();
            LmsCompanySetting = new HashSet<LmsCompanySetting>();
            LmsCourseMeeting = new HashSet<LmsCourseMeeting>();
            LmsUser = new HashSet<LmsUser>();
            LmsUserParameters = new HashSet<LmsUserParameters>();
            LmsUserSession = new HashSet<LmsUserSession>();
            SubModuleCategory = new HashSet<SubModuleCategory>();
        }

        public int CompanyLmsId { get; set; }
        public int CompanyId { get; set; }
        public int LmsProviderId { get; set; }
        public string AcServer { get; set; }
        public string AcUsername { get; set; }
        public string AcPassword { get; set; }
        public string ConsumerKey { get; set; }
        public string SharedSecret { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string PrimaryColor { get; set; }
        public int? AdminUserId { get; set; }
        public string AcScoId { get; set; }
        public string AcTemplateScoId { get; set; }
        public string LmsDomain { get; set; }
        public bool? ShowAnnouncements { get; set; }
        public string Title { get; set; }
        public bool? UseSsl { get; set; }
        public long LastSignalId { get; set; }
        public bool? UseUserFolder { get; set; }
        public string UserFolderName { get; set; }
        public bool? CanRemoveMeeting { get; set; }
        public bool? CanEditMeeting { get; set; }
        public bool? IsSettingsVisible { get; set; }
        public bool? EnableOfficeHours { get; set; }
        public bool? EnableStudyGroups { get; set; }
        public bool? EnableCourseMeetings { get; set; }
        public bool? ShowLmsHelp { get; set; }
        public bool? ShowEgchelp { get; set; }
        public bool? EnableProxyToolMode { get; set; }
        public string ProxyToolSharedPassword { get; set; }
        public bool? AcUsesEmailAsLogin { get; set; }
        public bool? LoginUsingCookie { get; set; }
        public bool? AddPrefixToMeetingName { get; set; }
        public bool IsActive { get; set; }

        public virtual LmsUser AdminUser { get; set; }
        public virtual Company Company { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual LmsProvider LmsProvider { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual ICollection<AcCachePrincipal> AcCachePrincipal { get; set; }
        public virtual ICollection<LmsCompanyRoleMapping> LmsCompanyRoleMapping { get; set; }
        public virtual ICollection<LmsCompanySetting> LmsCompanySetting { get; set; }
        public virtual ICollection<LmsCourseMeeting> LmsCourseMeeting { get; set; }
        public virtual ICollection<LmsUser> LmsUser { get; set; }
        public virtual ICollection<LmsUserParameters> LmsUserParameters { get; set; }
        public virtual ICollection<LmsUserSession> LmsUserSession { get; set; }
        public virtual ICollection<SubModuleCategory> SubModuleCategory { get; set; }
    }
}
