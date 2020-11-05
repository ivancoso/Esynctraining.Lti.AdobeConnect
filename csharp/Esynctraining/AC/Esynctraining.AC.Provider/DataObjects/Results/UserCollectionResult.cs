namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System.Collections.Generic;

    using Esynctraining.AC.Provider.Entities;

    public class UserCollectionResult : CollectionResult<User>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public UserCollectionResult(StatusInfo status)
            : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public UserCollectionResult(StatusInfo status, IEnumerable<User> values)
            : base(status, values)
        {
        }

        #endregion
    }
}