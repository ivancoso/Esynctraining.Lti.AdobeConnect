namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The principal result.
    /// </summary>
    public class PrincipalInfoResult : ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrincipalInfoResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public PrincipalInfoResult(StatusInfo status) : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrincipalInfoResult"/> class. 
        /// Initializes a new instance of the <see cref="PrincipalResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="principalInfo">
        /// The principal.
        /// </param>
        public PrincipalInfoResult(StatusInfo status, PrincipalInfo principalInfo)
            : base(status)
        {
            this.PrincipalInfo = principalInfo;
        }

        /// <summary>
        /// Gets or sets the principal info.
        /// </summary>
        public PrincipalInfo PrincipalInfo { get; set; }

    }
}
