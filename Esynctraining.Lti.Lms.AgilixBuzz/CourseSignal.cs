namespace Esynctraining.Lti.Lms.AgilixBuzz
{
    /// <summary>
    ///     The AgilixBuzz course.
    /// </summary>
    internal sealed class CourseSignal : Signal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseSignal"/> class.
        /// </summary>
        /// <param name="signalId">
        /// The signal id.
        /// </param>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public CourseSignal(long signalId, int entityId, string type)
            : base(signalId, entityId, type)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        public string ItemId { get; set; }

        #endregion
    }
}
