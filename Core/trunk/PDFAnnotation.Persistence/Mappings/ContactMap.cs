namespace PDFAnnotation.Persistence.Mappings
{
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The contact map.
    /// </summary>
    public class ContactMap : BaseClassMap<Contact>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactMap"/> class.
        /// </summary>
        public ContactMap()
        {
            this.Map(x => x.FirstName).Length(255).Not.Nullable();
            this.Map(x => x.LastName).Length(255).Not.Nullable();
            this.Map(x => x.Password).Length(255).Nullable();
            this.Map(x => x.Email).Length(255).Not.Nullable();
            this.Map(x => x.ACPrincipalId).Length(500).Nullable();
            this.Map(x => x.RBContactId).Nullable();
            this.Map(x => x.OfficePhone).Length(50).Nullable();
            this.Map(x => x.MobilePhone).Length(255).Nullable();
            this.Map(x => x.IsSuperAdmin).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Nullable();
            this.Map(x => x.Status).CustomType<ContactStatusEnum>().Not.Nullable().Default(1.ToString());
            
            this.HasMany(x => x.PasswordActivation).Cascade.Delete().Inverse().ExtraLazyLoad();
            this.HasMany(x => x.CompanyContacts).Cascade.Delete().Inverse().ExtraLazyLoad();

            this.HasManyToMany(x => x.Categories)
                .Table("CategoryContact")
                .ParentKeyColumn("contactId")
                .ChildKeyColumn("categoryId");
        }

        #endregion
    }
}