﻿namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Extensions;

    using NHibernate.Criterion;
    using NHibernate.SqlCommand;
    using NHibernate.Transform;

    /// <summary>
    ///     The SubModuleItem model.
    /// </summary>
    public class SubModuleItemModel : BaseModel<SubModuleItem, int>
    {
        #region Fields

        /// <summary>
        /// The theme repository.
        /// </summary>
        private readonly IRepository<SubModuleItemTheme, int> themeRepository;

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

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleItemModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="themeRepository">
        /// The theme Repository.
        /// </param>
        /// <param name="appletItemModel">
        /// The applet item model.
        /// </param>
        /// <param name="quizModel">
        /// The quiz model.
        /// </param>
        /// <param name="surveyModel">
        /// The survey Model.
        /// </param>
        /// <param name="socialProfileModel">
        /// The social network Profile Model.
        /// </param>
        /// <param name="testModel">
        /// The test model.
        /// </param>
        public SubModuleItemModel(
            IRepository<SubModuleItem, int> repository,
            IRepository<SubModuleItemTheme, int> themeRepository, 
            AppletItemModel appletItemModel,
            QuizModel quizModel,
            SurveyModel surveyModel,
            SNProfileModel socialProfileModel,
            TestModel testModel)
            : base(repository)
        {
            this.themeRepository = themeRepository;
            this.appletItemModel = appletItemModel;
            this.testModel = testModel;
            this.quizModel = quizModel;
            this.surveyModel = surveyModel;
            this.socialProfileModel = socialProfileModel;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleItem}"/>.
        /// </returns>
        public override IEnumerable<SubModuleItem> GetAll()
        {
            var pagedQuery = new DefaultQueryOver<SubModuleItem, int>().GetQueryOver().Fetch(x => x.SubModuleCategory).Eager;
            return this.Repository.FindAll(pagedQuery);
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
        /// The <see cref="IEnumerable{SubModuleItem}"/>.
        /// </returns>
        public IEnumerable<SubModuleItem> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            var queryOver = new DefaultQueryOver<SubModuleItem, int>().GetQueryOver();
            var rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            var pagedQuery = queryOver.Fetch(x => x.SubModuleCategory).Eager.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            return this.Repository.FindAll(pagedQuery);
        }

        /// <summary>
        /// The get applet sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleItem}"/>.
        /// </returns>
        public IEnumerable<SubModuleItemDTO> GetQuizSMItemsByUserId(int userId)
        {
            SubModuleItemDTO dto = null;
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            Quiz quiz = null;
            SubModuleItemTheme theme = null;
            var queryOver =
                new DefaultQueryOver<SubModuleItem, int>().GetQueryOver(() => smi)
                    .JoinQueryOver(x => x.SubModuleCategory, () => smc)
                    .JoinQueryOver(() => smi.Quizes, () => quiz)
                    .JoinQueryOver(() => smi.Themes, () => theme, JoinType.LeftOuterJoin)
                    .Where(() => smi.CreatedBy != null && smi.CreatedBy.Id == userId && smc.User != null && smc.User.Id == userId)
                    .SelectList(res =>
                        res.Select(() => smi.CreatedBy.Id)
                            .WithAlias(() => dto.createdBy)
                            .Select(() => smi.Id)
                            .WithAlias(() => dto.subModuleItemId)
                            .Select(() => smc.SubModule.Id)
                            .WithAlias(() => dto.subModuleId)
                            .Select(() => smi.SubModuleCategory.Id)
                            .WithAlias(() => dto.subModuleCategoryId)
                            .Select(() => smi.IsShared)
                            .WithAlias(() => dto.isShared)
                            .Select(() => smi.ModifiedBy.Id)
                            .WithAlias(() => dto.modifiedBy)
                            .Select(() => smi.DateCreated)
                            .WithAlias(() => dto.dateCreated)
                            .Select(() => smi.DateModified)
                            .WithAlias(() => dto.dateModified)
                            .Select(() => smi.IsActive)
                            .WithAlias(() => dto.isActive)
                            .Select(() => theme.Id)
                            .WithAlias(() => dto.themeId))
                            .TransformUsing(Transformers.AliasToBean<SubModuleItemDTO>());
            var result = this.Repository.FindAll<SubModuleItemDTO>(queryOver).ToList();
            var themeIds = result.Where(x => x.themeId.HasValue).Select(x => x.themeId.Value).ToList();
            var themeQuery = new DefaultQueryOver<SubModuleItemTheme, int>().GetQueryOver().WhereRestrictionOn(x => x.Id).IsIn(themeIds);
            var themes = this.themeRepository.FindAll(themeQuery).ToList();
            result.ForEach(x => x.themeVO = x.themeId.HasValue ? themes.FirstOrDefault(t => t.Id == x.themeId).Return(tt => new SubModuleItemThemeDTO(tt), null) : null);
            return result;
        }

        /// <summary>
        /// The get applet sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleItem}"/>.
        /// </returns>
        public IEnumerable<SubModuleItemDTO> GetAppletSubModuleItemsByUserId(int userId)
        {
            SubModuleItemDTO dto = null;
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            AppletItem appletItem = null;
            SubModuleItemTheme theme = null;
            var queryOver =
                new DefaultQueryOver<SubModuleItem, int>().GetQueryOver(() => smi)
                    .JoinQueryOver(x => x.SubModuleCategory, () => smc)
                    .JoinQueryOver(() => smi.AppletItems, () => appletItem)
                    .JoinQueryOver(() => smi.Themes, () => theme, JoinType.LeftOuterJoin)
                    .Where(() => smi.CreatedBy != null && smi.CreatedBy.Id == userId && smc.User != null && smc.User.Id == userId)
                    .SelectList(res =>
                        res.Select(() => smi.CreatedBy.Id)
                            .WithAlias(() => dto.createdBy)
                            .Select(() => smi.Id)
                            .WithAlias(() => dto.subModuleItemId)
                            .Select(() => smc.SubModule.Id)
                            .WithAlias(() => dto.subModuleId)
                            .Select(() => smi.SubModuleCategory.Id)
                            .WithAlias(() => dto.subModuleCategoryId)
                            .Select(() => smi.IsShared)
                            .WithAlias(() => dto.isShared)
                            .Select(() => smi.ModifiedBy.Id)
                            .WithAlias(() => dto.modifiedBy)
                            .Select(() => smi.DateCreated)
                            .WithAlias(() => dto.dateCreated)
                            .Select(() => smi.DateModified)
                            .WithAlias(() => dto.dateModified)
                            .Select(() => smi.IsActive)
                            .WithAlias(() => dto.isActive)
                            .Select(() => theme.Id)
                            .WithAlias(() => dto.themeId))
                            .TransformUsing(Transformers.AliasToBean<SubModuleItemDTO>());
            var result = this.Repository.FindAll<SubModuleItemDTO>(queryOver).ToList();
            var themeIds = result.Where(x => x.themeId.HasValue).Select(x => x.themeId.Value).ToList();
            var themeQuery = new DefaultQueryOver<SubModuleItemTheme, int>().GetQueryOver().WhereRestrictionOn(x => x.Id).IsIn(themeIds);
            var themes = this.themeRepository.FindAll(themeQuery).ToList();
            result.ForEach(x => x.themeVO = x.themeId.HasValue ? themes.FirstOrDefault(t => t.Id == x.themeId).Return(tt => new SubModuleItemThemeDTO(tt), null) : null);
            return result;
        }

        /// <summary>
        /// The get recent splash screen reports paged.
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
        /// The <see cref="IEnumerable{RecentReportDTO}"/>.
        /// </returns>
        // ReSharper disable ImplicitlyCapturedClosure
        public IEnumerable<RecentReportDTO> GetRecentSplashScreenReportsPaged(
            int userId, int pageIndex, int pageSize, out int totalCount)
        {
            RecentReportDTO dto = null;
            SubModuleCategory category = null;
            QueryOver<SubModuleItem, SubModuleItem> queryOver =
                new DefaultQueryOver<SubModuleItem, int>().GetQueryOver()
                                                          .Where(x => x.CreatedBy != null && x.CreatedBy.Id == userId)
                                                          .JoinAlias(x => x.SubModuleCategory, () => category)
                                                          .OrderBy(x => x.DateModified)
                                                          .Desc.SelectList(
                                                              list =>
                                                              list.Select(x => x.DateModified)
                                                                  .WithAlias(() => dto.dateModified)
                                                                  .Select(() => category.Id)
                                                                  .WithAlias(() => dto.subModuleCategoryId)
                                                                  .Select(x => x.Id)
                                                                  .WithAlias(() => dto.subModuleItemId)
                                                                  .Select(() => category.SubModule.Id)
                                                                  .WithAlias(() => dto.type))
                                                          .TransformUsing(Transformers.AliasToBean<RecentReportDTO>());
            QueryOver<SubModuleItem, SubModuleItem> rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            QueryOver<SubModuleItem> pagedQuery = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            var reports = this.Repository.FindAll<RecentReportDTO>(pagedQuery).ToList();
            List<int> reportsIds = reports.Select(x => x.subModuleItemId).ToList();
            Dictionary<int, RecentReportDTO> crosswords = this.appletItemModel.GetCrosswords(reportsIds);
            Dictionary<int, RecentReportDTO> quizes = this.quizModel.GetQuizes(reportsIds);
            Dictionary<int, RecentReportDTO> surveys = this.surveyModel.GetSurveys(reportsIds);
            Dictionary<int, RecentReportDTO> tests = this.testModel.GetTests(reportsIds);
            Dictionary<int, RecentReportDTO> socialProfiles = this.socialProfileModel.GetSNProfiles(reportsIds);
            return this.MergeData(reports, new List<Dictionary<int, RecentReportDTO>> { crosswords, quizes, surveys, socialProfiles, tests });
        }
        // ReSharper restore ImplicitlyCapturedClosure

        /// <summary>
        /// The get profile sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleItemDTO}"/>.
        /// </returns>
        public IEnumerable<SubModuleItemDTO> GetSNProfileSubModuleItemsByUserId(int userId)
        {
            SubModuleItemDTO dto = null;
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            SNProfile survey = null;
            SubModuleItemTheme theme = null;
            var queryOver =
                new DefaultQueryOver<SubModuleItem, int>().GetQueryOver(() => smi)
                    .JoinQueryOver(x => x.SubModuleCategory, () => smc)
                    .JoinQueryOver(() => smi.SNProfiles, () => survey)
                    .JoinQueryOver(() => smi.Themes, () => theme, JoinType.LeftOuterJoin)
                    .Where(() => smi.CreatedBy != null && smi.CreatedBy.Id == userId && smc.User != null && smc.User.Id == userId)
                    .SelectList(res =>
                        res.Select(() => smi.CreatedBy.Id)
                            .WithAlias(() => dto.createdBy)
                            .Select(() => smi.Id)
                            .WithAlias(() => dto.subModuleItemId)
                            .Select(() => smc.SubModule.Id)
                            .WithAlias(() => dto.subModuleId)
                            .Select(() => smi.SubModuleCategory.Id)
                            .WithAlias(() => dto.subModuleCategoryId)
                            .Select(() => smi.IsShared)
                            .WithAlias(() => dto.isShared)
                            .Select(() => smi.ModifiedBy.Id)
                            .WithAlias(() => dto.modifiedBy)
                            .Select(() => smi.DateCreated)
                            .WithAlias(() => dto.dateCreated)
                            .Select(() => smi.DateModified)
                            .WithAlias(() => dto.dateModified)
                            .Select(() => smi.IsActive)
                            .WithAlias(() => dto.isActive)
                            .Select(() => theme.Id)
                            .WithAlias(() => dto.themeId))
                            .TransformUsing(Transformers.AliasToBean<SubModuleItemDTO>());
            var result = this.Repository.FindAll<SubModuleItemDTO>(queryOver).ToList();
            var themeIds = result.Where(x => x.themeId.HasValue).Select(x => x.themeId.Value).ToList();
            var themeQuery = new DefaultQueryOver<SubModuleItemTheme, int>().GetQueryOver().WhereRestrictionOn(x => x.Id).IsIn(themeIds);
            var themes = this.themeRepository.FindAll(themeQuery).ToList();
            result.ForEach(x => x.themeVO = x.themeId.HasValue ? themes.FirstOrDefault(t => t.Id == x.themeId).Return(tt => new SubModuleItemThemeDTO(tt), null) : null);
            return result;
        }

        /// <summary>
        /// The get survey sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleItemDTO}"/>.
        /// </returns>
        public IEnumerable<SubModuleItemDTO> GetSurveySubModuleItemsByUserId(int userId)
        {
            SubModuleItemDTO dto = null;
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            Survey survey = null;
            SubModuleItemTheme theme = null;
            var queryOver =
                new DefaultQueryOver<SubModuleItem, int>().GetQueryOver(() => smi)
                    .JoinQueryOver(x => x.SubModuleCategory, () => smc)
                    .JoinQueryOver(() => smi.Surveys, () => survey)
                    .JoinQueryOver(() => smi.Themes, () => theme, JoinType.LeftOuterJoin)
                    .Where(() => smi.CreatedBy != null && smi.CreatedBy.Id == userId && smc.User != null && smc.User.Id == userId)
                    .SelectList(res =>
                        res.Select(() => smi.CreatedBy.Id)
                            .WithAlias(() => dto.createdBy)
                            .Select(() => smi.Id)
                            .WithAlias(() => dto.subModuleItemId)
                            .Select(() => smc.SubModule.Id)
                            .WithAlias(() => dto.subModuleId)
                            .Select(() => smi.SubModuleCategory.Id)
                            .WithAlias(() => dto.subModuleCategoryId)
                            .Select(() => smi.IsShared)
                            .WithAlias(() => dto.isShared)
                            .Select(() => smi.ModifiedBy.Id)
                            .WithAlias(() => dto.modifiedBy)
                            .Select(() => smi.DateCreated)
                            .WithAlias(() => dto.dateCreated)
                            .Select(() => smi.DateModified)
                            .WithAlias(() => dto.dateModified)
                            .Select(() => smi.IsActive)
                            .WithAlias(() => dto.isActive)
                            .Select(() => theme.Id)
                            .WithAlias(() => dto.themeId))
                            .TransformUsing(Transformers.AliasToBean<SubModuleItemDTO>());
            var result = this.Repository.FindAll<SubModuleItemDTO>(queryOver).ToList();
            var themeIds = result.Where(x => x.themeId.HasValue).Select(x => x.themeId.Value).ToList();
            var themeQuery = new DefaultQueryOver<SubModuleItemTheme, int>().GetQueryOver().WhereRestrictionOn(x => x.Id).IsIn(themeIds);
            var themes = this.themeRepository.FindAll(themeQuery).ToList();
            result.ForEach(x => x.themeVO = x.themeId.HasValue ? themes.FirstOrDefault(t => t.Id == x.themeId).Return(tt => new SubModuleItemThemeDTO(tt), null) : null);
            return result;
        }

        /// <summary>
        /// The get test sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleItemDTO}"/>.
        /// </returns>
        public IEnumerable<SubModuleItemDTO> GetTestSubModuleItemsByUserId(int userId)
        {
            SubModuleItemDTO dto = null;
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            Test test = null;
            SubModuleItemTheme theme = null;
            var queryOver =
                new DefaultQueryOver<SubModuleItem, int>().GetQueryOver(() => smi)
                    .JoinQueryOver(x => x.SubModuleCategory, () => smc)
                    .JoinQueryOver(() => smi.Tests, () => test)
                    .JoinQueryOver(() => smi.Themes, () => theme, JoinType.LeftOuterJoin)
                    .Where(() => smi.CreatedBy != null && smi.CreatedBy.Id == userId && smc.User != null && smc.User.Id == userId)
                    .SelectList(res =>
                        res.Select(() => smi.CreatedBy.Id)
                            .WithAlias(() => dto.createdBy)
                            .Select(() => smi.Id)
                            .WithAlias(() => dto.subModuleItemId)
                            .Select(() => smc.SubModule.Id)
                            .WithAlias(() => dto.subModuleId)
                            .Select(() => smi.SubModuleCategory.Id)
                            .WithAlias(() => dto.subModuleCategoryId)
                            .Select(() => smi.IsShared)
                            .WithAlias(() => dto.isShared)
                            .Select(() => smi.ModifiedBy.Id)
                            .WithAlias(() => dto.modifiedBy)
                            .Select(() => smi.DateCreated)
                            .WithAlias(() => dto.dateCreated)
                            .Select(() => smi.DateModified)
                            .WithAlias(() => dto.dateModified)
                            .Select(() => smi.IsActive)
                            .WithAlias(() => dto.isActive)
                            .Select(() => theme.Id)
                            .WithAlias(() => dto.themeId))
                            .TransformUsing(Transformers.AliasToBean<SubModuleItemDTO>());
            var result = this.Repository.FindAll<SubModuleItemDTO>(queryOver).ToList();
            var themeIds = result.Where(x => x.themeId.HasValue).Select(x => x.themeId.Value).ToList();
            var themeQuery = new DefaultQueryOver<SubModuleItemTheme, int>().GetQueryOver().WhereRestrictionOn(x => x.Id).IsIn(themeIds);
            var themes = this.themeRepository.FindAll(themeQuery).ToList();
            result.ForEach(x => x.themeVO = x.themeId.HasValue ? themes.FirstOrDefault(t => t.Id == x.themeId).Return(tt => new SubModuleItemThemeDTO(tt), null) : null);
            return result;
        }

        /// <summary>
        /// The get quiz sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleItemDTO}"/>.
        /// </returns>
        public IEnumerable<SubModuleItemDTO> GetQuizSubModuleItemsByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SubModuleItemDTO>("getQuizSubModuleItemsByUserID", new StoreProcedureParam<int>("userId", userId));
        }

        #endregion

        #region Methods

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
        private IEnumerable<RecentReportDTO> MergeData(List<RecentReportDTO> reports, List<Dictionary<int, RecentReportDTO>> names)
        {
            foreach (RecentReportDTO dto in reports)
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

        #endregion
    }
}