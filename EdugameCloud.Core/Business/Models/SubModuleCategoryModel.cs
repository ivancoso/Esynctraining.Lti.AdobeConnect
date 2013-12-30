namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;

    using EdugameCloud.Core.Business.Queries;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    /// <summary>
    ///     The SubModuleCategory model.
    /// </summary>
    public class SubModuleCategoryModel : BaseModel<SubModuleCategory, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleCategoryModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SubModuleCategoryModel(IRepository<SubModuleCategory, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

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
        /// The <see cref="IEnumerable{SubModuleCategory}"/>.
        /// </returns>
        public IEnumerable<SubModuleCategory> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            var queryOver = new DefaultQueryOver<SubModuleCategory, int>().GetQueryOver();
            var rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            var pagedQuery = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            return this.Repository.FindAll(pagedQuery);
        }

        /// <summary>
        /// The get applet categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleCategoryDTO}"/>.
        /// </returns>
        public IEnumerable<SubModuleCategoryDTO> GetAppletCategoriesByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SubModuleCategoryDTO>(
                "getAppletCategoriesByUserID", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get quiz categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleCategoryDTO}"/>.
        /// </returns>
        public IEnumerable<SubModuleCategoryDTO> GetQuizCategoriesByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SubModuleCategoryDTO>(
                "getQuizCategoriesByUserID", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get sn profile categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleCategoryDTO}"/>.
        /// </returns>
        public IEnumerable<SubModuleCategoryDTO> GetSNProfileCategoriesByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SubModuleCategoryDTO>(
                "getSNProfileCategoriesByUserID", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get survey categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleCategoryDTO}"/>.
        /// </returns>
        public IEnumerable<SubModuleCategoryDTO> GetSurveyCategoriesByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SubModuleCategoryDTO>(
                "getSurveyCategoriesByUserID", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get test categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleCategoryDTO}"/>.
        /// </returns>
        public IEnumerable<SubModuleCategoryDTO> GetTestCategoriesByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SubModuleCategoryDTO>(
                "getTestCategoriesByUserID", new StoreProcedureParam<int>("userId", userId));
        }

        #endregion
    }
}