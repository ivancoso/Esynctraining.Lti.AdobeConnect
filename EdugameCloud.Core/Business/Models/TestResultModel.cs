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

            res.questions = new List<QuestionForAdminDTO>(this.Repository.StoreProcedureForMany<QuestionForAdminDTO>("getTestQuestionsForAdminBySMIId", new StoreProcedureParam<int>("smiId", smiId), new StoreProcedureParam<int>("acSessionId", adobeConnectSessionId)));
            res.players = new List<TestPlayerDTO>(this.Repository.StoreProcedureForMany<TestPlayerDTO>("getTestResultByACSessionId", new StoreProcedureParam<int>("acSessionId", adobeConnectSessionId), new StoreProcedureParam<int>("subModuleItemId", smiId)));

            if (res.players != null && res.players.Any())
            {
                foreach (var player in res.players)
                {
                    long duration = (player.endTime - player.startTime).Ticks;
                    float passingScore = test.PassingScore.HasValue ? (res.questions.Count * (test.PassingScore.Value / 100)) : 0;
                    bool scorePassed = !test.PassingScore.HasValue ? player.score > 0 : test.PassingScore == 0 || passingScore <= player.score;
                    bool timePassed = !test.TimeLimit.HasValue || test.TimeLimit == 0 || test.TimeLimit > TimeSpan.FromTicks(duration).TotalMinutes;
                    player.passingScore = passingScore;
                    player.timeLimit = test.TimeLimit;
                    player.scorePassed = scorePassed;
                    player.timePassed = timePassed;
                }
            }

            var questionIds = res.questions.Select(q => q.questionId).ToList();

            var distractorsQuery = new DefaultQueryOver<Distractor, int>().GetQueryOver().WhereRestrictionOn(x => x.Question.Id).IsIn(questionIds);

            var distractors = this.distractorRepository.FindAll(distractorsQuery).ToList();

            foreach (var questionForAdminDTO in res.questions)
            {
                questionForAdminDTO.distractors = distractors.Where(x => x.Question.Id == questionForAdminDTO.questionId).Select(x => new DistractorDTO(x)).ToList();
            }

            return res;
        }
    }
}