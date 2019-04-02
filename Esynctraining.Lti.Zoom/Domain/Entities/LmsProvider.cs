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

        /// <summary>
        /// Gets or sets the user guide file url.
        /// </summary>
        public virtual string UserGuideFileUrl { get; set; }

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
                        UserGuideFileUrl = "CanvasZoomIntegration.pdf",
                    };
                    break;
                case LMS.AgilixBuzz:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "AgilixBuzz",
                        ShortName = "AgilixBuzz",
                        UserGuideFileUrl = "BuzzZoomIntegration.pdf",
                    };
                    break;
                case LMS.Schoology:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "Schoology",
                        ShortName = "Schoology",
                        UserGuideFileUrl = "SchoologyZoomIntegration.pdf",
                    };
                    break;
                case LMS.BlackBoard:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "BlackBoard",
                        ShortName = "BlackBoard",
                        UserGuideFileUrl = "BlackBoardZoomIntegration.pdf",
                    };
                    break;
                case LMS.Moodle:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "Moodle",
                        ShortName = "Moodle",
                        UserGuideFileUrl = "MoodleZoomIntegration.pdf",
                    };
                    break;
                case LMS.Sakai:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "Sakai",
                        ShortName = "Sakai",
                        UserGuideFileUrl = "SakaiZoomIntegration.pdf",
                    };
                    break;
                case LMS.Desire2Learn:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "BrigthSpace",
                        ShortName = "BrigthSpace",
                        UserGuideFileUrl = "BrigthSpaceZoomIntegration.pdf",
                    };
                    break;

            }

            return lmsProvider;
        }
    }
}
