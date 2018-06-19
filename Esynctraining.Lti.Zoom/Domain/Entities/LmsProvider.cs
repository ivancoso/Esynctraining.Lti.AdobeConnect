using System;
using System.Collections.Generic;
using System.Text;

namespace Esynctraining.Lti.Zoom.Domain.Entities
{
    public class LmsProvider
    {
        #region Public Properties

        public int lmsProviderId { get; set; }

        /// <summary>
        /// Gets or sets the LMS provider name.
        /// </summary>
        public virtual string LmsProviderName { get; set; }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        public virtual string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the configuration url.
        /// </summary>
        public virtual string ConfigurationUrl { get; set; }

        /// <summary>
        /// Gets or sets the user guide file url.
        /// </summary>
        public virtual string UserGuideFileUrl { get; set; }

        #endregion

        public static LmsProvider Generate()
        {
            var lmsProvider = new LmsProvider
            {
                LmsProviderName = "Canvas",
                ShortName = "canvas",
                lmsProviderId = 2,
            };
            return lmsProvider;
        }
    }
}
