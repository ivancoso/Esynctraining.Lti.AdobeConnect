namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The address DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(StateDTO))]
    [KnownType(typeof(CountryDTO))]
    public class AddressDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddressDTO" /> class.
        /// </summary>
        public AddressDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressDTO"/> class.
        /// </summary>
        /// <param name="a">
        /// The address.
        /// </param>
        public AddressDTO(Address a)
        {
            this.addressId = a.Id;
            this.stateVO = a.State == null ? null : new StateDTO(a.State);
            this.stateId = a.State.Return(x => x.Id, (int?)null);
            this.countryId = a.Country.Return(x => x.Id, (int?)null);
            this.countryVO = a.Country == null ? null : new CountryDTO(a.Country);
            this.city = a.City;
            this.address1 = a.Address1;
            this.address2 = a.Address2;
            this.latitude = a.Latitude;
            this.longitude = a.Longitude;
            this.dateCreated = a.DateCreated.With(x => x.ConvertToUnixTimestamp());
            this.dateModified = a.DateModified.With(x => x.ConvertToUnixTimestamp());
            this.zip = a.Zip;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        [DataMember]
        public float? latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        [DataMember]
        public float? longitude { get; set; }

        /// <summary>
        /// Gets or sets the address id.
        /// </summary>
        [DataMember]
        public int addressId { get; set; }

        /// <summary>
        /// Gets or sets the state id.
        /// </summary>
        [DataMember]
        public int? stateId { get; set; }

        /// <summary>
        /// Gets or sets the country id.
        /// </summary>
        [DataMember]
        public int? countryId { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        [DataMember]
        public string city { get; set; }

        /// <summary>
        /// Gets or sets the address 1.
        /// </summary>
        [DataMember]
        public string address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2.
        /// </summary>
        [DataMember]
        public string address2 { get; set; }

        /// <summary>
        /// Gets or sets the zip.
        /// </summary>
        [DataMember]
        public string zip { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets the state vo.
        /// </summary>
        [DataMember]
        public StateDTO stateVO { get; set; }

        /// <summary>
        /// Gets or sets the country vo.
        /// </summary>
        [DataMember]
        public CountryDTO countryVO { get; set; }

        #endregion
    }
}