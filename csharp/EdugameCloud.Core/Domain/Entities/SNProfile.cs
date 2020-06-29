namespace EdugameCloud.Core.Domain.Entities
{
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The SN Profile.
    /// </summary>
    public class SNProfile : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the about.
        /// </summary>
        public virtual string About { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string ProfileName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Gets or sets the job title.
        /// </summary>
        public virtual string JobTitle { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        public virtual string Phone { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        public virtual IList<SNLink> Links { get; protected set; }

        /// <summary>
        /// Gets or sets the map settings.
        /// </summary>
        public virtual SNMapSettings MapSettings { get; set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        /// <summary>
        /// Gets or sets the services.
        /// </summary>
        public virtual IList<SNProfileSNService> Services { get; protected set; }

        #endregion

        public SNProfile()
        { 
            Links = new List<SNLink>();
            Services = new List<SNProfileSNService>();
        }

    }

}