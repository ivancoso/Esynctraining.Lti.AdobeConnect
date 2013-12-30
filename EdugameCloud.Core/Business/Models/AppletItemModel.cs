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
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppletItemModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public AppletItemModel(IRepository<AppletItem, int> repository)
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
            return this.Repository.StoreProcedureForMany<CrosswordDTO>(
                "getUsersCrosswordsByUserId", new StoreProcedureParam<int>("userId", userId));
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
            return this.Repository.StoreProcedureForMany<CrosswordDTO>("getSharedForUserCrosswordsByUserId", new StoreProcedureParam<int>("userId", userId));
        }

        #endregion
    }
}