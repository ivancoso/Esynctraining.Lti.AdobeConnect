using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class User
    {
        public User()
        {
            Acsession = new HashSet<Acsession>();
            Company = new HashSet<Company>();
            CompanyLicenseCreatedByNavigation = new HashSet<CompanyLicense>();
            CompanyLicenseHistory = new HashSet<CompanyLicenseHistory>();
            CompanyLicenseModifiedByNavigation = new HashSet<CompanyLicense>();
            CompanyLmsCreatedByNavigation = new HashSet<CompanyLms>();
            CompanyLmsModifiedByNavigation = new HashSet<CompanyLms>();
            DistractorCreatedByNavigation = new HashSet<Distractor>();
            DistractorModifiedByNavigation = new HashSet<Distractor>();
            File = new HashSet<File>();
            InverseCreatedByNavigation = new HashSet<User>();
            InverseModifiedByNavigation = new HashSet<User>();
            SocialUserTokens = new HashSet<SocialUserTokens>();
            SubModuleCategoryModifiedByNavigation = new HashSet<SubModuleCategory>();
            SubModuleCategoryUser = new HashSet<SubModuleCategory>();
            ThemeCreatedByNavigation = new HashSet<Theme>();
            ThemeModifiedByNavigation = new HashSet<Theme>();
            UserActivation = new HashSet<UserActivation>();
            UserLoginHistory = new HashSet<UserLoginHistory>();
        }

        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public int LanguageId { get; set; }
        public int TimeZoneId { get; set; }
        public int UserRoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public short Status { get; set; }
        public string SessionToken { get; set; }
        public DateTime? SessionTokenExpirationDate { get; set; }
        public Guid? LogoId { get; set; }
        public bool? IsUnsubscribed { get; set; }

        public virtual Company CompanyNavigation { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual Language Language { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual TimeZone TimeZone { get; set; }
        public virtual UserRole UserRole { get; set; }
        public virtual ICollection<Acsession> Acsession { get; set; }
        public virtual ICollection<Company> Company { get; set; }
        public virtual ICollection<CompanyLicense> CompanyLicenseCreatedByNavigation { get; set; }
        public virtual ICollection<CompanyLicenseHistory> CompanyLicenseHistory { get; set; }
        public virtual ICollection<CompanyLicense> CompanyLicenseModifiedByNavigation { get; set; }
        public virtual ICollection<CompanyLms> CompanyLmsCreatedByNavigation { get; set; }
        public virtual ICollection<CompanyLms> CompanyLmsModifiedByNavigation { get; set; }
        public virtual ICollection<Distractor> DistractorCreatedByNavigation { get; set; }
        public virtual ICollection<Distractor> DistractorModifiedByNavigation { get; set; }
        public virtual ICollection<File> File { get; set; }
        public virtual ICollection<User> InverseCreatedByNavigation { get; set; }
        public virtual ICollection<User> InverseModifiedByNavigation { get; set; }
        public virtual ICollection<SocialUserTokens> SocialUserTokens { get; set; }
        public virtual ICollection<SubModuleCategory> SubModuleCategoryModifiedByNavigation { get; set; }
        public virtual ICollection<SubModuleCategory> SubModuleCategoryUser { get; set; }
        public virtual ICollection<Theme> ThemeCreatedByNavigation { get; set; }
        public virtual ICollection<Theme> ThemeModifiedByNavigation { get; set; }
        public virtual ICollection<UserActivation> UserActivation { get; set; }
        public virtual ICollection<UserLoginHistory> UserLoginHistory { get; set; }
    }
}
