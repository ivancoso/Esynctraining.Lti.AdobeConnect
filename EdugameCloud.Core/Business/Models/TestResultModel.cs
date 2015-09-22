using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;

    /// <summary>
    ///     The TestResult model.
    /// </summary>
    public class TestResultModel : BaseModel<TestResult, int>
    {
        /// <summary>
        /// The distractor repository.
        /// </summary>
        private readonly IRepository<Distractor, int> distractorRepository;

        private readonly IRepository<Test, int> testRepository;

        private readonly IRepository<Question, int> questionRepository;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultModel"/> class. 
        /// </summary>
        /// <param name="distractorRepository">
        /// The distractor Repository.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public TestResultModel(IRepository<Distractor, int> distractorRepository, IRepository<Test, int> testRepository, IRepository<Question, int> questionRepository, IRepository<TestResult, int> repository)
            : base(repository)
        {
            this.distractorRepository = distractorRepository;
            this.testRepository = testRepository;
            this.questionRepository = questionRepository;
        }

        #endregion

        /// <summary>
        /// The get one by ac session id and email.
        /// </summary>
        /// <param name="adobeConnectSessionId">
        /// The adobe connect session id.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{TestResult}"/>.
        /// </returns>
        public IFutureValue<TestResult> GetOneByACSessionIdAndEmail(int adobeConnectSessionId, string email)
        {
            var query =
                new DefaultQueryOver<TestResult, int>().GetQueryOver()
                    .Where(x => x.ACSessionId == adobeConnectSessionId)
                    .AndRestrictionOn(x => x.Email)
                    .IsNotNull.AndRestrictionOn(x => x.Email)
                    .IsInsensitiveLike(email).Take(1);
            return this.Repository.FindOne(query);
        }

        /// <summary>
        /// The get quiz result by adobe connect session id.
        /// </summary>
        /// <param name="adobeConnectSessionId">
        /// The adobe connect session id.
        /// </param>
        /// <param name="smiId">
        /// The sub module item id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizResultDataDTO"/>.
        /// </returns>
        public TestResultDataDTO GetTestResultByACSessionId(int adobeConnectSessionId, int smiId)
        {
            var test = this.testRepository.FindOne(new DefaultQueryOver<Test, int>().GetQueryOver().Where(x => x.SubModuleItem.Id == smiId).Take(1)).Value;
            var res = new TestResultDataDTO();
	        Question q = null;
	        SubModuleItem smi = null;
	        QuestionType qt = null;
	        QuestionForAdminDTO dto = null;
	        var queryOver = new DefaultQueryOver<Question, int>().GetQueryOver(() => q)
		        .JoinQueryOver(x => x.SubModuleItem, () => smi, JoinType.InnerJoin)
		        .JoinQueryOver(() => q.QuestionType, () => qt, JoinType.InnerJoin)
		        .Where(() => q.SubModuleItem.Id == smiId && q.IsActive == true)
		        .SelectList(result =>
			        result.Select(() => q.Id)
				        .WithAlias(() => dto.questionId)
						.Select(()=>q.QuestionName)
						.WithAlias(()=>dto.question)
						.Select(()=>q.QuestionType.Id)
						.WithAlias(()=>dto.questionTypeId)
						.Select(()=>qt.Type)
						.WithAlias(()=>dto.questionTypeName)
		        ).TransformUsing(Transformers.AliasToBean<QuestionForAdminDTO>());
			var questionqs = questionRepository.FindAll<QuestionForAdminDTO>(queryOver).ToList();

	        TestResult tr = null;
	        TestQuestionResult tqr = null;
	        q = null;
	        var queryOver1 = new DefaultQueryOver<TestResult, int>().GetQueryOver(() => tr)
		        .JoinQueryOver(x => x.Results, () => tqr, JoinType.LeftOuterJoin)
		        .JoinQueryOver(() => tqr.QuestionRef, () => q, JoinType.LeftOuterJoin)
		        .Where(() => tr.ACSessionId == adobeConnectSessionId)
		        .SelectList(res1 =>
			        res1.SelectGroup(() => q.Id)
				        .WithAlias(() => dto.questionId)
				        .Select(Projections.Sum(Projections.Cast(NHibernateUtil.Int32, Projections.Property(() => tqr.IsCorrect))))
				        .WithAlias(() => dto.сorrectAnswerCount))
		        .TransformUsing(Transformers.AliasToBean<QuestionForAdminDTO>());
			var questionqsWithCorrectAnswerCount = Repository.FindAll<QuestionForAdminDTO>(queryOver1).ToList();
			questionqs.ForEach(x => x.сorrectAnswerCount = (questionqsWithCorrectAnswerCount.Any(t => t.questionId == x.questionId)? questionqsWithCorrectAnswerCount.First(t => t.questionId == x.questionId).сorrectAnswerCount: 0));
            res.questions = questionqs.ToArray();
            res.players =
                this.Repository.StoreProcedureForMany<TestPlayerFromStoredProcedureDTO>(
                    "getTestResultByACSessionId",
                    new StoreProcedureParam<int>("acSessionId", adobeConnectSessionId),
                    new StoreProcedureParam<int>("subModuleItemId", smiId))
                    .ToList()
                    .Select(x => new TestPlayerDTO(x))
                    .ToArray();

            if (res.players != null && res.players.Any())
            {
                foreach (var player in res.players)
                {
                    long duration = (player.endTime.ConvertFromUnixTimeStamp() - player.startTime.ConvertFromUnixTimeStamp()).Ticks;
                    var passingScore = test.PassingScore.HasValue ? (res.questions.Length * (test.PassingScore.Value / 100)) : 0;
                    bool scorePassed = !test.PassingScore.HasValue ? player.score > 0 : test.PassingScore == 0 || passingScore <= player.score;
                    bool timePassed = !test.TimeLimit.HasValue || test.TimeLimit == 0 || test.TimeLimit > TimeSpan.FromTicks(duration).TotalMinutes;
                    player.passingScore = passingScore;
                    player.timeLimit = test.TimeLimit;
                    player.scorePassed = scorePassed;
                    player.timePassed = timePassed;
                }
            }

			var questionIds = res.questions.Select(question => question.questionId).ToList();

            var distractorsQuery = new DefaultQueryOver<Distractor, int>().GetQueryOver().WhereRestrictionOn(x => x.Question.Id).IsIn(questionIds);

            var distractors = this.distractorRepository.FindAll(distractorsQuery).ToList();

            foreach (var questionForAdminDTO in res.questions)
            {
                questionForAdminDTO.distractors = distractors.Where(x => x.Question.Id == questionForAdminDTO.questionId).Select(x => new DistractorDTO(x)).ToArray();
            }

            return res;
        }
    }
}