namespace Esynctraining.Core.Extensions
{
    using System;
    using System.Collections.Generic;

    using NHibernate.Criterion;

    /// <summary>
    ///     The detached criteria extensions.
    /// </summary>
    public static class DetachedCriteriaExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get in criteria.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{DetachedCriteria}"/>.
        /// </returns>
        public static IEnumerable<DetachedCriteria> GetInCriteries<T>(
            this DetachedCriteria criteria, string property, IList<T> values)
        {
            return GetInCriteries(criteria, property, values, false);
        }

        /// <summary>
        /// The get not in criteries.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{DetachedCriteria}"/>.
        /// </returns>
        public static IEnumerable<DetachedCriteria> GetNotInCriteries<T>(this DetachedCriteria criteria, string property, IList<T> values)
        {
            return GetInCriteries(criteria, property, values, true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get in criteria.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="invert">
        /// The invert.
        /// </param>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{DetachedCriteria}"/>.
        /// </returns>
        private static IEnumerable<DetachedCriteria> GetInCriteries<T>(this DetachedCriteria criteria, string property, IList<T> values, bool invert)
        {
            // Maximum count of parameters for MS Sql Server is 2100
            const int ParametersCount = 2000;

            int count = values.Count / ParametersCount;

            if (values.Count % ParametersCount != 0)
            {
                count++;
            }

            for (int i = 0; i < count; i++)
            {
                var parameters = new List<object>();

                for (int j = i * ParametersCount; j < Math.Min((i + 1) * ParametersCount, values.Count); j++)
                {
                    parameters.Add(values[j]);
                }

                DetachedCriteria newCriteria = criteria.DeepClone();

                newCriteria.Add(
                    !invert
                        ? Restrictions.In(property, parameters)
                        : Restrictions.Not(Restrictions.In(property, parameters)));

                yield return newCriteria;
            }
        }

        #endregion
    }
}