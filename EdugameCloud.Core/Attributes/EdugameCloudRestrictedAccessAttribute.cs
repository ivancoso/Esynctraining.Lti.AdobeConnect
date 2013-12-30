namespace EdugameCloud.Core.Attributes
{
    using System.Linq;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Attributes;

    /// <summary>
    /// The restricted access attribute.
    /// </summary>
    public class EdugameCloudRestrictedAccessAttribute : WebOrbRestrictedAccessAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdugameCloudRestrictedAccessAttribute"/> class.
        /// </summary>
        public EdugameCloudRestrictedAccessAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdugameCloudRestrictedAccessAttribute"/> class.
        /// </summary>
        /// <param name="roles">
        /// The roles.
        /// </param>
        public EdugameCloudRestrictedAccessAttribute(params UserRoleEnum[] roles)
        {
            this.Roles = roles.Select(x => x.ToString()).ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdugameCloudRestrictedAccessAttribute"/> class.
        /// </summary>
        /// <param name="roles">
        /// The roles.
        /// </param>
        public EdugameCloudRestrictedAccessAttribute(params string[] roles)
            : base(roles)
        {
        }
    }
}
