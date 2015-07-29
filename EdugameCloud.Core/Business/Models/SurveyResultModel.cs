using System.Linq;
using System.Xml.Linq;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.Core.Domain.Entities;
using Esynctraining.Core.Business;
using Esynctraining.Core.Business.Models;
using Esynctraining.Core.Business.Queries;
using NHibernate;
using NHibernate.Transform;

namespace EdugameCloud.Core.Business.Models
{
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
            res.questions = this.Repository.StoreProcedureForMany<QuestionForAdminDTO>("getSurveyQuestionsForAdminBySMIId", new StoreProcedureParam<int>("smiId", smiId), new StoreProcedureParam<int>("acSessionId", adobeConnectSessionId)).ToArray();
            res.players =
                this.Repository.StoreProcedureForMany<SurveyPlayerFromStoredProcedureDTO>(
                    "getSurveyResultByACSessionId",
                    new StoreProcedureParam<int>("acSessionId", adobeConnectSessionId),
                    new StoreProcedureParam<int>("subModuleItemId", smiId))
                    .ToList()
                    .Select(x => new SurveyPlayerDTO(x))
                    .ToArray();

            var questionIds = res.questions.Select(q => q.questionId).ToList();

            var query = new DefaultQueryOver<SurveyQuestionResult, int>().GetQueryOver()
                .WhereRestrictionOn(x => x.QuestionRef.Id).IsIn(questionIds)
                .AndRestrictionOn(x => x.SurveyResult.Id).IsIn(res.players.Select(q => q.surveyResultId).ToList());

            var distractorsQuery = new DefaultQueryOver<Distractor, int>().GetQueryOver().WhereRestrictionOn(x => x.Question.Id).IsIn(questionIds);

            var surveyQuestionResults = this.surveyQuestionResultRepository.FindAll(query).ToList();

            var distractors = this.distractorRepository.FindAll(distractorsQuery).ToList();
            
            var xmlSurveyResultIds = new XElement("Ids", surveyQuestionResults.Select(q => new XElement("Id", q.Id)));
            var answersQuery = this.Repository.Session.GetNamedQuery("getSurveyResultAnswers");
            answersQuery.SetParameter("surveyResultIds", xmlSurveyResultIds.ToString(), NHibernateUtil.StringClob);
            answersQuery = answersQuery.SetResultTransformer(new AliasToBeanResultTransformer(typeof(SurveyQuestionResultAnswerDTO)));
            var answers = answersQuery.List<SurveyQuestionResultAnswerDTO>();
            
            foreach (var questionForAdminDTO in res.questions)
            {
                questionForAdminDTO.questionResultIds = surveyQuestionResults.Where(x => x.QuestionRef.Id == questionForAdminDTO.questionId).Select(x => x.Id).ToArray();
                questionForAdminDTO.distractors = distractors.Where(x => x.Question.Id == questionForAdminDTO.questionId).Select(x => new DistractorDTO(x)).ToArray();
            }

            foreach (var surveyPlayerDTO in res.players)
            {
                var playerSurveyQuestionResultIds = surveyQuestionResults.Where(x => x.SurveyResult.Id == surveyPlayerDTO.surveyResultId).Select(x => x.Id).ToList();
                surveyPlayerDTO.answers = answers.Where(x => playerSurveyQuestionResultIds.Contains(x.surveyQuestionResultId)).ToArray();
            }

            return res;
        }
    }
}