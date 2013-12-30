namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Business.Queries;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate.Criterion;

    /// <summary>
    ///     The company model.
    /// </summary>
    public class CompanyModel : BaseModel<Company, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CompanyModel(IRepository<Company, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get all for company.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public override IEnumerable<Company> GetAll()
        {
            var defaultQuery = new QueryOverCompany().GetQueryOver();
            return this.Repository.FindAll(defaultQuery);
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
        public override void RegisterDelete(Company entity, bool flush)
        {
            entity.Status = CompanyStatus.Deleted;
            this.RegisterSave(entity, flush);
        }

        /// <summary>
        /// The get all by users.
        /// </summary>
        /// <param name="users">
        /// The to list.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Company}"/>.
        /// </returns>
        public IEnumerable<Company> GetAllByUsers(List<int> users)
        {
            User @usr = null;
            var idSubQuery = QueryOver.Of(() => @usr)
                    .WhereRestrictionOn(() => @usr.Id).IsIn(users)
                    .Select(Projections.Distinct(Projections.Property<User>(s => s.Company.Id)));
            var query = new DefaultQueryOver<Company, int>().GetQueryOver()
                .OrderBy(x => x.CompanyName)
                .Asc.WithSubquery.WhereProperty(x => x.Id)
                .In(idSubQuery);
            return this.Repository.FindAll(query);
        }
    }
}