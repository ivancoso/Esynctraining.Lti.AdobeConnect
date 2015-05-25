using System.Collections.Generic;
using EdugameCloud.Lti.Core.Domain.Entities;
using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Lti.Domain.Entities
{
    /// <summary>
    /// The LMS user.
    /// </summary>
    public class LmsUser : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the company LMS;
        /// </summary>
        public virtual LmsCompany LmsCompany { get; set; }

        /// <summary>
        /// Gets or sets the AC connection mode.
        /// </summary>
        public virtual AcConnectionMode AcConnectionMode { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// Gets or sets the primary color.
        /// </summary>
        public virtual string PrimaryColor { get; set; }

        /// <summary>
        /// Gets or sets the principal id.
        /// </summary>
        public virtual string PrincipalId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Email { get; set; }

        // Blackboard returns guid identifier instead of actual user id in lti parameter
        // The idea is to store actual ids in UserId field and other identifiers in UserIdExtended
        // It would improve at least users search in API
        // Currently it's not implemented and not used, guid is stored in UserId parameter
        public virtual string UserIdExtended { get; set; }

        /// <summary>
        /// Gets or sets the LMS user parameters.
        /// </summary>
        public virtual IList<LmsUserParameters> LmsUserParameters { get; protected set; }

        public virtual IList<LmsUserMeetingRole> MeetingRoles { get; protected set; }

        #endregion
    }
}