namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System.Collections.Generic;

    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The telephony profile collection result.
    /// </summary>
    public class TelephonyProfilesCollectionResult : GenericCollectionResultBase<TelephonyProfile>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TelephonyProfilesCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public TelephonyProfilesCollectionResult(StatusInfo status)
            : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelephonyProfilesCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public TelephonyProfilesCollectionResult(StatusInfo status, IEnumerable<TelephonyProfile> values)
            : base(status, values)
        {
        }

        #endregion
    }
}