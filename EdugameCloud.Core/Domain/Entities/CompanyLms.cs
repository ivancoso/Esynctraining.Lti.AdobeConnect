namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The company LMS.
    /// </summary>
    public class CompanyLms : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the AC SCO id.
        /// </summary>
        public virtual string ACScoId { get; set; }

        /// <summary>
        /// Gets or sets the AC template SCO id.
        /// </summary>
        public virtual string ACTemplateScoId { get; set; }

        /// <summary>
        /// Gets or sets the AC password.
        /// </summary>
        public virtual string AcPassword { get; set; }

        /// <summary>
        /// Gets or sets the AC server.
        /// </summary>
        public virtual string AcServer { get; set; }

        /// <summary>
        /// Gets or sets the AC username.
        /// </summary>
        public virtual string AcUsername { get; set; }

        /// <summary>
        /// Gets or sets the admin user.
        /// </summary>
        public virtual LmsUser AdminUser { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        public virtual Company Company { get; set; }

        /// <summary>
        /// Gets or sets the consumer key.
        /// </summary>
        public virtual string ConsumerKey { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public virtual User CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        public virtual string Layout { get; set; }

        /// <summary>
        /// Gets or sets the LMS domain.
        /// </summary>
        public virtual string LmsDomain { get; set; }

        /// <summary>
        /// Gets or sets the LMS provider.
        /// </summary>
        public virtual LmsProvider LmsProvider { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public virtual User ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the primary color.
        /// </summary>
        public virtual string PrimaryColor { get; set; }

        /// <summary>
        /// Gets or sets the shared secret.
        /// </summary>
        public virtual string SharedSecret { get; set; }

        #endregion
    }
}