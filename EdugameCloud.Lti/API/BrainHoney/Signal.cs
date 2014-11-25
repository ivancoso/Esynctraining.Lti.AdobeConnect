namespace EdugameCloud.Lti.API.BrainHoney
{
    /// <summary>
    ///     The brain honey signal.
    /// </summary>
    public class Signal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Signal"/> class.
        /// </summary>
        /// <param name="signalId">
        /// The signal id.
        /// </param>
        /// <param name="entityId">
        /// The entity Id.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public Signal(long signalId, int entityId, string type)
        {
            this.SignalId = signalId;
            this.EntityId = entityId;
            this.Type = type;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the entity id.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        ///     Gets or sets the signal id.
        /// </summary>
        public long SignalId { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        public string Type { get; set; }

        #endregion
    }
}