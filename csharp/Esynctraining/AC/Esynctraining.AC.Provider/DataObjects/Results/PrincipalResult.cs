namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The principal result.
    /// </summary>
    public class PrincipalResult : ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrincipalResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public PrincipalResult(StatusInfo status) : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrincipalResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="principal">
        /// The principal.
        /// </param>
        public PrincipalResult(StatusInfo status, Principal principal)
            : base(status)
        {
            this.Principal = principal;
        }

        /// <summary>
        /// Gets or sets the sco info.
        /// </summary>
        public Principal Principal { get; set; }

        /// <summary>
        /// Gets a value indicating whether success.
        /// </summary>
        public override bool Success
        {
            get
            {
                return base.Success;
            }
        }
    }
}
