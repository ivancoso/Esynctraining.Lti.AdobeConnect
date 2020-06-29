using Esynctraining.Lti.Zoom.Common.Dto.Enums;
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

        #endregion

        public static LmsProvider Generate(int productId)
        {
            LmsProvider lmsProvider = null;
            switch ((LMS)productId)
            {
                case LMS.Canvas:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "Canvas",
                        ShortName = "canvas",
                    };
                    break;
                case LMS.AgilixBuzz:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "AgilixBuzz",
                        ShortName = "AgilixBuzz",
                    };
                    break;
                case LMS.Schoology:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "Schoology",
                        ShortName = "Schoology",
                    };
                    break;
                case LMS.BlackBoard:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "BlackBoard",
                        ShortName = "BlackBoard",
                    };
                    break;
                case LMS.Moodle:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "Moodle",
                        ShortName = "Moodle",
                    };
                    break;
                case LMS.Sakai:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "Sakai",
                        ShortName = "Sakai",
                    };
                    break;
                case LMS.Desire2Learn:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "BrigthSpace",
                        ShortName = "BrigthSpace",
                    };
                    break;

            }

            return lmsProvider;
        }
    }
}
