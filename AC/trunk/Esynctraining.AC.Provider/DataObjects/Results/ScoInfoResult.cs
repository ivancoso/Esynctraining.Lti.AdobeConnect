namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The sco info result.
    /// </summary>
    public class ScoInfoResult : ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoInfoResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public ScoInfoResult(StatusInfo status) : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoInfoResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="scoInfo">
        /// The sco Info.
        /// </param>
        public ScoInfoResult(StatusInfo status, ScoInfo scoInfo)
            : base(status)
        {
            this.ScoInfo = scoInfo;
        }

        /// <summary>
        /// Gets or sets the sco info.
        /// </summary>
        public ScoInfo ScoInfo { get; set; }

        /// <summary>
        /// Gets a value indicating whether success.
        /// </summary>
        public override bool Success
        {
            get
            {
                return base.Success && this.ScoInfo != null;
            }
        }
    }
}
