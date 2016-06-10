namespace PDFAnnotation.Core.Extensions
{
    using NHibernate.Criterion;

    /// <summary>
    /// The n hibernate query extensions.
    /// </summary>
    public static class NHibernateQueryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The take.
        /// </summary>
        /// <param name="q">
        /// The q.
        /// </param>
        /// <param name="maxResults">
        /// The max results.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="V">
        /// </typeparam>
        /// <returns>
        /// The <see cref="QueryOver"/>.
        /// </returns>
        public static QueryOver<T, V> TakeOnly<T, V>(this QueryOver<T, V> q, int maxResults)
        {
            return (QueryOver<T, V>)q.Take(maxResults);
        }

        #endregion
    }
}