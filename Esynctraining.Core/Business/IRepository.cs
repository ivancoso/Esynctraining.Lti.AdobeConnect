namespace Esynctraining.Core.Business
{
    using System.Collections.Generic;

    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Domain.Entities;

    using NHibernate;
    using NHibernate.Criterion;

    /// <summary>
    /// The Repository interface.
    /// </summary>
    /// <typeparam name="T">
    /// Type of entity
    /// </typeparam>
    /// <typeparam name="TId">
    /// Type of entity id
    /// </typeparam>
    public interface IRepository<T, TId> where TId : struct where T : IEntity<TId>
    {
        #region Public Properties

        /// <summary>
        /// Gets the session.
        /// </summary>
        ISession Session { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The find all.
        /// </summary>
        /// <param name="defaultQueryOver">
        /// The default query over.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<T> FindAll(DefaultQueryOver<T, TId> defaultQueryOver);

        /// <summary>
        /// The find all.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<T> FindAll(DetachedCriteria criteria);

        /// <summary>
        /// The find all.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<T> FindAll(DetachedCriteria criteria, int timeout);

        /// <summary>
        /// The find all.
        /// </summary>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<T> FindAll(QueryOver<T> queryOver);

        /// <summary>
        /// The find all.
        /// </summary>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        /// <typeparam name="TOtherEntity">
        /// Any entity you want to return
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{TOtherEntity}"/>.
        /// </returns>
        IEnumerable<TOtherEntity> FindAll<TOtherEntity>(QueryOver<T> queryOver);

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="defaultQueryOver">
        /// The default query over.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        IFutureValue<T> FindOne(DefaultQueryOver<T, TId> defaultQueryOver);

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        IFutureValue<T> FindOne(DetachedCriteria criteria);

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        IFutureValue<T> FindOne(QueryOver<T, T> queryOver);

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        IFutureValue<T> FindOne(QueryOver<T> queryOver);

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <typeparam name="TReturn">
        /// Type of returned object
        /// </typeparam>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        IFutureValue<TReturn> FindOne<TReturn>(DetachedCriteria criteria);

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        /// <typeparam name="TReturn">
        /// Type of returned object
        /// </typeparam>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        IFutureValue<TReturn> FindOne<TReturn>(QueryOver<T> queryOver);

        /// <summary>
        /// The flush.
        /// </summary>
        void Flush();

        /// <summary>
        /// The register delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T RegisterDelete(T entity);

        /// <summary>
        /// The register save.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T RegisterSave(T entity);

        /// <summary>
        /// The store procedure for one.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="storeProcedureParams">
        /// The store procedure parameters.
        /// </param>
        /// <typeparam name="TSome">
        /// Type of resulting value
        /// </typeparam>
        /// <returns>
        /// The <see cref="IFutureValue{TSome}"/>.
        /// </returns>
        IFutureValue<TSome> StoreProcedureForOne<TSome>(string name, params StoreProcedureParam[] storeProcedureParams);

        /// <summary>
        /// The store procedure for many.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="storeProcedureParams">
        /// The store procedure parameters.
        /// </param>
        /// <typeparam name="TSome">
        /// Type of resulting value
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{TSome}"/>.
        /// </returns>
        IEnumerable<TSome> StoreProcedureForMany<TSome>(string name, params StoreProcedureParam[] storeProcedureParams);

        #endregion
    }
}