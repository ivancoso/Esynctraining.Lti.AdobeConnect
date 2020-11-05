namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The password activation.
    /// </summary>
    [DataContract]
    public class PasswordActivation : Entity
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the pasword activation code.
        /// </summary>
        public virtual Guid PasswordActivationCode { get; set; }

        /// <summary>
        ///     Gets or sets the activation date time.
        /// </summary>
        public virtual DateTime ActivationDateTime { get; set; }

        /// <summary>
        /// Gets or sets the contact.
        /// </summary>
        public virtual Contact Contact { get; set; }

        #endregion
    }
}