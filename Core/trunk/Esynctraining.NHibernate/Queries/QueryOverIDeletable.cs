using Esynctraining.Core.Domain.Entities;

using NHibernate.Criterion;

namespace Esynctraining.NHibernate.Queries
{
    /// <summary>
    /// The query over i delete able.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entity type
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Key type
    /// </typeparam>
    public class QueryOverIDeletable<TEntity, TKey> : DefaultQueryOver<TEntity, TKey>
        where TEntity : IEntity<TKey>, IDeletable where TKey : struct
    {
        #region Methods

        /// <summary>
        /// The apply.
        /// </summary>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        /// <returns>
        /// The <see cref="QueryOver"/>.
        /// </returns>
        protected override QueryOver<TEntity, TEntity> Apply(QueryOver<TEntity, TEntity> queryOver)
        {
            return base.Apply(queryOver).Where(x => x.IsDeleted == false);
        }

        #endregion
    }

}
