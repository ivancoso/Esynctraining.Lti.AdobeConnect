namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The SN Profile.
    /// </summary>
    public class SNProfile : Entity
    {
        #region Fields

        /// <summary>
        ///     The links.
        /// </summary>
        private ISet<SNLink> links = new HashedSet<SNLink>();

        /// <summary>
        ///     The services.
        /// </summary>
        private ISet<SNProfileSNService> services = new HashedSet<SNProfileSNService>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the about.
        /// </summary>
        public virtual string About { get; set; }

        /// <summary>
        ///     Gets or sets the address.
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public virtual string ProfileName { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the job title.
        /// </summary>
        public virtual string JobTitle { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        public virtual string Phone { get; set; }

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        public virtual ISet<SNLink> Links
        {
            get
            {
                return this.links;
            }

            set
            {
                this.links = value;
            }
        }

        /// <summary>
        /// Gets or sets the map settings.
        /// </summary>
        public virtual SNMapSettings MapSettings { get; set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        /// <summary>
        ///     Gets or sets the services.
        /// </summary>
        public virtual ISet<SNProfileSNService> Services
        {
            get
            {
                return this.services;
            }

            set
            {
                this.services = value;
            }
        }

        #endregion
    }
}