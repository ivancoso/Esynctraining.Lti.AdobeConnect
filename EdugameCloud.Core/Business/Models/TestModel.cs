using Esynctraining.Core.Utils;
using NHibernate.SqlCommand;

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
		#region Fields

		/// <summary>
		/// The user repository.
		/// </summary>
		private readonly IRepository<User, int> userRepository;

		/// <summary>
		/// The file model.
		/// </summary>
		private readonly FileModel fileModel;

		/// <summary>
		/// The distractor model.
		/// </summary>
		private readonly DistractorModel distractorModel;

		#endregion

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
		public TestModel(FileModel fileModel, DistractorModel distractorModel, IRepository<User, int> userRepository, IRepository<Test, int> repository)
            : base(repository)
        {
            this.fileModel = fileModel;
	        this.distractorModel = distractorModel;
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
			TestFromStoredProcedureDTO dto = null;
	        Test t = null;
			SubModuleItem smi = null;
			SubModuleCategory smc = null;
	        User u = null;
			User u2 = null;

			var queryOver = new DefaultQueryOver<Test, int>().GetQueryOver(()=>t)
				.JoinQueryOver(x => x.SubModuleItem,() => smi, JoinType.InnerJoin).Where(()=>smi.IsActive == true)
				.JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin).Where(()=>smc.IsActive == true)
				.JoinQueryOver(() => smc.User, () => u, JoinType.InnerJoin)
				.JoinQueryOver(() => smi.CreatedBy, () => u2, JoinType.LeftOuterJoin)
				.Where(() => u2.Id == userId)
				.SelectList(res=>
					res.Select(() => t.ScoreFormat)
							.WithAlias(() => dto.scoreFormat)
							.Select(() => t.InstructionDescription )
							.WithAlias(() => dto.instructionDescription)
							.Select(() => t.InstructionTitle )
							.WithAlias(() => dto.instructionTitle)
							.Select(() => t.TimeLimit)
							.WithAlias(() => dto.timeLimit)
							.Select(() => t.PassingScore)
							.WithAlias(() => dto.passingScore)
							.Select(() => t.Description)
							.WithAlias(() => dto.description)
							.Select(() => t.TestName)
							.WithAlias(() => dto.testName)
							.Select(() => t.Id)
							.WithAlias(() => dto.testId)
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
							.WithAlias(() => dto.subModuleCategoryId)
							)
							 .TransformUsing(Transformers.AliasToBean<TestFromStoredProcedureDTO>());

			var result = this.Repository.FindAll<TestFromStoredProcedureDTO>(queryOver).ToList();
			return result;
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
			var query =
				new DefaultQueryOver<User, int>().GetQueryOver()
					.Where(x => x.Id == userId)
					.Select(res =>
					res.Company.Id);
			var id = this.userRepository.FindOne<int>(query);

			SubModuleItem smi = null;
			SubModuleCategory smc = null;
			Test t = null;
	        User u = null;
	        User u2 = null;
			TestFromStoredProcedureDTO dto = null;
			var qieryOver = new DefaultQueryOver<Test, int>().GetQueryOver(() => t)
				.JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.InnerJoin).Where(()=>smi.IsActive == true && smi.IsShared == true)
				.JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin).Where(()=>smc.IsActive == true)
				.JoinQueryOver(()=>smc.User, ()=>u, JoinType.InnerJoin)
				.JoinQueryOver(()=>smi.CreatedBy, ()=>u2).Where(()=>(int)u2.Status==1)
				.Where(() =>  u2.Id!=userId && u2.Company.Id == id.Value)
				.SelectList(res =>
					res.Select(() => t.Description)
					.WithAlias(() => dto.description)
					.Select(() => t.TestName)
					.WithAlias(() => dto.testName)
					.Select(() => t.Id)
					.WithAlias(() => dto.testId)
					.Select(() => u.LastName)
					.WithAlias(() => dto.lastName)
					.Select(() => u.FirstName)
					.WithAlias(() => dto.firstName)
					.Select(() => u.Id)
					.WithAlias(() => dto.userId)
					.Select(() => u2.LastName)
					.WithAlias(() => dto.createdByLastName)
					.Select(() =>u2.FirstName)
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
					.WithAlias(() => dto.subModuleCategoryId)
				)
				.TransformUsing(Transformers.AliasToBean<TestFromStoredProcedureDTO>());
			var result = Repository.FindAll<TestFromStoredProcedureDTO>(qieryOver).ToList();
			return result;
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
			SubModuleItem smi = null;
			SubModuleCategory smc = null;
			Test t = null;
			SMICategoriesFromStoredProcedureDTO dto = null;
			var qieryOver = new DefaultQueryOver<Test, int>().GetQueryOver(() => t)
				.JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.RightOuterJoin)
				.JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin)
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
				.TransformUsing(Transformers.AliasToBean<SMICategoriesFromStoredProcedureDTO>());
			var result = Repository.FindAll<SMICategoriesFromStoredProcedureDTO>(qieryOver).ToList();
	        return result;
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
			var distructors = distractorModel.GetTestDistractorsWithoutImagesBySMIId(smiId).ToList();
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