using System.Collections.Generic;
using Castle.Windsor;
using EdugameCloud.Lti.API.AdobeConnect;
using Esynctraining.Core.Utils;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.BlackBoard;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.API.Moodle;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Lms.Common.API.Sakai;

namespace EdugameCloud.Lti.API
{
    public class LmsFactory
    {
        private readonly IEGCEnabledCanvasAPI canvasApi;
        private readonly IEGCEnabledMoodleApi moodleApi;
        private readonly IEGCEnabledBlackBoardApi blackboardApi;
        private readonly IEGCEnabledSakaiApi sakaiApi;

        public LmsFactory(IEGCEnabledCanvasAPI canvasApi, IEGCEnabledMoodleApi moodleApi, IEGCEnabledBlackBoardApi blackboardApi, IEGCEnabledSakaiApi sakaiApi)
        {
            this.sakaiApi = sakaiApi;
            this.canvasApi = canvasApi;
            this.moodleApi = moodleApi;
            this.blackboardApi = blackboardApi;
        }

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

        public LmsUserServiceBase GetUserService(LmsProviderEnum lmsId)
        {
            return IoC.Resolve<LmsUserServiceBase>(lmsId.ToString());
        }

        public LmsCalendarEventServiceBase GetCalendarEventService(LmsProviderEnum lmsId, ILmsLicense lmsLicense)
        {
            switch (lmsId)
            {
                case LmsProviderEnum.Canvas:
                    bool enableExportToCalendar = lmsLicense.GetSetting<bool>(LmsLicenseSettingNames.EnableCanvasExportToCalendar);
                    if (!enableExportToCalendar)
                        return null;

                    return IoC.Resolve<LmsCalendarEventServiceBase>(lmsId + "CalendarService");
            }

            return null;
        }

        public LmsCourseSectionsServiceBase GetCourseSectionsService(int lmsProviderId, Dictionary<string, object> licenseSettings, ILtiParam param)
        {
            var lmsId = (LmsProviderEnum)lmsProviderId;

            switch (lmsId)
            {
                case LmsProviderEnum.Canvas:
                case LmsProviderEnum.Haiku:
                //case LmsProviderEnum.Sakai:
                    var container = IoC.Resolve<IWindsorContainer>(); //bad hack - until parameterized methods are added to IoC and IServiceLocator
                    return container.Resolve<LmsCourseSectionsServiceBase>(lmsId + "SectionsService",
                        new { licenseSettings, param});
            }

            return new LmsCourseSectionsServiceBase(new Dictionary<string, object>(), param);
        }

        public IMeetingSessionService GetMeetingSessionService(ILmsLicense lmsLicense, ILtiParam param)
        {
            var lmsCourseMeetingModel = IoC.Resolve<LmsCourseMeetingModel>();
            var logger = IoC.Resolve<ILogger>();
            ICalendarExportService calendarExportService = null;
            var lmsId = (LmsProviderEnum)lmsLicense.LmsProviderId;
            switch (lmsId)
            {
                //    case LmsProviderEnum.Sakai:
                case LmsProviderEnum.Bridge:
                    var container = IoC.Resolve<IWindsorContainer>(); //bad hack - until parameterized methods are added to IoC and IServiceLocator
                    calendarExportService = IoC.Resolve<ICalendarExportService>(lmsId + "CalendarExportService");
                    return container.Resolve<IMeetingSessionService>(lmsId + "SessionsService",
                        new { license = lmsLicense, param, calendarExportService });
            }

            return new MeetingSessionService(lmsCourseMeetingModel, logger, calendarExportService, lmsLicense, GetCalendarEventService(lmsId, lmsLicense));
        }
        #endregion

    }

}