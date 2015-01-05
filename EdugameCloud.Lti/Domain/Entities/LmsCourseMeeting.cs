namespace EdugameCloud.Lti.Domain.Entities
{
    using System;
    using System.Collections.Generic;

    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Domain.Entities;

    using Newtonsoft.Json;

    using NHibernate.Mapping;

    /// <summary>
    ///     The LMS AC meeting
    /// </summary>
    public class LmsCourseMeeting : Entity
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the added to cache.
        /// </summary>
        public virtual DateTime? AddedToCache { get; set; }

        /// <summary>
        ///     Gets or sets the cached users.
        /// </summary>
        public virtual string CachedUsers { get; set; }

        /// <summary>
        ///     Gets or sets the company LMS.
        /// </summary>
        public virtual CompanyLms CompanyLms { get; set; }

        /// <summary>
        ///     Gets or sets the course id.
        /// </summary>
        public virtual int CourseId { get; set; }

        /// <summary>
        ///     Gets or sets the SCO id.
        /// </summary>
        public virtual string ScoId { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The cached users parsed.
        /// </summary>
        /// <param name="validCacheTimeout">
        /// The valid cache timeout.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public virtual List<LmsUserDTO> CachedUsersParsed(TimeSpan validCacheTimeout)
        {
            try
            {
                return this.CachedUsers != null && this.AddedToCache != null
                   && DateTime.Now - this.AddedToCache <= validCacheTimeout
                       ? JsonConvert.DeserializeObject<List<LmsUserDTO>>(this.CachedUsers)
                       : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}