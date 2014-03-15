﻿using NHibernate.SqlCommand;

namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.Transform;

    /// <summary>
    ///     The AppletItem model.
    /// </summary>
    public class AppletItemModel : BaseModel<AppletItem, int>
    {        
		#region Fields

        /// <summary>
        /// The user repository.
        /// </summary>
        private readonly IRepository<User, int> userRepository;

		#endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppletItemModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public AppletItemModel(IRepository<User, int> userRepository,IRepository<AppletItem, int> repository)
            : base(repository)
        {
	        this.userRepository = userRepository;
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
        /// The <see cref="IEnumerable{AppletItem}"/>.
        /// </returns>
        public IEnumerable<AppletItem> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            QueryOver<AppletItem, AppletItem> queryOver = new DefaultQueryOver<AppletItem, int>().GetQueryOver();
            QueryOver<AppletItem, AppletItem> rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            QueryOver<AppletItem> pagedQuery = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            return this.Repository.FindAll(pagedQuery);
        }

        /// <summary>
        /// The get crossword result by AC session id.
        /// </summary>
        /// <param name="adobeConnectSessionId">
        /// The AC session id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{CrosswordResultByAcSessionDTO}"/>.
        /// </returns>
        public IEnumerable<CrosswordResultByAcSessionDTO> GetCrosswordResultByACSessionId(int adobeConnectSessionId)
        {
            return
                this.Repository.StoreProcedureForMany<CrosswordResultByAcSessionDTO>(
                    "getCrosswordResultByACSessionId", new StoreProcedureParam<int>("acSessionId", adobeConnectSessionId));
        }

        /// <summary>
        /// The get crossword sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{CrosswordSessionDTO}"/>.
        /// </returns>
        public IEnumerable<CrosswordSessionDTO> GetCrosswordSessionsByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<CrosswordSessionDTO>(
                "getCrosswordSessionsByUserId", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get crosswords.
        /// </summary>
        /// <param name="reportsIds">
        /// The reports ids.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary{Integer, RecentReportDTO}"/>.
        /// </returns>
        public Dictionary<int, RecentReportDTO> GetCrosswords(List<int> reportsIds)
        {
            RecentReportDTO dto = null;
            return
                this.Repository.FindAll<RecentReportDTO>(
                    new DefaultQueryOver<AppletItem, int>().GetQueryOver()
                                                           .WhereRestrictionOn(x => x.SubModuleItem.Id)
                                                           .IsIn(reportsIds)
                                                           .SelectList(
                                                               list =>
                                                               list.Select(x => x.AppletName)
                                                                   .WithAlias(() => dto.name)
                                                                   .Select(x => x.SubModuleItem.Id)
                                                                   .WithAlias(() => dto.subModuleItemId))
                                                           .TransformUsing(Transformers.AliasToBean<RecentReportDTO>()))
                    .ToDictionary(x => x.subModuleItemId, x => x);
        }

        /// <summary>
        /// The get crosswords by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{CrosswordDTO}"/>.
        /// </returns>
        public IEnumerable<CrosswordDTO> GetCrosswordsByUserId(int userId)
        {
	        AppletItem ai = null;
	        SubModuleItem smi = null;
	        SubModuleCategory smc = null;
	        User u = null;
	        User u2 = null;
	        CrosswordDTO dto = null;
	        var queryOver = new DefaultQueryOver<AppletItem, int>().GetQueryOver(() => ai)
		        .JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.InnerJoin).Where(() => smi.IsActive == true)
		        .JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin).Where(() => smc.IsActive == true)
		        .JoinQueryOver(() => smc.User, () => u, JoinType.InnerJoin)
		        .JoinQueryOver(() => smi.CreatedBy, () => u2, JoinType.LeftOuterJoin)
		        .Where(() => u2.Id == userId)
		        .SelectList(res =>
			        res.Select(() => ai.AppletName)
				        .WithAlias(() => dto.appletName)
				        .Select(() => ai.Id)
				        .WithAlias(() => dto.appletItemId)
				        .Select(() => u.LastName)
				        .WithAlias(() => dto.lastName)
				        .Select(() => u.FirstName)
				        .WithAlias(() => dto.firstName)
				        .Select(() => u.Id)
				        .WithAlias(() => dto.userId)
				        .Select(() => u2.LastName)
				        .WithAlias(() => dto.createdByLastName)
				        .Select(() => u2.FirstName)
				        .WithAlias(() => dto.createdByName)
				        .Select(() => smi.CreatedBy.Id)
				        .WithAlias(() => dto.createdBy)
				        .Select(() => smi.DateModified)
				        .WithAlias(() => dto.dateModified)
				        .Select(() => smi.Id)
				        .WithAlias(() => dto.subModuleItemId)
				        .Select(() => smc.CategoryName)
				        .WithAlias(() => dto.categoryName)
				        .Select(() => smc.Id)
				        .WithAlias(() => dto.subModuleCategoryId))
		        .TransformUsing(Transformers.AliasToBean<CrosswordDTO>());
	        var result = this.Repository.FindAll<CrosswordDTO>(queryOver).ToList();
	        return result;
        }

        /// <summary>
        /// The get by sub module item id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{AppletItem}"/>.
        /// </returns>
        public IFutureValue<AppletItem> GetOneBySMIId(int id)
        {
            QueryOver<AppletItem> queryOver =
                new DefaultQueryOver<AppletItem, int>().GetQueryOver().Where(x => x.SubModuleItem.Id == id).Take(1);
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get shared crosswords by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{CrosswordDTO}"/>.
        /// </returns>
        public IEnumerable<CrosswordDTO> GetSharedCrosswordsByUserId(int userId)
        {
	        var query =
		        new DefaultQueryOver<User, int>().GetQueryOver()
			        .Where(x => x.Id == userId)
					.Select(res=>
					res.Company.Id);
			var id = this.userRepository.FindOne<int>(query);

			AppletItem ai = null;
			SubModuleItem smi = null;
			SubModuleCategory smc = null;
			User u = null;
			User u2 = null;
			CrosswordDTO dto = null;
			var queryOver = new DefaultQueryOver<AppletItem, int>().GetQueryOver(() => ai)
				.JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.InnerJoin).Where(() => smi.IsActive == true && smi.IsShared == true)
				.JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin).Where(() => smc.IsActive == true)
				.JoinQueryOver(() => smc.User, () => u, JoinType.InnerJoin)
				.JoinQueryOver(() => smi.CreatedBy, () => u2, JoinType.InnerJoin)
				.Where(() => u2.Id != userId && u2.Company.Id == id.Value && (int)u2.Status == 1)
				.SelectList(res =>
					res.Select(() => ai.AppletName)
						.WithAlias(() => dto.appletName)
						.Select(() => ai.Id)
						.WithAlias(() => dto.appletItemId)
						.Select(() => u.LastName)
						.WithAlias(() => dto.lastName)
						.Select(() => u.FirstName)
						.WithAlias(() => dto.firstName)
						.Select(() => u.Id)
						.WithAlias(() => dto.userId)
						.Select(() => u2.LastName)
						.WithAlias(() => dto.createdByLastName)
						.Select(() => u2.FirstName)
						.WithAlias(() => dto.createdByName)
						.Select(() => smi.CreatedBy.Id)
						.WithAlias(() => dto.createdBy)
						.Select(() => smi.DateModified)
						.WithAlias(() => dto.dateModified)
						.Select(() => smi.Id)
						.WithAlias(() => dto.subModuleItemId)
						.Select(() => smc.CategoryName)
						.WithAlias(() => dto.categoryName)
						.Select(() => smc.Id)
						.WithAlias(() => dto.subModuleCategoryId))
				.TransformUsing(Transformers.AliasToBean<CrosswordDTO>());
			var result = this.Repository.FindAll<CrosswordDTO>(queryOver).ToList();
			return result;
		}

        #endregion
    }
}