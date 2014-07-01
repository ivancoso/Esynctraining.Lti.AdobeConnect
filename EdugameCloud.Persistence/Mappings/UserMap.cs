namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Utils;

    /// <summary>
    /// The user map.
    /// </summary>
    public class UserMap : BaseClassMap<User>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMap"/> class.
        /// </summary>
        public UserMap()
        {
            this.Map(x => x.FirstName).Length(100).Not.Nullable();
            this.Map(x => x.LastName).Length(100).Nullable();
            this.Map(x => x.Password).Length(100).Nullable();
            this.Map(x => x.Email).Nullable();
            this.Map(x => x.SessionToken).Nullable();
            this.Map(x => x.SessionTokenExpirationDate).Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Not.Nullable();
            this.Map(x => x.Status).Default(1.ToString());
            this.References(x => x.Logo).Nullable().LazyLoad().Column("logoId");
            this.References(x => x.ModifiedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<User>(x => x.ModifiedBy)));

            this.HasMany(x => x.Activations).Cascade.Delete().Inverse().ExtraLazyLoad();
            this.HasMany(x => x.ACSessions).Cascade.Delete().Inverse().ExtraLazyLoad();
            this.HasMany(x => x.LoginHistory).Cascade.Delete().Inverse().ExtraLazyLoad();
            this.HasMany(x => x.Files).Cascade.Delete().Inverse().ExtraLazyLoad().KeyColumn("createdBy");
            this.HasMany(x => x.DistractorsCreated).ExtraLazyLoad().KeyColumn("createdBy");
            this.HasMany(x => x.DistractorsModified).ExtraLazyLoad().KeyColumn("modifiedBy");

            this.HasMany(x => x.ThemeAttributesCreated).ExtraLazyLoad().KeyColumn("createdBy");
            this.HasMany(x => x.ThemeAttributesModified).ExtraLazyLoad().KeyColumn("modifiedBy");

            this.HasMany(x => x.SubModuleCategories).Cascade.Delete().Inverse().ExtraLazyLoad().KeyColumn("userId");
            this.HasMany(x => x.SubModuleCategoriesModified).ExtraLazyLoad().KeyColumn("modifiedBy");

            this.References(x => x.UserRole).Not.Nullable();
            this.References(x => x.Company).Not.Nullable().LazyLoad();
            this.References(x => x.Language).Not.Nullable().LazyLoad();
            this.References(x => x.TimeZone).Not.Nullable().LazyLoad();
            this.Map(x => x.IsUnsubscribed).Nullable();

        }


        #endregion
    }
}