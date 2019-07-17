namespace EdugameCloud.Lti.Domain.Entities
{
    using System;

    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Domain.Entities;

    using Newtonsoft.Json;

    /// <summary>
    /// The LMS user session.
    /// </summary>
    public class LmsUserSession : EntityGuid, IDatesContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserSession"/> class.
        /// </summary>
        public LmsUserSession()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.DateCreated = DateTime.Now;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the company LMS.
        /// Consider using LmsCompany in your BaseApiController.
        /// </summary>
        public virtual LmsCompany LmsCompany { get; set; }

        /// <summary>
        /// Gets or sets the LMS user Id.
        /// </summary>
        public virtual LmsUser LmsUser { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime? DateModified { get; set; }

        /// <summary>
        /// Gets or sets the LMS course id.
        /// </summary>
        public virtual string LmsCourseId { get; set; }

        /// <summary>
        /// Gets or sets the session data.
        /// </summary>
        public virtual string SessionData { get; set; }

        /// <summary>
        /// Gets the LTI session.
        /// </summary>
        public virtual LtiSessionDTO LtiSession
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.SessionData)
                    ? JsonConvert.DeserializeObject<LtiSessionDTO>(this.SessionData)
                    : null;
            }
        }

        #endregion
    }
}