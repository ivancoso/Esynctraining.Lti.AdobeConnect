namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The address.
    /// </summary>
    public class Address : Entity
    {
        /// <summary>
        ///     Gets or sets the address 1.
        /// </summary>
        public virtual string Address1 { get; set; }

        /// <summary>
        ///     Gets or sets the address 2.
        /// </summary>
        public virtual string Address2 { get; set; }

        /// <summary>
        ///     Gets or sets the city.
        /// </summary>
        public virtual string City { get; set; }

        /// <summary>
        ///     Gets or sets the zip.
        /// </summary>
        public virtual string Zip { get; set; }

        /// <summary>
        ///     Gets or sets the province.
        /// </summary>
        public virtual string Province { get; set; }

        /// <summary>
        ///     Gets or sets the country.
        /// </summary>
        public virtual Country Country { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        public virtual DateTime? DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        public virtual DateTime? DateModified { get; set; }

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        public virtual State State { get; set; }

    }

}