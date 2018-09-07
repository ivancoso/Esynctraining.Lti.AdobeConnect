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
            switch (productId)
            {
                case 1010:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "Canvas",
                        ShortName = "canvas",
                        UserGuideFileUrl = "CanvasZoomIntegration.pdf",
                    };
                    break;
                case 1020:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "AgilixBuzz",
                        ShortName = "AgilixBuzz",
                        UserGuideFileUrl = "BuzzZoomIntegration.pdf",
                    };
                    break;
                case 1030:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "Schoology",
                        ShortName = "Schoology",
                        UserGuideFileUrl = "SchoologyZoomIntegration.pdf",
                    };
                    break;
                case 1040:
                    lmsProvider = new LmsProvider
                    {
                        LmsProviderName = "BlackBoard",
                        ShortName = "BlackBoard",
                        UserGuideFileUrl = "BlackBoardZoomIntegration.pdf",
                    };
                    break;
            }

            return lmsProvider;
        }
    }
}
