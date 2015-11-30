namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;
    using NHibernate;
    using NHibernate.Transform;

    /// <summary>
    ///     The QuizResult model.
    /// </summary>
    public class QuizResultModel : BaseModel<QuizResult, int>
    {
        /// <summary>
        /// The distractor repository.
        /// </summary>
        private readonly IRepository<Distractor, int> distractorRepository;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizResultModel"/> class. 
        /// </summary>
        /// <param name="distractorRepository">
        /// The distractor Repository.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public QuizResultModel(IRepository<Distractor, int> distractorRepository, IRepository<QuizResult, int> repository)
            : base(repository)
        {
            this.distractorRepository = distractorRepository;
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
        /// The <see cref="IFutureValue{QuizResult}"/>.
        /// </returns>
        public IFutureValue<QuizResult> GetOneByACSessionIdAndEmail(int adobeConnectSessionId, string email)
        {
            var query =
                new DefaultQueryOver<QuizResult, int>().GetQueryOver()
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
        public QuizResultDataDTO GetQuizResultByACSessionId(int adobeConnectSessionId, int smiId)
        {
            var res = new QuizResultDataDTO();
            res.questions = this.Repository.StoreProcedureForMany<QuestionForAdminDTO>("getQuizQuestionsForAdminBySMIId", 
                new StoreProcedureParam<int>("smiId", smiId), 
                new StoreProcedureParam<int>("acSessionId", adobeConnectSessionId)).ToArray();
            res.players =
                this.Repository.StoreProcedureForMany<QuizPlayerFromStoredProcedureDTO>(
                    "getQuizResultByACSessionId",
                    new StoreProcedureParam<int>("acSessionId", adobeConnectSessionId),
                    new StoreProcedureParam<int>("subModuleItemId", smiId))
                    .ToList()
                    .Select(x => new QuizPlayerDTO(x))
                    .ToArray();

            Array.ForEach(res.questions, q => q.question = Regex.Replace(q.question ?? string.Empty, "<[^>]*(>|$)", string.Empty).Replace("&nbsp;", " "));

            var questionIds = res.questions.Select(q => q.questionId).ToList();

            var distractorsQuery =
                new DefaultQueryOver<Distractor, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Question.Id)
                    .IsIn(questionIds);
            
            var distractors = this.distractorRepository.FindAll(distractorsQuery).ToList();
            
            foreach (var questionForAdminDTO in res.questions)
            {
                questionForAdminDTO.distractors = distractors
                    .Where(x => x.Question.Id == questionForAdminDTO.questionId)
                    .Select(x => new DistractorDTO(x))
                    .ToArray();
            }

            return res;
        }

        public QuizResultDataDTO GetQuizResultByACSessionIdAcEmail(int adobeConnectSessionId, int smiId, string acEmail)
        {
            var res = new QuizResultDataDTO();
            res.questions = this.Repository.StoreProcedureForMany<QuestionForAdminDTO>("getQuizQuestionsForAdminBySMIId",
                new StoreProcedureParam<int>("smiId", smiId),
                new StoreProcedureParam<int>("acSessionId", adobeConnectSessionId)).ToArray();
            res.players =
                this.Repository.StoreProcedureForMany<QuizPlayerFromStoredProcedureDTO>(
                    "getQuizResultByACSessionIdAcEmail",
                    new StoreProcedureParam<int>("acSessionId", adobeConnectSessionId),
                    new StoreProcedureParam<int>("subModuleItemId", smiId),
                    new StoreProcedureParam<string>("acEmail", acEmail))
                    .ToList()
                    .Select(x => new QuizPlayerDTO(x))
                    .ToArray();

            Array.ForEach(res.questions, q => q.question = Regex.Replace(q.question ?? string.Empty, "<[^>]*(>|$)", string.Empty).Replace("&nbsp;", " "));

            var questionIds = res.questions.Select(q => q.questionId).ToList();

            var distractorsQuery =
                new DefaultQueryOver<Distractor, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Question.Id)
                    .IsIn(questionIds);

            var distractors = this.distractorRepository.FindAll(distractorsQuery).ToList();

            foreach (var questionForAdminDTO in res.questions)
            {
                questionForAdminDTO.distractors = distractors
                    .Where(x => x.Question.Id == questionForAdminDTO.questionId)
                    .Select(x => new DistractorDTO(x))
                    .ToArray();
            }

            return res;
        }

        /// <summary>
        /// The get quiz results by quiz ids.
        /// </summary>
        /// <param name="quizIds">
        /// The quiz ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{QuizResult}"/>.
        /// </returns>
        public IEnumerable<QuizResult> GetQuizResultsByQuizIds(List<int> quizIds)
        {
            var query =
                new DefaultQueryOver<QuizResult, int>().GetQueryOver().WhereRestrictionOn(x => x.Quiz.Id).IsIn(quizIds)
                .Fetch(x => x.Quiz).Eager
                .Fetch(x => x.Results).Eager
                .TransformUsing(Transformers.RootEntity);
            return this.Repository.FindAll(query);
        }
    }
}