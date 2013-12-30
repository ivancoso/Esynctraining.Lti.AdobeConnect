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
    ///     The Test model.
    /// </summary>
    public class TestModel : BaseModel<Test, int>
    {
        /// <summary>
        /// The file model.
        /// </summary>
        private readonly FileModel fileModel;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestModel"/> class.
        /// </summary>
        /// <param name="fileModel">
        /// The file Model.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public TestModel(FileModel fileModel, IRepository<Test, int> repository)
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
        /// The <see cref="IEnumerable{Test}"/>.
        /// </returns>
        public IEnumerable<Test> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            var queryOver = new DefaultQueryOver<Test, int>().GetQueryOver();
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
        public Dictionary<int, RecentReportDTO> GetTests(List<int> reportsIds)
        {
            RecentReportDTO dto = null;
            return
                this.Repository.FindAll<RecentReportDTO>(
                    new DefaultQueryOver<Test, int>().GetQueryOver()
                                                     .WhereRestrictionOn(x => x.SubModuleItem.Id)
                                                     .IsIn(reportsIds)
                                                     .SelectList(
                                                         list =>
                                                         list.Select(x => x.TestName)
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
        public IFutureValue<Test> GetOneBySMIId(int id)
        {
            QueryOver<Test> queryOver =
                new DefaultQueryOver<Test, int>().GetQueryOver().Where(x => x.SubModuleItem.Id == id).Take(1);
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
        public IEnumerable<TestFromStoredProcedureDTO> GetTestsByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<TestFromStoredProcedureDTO>(
                "getUsersTestsByUserId", new StoreProcedureParam<int>("userId", userId));
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
        public IEnumerable<TestFromStoredProcedureDTO> GetSharedForUserTestsByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<TestFromStoredProcedureDTO>(
                "getSharedForUserTestsByUserId", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get quiz categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleItemDTOFromStoredProcedureDTO}"/>.
        /// </returns>
        public IEnumerable<SMICategoriesFromStoredProcedureDTO> GetTestCategoriesbyUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SMICategoriesFromStoredProcedureDTO>(
                "getTestCategoriesByUserID", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get quiz sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SubModuleItemDTOFromStoredProcedureDTO}"/>.
        /// </returns>
        public IEnumerable<SubModuleItemDTOFromStoredProcedureDTO> GetTestSMItemsByUserId(int userId)
        {
            return this.Repository.StoreProcedureForMany<SubModuleItemDTOFromStoredProcedureDTO>(
                "getTestSubModuleItemsByUserID", new StoreProcedureParam<int>("userId", userId));
        }

        /// <summary>
        /// The get quiz data by quiz id.
        /// </summary>
        /// <param name="testId">
        /// The quiz id.
        /// </param>
        /// <returns>
        /// The <see cref="TestDataDTO"/>.
        /// </returns>
        public TestDataDTO GetTestDataByTestId(int testId)
        {
            var result = new TestDataDTO { testVO = new TestDTO(this.GetOneById(testId).Value) };
            if (result.testVO != null && result.testVO.subModuleItemId.HasValue)
            {
                result.questions = this.GetTestQuestionsBySMIId(result.testVO.subModuleItemId.Value).ToList();
                result.distractors = this.GetTestDistractorsBySMIId(result.testVO.subModuleItemId.Value).ToList();
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
        public IEnumerable<QuestionFromStoredProcedureDTO> GetTestQuestionsBySMIId(int smiId)
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
        public IEnumerable<DistractorFromStoredProcedureDTO> GetTestDistractorsBySMIId(int smiId)
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