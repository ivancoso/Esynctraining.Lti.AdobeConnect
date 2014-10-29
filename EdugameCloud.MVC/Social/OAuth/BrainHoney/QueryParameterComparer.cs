namespace EdugameCloud.MVC.Social.OAuth.BrainHoney
{
    using System.Collections.Generic;

    /// <summary>
    /// The query parameter comparer.
    /// </summary>
    public class QueryParameterComparer : IComparer<QueryParameter>
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
        public int Compare(QueryParameter x, QueryParameter y)
        {
            if (x.Name == y.Name)
            {
                return string.CompareOrdinal(x.Value, y.Value);
            }

            return string.CompareOrdinal(x.Name, y.Name);
        }

        #endregion
    }
}