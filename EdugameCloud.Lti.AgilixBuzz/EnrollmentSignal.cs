namespace EdugameCloud.Lti.AgilixBuzz
{
    /// <summary>
    ///     The AgilixBuz enrollment.
    /// </summary>
    internal sealed class EnrollmentSignal : Signal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnrollmentSignal"/> class.
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
        public EnrollmentSignal(long signalId, int entityId, string type)
            : base(signalId, entityId, type)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the new role.
        /// </summary>
        public string NewRole { get; set; }

        /// <summary>
        /// Gets or sets the new status.
        /// </summary>
        public int NewStatus { get; set; }

        /// <summary>
        /// Gets or sets the old role.
        /// </summary>
        public string OldRole { get; set; }

        /// <summary>
        /// Gets or sets the old status.
        /// </summary>
        public int OldStatus { get; set; }

        #endregion
    }
}