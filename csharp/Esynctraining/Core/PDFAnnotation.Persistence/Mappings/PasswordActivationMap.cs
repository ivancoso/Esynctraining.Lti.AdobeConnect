﻿namespace PDFAnnotation.Persistence.Mappings
{
    using Esynctraining.Persistence.Mappings;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The password activation map.
    /// </summary>
    public class PasswordActivationMap : BaseClassMap<PasswordActivation>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordActivationMap"/> class.
        /// </summary>
        public PasswordActivationMap()
        {
            this.Map(x => x.PasswordActivationCode).Not.Nullable();
            this.Map(x => x.ActivationDateTime).Not.Nullable();
            this.References(x => x.Contact).Not.Nullable();
        }

        #endregion
    }
}