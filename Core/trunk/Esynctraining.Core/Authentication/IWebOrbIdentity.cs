namespace Esynctraining.Core.Authentication
{
    using System.Collections.Generic;
    using System.Security.Principal;

    /// <summary>
    /// The WebOrbIdentity interface.
    /// </summary>
    public interface IWebOrbIdentity : IIdentity
    {
        /// <summary>
        /// Gets the roles.
        /// </summary>
        List<string> Roles { get; }

        /// <summary>
        /// Gets or sets the internal id.
        /// </summary>
        int? InternalId { get; set; }
    }
}