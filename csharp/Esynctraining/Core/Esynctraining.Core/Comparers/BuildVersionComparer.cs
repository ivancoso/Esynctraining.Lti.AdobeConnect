namespace Esynctraining.Core.Comparers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The build version comparer.
    /// </summary>
    public class BuildVersionComparer : IComparer<KeyValuePair<Version, string>>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Compare(KeyValuePair<Version, string> x, KeyValuePair<Version, string> y)
        {
            return y.Key.CompareTo(x.Key);
        }

        #endregion
    }
}