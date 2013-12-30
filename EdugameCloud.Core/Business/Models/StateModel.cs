namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    /// <summary>
    ///     The state model.
    /// </summary>
    public class StateModel : BaseModel<State, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public StateModel(IRepository<State, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get one by name.
        /// </summary>
        /// <param name="stateName">
        /// The state Name.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        public IFutureValue<State> GetOneByName(string stateName)
        {
            var query =
                new DefaultQueryOver<State, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.StateName)
                    .IsInsensitiveLike(stateName)
                    .Take(1);
            return this.Repository.FindOne(query);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public override IEnumerable<State> GetAll()
        {
            var query = new DefaultQueryOver<State, int>().GetQueryOver().OrderBy(x => x.StateName).Asc;
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The get one by name.
        /// </summary>
        /// <param name="stateCode">
        /// The state Code.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        public IFutureValue<State> GetOneByCode(string stateCode)
        {
            var query =
                new DefaultQueryOver<State, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.StateCode)
                    .IsInsensitiveLike(stateCode)
                    .Take(1);
            return this.Repository.FindOne(query);
        }
    }
}