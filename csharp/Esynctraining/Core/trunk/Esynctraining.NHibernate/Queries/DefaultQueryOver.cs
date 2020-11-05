using System;
using System.Linq.Expressions;

using Esynctraining.Core.Domain.Entities;

using NHibernate.Criterion;

namespace Esynctraining.NHibernate.Queries
{
    /// <summary>
    /// The default query over.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Type of entity <see cref="IEntity{V}" />
    /// </typeparam>
    /// <typeparam name="TV">
    /// The entity Id type
    /// </typeparam>
    public class DefaultQueryOver<TEntity, TV>
        where TV : struct where TEntity : IEntity<TV>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get query over.
        /// </summary>
        /// <returns>
        /// The <see cref="QueryOver"/>.
        /// </returns>
        public virtual QueryOver<TEntity, TEntity> GetQueryOver()
        {
            return this.Apply(QueryOver.Of<TEntity>());
        }

        /// <summary>
        /// The get query over.
        /// </summary>
        /// <returns>
        /// The <see cref="QueryOver"/>.
        /// </returns>
        public virtual QueryOver<TEntity, TEntity> GetQueryOver(Expression<Func<TEntity>> aliasExpression)
        {
            return this.Apply(QueryOver.Of(aliasExpression));
        }

        #endregion

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
        protected virtual QueryOver<TEntity, TEntity> Apply(QueryOver<TEntity, TEntity> queryOver)
        {
            return queryOver;
        }

        #endregion

    }

}
