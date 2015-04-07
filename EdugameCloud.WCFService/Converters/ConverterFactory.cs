namespace EdugameCloud.WCFService.Converters
{
    using EdugameCloud.Lti.Domain.Entities;

    public class ConverterFactory
    {
        #region Fields

        private readonly BlackboardResultConverter blackboardResultConverter;

        private readonly CanvasResultConverter canvasResultConverter;
        
        private readonly MoodleResultConverter moodleResultConverter;

        #endregion

        #region Constructors and Destructors

        public ConverterFactory(BlackboardResultConverter blackboardResultConverter, 
            CanvasResultConverter canvasResultConverter, 
            MoodleResultConverter moodleResultConverter)
        {
            this.blackboardResultConverter = blackboardResultConverter;
            this.canvasResultConverter = canvasResultConverter;
            this.moodleResultConverter = moodleResultConverter;
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
        public QuizResultConverter GetResultConverter(LmsProviderEnum lmsId)
        {
            switch (lmsId)
            {
                case LmsProviderEnum.Canvas:
                    return this.canvasResultConverter;
                case LmsProviderEnum.Moodle:
                    return this.moodleResultConverter;
                case LmsProviderEnum.Blackboard:
                    return this.blackboardResultConverter;
            }

            return null;
        }

        #endregion
    }
}