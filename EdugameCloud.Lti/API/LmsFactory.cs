﻿namespace EdugameCloud.Lti.API
{
    using EdugameCloud.Lti.API.BlackBoard;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.API.Moodle;
    using EdugameCloud.Lti.Domain.Entities;

    /// <summary>
    ///     The LMS factory.
    /// </summary>
    public class LmsFactory
    {
        #region Fields

        /// <summary>
        /// The Canvas API.
        /// </summary>
        private readonly EGCEnabledCanvasAPI canvasApi;

        /// <summary>
        /// The Moodle API.
        /// </summary>
        private readonly EGCEnabledMoodleAPI moodleApi;

        /// <summary>
        /// The blackboard api.
        /// </summary>
        private readonly EGCEnabledBlackboardAPI blackboardApi;

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
        public LmsFactory(EGCEnabledCanvasAPI canvasApi, EGCEnabledMoodleAPI moodleApi, EGCEnabledBlackboardAPI blackboardApi)
        {
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
            }

            return null;
        }

        #endregion
    }
}