namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;
    using NHibernate.Transform;

    /// <summary>
    ///     The company model.
    /// </summary>
    public class CompanyLicenseModel : BaseModel<CompanyLicense, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLicenseModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CompanyLicenseModel(IRepository<CompanyLicense, int> repository)
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
        /// The <see cref="IFutureValue{CompanyLicense}"/>.
        /// </returns>
        public IFutureValue<CompanyLicense> GetOneByCompanyId(int companyId)
        {
            var queryOver =
                new DefaultQueryOver<CompanyLicense, int>().GetQueryOver()
                                                           .JoinQueryOver(x => x.Company)
                                                           .Where(c => c.Id == companyId);
            queryOver.TransformUsing(Transformers.DistinctRootEntity).Take(1);
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get one by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{CompanyLicense}"/>.
        /// </returns>
        public IFutureValue<CompanyLicense> GetOneByUserId(int userId)
        {
            User user = null;
            var queryOver =
                new DefaultQueryOver<CompanyLicense, int>().GetQueryOver()
                                                           .JoinQueryOver(x => x.Company)
                                                           .JoinQueryOver(c => c.Users, () => user)
                                                           .Where(() => user.Id == userId);
            queryOver.TransformUsing(Transformers.DistinctRootEntity).Take(1);
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get one by user id.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{CompanyLicense}"/>.
        /// </returns>
        public IEnumerable<CompanyLicense> GetAllTrial()
        {
            var queryOver =
                new DefaultQueryOver<CompanyLicense, int>().GetQueryOver().Where(x => x.LicenseStatus == CompanyLicenseStatus.Trial);
            return this.Repository.FindAll(queryOver);
        }

        #endregion
    }
}