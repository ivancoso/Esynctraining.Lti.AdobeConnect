// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyContactModel.cs" company="">
//   
// </copyright>
// <summary>
//   The CompanyContact model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PDFAnnotation.Core.Business.Models
{
    using System.Collections.Generic;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate.Criterion;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The CompanyContact model.
    /// </summary>
    public class CompanyContactModel : BaseModel<CompanyContact, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyContactModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CompanyContactModel(IRepository<CompanyContact, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get all by company id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{CompanyContact}"/>.
        /// </returns>
        public IEnumerable<CompanyContact> GetAllByCompanyId(int companyId)
        {
            QueryOver<CompanyContact, CompanyContact> query =
                new DefaultQueryOver<CompanyContact, int>().GetQueryOver()
                    .Where(x => x.Company.Id == companyId)
                    .Fetch(x => x.Contact)
                    .Eager;
            return this.Repository.FindAll(query);
        }

        #endregion
    }
}