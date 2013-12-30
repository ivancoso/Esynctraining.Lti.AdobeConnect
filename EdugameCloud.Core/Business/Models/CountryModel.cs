namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    /// <summary>
    ///     The country model.
    /// </summary>
    public class CountryModel : BaseModel<Country, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CountryModel(IRepository<Country, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get one by name.
        /// </summary>
        /// <param name="country">
        /// The country.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Country}"/>.
        /// </returns>
        public IFutureValue<Country> GetOneByName(string country)
        {
            var query =
                new DefaultQueryOver<Country, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.CountryName)
                    .IsInsensitiveLike(country)
                    .Take(1);
            return this.Repository.FindOne(query);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Country}"/>.
        /// </returns>
        public override IEnumerable<Country> GetAll()
        {
            var query = new DefaultQueryOver<Country, int>().GetQueryOver().OrderBy(x => x.CountryName).Asc;
            return this.Repository.FindAll(query);
        }
    }
}