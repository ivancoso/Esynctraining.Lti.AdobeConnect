namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System;
    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The result base.
    /// </summary>
    public abstract class ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultBase"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        protected ResultBase(StatusInfo status)
        {
            if (status == null)
                throw new ArgumentNullException(nameof(status));

            Status = status;
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public StatusInfo Status { get; private set; }

        /// <summary>
        /// Gets a value indicating whether success.
        /// </summary>
        public virtual bool Success
        {
            get
            {
                return Status != null && Status.Code == StatusCodes.ok;
            }
        }

    }

}
