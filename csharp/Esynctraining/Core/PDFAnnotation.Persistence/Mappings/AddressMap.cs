namespace PDFAnnotation.Persistence.Mappings
{
    using Esynctraining.Persistence.Mappings;

    using PDFAnnotation.Core.Domain.Entities;

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
            this.Map(x => x.Zip).Length(30).Nullable();
            this.Map(x => x.Province).Length(500).Nullable();
            this.Map(x => x.DateCreated).Nullable();
            this.Map(x => x.DateModified).Nullable();

            this.References(x => x.Country).Nullable();
            this.References(x => x.State).Nullable();
        }

        #endregion
    }
}