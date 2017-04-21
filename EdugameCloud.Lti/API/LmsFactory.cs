﻿using EdugameCloud.Lti.API.AdobeConnect;
using Esynctraining.Core.Utils;
using EdugameCloud.Lti.API.BlackBoard;
using EdugameCloud.Lti.API.Canvas;
using EdugameCloud.Lti.API.Moodle;
using EdugameCloud.Lti.API.Sakai;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.API
{
    /// <summary>
    /// The LMS factory.
    /// </summary>
    public class LmsFactory
    {
        #region Fields

        /// <summary>
        /// The Canvas API.
        /// </summary>
        private readonly IEGCEnabledCanvasAPI canvasApi;

        /// <summary>
        /// The Moodle API.
        /// </summary>
        private readonly IEGCEnabledMoodleApi moodleApi;

        /// <summary>
        /// The blackboard api.
        /// </summary>
        private readonly IEGCEnabledBlackBoardApi blackboardApi;


        private readonly IEGCEnabledSakaiApi sakaiApi;
        

        #endregion

        #region Constructors and Destructors

        public LmsFactory(IEGCEnabledCanvasAPI canvasApi, IEGCEnabledMoodleApi moodleApi, IEGCEnabledBlackBoardApi blackboardApi, IEGCEnabledSakaiApi sakaiApi)
        {
            this.sakaiApi = sakaiApi;
            this.canvasApi = canvasApi;
            this.moodleApi = moodleApi;
            this.blackboardApi = blackboardApi;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get LMS API.
        /// </summary>
        /// <param name="lmsId">
        /// The LMS id.
        /// </param>
        /// <returns>
        /// The <see cref="ILmsAPI"/>.
        /// </returns>
        public IEGCEnabledLmsAPI GetEGCEnabledLmsAPI(LmsProviderEnum lmsId)
        {
            switch (lmsId)
            {
                case LmsProviderEnum.Canvas:
                    return this.canvasApi;
                case LmsProviderEnum.Moodle:
                    return this.moodleApi;
                case LmsProviderEnum.Blackboard:
                    return this.blackboardApi;
                case LmsProviderEnum.Sakai:
                    return this.sakaiApi;
            }

            return null;
        }

        public LmsUserServiceBase GetUserService(LmsProviderEnum lmsProvider)
        {
            return IoC.Resolve<LmsUserServiceBase>(lmsProvider.ToString());
        }

        public IMeetingSessionService GetMeetingSessionService(LmsProviderEnum lmsId)
        {
            var lmsCourseMeetingModel = IoC.Resolve<LmsCourseMeetingModel>();
            var logger = IoC.Resolve<ILogger>();
            //switch (lmsId)
            //{
            //    case LmsProviderEnum.Sakai:
            //        var calendarExportService = IoC.Resolve<ICalendarExportService>("SakaiCalendarExportService");
            //        return new MeetingSessionService(lmsCourseMeetingModel, logger, calendarExportService);
            //}

            return new MeetingSessionService(lmsCourseMeetingModel, logger, null);
        }
        #endregion

    }

}