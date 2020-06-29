namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Utils;

    /// <summary>
    /// The company mapping
    /// </summary>
    public class CompanyMap : BaseClassMap<Company>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyMap"/> class.
        /// </summary>
        public CompanyMap()
        {
            this.Map(x => x.CompanyName).Length(50).Not.Nullable();
            this.Map(x => x.Status).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.UseEventMapping).Not.Nullable();
            this.Map(x => x.DateModified).Not.Nullable();
            this.References(x => x.Address).Nullable().Cascade.Delete();
            this.References(x => x.Theme).Nullable().Cascade.Delete();
            this.References(x => x.PrimaryContact).Nullable().Column(Inflector.Uncapitalize(Lambda.Property<Company>(x => x.PrimaryContact)) + "Id");
            this.HasMany(x => x.Users).Cascade.Delete().Inverse(); 
            this.HasMany(x => x.Licenses).Cascade.Delete().Inverse();
        }

        #endregion
    }
}