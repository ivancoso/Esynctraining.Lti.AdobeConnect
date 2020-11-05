namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The address DTO.
    /// </summary>
    [DataContract]
    [Serializable]
    [KnownType(typeof(StateDTO))]
    [KnownType(typeof(CountryDTO))]
    public class AddressDTO
    {
        #region Constructors and Destructors

        public AddressDTO() { }

        public AddressDTO(Address a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            this.addressId = a.Id;
            this.stateVO = a.State == null ? null : new StateDTO(a.State);
            this.stateId = a.State?.Id;
            this.countryId = a.Country?.Id;
            this.countryVO = a.Country == null ? null : new CountryDTO(a.Country);
            this.city = a.City;
            this.address1 = a.Address1;
            this.address2 = a.Address2;
            this.zip = a.Zip;
            this.dateCreated = a.DateCreated;
            this.dateModified = a.DateModified;
            this.province = a.Province;
        }

        #endregion

        #region Public Properties

        [DataMember]
        public int addressId { get; set; }

        [DataMember]
        public int? stateId { get; set; }

        [DataMember]
        public int? countryId { get; set; }

        [DataMember]
        public string city { get; set; }

        [DataMember]
        public string address1 { get; set; }

        [DataMember]
        public string address2 { get; set; }

        [DataMember]
        public string zip { get; set; }

        [DataMember]
        public string province { get; set; }

        [DataMember]
        public DateTime? dateCreated { get; set; }

        [DataMember]
        public DateTime? dateModified { get; set; }

        [DataMember]
        public StateDTO stateVO { get; set; }

        [DataMember]
        public CountryDTO countryVO { get; set; }

        #endregion

    }

}