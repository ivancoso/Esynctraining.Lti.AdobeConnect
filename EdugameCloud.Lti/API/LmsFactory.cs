using Castle.Windsor;
using EdugameCloud.Lti.API.AdobeConnect;
using Esynctraining.Core.Utils;
using EdugameCloud.Lti.API.BlackBoard;
using EdugameCloud.Lti.API.Canvas;
using EdugameCloud.Lti.API.Moodle;
using EdugameCloud.Lti.API.Sakai;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
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

        public LmsUserServiceBase GetUserService(LmsProviderEnum lmsId)
        {
            return IoC.Resolve<LmsUserServiceBase>(lmsId.ToString());
        }

        public LmsCourseSectionsServiceBase GetCourseSectionsService(ILmsLicense lmsLicense, LtiParamDTO param)
        {
            var lmsId = (LmsProviderEnum)lmsLicense.LmsProviderId;

            switch (lmsId)
            {
                case LmsProviderEnum.Canvas:
                case LmsProviderEnum.Haiku:
                //case LmsProviderEnum.Sakai:
                    var container = IoC.Resolve<IWindsorContainer>(); //bad hack - until parameterized methods are added to IoC and IServiceLocator
                    return container.Resolve<LmsCourseSectionsServiceBase>(lmsId + "SectionsService",
                        new {license = lmsLicense, param});
            }

            return new LmsCourseSectionsServiceBase(lmsLicense, param);
        }

        public IMeetingSessionService GetMeetingSessionService(ILmsLicense lmsLicense, LtiParamDTO param)
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

            return new MeetingSessionService(lmsCourseMeetingModel, logger, calendarExportService, lmsLicense);
        }
        #endregion

    }

}