namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using NHibernate;

    /// <summary>
    ///     The question model.
    /// </summary>
    public class QuestionModel : BaseModel<Question, int>
    {
        /// <summary>
        /// The like repository.
        /// </summary>
        private readonly IRepository<QuestionForLikert, int> likertRepository;

        /// <summary>
        /// The open answer repository.
        /// </summary>
        private readonly IRepository<QuestionForOpenAnswer, int> openAnswerRepository;

        /// <summary>
        /// The rate repository.
        /// </summary>
        private readonly IRepository<QuestionForRate, int> rateRepository;

        /// <summary>
        /// The rate repository.
        /// </summary>
        private readonly IRepository<QuestionForTrueFalse, int> trueFalseRepository;

        /// <summary>
        /// The weight repository.
        /// </summary>
        private readonly IRepository<QuestionForWeightBucket, int> weightRepository;

        /// <summary>
        /// The choice repository.
        /// </summary>
        private readonly IRepository<QuestionForSingleMultipleChoice, int> choiceRepository;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="likertRepository">
        /// The like Repository.
        /// </param>
        /// <param name="openAnswerRepository">
        /// The open Answer Repository.
        /// </param>
        /// <param name="rateRepository">
        /// The rate Repository.
        /// </param>
        /// <param name="trueFalseRepository">
        /// The true False Repository.
        /// </param>
        /// <param name="weightRepository">
        /// The weight Repository.
        /// </param>
        /// <param name="choiceRepository">
        /// The choice Repository.
        /// </param>
        public QuestionModel(IRepository<Question, int> repository, IRepository<QuestionForLikert, int> likertRepository, IRepository<QuestionForOpenAnswer, int> openAnswerRepository, IRepository<QuestionForRate, int> rateRepository, IRepository<QuestionForTrueFalse, int> trueFalseRepository, IRepository<QuestionForWeightBucket, int> weightRepository, IRepository<QuestionForSingleMultipleChoice, int> choiceRepository)
            : base(repository)
        {
            this.trueFalseRepository = trueFalseRepository;
            this.likertRepository = likertRepository;
            this.openAnswerRepository = openAnswerRepository;
            this.rateRepository = rateRepository;
            this.weightRepository = weightRepository;
            this.choiceRepository = choiceRepository;
        }

        #endregion

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
        /// The <see cref="IEnumerable{Question}"/>.
        /// </returns>
        public IEnumerable<Question> GetAllPaged(int pageIndex, int pageSize, out int totalCount)
        {
            var queryOver = new DefaultQueryOver<Question, int>().GetQueryOver().Where(x => x.IsActive == true);
            var rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;
            var pagedQuery = queryOver.Fetch(x => x.Image).Eager.Take(pageSize).Skip((pageIndex - 1) * pageSize);
            return this.Repository.FindAll(pagedQuery);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Question}"/>.
        /// </returns>
        public override IEnumerable<Question> GetAll()
        {
            var defaultQuery = new DefaultQueryOver<Question, int>().GetQueryOver().Where(x => x.IsActive == true).Fetch(x => x.Image).Eager;
            return this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The get one by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Question}"/>.
        /// </returns>
        public override IFutureValue<Question> GetOneById(int id)
        {
            var queryOver = new DefaultQueryOver<Question, int>().GetQueryOver().Where(x => x.Id == id).Fetch(x => x.Image).Eager;
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get one by submodule item id and lms id.
        /// </summary>
        /// <param name="subModuleItemId">
        /// The submodule item id.
        /// </param>
        /// <param name="lmsId">
        /// The lms id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Question}"/>.
        /// </returns>
        public IFutureValue<Question> GetOneBySubmoduleItemIdAndLmsId(int subModuleItemId, int lmsId)
        {
            var queryOver = new DefaultQueryOver<Question, int>().GetQueryOver().Where(x => x.SubModuleItem.Id == subModuleItemId && x.LmsQuestionId == lmsId).Fetch(x => x.Image).Eager;
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The get all by user id and sub module item id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="subModuleItemId">
        /// The sub module item id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Distractor}"/>.
        /// </returns>
        public IEnumerable<Question> GetAllByUserIdAndSubModuleItemId(int userId, int subModuleItemId)
        {
            var queryOver = new DefaultQueryOver<Question, int>().GetQueryOver().Where(x => x.CreatedBy != null && x.CreatedBy.Id == userId && x.SubModuleItem.Id == subModuleItemId && x.IsActive == true);
            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get all by sub module item id.
        /// </summary>
        /// <param name="subModuleItemId">
        /// The sub module item id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Distractor}"/>.
        /// </returns>
        public IEnumerable<Question> GetAllBySubModuleItemId(int subModuleItemId)
        {
            var queryOver = new DefaultQueryOver<Question, int>().GetQueryOver().Where(x => x.SubModuleItem.Id == subModuleItemId && x.IsActive == true);
            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get custom questions by question ids with types.
        /// </summary>
        /// <param name="questionIdsWithTypes">
        /// The question ids with types.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuestionFor}"/>.
        /// </returns>
        public IEnumerable<QuestionFor> GetCustomQuestionsByQuestionIdsWithTypes(IEnumerable<KeyValuePair<int, int>> questionIdsWithTypes)
        {
            return this.GetCustomQuestionsByQuestionIdsGroupedByTypes(questionIdsWithTypes.GroupBy(x => x.Value).ToDictionary(x => x.Key, g => g.Select(s => s.Key)).ToList());
        }

        /// <summary>
        /// The get custom questions by question ids grouped by types.
        /// </summary>
        /// <param name="typesWithQuestionIds">
        /// The types with question ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuestionFor}"/>.
        /// </returns>
        public IEnumerable<QuestionFor> GetCustomQuestionsByQuestionIdsGroupedByTypes(IEnumerable<KeyValuePair<int, IEnumerable<int>>> typesWithQuestionIds)
        {
            var result = new List<QuestionFor>();
            foreach (var typeWithQuestions in typesWithQuestionIds)
            {
                switch (typeWithQuestions.Key)
                {
                    case (int)QuestionTypeEnum.Rate:
                        var ratesListQuery =
                            new DefaultQueryOver<QuestionForRate, int>().GetQueryOver()
                                .WhereRestrictionOn(x => x.Question.Id)
                                .IsIn(typeWithQuestions.Value.ToList());
                        result.AddRange(this.rateRepository.FindAll(ratesListQuery));
                        break;
                    case (int)QuestionTypeEnum.TrueFalse:
                        var trueFalseListQuery = new DefaultQueryOver<QuestionForTrueFalse, int>().GetQueryOver()
                                .WhereRestrictionOn(x => x.Question.Id)
                                .IsIn(typeWithQuestions.Value.ToList());
                        result.AddRange(this.trueFalseRepository.FindAll(trueFalseListQuery));
                        break;
                    case (int)QuestionTypeEnum.OpenAnswerMultiLine:
                    case (int)QuestionTypeEnum.OpenAnswerSingleLine:
                        var openAnswerListQuery =
                            new DefaultQueryOver<QuestionForOpenAnswer, int>().GetQueryOver()
                                .WhereRestrictionOn(x => x.Question.Id)
                                .IsIn(typeWithQuestions.Value.ToList());
                        result.AddRange(this.openAnswerRepository.FindAll(openAnswerListQuery));
                        break;
                    case (int)QuestionTypeEnum.RateScaleLikert:
                        var likersListQuery =
                            new DefaultQueryOver<QuestionForLikert, int>().GetQueryOver()
                                .WhereRestrictionOn(x => x.Question.Id)
                                .IsIn(typeWithQuestions.Value.ToList());
                        result.AddRange(this.likertRepository.FindAll(likersListQuery));
                        break;
                    case (int)QuestionTypeEnum.WeightedBucketRatio:
                        var weightListQuery =
                            new DefaultQueryOver<QuestionForWeightBucket, int>().GetQueryOver()
                                .WhereRestrictionOn(x => x.Question.Id)
                                .IsIn(typeWithQuestions.Value.ToList());
                        result.AddRange(this.weightRepository.FindAll(weightListQuery));
                        break;
                    case (int)QuestionTypeEnum.SingleMultipleChoiceImage:
                    case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                        var choiceListQuery =
                            new DefaultQueryOver<QuestionForSingleMultipleChoice, int>().GetQueryOver()
                                .WhereRestrictionOn(x => x.Question.Id)
                                .IsIn(typeWithQuestions.Value.ToList());
                        result.AddRange(this.choiceRepository.FindAll(choiceListQuery));
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// The get by question ids and sub module item id.
        /// </summary>
        /// <param name="smiId">
        /// The sub module item id.
        /// </param>
        /// <param name="questionsIds">
        /// The questions ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Question}"/>.
        /// </returns>
        public IEnumerable<Question> GetByQuestionIdsAndSmiID(int smiId, List<int> questionsIds = null)
        {
            var queryOver = new DefaultQueryOver<Question, int>().GetQueryOver().Where(x => x.SubModuleItem.Id == smiId && x.IsActive == true);
            if (questionsIds != null)
            {
                queryOver = queryOver.AndRestrictionOn(x => x.Id).IsIn(questionsIds);
            }

            return this.Repository.FindAll(queryOver);
        }
    }
}