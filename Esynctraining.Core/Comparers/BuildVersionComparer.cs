namespace Esynctraining.Core.Comparers
{
    using System.Collections.Generic;

    /// <summary>
    /// The build version comparer.
    /// </summary>
    public class BuildVersionComparer : IComparer<KeyValuePair<int, string>>
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
        public int Compare(KeyValuePair<int, string> x, KeyValuePair<int, string> y)
        {
            return y.Key.CompareTo(x.Key);
        }

        #endregion
    }
}