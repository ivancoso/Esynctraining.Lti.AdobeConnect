namespace Esynctraining.Core.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The web orb restricted access attribute.
    /// </summary>
    public class WebOrbRestrictedAccessAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebOrbRestrictedAccessAttribute"/> class.
        /// </summary>
        public WebOrbRestrictedAccessAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebOrbRestrictedAccessAttribute"/> class.
        /// </summary>
        /// <param name="roles">
        /// The roles.
        /// </param>
        public WebOrbRestrictedAccessAttribute(params string[] roles)
        {
            this.Roles = roles.ToList();
        }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        public List<string> Roles { get; set; }
    }
}
