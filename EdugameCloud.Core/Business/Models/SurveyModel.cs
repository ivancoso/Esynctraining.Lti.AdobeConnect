using NHibernate.SqlCommand;

namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
	    /// Initializes a new instance of the <see cref="SurveyModel"/> class. 
	    /// </summary>
	    /// <param name="fileModel">
	    /// The file Model.
	    /// </param>
	    /// <param name="userRepository">
	    /// The user repository.
	    /// </param>
	    /// <param name="repository">
	    /// The repository.
	    /// </param>
		[SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1027:TabsMustNotBeUsed", Justification = "Reviewed. Suppression is OK here.")]
		public SurveyModel(FileModel fileModel, DistractorModel distractorModel, IRepository<User, int> userRepository, IRepository<Survey, int> repository)
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
        /// The get one by moodle id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsSurveyId">
        /// The lms quiz id.
        /// </param>
        /// <param name="companyLmsId">
        /// The company lms id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Quiz}"/>.
        /// </returns>
        public IFutureValue<Survey> GetOneByLmsSurveyId(int userId, int lmsSurveyId, int companyLmsId)
        {
            QueryOver<User, User> companyQuery =
               new DefaultQueryOver<User, int>().GetQueryOver()
                   .Where(x => x.Id == userId)
                   .Select(res => res.Company.Id);
            IFutureValue<int> id = this.userRepository.FindOne<int>(companyQuery);

            Survey s = null;
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            User u2 = null;

            var query =
                new DefaultQueryOver<Survey, int>().GetQueryOver(() => s)
                    .WhereRestrictionOn(x => x.LmsSurveyId).IsNotNull
                    .And(x => x.LmsSurveyId == lmsSurveyId)
                    .JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.InnerJoin)
                    .JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin)
                    .Where(() => smc.CompanyLms != null && smc.CompanyLms.Id == companyLmsId)
                    .JoinQueryOver(() => smi.CreatedBy, () => u2, JoinType.InnerJoin)
                    .Where(() => u2.Company.Id == id.Value && (int)u2.Status == 1)
                    .Take(1);
            return this.Repository.FindOne(query);
        }

        /// <summary>
        /// The get surveys by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="showLms">
        /// The show lms
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SurveyFromStoredProcedureDTO}"/>.
        /// </returns>
        public IEnumerable<SurveyFromStoredProcedureDTO> GetSurveysByUserId(int userId, bool showLms)
        {
	        Survey s = null;
	        SubModuleItem smi = null;
	        SubModuleCategory smc = null;
	        User u = null;
	        User u2 = null;
	        SurveyFromStoredProcedureDTO dto = null;
	        var queryOver =new DefaultQueryOver<Survey, int>().GetQueryOver(()=>s)
                .JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.InnerJoin).Where(() => smi.IsActive == true).And(() => s.LmsSurveyId == null || showLms == true)
				.JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin).Where(() => smc.IsActive == true)
				.JoinQueryOver(()=>smc.User, ()=>u,JoinType.InnerJoin)
 				.JoinQueryOver(()=>smi.CreatedBy, ()=>u2, JoinType.LeftOuterJoin)
				.Where(()=>u2.Id == userId)
				.SelectList(res=>
				res.Select(()=>s.SurveyGroupingType.Id)
					.WithAlias(()=>dto.surveyGroupingTypeId)
					.Select(()=>s.Description)
					.WithAlias(()=>dto.description)
					.Select(()=>s.SurveyName)
					.WithAlias(()=>dto.surveyName)
                    .Select(() => s.LmsSurveyId)
                    .WithAlias(() => dto.lmsSurveyId)
					.Select(()=>s.Id)
					.WithAlias(()=>dto.surveyId)
					.Select(()=>u.LastName)
					.WithAlias(()=>dto.lastName)
					.Select(()=>u.FirstName)
					.WithAlias(()=>dto.firstName)
					.Select(()=>u.Id)
					.WithAlias(()=>dto.userId)
					.Select(()=>u2.LastName)
					.WithAlias(()=>dto.createdByLastName)
					.Select(()=>u2.FirstName)
					.WithAlias(()=>dto.createdByName)
					.Select(()=>smi.CreatedBy.Id)
					.WithAlias(()=>dto.createdBy)
					.Select(()=>smi.DateModified)
					.WithAlias(()=>dto.dateModified)
					.Select(()=>smi.Id)
					.WithAlias(()=>dto.subModuleItemId)
					.Select(()=>smc.CategoryName)
					.WithAlias(()=>dto.categoryName)
					.Select(()=>smc.Id)
					.WithAlias(()=>dto.subModuleCategoryId))
					.TransformUsing(Transformers.AliasToBean<SurveyFromStoredProcedureDTO>());
	        var result = this.Repository.FindAll<SurveyFromStoredProcedureDTO>(queryOver).ToList();
	        return result;
        }

        /// <summary>
        /// The get shared for user surveys by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="courseid">
        /// The courseid.
        /// </param>
        /// <param name="companyLmsId">
        /// The company Lms Id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SurveyFromStoredProcedureDTO}"/>.
        /// </returns>
        public IEnumerable<SurveyFromStoredProcedureDTO> GetLmsSurveys(int userId, int courseid, int companyLmsId)
        {
            var query =
                new DefaultQueryOver<User, int>().GetQueryOver()
                    .Where(x => x.Id == userId)
                    .Select(res =>
                    res.Company.Id);
            var id = this.userRepository.FindOne<int>(query);

            Survey s = null;
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            User u = null;
            User u2 = null;
            SurveyFromStoredProcedureDTO dto = null;
            var queryOver = new DefaultQueryOver<Survey, int>().GetQueryOver(() => s)
                .JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.InnerJoin)
                .Where(() => smi.IsActive == true)
                .And(() => (smc.CompanyLms == null && courseid == 0) || (smc.LmsCourseId == courseid && smc.CompanyLms != null && smc.CompanyLms.Id == companyLmsId))
                .JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin).Where(() => smc.IsActive == true)
                .JoinQueryOver(() => smc.User, () => u, JoinType.InnerJoin)
                .JoinQueryOver(() => smi.CreatedBy, () => u2, JoinType.InnerJoin).Where(() => (int)u2.Status == 1)
                .Where(() => u2.Company.Id == id.Value)
                .SelectList(res =>
                res.Select(() => s.SurveyGroupingType.Id)
                    .WithAlias(() => dto.surveyGroupingTypeId)
                    .Select(() => s.Description)
                    .WithAlias(() => dto.description)
                    .Select(() => s.SurveyName)
                    .WithAlias(() => dto.surveyName)
                    .Select(() => s.LmsSurveyId)
                    .WithAlias(() => dto.lmsSurveyId)
                    .Select(() => s.Id)
                    .WithAlias(() => dto.surveyId)
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
                    .TransformUsing(Transformers.AliasToBean<SurveyFromStoredProcedureDTO>());
            var result = this.Repository.FindAll<SurveyFromStoredProcedureDTO>(queryOver).ToList();
            return result;
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
			var query =
				new DefaultQueryOver<User, int>().GetQueryOver()
					.Where(x => x.Id == userId)
					.Select(res =>
					res.Company.Id);
			var id = this.userRepository.FindOne<int>(query);

			Survey s = null;
			SubModuleItem smi = null;
			SubModuleCategory smc = null;
			User u = null;
			User u2 = null;
			SurveyFromStoredProcedureDTO dto = null;
			var queryOver = new DefaultQueryOver<Survey, int>().GetQueryOver(() => s)
				.JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.InnerJoin).Where(() => smi.IsActive == true && smi.IsShared == true)
				.JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin).Where(() => smc.IsActive == true)
				.JoinQueryOver(() => smc.User, () => u, JoinType.InnerJoin)
				.JoinQueryOver(() => smi.CreatedBy, () => u2, JoinType.InnerJoin).Where(()=>(int)u2.Status == 1)
				.Where(() => u2.Id != userId&& u2.Company.Id == id.Value)
				.SelectList(res =>
				res.Select(() => s.SurveyGroupingType.Id)
					.WithAlias(() => dto.surveyGroupingTypeId)
					.Select(() => s.Description)
					.WithAlias(() => dto.description)
					.Select(() => s.SurveyName)
					.WithAlias(() => dto.surveyName)
                    .Select(() => s.LmsSurveyId)
                    .WithAlias(() => dto.lmsSurveyId)
					.Select(() => s.Id)
					.WithAlias(() => dto.surveyId)
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
					.TransformUsing(Transformers.AliasToBean<SurveyFromStoredProcedureDTO>());
			var result = this.Repository.FindAll<SurveyFromStoredProcedureDTO>(queryOver).ToList();
			return result;
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
			Survey s = null;
	        SubModuleCategory smc = null;
	        SubModuleItem smi = null;
	        SMICategoriesFromStoredProcedureDTO dto = null;
	        var queryOver = new DefaultQueryOver<Survey, int>().GetQueryOver(() => s)
		        .JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.RightOuterJoin)
		        .JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin)
		        .Where(() => smc.User.Id == userId && smi.CreatedBy.Id == userId && s.Id != 0)
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
		        .TransformUsing(Transformers.AliasToBean<SMICategoriesFromStoredProcedureDTO>());
	        var result = Repository.FindAll<SMICategoriesFromStoredProcedureDTO>(queryOver).ToList();
	        return result;
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
	        var distructors = distractorModel.GetDistractorsBySMIId(smiId).ToList(); 
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