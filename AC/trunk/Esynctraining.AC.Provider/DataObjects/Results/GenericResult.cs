namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The principal delete result.
    /// </summary>
    public class GenericResult : ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public GenericResult(StatusInfo status) : base(status)
        {
        }
    }
}
