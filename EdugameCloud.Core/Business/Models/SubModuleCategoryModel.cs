using System.Linq;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;
    using NHibernate;

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
        /// The get one by name and user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{SubModuleCategory}"/>.
        /// </returns>
        public IFutureValue<SubModuleCategory> GetOneByNameAndUser(int userId, string name)
        {
            var query =
                new DefaultQueryOver<SubModuleCategory, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.CategoryName)
                    .IsInsensitiveLike(name, MatchMode.Exact)
                    .And(x => x.User.Id == userId);
            return this.Repository.FindOne(query);
        }

        public IFutureValue<SubModuleCategory> GetOneByLmsCourseIdAndCompanyLms(string lmsCourseId, int companyLmsId)
        {
            var query =
                new DefaultQueryOver<SubModuleCategory, int>().GetQueryOver().Where(x => x.LmsCourseId == lmsCourseId && x.CompanyLmsId != null && x.CompanyLmsId == companyLmsId);
            return this.Repository.FindOne(query);
        }

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

        public IEnumerable<SubModuleCategory> GetByUser(int userId)
        {
            var queryOver = new DefaultQueryOver<SubModuleCategory, int>().GetQueryOver()
                .Where(x => x.User.Id == userId);

            return this.Repository.FindAll(queryOver);
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
			SubModuleItem smi = null;
			SubModuleCategory smc = null;
			AppletItem ai = null;
			SubModuleCategoryFromStoredProcedureDTO dto = null;
	        var queryOver = new DefaultQueryOver<SubModuleCategory, int>().GetQueryOver(() => smc)
		        .JoinQueryOver(x => x.SubModuleItems, () => smi, JoinType.InnerJoin)
		        .JoinQueryOver(() => smi.AppletItems, () => ai, JoinType.LeftOuterJoin)
		        .Where(() => smc.User.Id == userId && smi.CreatedBy.Id == userId && ai.Id != 0)
		        .SelectList(res =>
			        res.Select(Projections.Distinct(Projections.ProjectionList()
				        .Add(Projections.Property(() => smc.IsActive))
				        .Add(Projections.Property(() => smc.DateModified))
				        .Add(Projections.Property(() => smc.ModifiedBy.Id))
				        .Add(Projections.Property(() => smc.CategoryName))
				        .Add(Projections.Property(() => smc.SubModule.Id))
				        .Add(Projections.Property(() => smc.User.Id))
				        .Add(Projections.Property(() => smc.Id))
				        ))
				        .Select(() => smc.IsActive)
				        .WithAlias(() => dto.isActive)
				        .Select(() => smc.DateModified)
				        .WithAlias(() => dto.dateModified)
				        .Select(() => smc.ModifiedBy.Id)
				        .WithAlias(() => dto.modifiedBy)
				        .Select(() => smc.CategoryName)
				        .WithAlias(() => dto.categoryName)
				        .Select(() => smc.SubModule.Id)
				        .WithAlias(() => dto.subModuleId)
				        .Select(() => smc.User.Id)
				        .WithAlias(() => dto.userId)
				        .Select(() => smc.Id)
				        .WithAlias(() => dto.subModuleCategoryId))
                .TransformUsing(Transformers.AliasToBean<SubModuleCategoryFromStoredProcedureDTO>());
            var result =
                this.Repository.FindAll<SubModuleCategoryFromStoredProcedureDTO>(queryOver)
                    .ToList()
                    .Select(x => new SubModuleCategoryDTO(x));
            return result;
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
			SubModuleItem smi = null;
			SubModuleCategory smc = null;
			Quiz q = null;
            SubModuleCategoryFromStoredProcedureDTO dto = null;
			var queryOver = new DefaultQueryOver<SubModuleCategory, int>().GetQueryOver(() => smc)
				.JoinQueryOver(x => x.SubModuleItems, () => smi, JoinType.InnerJoin)
				.JoinQueryOver(() => smi.Quizes, () => q, JoinType.LeftOuterJoin)
				.Where(() => smc.User.Id == userId && smi.CreatedBy.Id == userId && q.Id != 0)
				.SelectList(res =>
					  res.Select(Projections.Distinct(Projections.ProjectionList()
						.Add(Projections.Property(() => smc.IsActive))
						.Add(Projections.Property(() => smc.DateModified))
						.Add(Projections.Property(() => smc.ModifiedBy.Id))
						.Add(Projections.Property(() => smc.CategoryName))
						.Add(Projections.Property(() => smc.SubModule.Id))
						.Add(Projections.Property(() => smc.User.Id))
						.Add(Projections.Property(() => smc.Id))
						))
						.Select(() => smc.IsActive)
						.WithAlias(() => dto.isActive)
						.Select(() => smc.DateModified)
						.WithAlias(() => dto.dateModified)
						.Select(() => smc.ModifiedBy.Id)
						.WithAlias(() => dto.modifiedBy)
						.Select(() => smc.CategoryName)
						.WithAlias(() => dto.categoryName)
						.Select(() => smc.SubModule.Id)
						.WithAlias(() => dto.subModuleId)
						.Select(() => smc.User.Id)
						.WithAlias(() => dto.userId)
						.Select(() => smc.Id)
						.WithAlias(() => dto.subModuleCategoryId))
                .TransformUsing(Transformers.AliasToBean<SubModuleCategoryFromStoredProcedureDTO>());
            var result =
                this.Repository.FindAll<SubModuleCategoryFromStoredProcedureDTO>(queryOver)
                    .ToList()
                    .Select(x => new SubModuleCategoryDTO(x));
	        return result;
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
			SubModuleItem smi = null;
	        SubModuleCategory smc = null;
	        Test t = null;
            SubModuleCategoryFromStoredProcedureDTO dto = null;
	        var qieryOver = new DefaultQueryOver<SubModuleCategory, int>().GetQueryOver(() => smc)
		        .JoinQueryOver(x => x.SubModuleItems, () => smi, JoinType.InnerJoin)
		        .JoinQueryOver(() => smi.Tests, () => t, JoinType.LeftOuterJoin)
		        .Where(() => smc.User.Id == userId && smi.CreatedBy.Id == userId && t.Id != 0)
		        .SelectList(res =>
			        res.Select(Projections.Distinct(Projections.ProjectionList()
				        .Add(Projections.Property(() => smc.IsActive))
				        .Add(Projections.Property(() => smc.DateModified))
				        .Add(Projections.Property(() => smc.ModifiedBy.Id))
				        .Add(Projections.Property(() => smc.CategoryName))
				        .Add(Projections.Property(() => smc.SubModule.Id))
				        .Add(Projections.Property(() => smc.User.Id))
				        .Add(Projections.Property(() => smc.Id))
				        ))
				        .Select(() => smc.IsActive)
				        .WithAlias(() => dto.isActive)
				        .Select(() => smc.DateModified)
				        .WithAlias(() => dto.dateModified)
				        .Select(() => smc.ModifiedBy.Id)
				        .WithAlias(() => dto.modifiedBy)
				        .Select(() => smc.CategoryName)
				        .WithAlias(() => dto.categoryName)
				        .Select(() => smc.SubModule.Id)
				        .WithAlias(() => dto.subModuleId)
				        .Select(() => smc.User.Id)
				        .WithAlias(() => dto.userId)
				        .Select(() => smc.Id)
				        .WithAlias(() => dto.subModuleCategoryId)
		        )
                .TransformUsing(Transformers.AliasToBean<SubModuleCategoryFromStoredProcedureDTO>());
            var result =
                Repository.FindAll<SubModuleCategoryFromStoredProcedureDTO>(qieryOver)
                    .ToList()
                    .Select(x => new SubModuleCategoryDTO(x));

	        return result;
        }

        #endregion

    }

}