namespace Esynctraining.Persistence
{
    using System.Collections.Generic;
    using System.Linq;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Domain.Entities;

    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.Transform;

    /// <summary>
    /// The repository.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Type of entity <see cref="TEntity"/>
    /// </typeparam>
    /// <typeparam name="TId">
    /// Type of entity id <see cref="TId"/>
    /// </typeparam>
    public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TId : struct where TEntity : IEntity<TId>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity,TId}"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public Repository(ISessionSource manager)
        {
            this.Manager = manager;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        public ISessionSource Manager { get; set; }

        /// <summary>
        /// Gets the session.
        /// </summary>
        public ISession Session
        {
            get
            {
                return this.Manager.Session;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The find all.
        /// </summary>
        /// <param name="defaultQuery">
        /// The default query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<TEntity> FindAll(DefaultQueryOver<TEntity, TId> defaultQuery)
        {
            return this.FindAll(defaultQuery.GetQueryOver());
        }

        /// <summary>
        /// The find all.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<TEntity> FindAll(DetachedCriteria criteria)
        {
            return criteria.GetExecutableCriteria(this.Manager.Session).Future<TEntity>();
        }

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
        public IEnumerable<TEntity> FindAll(DetachedCriteria criteria, int timeout)
        {
            return criteria.GetExecutableCriteria(this.Manager.Session).SetTimeout(timeout).List<TEntity>();
        }

        /// <summary>
        /// The find all.
        /// </summary>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<TEntity> FindAll(QueryOver<TEntity> queryOver)
        {
            return queryOver.GetExecutableQueryOver(this.Manager.Session).Future<TEntity>();
        }

        /// <summary>
        /// The find all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<TEntity> FindAll()
        {
            return this.FindAll(new DefaultQueryOver<TEntity, TId>().GetQueryOver());
        }

        /// <summary>
        /// The find all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IFutureValue<int> Count()
        {
            return this.FindOne<int>(new DefaultQueryOver<TEntity, TId>().GetQueryOver().ToRowCountQuery());
        }

        /// <summary>
        /// The find all paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TEntity}"/>.
        /// </returns>
        public IEnumerable<TEntity> FindAllPaged(int pageIndex, int pageSize)
        {
            if (pageIndex <= default(int))
            {
                pageIndex = 1;
            }

            var queryOver = new DefaultQueryOver<TEntity, TId>().GetQueryOver();
            var pagedQuery = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            return this.FindAll(pagedQuery);
        }

        /// <summary>
        /// The find all.
        /// </summary>
        /// <typeparam name="TOtherEntity">
        /// The other entity type
        /// </typeparam>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<TOtherEntity> FindAll<TOtherEntity>(QueryOver<TEntity> queryOver)
        {
            return queryOver.GetExecutableQueryOver(this.Manager.Session).Future<TOtherEntity>();
        }

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="defaultQuery">
        /// The default query.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        public IFutureValue<TEntity> FindOne(DefaultQueryOver<TEntity, TId> defaultQuery)
        {
            return this.FindOne<TEntity>(defaultQuery.GetQueryOver());
        }

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        public IFutureValue<TEntity> FindOne(QueryOver<TEntity> query)
        {
            return this.FindOne<TEntity>(query);
        }

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        public IFutureValue<TEntity> FindOne(DetachedCriteria criteria)
        {
            return this.FindOne<TEntity>(criteria);
        }

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        public IFutureValue<TEntity> FindOne(QueryOver<TEntity, TEntity> queryOver)
        {
            return this.FindOne<TEntity>(queryOver);
        }

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <typeparam name="TReturn">
        /// Returned object type
        /// </typeparam>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        public IFutureValue<TReturn> FindOne<TReturn>(DetachedCriteria criteria)
        {
            return criteria.GetExecutableCriteria(this.Manager.Session).FutureValue<TReturn>();
        }

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="defaultQuery">
        /// The default query.
        /// </param>
        /// <typeparam name="TReturn">
        /// Returned object type
        /// </typeparam>
        /// <returns>
        /// The <see cref="IFutureValue{TReturn}"/>.
        /// </returns>
        public IFutureValue<TReturn> FindOne<TReturn>(DefaultQueryOver<TEntity, TId> defaultQuery)
        {
            return this.FindOne<TReturn>(defaultQuery.GetQueryOver());
        }

        /// <summary>
        /// The find one.
        /// </summary>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        /// <typeparam name="TReturn">
        /// Returned object type
        /// </typeparam>
        /// <returns>
        /// The <see cref="IFutureValue{TReturn}"/>.
        /// </returns>
        public IFutureValue<TReturn> FindOne<TReturn>(QueryOver<TEntity> queryOver)
        {
            return queryOver.GetExecutableQueryOver(this.Manager.Session).FutureValue<TReturn>();
        }

        /// <summary>
        /// The flush.
        /// </summary>
        public void Flush()
        {
            this.Manager.Session.Flush();
        }

        /// <summary>
        /// The register delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public TEntity RegisterDelete(TEntity entity)
        {
            if (entity is IDeletable)
            {
                var deletedHolder = entity as IDeletable;
                deletedHolder.IsDeleted = true;
                this.RegisterSave(entity);
            }
            else
            {
                this.Manager.Session.Delete(entity);
            }

            return entity;
        }

        /// <summary>
        /// The register save.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public TEntity RegisterSave(TEntity entity)
        {
            this.Manager.Session.SaveOrUpdate(entity);
            return entity;
        }

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
        public IFutureValue<TSome> StoreProcedureForOne<TSome>(string name, params StoreProcedureParam[] storeProcedureParams)
        {
            var query = this.Manager.Session.GetNamedQuery(name);
            query = storeProcedureParams.Aggregate(query, (current, storeProcedureParam) => storeProcedureParam.AddParam(current));
            return query.SetResultTransformer(Transformers.AliasToBean<TSome>()).FutureValue<TSome>();
        }

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
        public IEnumerable<TSome> StoreProcedureForMany<TSome>(string name, params StoreProcedureParam[] storeProcedureParams)
        {
            var query = this.Manager.Session.GetNamedQuery(name);
            query = storeProcedureParams.Aggregate(query, (current, storeProcedureParam) => storeProcedureParam.AddParam(current));
            if (typeof(TSome).IsClass)
            {
                query = query.SetResultTransformer(new AliasToBeanResultTransformer(typeof(TSome)));
            }
            return query.List<TSome>();
        }

        #endregion
    }
}