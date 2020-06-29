using Esynctraining.Persistence.Mappings;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    /// <summary>
    /// The company LMS map.
    /// </summary>
    public sealed class LmsCompanyMap : BaseClassMap<LmsCompany>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LmsCompanyMap" /> class.
        /// </summary>
        public LmsCompanyMap()
        {
            this.Table("CompanyLms");
            this.Id(x => x.Id).Column("companyLmsId");
            this.Map(x => x.AcPassword).Not.Nullable();
            this.Map(x => x.AcServer).Not.Nullable();
            this.Map(x => x.AcUsername).Not.Nullable();
            this.Map(x => x.ACScoId).Nullable();
            this.Map(x => x.ACTemplateScoId).Nullable();
            this.Map(x => x.ConsumerKey).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Nullable();
            this.Map(x => x.SharedSecret).Not.Nullable();
            this.Map(x => x.PrimaryColor).Nullable();
            this.Map(x => x.LmsDomain).Nullable();
            this.Map(x => x.ShowAnnouncements).Nullable();
            this.Map(x => x.Title).Nullable();
            this.Map(x => x.LastSignalId).Not.Nullable().Default("(0)");
            this.Map(x => x.UseSSL).Nullable();
            this.Map(x => x.UseUserFolder).Nullable();
            this.Map(x => x.CanRemoveMeeting).Nullable();
            this.Map(x => x.CanEditMeeting).Nullable();
            this.Map(x => x.IsSettingsVisible).Nullable();
            this.Map(x => x.UserFolderName).Nullable();
            this.Map(x => x.EnableOfficeHours).Nullable();
            this.Map(x => x.EnableStudyGroups).Nullable();
            this.Map(x => x.EnableCourseMeetings).Nullable();
            this.Map(x => x.ShowEGCHelp).Nullable();
            this.Map(x => x.ShowLmsHelp).Nullable();
            this.Map(x => x.ACUsesEmailAsLogin).Nullable();
            this.Map(x => x.LoginUsingCookie).Nullable();
            this.Map(x => x.AddPrefixToMeetingName).Nullable();
            this.Map(x => x.IsActive).Not.Nullable();
            
            this.Map(x => x.EnableProxyToolMode).Nullable();
            this.Map(x => x.ProxyToolSharedPassword).Nullable();

            this.Map(x => x.CreatedBy).Not.Nullable().Column("createdBy");
            this.Map(x => x.ModifiedBy).Nullable().Column("modifiedBy");
            this.Map(x => x.CompanyId).Not.Nullable().Column("companyId");
            this.Map(x => x.LmsProviderId).Not.Nullable().Column("lmsProviderId");

            // TRICK: .Not.LazyLoad() removed for LTI !!
            this.References(x => x.AdminUser).Column("adminUserId").Nullable();
            this.HasMany(x => x.LmsUsers).KeyColumn("companyLmsId").Cascade.Delete().Inverse();
            this.HasMany(x => x.LmsCourseMeetings).KeyColumn("companyLmsId").Cascade.Delete().Inverse();
            this.HasMany(x => x.Settings).KeyColumn("lmsCompanyId").Cascade.All().Inverse();
            this.HasMany(x => x.RoleMappings).KeyColumn("lmsCompanyId").Cascade.AllDeleteOrphan().Inverse();            
        }

        #endregion
    }
}