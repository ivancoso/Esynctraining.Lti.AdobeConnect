namespace EdugameCloud.Lti.Persistence.Mappings
{
    using EdugameCloud.Lti.Domain.Entities;

    using Esynctraining.Persistence.Mappings;

    /// <summary>
    /// The company LMS map.
    /// </summary>
    public class CompanyLmsMap : BaseClassMap<CompanyLms>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyLmsMap" /> class.
        /// </summary>
        public CompanyLmsMap()
        {
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

            this.Map(x => x.CreatedBy).Not.Nullable().Column("createdBy");
            this.Map(x => x.ModifiedBy).Nullable().Column("modifiedBy");
            this.Map(x => x.CompanyId).Not.Nullable().Column("companyId");

            this.References(x => x.LmsProvider).Not.Nullable().Column("lmsProviderId");
            this.References(x => x.AdminUser).Column("adminUserId").Not.LazyLoad().Nullable();
            this.HasMany(x => x.LmsUsers).KeyColumn("companyLmsId").Cascade.Delete().Inverse();
            this.HasMany(x => x.LmsCourseMeetings).KeyColumn("companyLmsId").Cascade.Delete().Inverse();
        }

        #endregion
    }
}