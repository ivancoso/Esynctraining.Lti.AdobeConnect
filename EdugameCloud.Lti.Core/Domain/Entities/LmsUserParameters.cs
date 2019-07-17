namespace EdugameCloud.Lti.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The LMS user parameters.
    /// </summary>
    public class LmsUserParameters : Entity
    {
        /// <summary>
        /// Gets or sets the AC id.
        /// </summary>
        public virtual string AcId { get; set; }

        /// <summary>
        /// Gets or sets the course.
        /// </summary>
        public virtual string Course { get; set; }

        /// <summary>
        /// Gets or sets the LMS user.
        /// </summary>
        public virtual LmsUser LmsUser { get; set; }

        /// <summary>
        /// Gets or sets the WS token.
        /// </summary>
        public virtual string Wstoken { get; set; }

        /// <summary>
        /// Gets or sets the company LMS.
        /// </summary>
        public virtual LmsCompany CompanyLms { get; set; }

        /// <summary>
        /// Gets or sets the course name.
        /// </summary>
        public virtual string CourseName { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        public virtual string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the last logged in.
        /// </summary>
        public virtual DateTime LastLoggedIn { get; set; }

    }

}