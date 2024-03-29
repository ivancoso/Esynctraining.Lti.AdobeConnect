﻿namespace PDFAnnotation.Persistence.Mappings
{
    using Esynctraining.Persistence.Mappings;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The contact type mapping
    /// </summary>
    public class ContactTypeMap : BaseClassMap<ContactType>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactTypeMap"/> class.
        /// </summary>
        public ContactTypeMap()
        {
            this.Map(x => x.ContactTypeName).Length(255).Not.Nullable();
            this.Map(x => x.ACMappedType).Length(50).Not.Nullable();
        }

        #endregion
    }
}