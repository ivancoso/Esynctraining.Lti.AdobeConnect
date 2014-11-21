namespace EdugameCloud.Lti.API
{
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.API.Moodle;

    using Esynctraining.Core.Utils;

    /// <summary>
    /// The lms factory.
    /// </summary>
    public class LmsFactory
    {
        /// <summary>
        /// Gets the course api.
        /// </summary>
        private CanvasAPI CanvasApi
        {
            get
            {
                return IoC.Resolve<CanvasAPI>();
            }
        }

        /// <summary>
        /// Gets the moodle api.
        /// </summary>
        private MoodleAPI MoodleAPI
        {
            get
            {
                return IoC.Resolve<MoodleAPI>();
            }
        }

        /// <summary>
        /// The get lms api.
        /// </summary>
        /// <param name="lmsId">
        /// The lms id.
        /// </param>
        /// <returns>
        /// The <see cref="ILmsAPI"/>.
        /// </returns>
        public ILmsAPI GetLmsAPI(LmsProviderEnum lmsId)
        {
            switch (lmsId)
            {
                case LmsProviderEnum.Canvas:
                    return this.CanvasApi;
                case LmsProviderEnum.Moodle:
                    return MoodleAPI;
            }
            return null;
        }
    }
}
