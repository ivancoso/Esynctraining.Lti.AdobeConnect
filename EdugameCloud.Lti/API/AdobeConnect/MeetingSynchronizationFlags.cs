namespace EdugameCloud.Lti.API.AdobeConnect
{
    /// <summary>
    ///     The meeting synchronization flags.
    /// </summary>
    public class MeetingSynchronizationFlags
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingSynchronizationFlags"/> class.
        /// </summary>
        public MeetingSynchronizationFlags()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingSynchronizationFlags"/> class.
        /// </summary>
        /// <param name="canJoin">
        /// The can join.
        /// </param>
        /// <param name="areUsersSynched">
        /// The are users synched.
        /// </param>
        public MeetingSynchronizationFlags(bool canJoin, bool areUsersSynched)
        {
            this.CanUserJoin = canJoin;
            this.AreUsersSynched = areUsersSynched;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether are users synched.
        /// </summary>
        public bool AreUsersSynched { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether can user join.
        /// </summary>
        public bool CanUserJoin { get; set; }

        #endregion
    }
}