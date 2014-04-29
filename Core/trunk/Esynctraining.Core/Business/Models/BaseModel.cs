namespace Esynctraining.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;

    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Domain.Entities;

    using NHibernate;
    using NHibernate.Criterion;

    /// <summary>
    /// The base model.
    /// </summary>
    /// <typeparam name="T">
    /// Type of entity
    /// </typeparam>
    /// <typeparam name="TId">
    /// Type of entity Id
    /// </typeparam>
    public class BaseModel<T, TId> where TId : struct where T : IEntity<TId>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModel{T,TId}"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        protected BaseModel(IRepository<T, TId> repository)
        {
            this.Repository = repository;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the repository.
        /// </summary>
        protected IRepository<T, TId> Repository { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The flush.
        /// </summary>
        public virtual void Flush()
        {
            this.Repository.Flush();
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public virtual IEnumerable<T> GetAll()
        {
            QueryOver<T, T> defaultQuery = new DefaultQueryOver<T, TId>().GetQueryOver();
            return this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The get all by ids.
        /// </summary>
        /// <param name="ids">
        /// The ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public virtual IEnumerable<T> GetAllByIds(List<TId> ids)
        {
            var query = new DefaultQueryOver<T, TId>().GetQueryOver().WhereRestrictionOn(x => x.Id).IsIn(ids);
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The get one by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        public virtual IFutureValue<T> GetOneById(TId id)
        {
            QueryOver<T, T> queryOver = new DefaultQueryOver<T, TId>().GetQueryOver().WhereRestrictionOn(x => x.Id).IsLike(id);
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// Exists.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool Exists(TId id)
        {
            QueryOver<T, T> queryOver = new DefaultQueryOver<T, TId>().GetQueryOver().WhereRestrictionOn(x => x.Id).IsLike(id);
            return !(this.Repository.FindOne(queryOver).Value == null);
        }

        /// <summary>
        /// The register delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public virtual void RegisterDelete(T entity)
        {
            this.RegisterDelete(entity, false);
        }

        /// <summary>
        /// The register delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="flush">
        /// The flush.
        /// </param>
        public virtual void RegisterDelete(T entity, bool flush)
        {
            if (!entity.Equals(default(T)))
            {
                this.Repository.RegisterDelete(entity);

                if (flush)
                {
                    this.Repository.Flush();
                }
            }
        }

        /// <summary>
        /// The register save.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public virtual void RegisterSave(T entity)
        {
            this.RegisterSave(entity, false);
        }

        /// <summary>
        /// The register save.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="flush">
        /// The flush.
        /// </param>
        /// <param name="updateDateModified">
        /// The prevent Date Modification.
        /// </param>
        public virtual void RegisterSave(T entity, bool flush, bool updateDateModified = true)
        {
            if (!entity.Equals(default(T)))
            {
                if (typeof(IDatesContainer).IsAssignableFrom(typeof(T)))
                {
                    var date = DateTime.Now;
                    var datesContainer = (IDatesContainer)entity;
                    if (entity.Id.Equals(default(TId)))
                    {
                        datesContainer.DateCreated = date;
                        datesContainer.DateModified = null;
                    }
                    else if (updateDateModified)
                    {
                        datesContainer.DateModified = date;
                    }
                }

                this.Repository.RegisterSave(entity);

                if (flush)
                {
                    this.Repository.Flush();
                }
            }
        }

        /// <summary>
        /// The refresh.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public virtual void Refresh(ref T entity)
        {
            this.Repository.Session.Evict(entity);
            entity = this.Repository.Session.Get<T>(entity.Id);
        }

        #endregion

        /// <summary>
        /// The validate date time.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        protected DateTime ValidateDateTime(DateTime dt)
        {
            if (SqlDateTime.MinValue.Value > dt)
            {
                return SqlDateTime.MinValue.Value;
            }

            if (SqlDateTime.MaxValue.Value < dt)
            {
                return SqlDateTime.MaxValue.Value;
            }

            return dt;
        }

        /// <summary>
        /// The validate date time.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        protected DateTime? ValidateDateTime(DateTime? dt)
        {
            if (dt.HasValue)
            {
                if (SqlDateTime.MinValue.Value > dt)
                {
                    return SqlDateTime.MinValue.Value;
                }

                if (SqlDateTime.MaxValue.Value < dt)
                {
                    return SqlDateTime.MaxValue.Value;
                }
            }

            return dt;
        }
    }
}