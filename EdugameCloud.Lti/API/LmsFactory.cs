namespace EdugameCloud.Lti.API
{
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.Moodle;

    /// <summary>
    ///     The LMS factory.
    /// </summary>
    public class LmsFactory
    {
        #region Fields

        /// <summary>
        /// The Canvas API.
        /// </summary>
        private readonly EGCEnabledMoodleAPI canvasApi;

        /// <summary>
        /// The Moodle API.
        /// </summary>
        private readonly EGCEnabledMoodleAPI moodleApi;

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
        public LmsFactory(EGCEnabledMoodleAPI canvasApi, EGCEnabledMoodleAPI moodleApi)
        {
            this.canvasApi = canvasApi;
            this.moodleApi = moodleApi;
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
        public ILmsAPI GetLmsAPI(LmsProviderEnum lmsId)
        {
            switch (lmsId)
            {
                case LmsProviderEnum.Canvas:
                    return this.canvasApi;
                case LmsProviderEnum.Moodle:
                    return this.moodleApi;
            }

            return null;
        }

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
            }

            return null;
        }

        #endregion
    }
}