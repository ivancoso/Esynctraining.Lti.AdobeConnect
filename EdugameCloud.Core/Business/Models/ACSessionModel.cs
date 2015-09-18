namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate.Transform;

    /// <summary>
    ///     The ACSession model.
    /// </summary>
    public class ACSessionModel : BaseModel<ACSession, int>
    {
        /// <summary>
        /// The applet item model.
        /// </summary>
        private readonly AppletItemModel appletItemModel;

        /// <summary>
        /// The test model.
        /// </summary>
        private readonly TestModel testModel;

        /// <summary>
        /// The quiz model.
        /// </summary>
        private readonly QuizModel quizModel;

        /// <summary>
        /// The survey model.
        /// </summary>
        private readonly SurveyModel surveyModel;

        /// <summary>
        /// The social profile model.
        /// </summary>
        private readonly SNProfileModel socialProfileModel;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ACSessionModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="appletItemModel">
        /// The applet Item Model.
        /// </param>
        /// <param name="quizModel">
        /// The quiz Model.
        /// </param>
        /// <param name="surveyModel">
        /// The survey Model.
        /// </param>
        /// <param name="socialProfileModel">
        /// The social Profile Model.
        /// </param>
        /// <param name="testModel">
        /// The test Model.
        /// </param>
        public ACSessionModel(
            IRepository<ACSession, int> repository,
            AppletItemModel appletItemModel,
            QuizModel quizModel,
            SurveyModel surveyModel,
            SNProfileModel socialProfileModel,
            TestModel testModel)
            : base(repository)
        {
            this.appletItemModel = appletItemModel;
            this.testModel = testModel;
            this.quizModel = quizModel;
            this.surveyModel = surveyModel;
            this.socialProfileModel = socialProfileModel;
        }

        #endregion

        /// <summary>
        /// The get splash screen reports paged.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="totalCount">
        /// The total count.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ReportDTO}"/>.
        /// </returns>
        // ReSharper disable ImplicitlyCapturedClosure
        public IEnumerable<ReportDTO> GetSplashScreenReportsPaged(int userId, int pageIndex, int pageSize, out int totalCount)
        {
            ReportFromStoredProcedureDTO dto = null;
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            var queryOver = new DefaultQueryOver<ACSession, int>().GetQueryOver()
                .Where(x => x.User.Id == userId).And(x => x.Status == ACSessionStatusEnum.Played)
                .OrderBy(x => x.DateCreated).Desc
                .JoinAlias(x => x.SubModuleItem, () => smi)
                .JoinAlias(() => smi.SubModuleCategory, () => smc)
                .SelectList(list =>
                    list.Select(x => x.DateCreated)
                        .WithAlias(() => dto.dateCreated)
                        .Select(x => x.Id)
                        .WithAlias(() => dto.acSessionId)
                        .Select(x => smi.Id)
                        .WithAlias(() => dto.subModuleItemId)
                        .Select(x => smc.SubModule.Id)
                        .WithAlias(() => dto.type))
                .TransformUsing(Transformers.AliasToBean<ReportFromStoredProcedureDTO>());

            var rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            var pagedQuery = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            var reports =
                this.Repository.FindAll<ReportFromStoredProcedureDTO>(pagedQuery)
                    .ToList()
                    .Select(x => new ReportDTO(x))
                    .ToList();
            List<int> reportsIds = reports.Select(x => x.subModuleItemId).ToList();

            Dictionary<int, RecentReportDTO> crosswords = this.appletItemModel.GetCrosswords(reportsIds);
            Dictionary<int, RecentReportDTO> quizes = this.quizModel.GetQuizes(reportsIds);
            Dictionary<int, RecentReportDTO> surveys = this.surveyModel.GetSurveys(reportsIds);
            Dictionary<int, RecentReportDTO> tests = this.testModel.GetTests(reportsIds);
            Dictionary<int, RecentReportDTO> socialProfiles = this.socialProfileModel.GetSNProfiles(reportsIds);
            return this.MergeData(reports, new List<Dictionary<int, RecentReportDTO>> { crosswords, quizes, surveys, socialProfiles, tests });
        }
        // ReSharper restore ImplicitlyCapturedClosure

        public IEnumerable<SNSessionFromStoredProcedureDTO> GetSNSessionsByUserId(int userId)
        {
            return
                this.Repository.StoreProcedureForMany<SNSessionFromStoredProcedureDTO>(
                    "getSNSessionsByUserId",
                    new StoreProcedureParam<int>("userId", userId)).ToList();
        }

        /// <summary>
        /// The get quiz sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuizSessionDTO}"/>.
        /// </returns>
        public IEnumerable<QuizSessionFromStoredProcedureDTO> GetQuizSessionsByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<QuizSessionFromStoredProcedureDTO>(
                "getQuizSessionsByUserId",
                new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get quiz sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuizSessionDTO}"/>.
        /// </returns>
        public IEnumerable<TestSessionDTO> GetTestSessionsByUserId(int userId)
        {
            return
                this.Repository.StoreProcedureForMany<TestSessionFromStoredProcedureDTO>(
                    "getTestSessionsByUserId",
                    new StoreProcedureParam<int>("userId", userId)).ToList().Select(x => new TestSessionDTO(x));
        }

        /// <summary>
        /// The get quiz sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuizSessionDTO}"/>.
        /// </returns>
        public IEnumerable<SurveySessionFromStoredProcedureDTO> GetSurveySessionsByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SurveySessionFromStoredProcedureDTO>("getSurveySessionsByUserId",
                new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The merge data.
        /// </summary>
        /// <param name="reports">
        /// The reports.
        /// </param>
        /// <param name="names">
        /// The names.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{RecentReportDTO}"/>.
        /// </returns>
        private IEnumerable<ReportDTO> MergeData(List<ReportDTO> reports, List<Dictionary<int, RecentReportDTO>> names)
        {
            foreach (ReportDTO dto in reports)
            {
                foreach (var nameStorage in names)
                {
                    if (nameStorage.ContainsKey(dto.subModuleItemId))
                    {
                        dto.name = nameStorage[dto.subModuleItemId].name;
                    }
                }
            }

            return reports;
        }

        /// <summary>
        /// The get all by smi id.
        /// </summary>
        /// <param name="smiId">
        /// The smi id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ACSession}"/>.
        /// </returns>
        public IEnumerable<ACSession> GetAllBySmiId(int smiId)
        {
            var query = new DefaultQueryOver<ACSession, int>().GetQueryOver().Where(x => x.SubModuleItem.Id == smiId);
            return this.Repository.FindAll<ACSession>(query);
        }
    }
}