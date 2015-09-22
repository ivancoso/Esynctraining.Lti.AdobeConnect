namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The address.
    /// </summary>
    public class Address : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the address 1.
        /// </summary>
        public virtual string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the zip.
        /// </summary>
        public virtual string Zip { get; set; }

        /// <summary>
        /// Gets or sets the address 2.
        /// </summary>
        public virtual string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public virtual string City { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public virtual float? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public virtual float? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the companies.
        /// </summary>
        public virtual IList<Company> Companies { get; protected set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public virtual Country Country { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public virtual State State { get; set; }

        #endregion

        public Address()
        {
            Companies = new List<Company>();
        }

    }

}