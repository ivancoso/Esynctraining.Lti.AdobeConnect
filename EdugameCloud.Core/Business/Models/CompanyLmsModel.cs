namespace EdugameCloud.Core.Business.Models
{
    using System.Collections;
    using System.Collections.Generic;

    using DocumentFormat.OpenXml.Spreadsheet;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    public class CompanyLmsModel : BaseModel<CompanyLms, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLicenseModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CompanyLmsModel(IRepository<CompanyLms, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get one by company id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{CompanyLms}"/>.
        /// </returns>
        public IEnumerable<CompanyLms> GetAllByCompanyId(int companyId)
        {
            var queryOver =
                new DefaultQueryOver<CompanyLms, int>().GetQueryOver()
                    .Where(c => c.Company.Id == companyId);
            return this.Repository.FindAll(queryOver);
        }

        #endregion
    }
}