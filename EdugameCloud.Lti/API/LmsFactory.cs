using EdugameCloud.Lti.API.AdobeConnect;
using Esynctraining.Core.Utils;
using EdugameCloud.Lti.API.BlackBoard;
using EdugameCloud.Lti.API.Canvas;
using EdugameCloud.Lti.API.Moodle;
using EdugameCloud.Lti.API.Sakai;
using EdugameCloud.Lti.Domain.Entities;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsFactory"/> class.
        /// </summary>
        /// <param name="canvasApi">
        /// The Canvas API.
        /// </param>
        /// <param name="moodleApi">
        /// The Moodle API.
        /// </param>
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
            switch (lmsId)
            {
                case LmsProviderEnum.Sakai:
                    return IoC.Resolve<IMeetingSessionService>(lmsId.ToString() + "MeetingSessionService");
            }

            return IoC.Resolve<IMeetingSessionService>();
        }
        #endregion
    }
}