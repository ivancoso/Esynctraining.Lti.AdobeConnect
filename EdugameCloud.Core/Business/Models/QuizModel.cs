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
    ///     The Quiz model.
    /// </summary>
    public class QuizModel : BaseModel<Quiz, int>
    {
        /// <summary>
        /// The file model.
        /// </summary>
        private readonly FileModel fileModel;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizModel"/> class.
        /// </summary>
        /// <param name="fileModel">
        /// The file Model.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public QuizModel(FileModel fileModel, IRepository<Quiz, int> repository)
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
        public IEnumerable<Quiz> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            var queryOver = new DefaultQueryOver<Quiz, int>().GetQueryOver();
            var rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            var pagedQuery = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            return this.Repository.FindAll(pagedQuery);
        }

        /// <summary>
        /// The get quiz array.
        /// </summary>
        /// <param name="reportsIds">
        /// The reports ids.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary{Integer, RecentReportDTO}"/>.
        /// </returns>
        public Dictionary<int, RecentReportDTO> GetQuizes(List<int> reportsIds)
        {
            RecentReportDTO dto = null;
            return
                this.Repository.FindAll<RecentReportDTO>(
                    new DefaultQueryOver<Quiz, int>().GetQueryOver()
                                                     .WhereRestrictionOn(x => x.SubModuleItem.Id)
                                                     .IsIn(reportsIds)
                                                     .SelectList(
                                                         list =>
                                                         list.Select(x => x.QuizName)
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
        public IFutureValue<Quiz> GetOneBySMIId(int id)
        {
            QueryOver<Quiz> queryOver =
                new DefaultQueryOver<Quiz, int>().GetQueryOver().Where(x => x.SubModuleItem.Id == id).Take(1);
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get quizzes by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuizFromStoredProcedureDTO}"/>.
        /// </returns>
        public IEnumerable<QuizFromStoredProcedureDTO> GetQuizzesByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<QuizFromStoredProcedureDTO>(
                "getUsersQuizzesByUserId", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get shared for user quizzes by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuizFromStoredProcedureDTO}"/>.
        /// </returns>
        public IEnumerable<QuizFromStoredProcedureDTO> GetSharedForUserQuizzesByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<QuizFromStoredProcedureDTO>(
                "getSharedForUserQuizzesByUserId", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get quiz categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuizCategoriesFromStoredProcedureDTO}"/>.
        /// </returns>
        public IEnumerable<SMICategoriesFromStoredProcedureDTO> GetQuizCategoriesbyUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SMICategoriesFromStoredProcedureDTO>(
                "getQuizCategoriesByUserID", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get quiz data by quiz id.
        /// </summary>
        /// <param name="quizId">
        /// The quiz id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDataDTO"/>.
        /// </returns>
        public QuizDataDTO getQuizDataByQuizID(int quizId)
        {
            var result = new QuizDataDTO { quizVO = new QuizDTO(this.GetOneById(quizId).Value) };
            if (result.quizVO != null && result.quizVO.subModuleItemId.HasValue)
            {
                result.questions = this.GetQuizQuestionsBySMIId(result.quizVO.subModuleItemId.Value).ToList();
                result.distractors = this.GetQuizDistractorsBySMIId(result.quizVO.subModuleItemId.Value).ToList();
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
        public IEnumerable<QuestionFromStoredProcedureDTO> GetQuizQuestionsBySMIId(int smiId)
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
        public IEnumerable<DistractorFromStoredProcedureDTO> GetQuizDistractorsBySMIId(int smiId)
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