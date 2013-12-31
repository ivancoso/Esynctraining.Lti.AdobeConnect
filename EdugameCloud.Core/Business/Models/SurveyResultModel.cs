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

    /// <summary>
    ///     The SurveyResult model.
    /// </summary>
    public class SurveyResultModel : BaseModel<SurveyResult, int>
    {
        /// <summary>
        /// The distractor repository.
        /// </summary>
        private readonly IRepository<Distractor, int> distractorRepository;

        /// <summary>
        /// The survey question result repository.
        /// </summary>
        private readonly IRepository<SurveyQuestionResult, int> surveyQuestionResultRepository;

        /// <summary>
        /// The answer repository.
        /// </summary>
        private readonly IRepository<SurveyQuestionResultAnswer, int> answerRepository;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyResultModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SurveyResultModel(IRepository<Distractor, int> distractors, IRepository<SurveyQuestionResult, int> surveyQuestionResultRepository, IRepository<SurveyQuestionResultAnswer, int> answerRepository, IRepository<SurveyResult, int> repository)
            : base(repository)
        {
            this.distractorRepository = distractors;
            this.surveyQuestionResultRepository = surveyQuestionResultRepository;
            this.answerRepository = answerRepository;
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
        /// The <see cref="IFutureValue{SurveyResult}"/>.
        /// </returns>
        public IFutureValue<SurveyResult> GetOneByACSessionIdAndEmail(int adobeConnectSessionId, string email)
        {
            var query =
                new DefaultQueryOver<SurveyResult, int>().GetQueryOver()
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
        public SurveyResultDataDTO GetSurveyResultByACSessionId(int adobeConnectSessionId, int smiId)
        {
            var res = new SurveyResultDataDTO();
            res.questions = new List<QuestionForAdminDTO>(this.Repository.StoreProcedureForMany<QuestionForAdminDTO>("getSurveyQuestionsForAdminBySMIId", new StoreProcedureParam<int>("smiId", smiId), new StoreProcedureParam<int>("acSessionId", adobeConnectSessionId)));
            res.players = new List<SurveyPlayerDTO>(this.Repository.StoreProcedureForMany<SurveyPlayerDTO>("getSurveyResultByACSessionId", new StoreProcedureParam<int>("acSessionId", adobeConnectSessionId), new StoreProcedureParam<int>("subModuleItemId", smiId)));

            var questionIds = res.questions.Select(q => q.questionId).ToList();

            var query = new DefaultQueryOver<SurveyQuestionResult, int>().GetQueryOver()
                .WhereRestrictionOn(x => x.QuestionRef.Id).IsIn(questionIds)
                .AndRestrictionOn(x => x.SurveyResult.Id).IsIn(res.players.Select(q => q.surveyResultId).ToList());

            var distractorsQuery = new DefaultQueryOver<Distractor, int>().GetQueryOver().WhereRestrictionOn(x => x.Question.Id).IsIn(questionIds);

            var playerAnswers = this.surveyQuestionResultRepository.FindAll(query).ToList();

            var distractors = this.distractorRepository.FindAll(distractorsQuery).ToList();

            var answersQuery = new DefaultQueryOver<SurveyQuestionResultAnswer, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.SurveyQuestionResult.Id)
                    .IsIn(playerAnswers.Select(q => q.Id).ToList());

            var realAnswers = this.answerRepository.FindAll(answersQuery).ToList();

            foreach (var questionForAdminDTO in res.questions)
            {
                questionForAdminDTO.questionResultIds = playerAnswers.Where(x => x.QuestionRef.Id == questionForAdminDTO.questionId).Select(x => x.Id).ToList();
                questionForAdminDTO.distractors = distractors.Where(x => x.Question.Id == questionForAdminDTO.questionId).Select(x => new DistractorDTO(x)).ToList();
            }

            foreach (var surveyPlayerDTO in res.players)
            {
                var playerAnswersIds = playerAnswers.Where(x => x.SurveyResult.Id == surveyPlayerDTO.surveyResultId).Select(x => x.Id).ToList();
                surveyPlayerDTO.answers = realAnswers.Where(x => playerAnswersIds.Contains(x.SurveyQuestionResult.Id)).Select(x => new SurveyQuestionResultAnswerDTO(x)).ToList();
            }

            return res;
        }
    }
}