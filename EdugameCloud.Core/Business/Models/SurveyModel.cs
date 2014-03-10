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
    ///     The Survey model.
    /// </summary>
    public class SurveyModel : BaseModel<Survey, int>
    {
        /// <summary>
        /// The file model.
        /// </summary>
        private readonly FileModel fileModel;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyModel"/> class. 
        /// </summary>
        /// <param name="fileModel">
        /// The file Model.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SurveyModel(FileModel fileModel, IRepository<Survey, int> repository)
            : base(repository)
        {
            this.fileModel = fileModel;
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
        /// The <see cref="IEnumerable{Quiz}"/>.
        /// </returns>
        public IEnumerable<Survey> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            var queryOver = new DefaultQueryOver<Survey, int>().GetQueryOver();
            var rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            var pagedQuery = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            return this.Repository.FindAll(pagedQuery);
        }

        /// <summary>
        /// The get survey array.
        /// </summary>
        /// <param name="reportsIds">
        /// The reports ids.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary{Integer, RecentReportDTO}"/>.
        /// </returns>
        public Dictionary<int, RecentReportDTO> GetSurveys(List<int> reportsIds)
        {
            RecentReportDTO dto = null;
            return
                this.Repository.FindAll<RecentReportDTO>(
                    new DefaultQueryOver<Survey, int>().GetQueryOver()
                                                     .WhereRestrictionOn(x => x.SubModuleItem.Id)
                                                     .IsIn(reportsIds)
                                                     .SelectList(
                                                         list =>
                                                         list.Select(x => x.SurveyName)
                                                             .WithAlias(() => dto.name)
                                                             .Select(x => x.SubModuleItem.Id)
                                                             .WithAlias(() => dto.subModuleItemId))
                                                     .TransformUsing(Transformers.AliasToBean<RecentReportDTO>()))
                    .ToDictionary(x => x.subModuleItemId, x => x);
        }

        /// <summary>
        /// The get by sub module item id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{T}"/>.
        /// </returns>
        public IFutureValue<Survey> GetOneBySMIId(int id)
        {
            QueryOver<Survey> queryOver =
                new DefaultQueryOver<Survey, int>().GetQueryOver().Where(x => x.SubModuleItem.Id == id).Take(1);
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get surveys by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SurveyFromStoredProcedureDTO}"/>.
        /// </returns>
        public IEnumerable<SurveyFromStoredProcedureDTO> GetSurveysByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SurveyFromStoredProcedureDTO>(
                "getUsersSurveysByUserId", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get shared for user surveys by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SurveyFromStoredProcedureDTO}"/>.
        /// </returns>
        public IEnumerable<SurveyFromStoredProcedureDTO> GetSharedForUserSurveysByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SurveyFromStoredProcedureDTO>(
                "getSharedForUserSurveysByUserId", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get survey categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SurveyCategoriesFromStoredProcedureDTO}"/>.
        /// </returns>
        public IEnumerable<SMICategoriesFromStoredProcedureDTO> GetSurveyCategoriesbyUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SMICategoriesFromStoredProcedureDTO>(
                "getSurveyCategoriesByUserId", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get survey data by survey id.
        /// </summary>
        /// <param name="surveyId">
        /// The quiz id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDataDTO"/>.
        /// </returns>
        public SurveyDataDTO GetSurveyDataBySurveyId(int surveyId)
        {
            var result = new SurveyDataDTO { surveyVO = new SurveyDTO(this.GetOneById(surveyId).Value) };
            if (result.surveyVO != null && result.surveyVO.subModuleItemId.HasValue)
            {
                result.questions = this.GetSurveyQuestionsBySMIId(result.surveyVO.subModuleItemId.Value).ToList();
                result.distractors = this.GetSurveyDistractorsBySMIId(result.surveyVO.subModuleItemId.Value).ToList();
            }

            return result;
        }

        /// <summary>
        /// The get quiz questions by sub module item id.
        /// </summary>
        /// <param name="smiId">
        /// The sub module item id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuestionFromStoredProcedureDTO}"/>.
        /// </returns>
        public IEnumerable<QuestionFromStoredProcedureDTO> GetSurveyQuestionsBySMIId(int smiId)
        {
            var questions = this.Repository.StoreProcedureForMany<QuestionFromStoredProcedureDTO>("getSMIQuestionsBySMIId", new StoreProcedureParam<int>("subModuleItemId", smiId));
            var imageIds = questions.Where(x => x.imageId.HasValue).Select(x => x.imageId.Value).ToList();
            if (imageIds.Any())
            {
                var files = this.fileModel.GetAllByIds(imageIds).ToList();
                foreach (var dto in questions.Where(x => x.imageId.HasValue))
                {
                    dto.imageVO = new FileDTO(files.FirstOrDefault(x => x.Id == dto.imageId.Value));
                }
            }

            return questions;
        }

        /// <summary>
        /// The get quiz distractors by sub module item id.
        /// </summary>
        /// <param name="smiId">
        /// The sub module item id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuestionFromStoredProcedureDTO}"/>.
        /// </returns>
        public IEnumerable<DistractorFromStoredProcedureDTO> GetSurveyDistractorsBySMIId(int smiId)
        {
            var distructors = this.Repository.StoreProcedureForMany<DistractorFromStoredProcedureDTO>("getSMIDistractorsBySMIId", new StoreProcedureParam<int>("subModuleItemId", smiId)).ToList();
            var imageIds = distructors.Where(x => x.imageId.HasValue).Select(x => x.imageId.Value).ToList();
            if (imageIds.Any())
            {
                var files = this.fileModel.GetAllByIds(imageIds).ToList();
                foreach (var dto in distructors.Where(x => x.imageId.HasValue))
                {
                    dto.imageVO = new FileDTO(files.FirstOrDefault(x => x.Id == dto.imageId.Value));
                }
            }

            return distructors;
        }

        #endregion
    }
}