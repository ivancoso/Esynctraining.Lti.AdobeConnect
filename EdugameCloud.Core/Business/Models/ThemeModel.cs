namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;

    /// <summary>
    ///     The theme model.
    /// </summary>
    public class ThemeModel : BaseModel<Theme, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public ThemeModel(IRepository<Theme, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get all paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page items.
        /// </param>
        /// <param name="totalCount">
        /// The total Count.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Theme}"/>.
        /// </returns>
        public IEnumerable<Theme> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            var queryOver =
                new DefaultQueryOver<Theme, int>().GetQueryOver();
            var rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            var pagedQuery = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            return this.Repository.FindAll(pagedQuery);
        }
    }
}