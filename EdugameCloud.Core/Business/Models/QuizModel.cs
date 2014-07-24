namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.SqlCommand;
    using NHibernate.Transform;

    /// <summary>
    ///     The Quiz model.
    /// </summary>
    public class QuizModel : BaseModel<Quiz, int>
    {
        #region Fields

        /// <summary>
        ///     The distractor model.
        /// </summary>
        private readonly DistractorModel distractorModel;

        /// <summary>
        ///     The file model.
        /// </summary>
        private readonly FileModel fileModel;

        /// <summary>
        ///     The user repository.
        /// </summary>
        private readonly IRepository<User, int> userRepository;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizModel"/> class.
        /// </summary>
        /// <param name="fileModel">
        /// The file Model.
        /// </param>
        /// <param name="distractorModel">
        /// The distractor Model.
        /// </param>
        /// <param name="userRepository">
        /// The user Repository.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public QuizModel(
            FileModel fileModel, 
            DistractorModel distractorModel, 
            IRepository<User, int> userRepository, 
            IRepository<Quiz, int> repository)
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
        public IEnumerable<Quiz> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            QueryOver<Quiz, Quiz> queryOver = new DefaultQueryOver<Quiz, int>().GetQueryOver();
            QueryOver<Quiz, Quiz> rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            QueryOver<Quiz> pagedQuery = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            return this.Repository.FindAll(pagedQuery);
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
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            Quiz q = null;
            SMICategoriesFromStoredProcedureDTO dto = null;
            QueryOver<Quiz, SubModuleCategory> queryOver =
                new DefaultQueryOver<Quiz, int>().GetQueryOver(() => q)
                    .JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.RightOuterJoin)
                    .JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin)
                    .Where(() => smc.User.Id == userId && smi.CreatedBy.Id == userId && q.Id != 0)
                    .SelectList(
                        res =>
                        res.Select(
                            Projections.Distinct(
                                Projections.ProjectionList()
                            .Add(Projections.Property(() => smc.IsActive))
                            .Add(Projections.Property(() => smc.DateModified))
                            .Add(Projections.Property(() => smc.ModifiedBy.Id))
                            .Add(Projections.Property(() => smc.CategoryName))
                            .Add(Projections.Property(() => smc.SubModule.Id))
                            .Add(Projections.Property(() => smc.User.Id))
                            .Add(Projections.Property(() => smc.Id))))
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
            List<SMICategoriesFromStoredProcedureDTO> result =
                this.Repository.FindAll<SMICategoriesFromStoredProcedureDTO>(queryOver).ToList();
            return result;
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
            List<DistractorFromStoredProcedureDTO> distructors =
                this.distractorModel.GetDistractorsBySMIId(smiId).ToList();
            List<Guid> imageIds = distructors.Where(x => x.imageId.HasValue).Select(x => x.imageId.Value).ToList();
            if (imageIds.Any())
            {
                List<File> files = this.fileModel.GetAllByIds(imageIds).ToList();
                foreach (DistractorFromStoredProcedureDTO dto in distructors.Where(x => x.imageId.HasValue))
                {
                    dto.imageVO = new FileDTO(files.FirstOrDefault(x => x.Id == dto.imageId.Value));
                }
            }

            return distructors;
        }

        /// <summary>
        /// The get one by moodle id.
        /// </summary>
        /// <param name="moodleId">
        /// The moodle id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Quiz}"/>.
        /// </returns>
        public IFutureValue<Quiz> GetOneByMoodleId(int moodleId)
        {
            var query =
                new DefaultQueryOver<Quiz, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.MoodleId)
                    .IsNotNull.And(x => x.MoodleId == moodleId).Take(1);
            return this.Repository.FindOne(query);
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
            IEnumerable<QuestionFromStoredProcedureDTO> questions =
                this.Repository.StoreProcedureForMany<QuestionFromStoredProcedureDTO>(
                    "getSMIQuestionsBySMIId", 
                    new StoreProcedureParam<int>("subModuleItemId", smiId));
            List<Guid> imageIds = questions.Where(x => x.imageId.HasValue).Select(x => x.imageId.Value).ToList();
            if (imageIds.Any())
            {
                List<File> files = this.fileModel.GetAllByIds(imageIds).ToList();
                foreach (QuestionFromStoredProcedureDTO dto in questions.Where(x => x.imageId.HasValue))
                {
                    dto.imageVO = new FileDTO(files.FirstOrDefault(x => x.Id == dto.imageId.Value));
                }
            }

            return questions;
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
            Quiz q = null;
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            User u = null;
            User u2 = null;
            QuizFromStoredProcedureDTO dto = null;
            QueryOver<Quiz, User> queryOver =
                new DefaultQueryOver<Quiz, int>().GetQueryOver(() => q)
                    .JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.InnerJoin)
                    .Where(() => smi.IsActive == true)
                    .JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin)
                    .Where(() => smc.IsActive == true)
                    .JoinQueryOver(() => smc.User, () => u, JoinType.InnerJoin)
                    .JoinQueryOver(() => smi.CreatedBy, () => u2, JoinType.LeftOuterJoin)
                    .Where(() => u2.Id == userId)
                    .SelectList(
                        res =>
                        res.Select(() => q.Description)
                            .WithAlias(() => dto.description)
                            .Select(() => q.QuizName)
                            .WithAlias(() => dto.quizName)
                            .Select(() => q.Id)
                            .WithAlias(() => dto.quizId)
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
                    .TransformUsing(Transformers.AliasToBean<QuizFromStoredProcedureDTO>());
            List<QuizFromStoredProcedureDTO> result =
                this.Repository.FindAll<QuizFromStoredProcedureDTO>(queryOver).ToList();
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
        public IEnumerable<QuizFromStoredProcedureDTO> GetSharedForUserQuizzesByUserId(int userId)
        {
            QueryOver<User, User> query =
                new DefaultQueryOver<User, int>().GetQueryOver()
                    .Where(x => x.Id == userId)
                    .Select(res => res.Company.Id);
            IFutureValue<int> id = this.userRepository.FindOne<int>(query);

            Quiz q = null;
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            User u = null;
            User u2 = null;
            QuizFromStoredProcedureDTO dto = null;
            QueryOver<Quiz, User> queryOver =
                new DefaultQueryOver<Quiz, int>().GetQueryOver(() => q)
                    .JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.InnerJoin)
                    .Where(() => smi.IsActive == true && smi.IsShared == true)
                    .JoinQueryOver(() => smi.SubModuleCategory, () => smc, JoinType.InnerJoin)
                    .Where(() => smc.IsActive == true)
                    .JoinQueryOver(() => smc.User, () => u, JoinType.InnerJoin)
                    .JoinQueryOver(() => smi.CreatedBy, () => u2, JoinType.InnerJoin)
                    .Where(() => u2.Id != userId && u2.Company.Id == id.Value && (int)u2.Status == 1)
                    .SelectList(
                        res =>
                        res.Select(() => q.Description)
                            .WithAlias(() => dto.description)
                            .Select(() => q.QuizName)
                            .WithAlias(() => dto.quizName)
                            .Select(() => q.Id)
                            .WithAlias(() => dto.quizId)
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
                    .TransformUsing(Transformers.AliasToBean<QuizFromStoredProcedureDTO>());
            List<QuizFromStoredProcedureDTO> result =
                this.Repository.FindAll<QuizFromStoredProcedureDTO>(queryOver).ToList();
            return result;
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

        #endregion
    }
}