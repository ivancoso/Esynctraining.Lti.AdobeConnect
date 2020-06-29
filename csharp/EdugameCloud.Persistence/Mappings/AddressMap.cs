namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The address mapping
    /// </summary>
    public class AddressMap : BaseClassMap<Address>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressMap"/> class.
        /// </summary>
        public AddressMap()
        {
            this.Map(x => x.City).Length(255).Nullable();
            this.Map(x => x.Address1).Length(255).Nullable();
            this.Map(x => x.Address2).Length(255).Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Not.Nullable();
            this.Map(x => x.Latitude).Nullable();
            this.Map(x => x.Longitude).Nullable();
            this.Map(x => x.Zip).Nullable();

            this.HasMany(x => x.Companies);

            this.References(x => x.Country).Nullable();
            this.References(x => x.State).Nullable();
        }

        #endregion
    }
}