using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.Core.Domain.DTO.OfflineQuiz;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.WCFService.Base;
using EdugameCloud.WCFService.Contracts;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "OfflineQuizService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select OfflineQuizService.svc or CompanyAcDomainsService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall,
        IncludeExceptionDetailInFaults = true)]
    public class OfflineQuizService : BaseService, IOfflineQuizService
    {
        /// <summary>
        /// Gets the company model.
        /// </summary>
        private CompanyAcServerModel CompanyAcServerModel
        {
            get { return IoC.Resolve<CompanyAcServerModel>(); }
        }

        private CompanyEventQuizMappingModel CompanyEventQuizMappingModel
        {
            get { return IoC.Resolve<CompanyEventQuizMappingModel>(); }
        }

        private QuizModel QuizModel
        {
            get { return IoC.Resolve<QuizModel>(); }
        }

        private ILogger Logger
        {
            get { return IoC.Resolve<ILogger>(); }
        }

        public OfflineQuizDTO GetQuizByKey(string key)
        {
            var guid = Guid.Parse(key);
            var quiz = QuizModel.getQuizDataByQuizGuid(guid);
            
            var questions = new List<OfflineQuestionDTO>();
            var result = new OfflineQuizDTO();
            if (quiz.questions == null || !quiz.questions.Any())
                return result;
            foreach (var question in quiz.questions)
            {
                var distractors = new List<OfflineDistractorDTO>();
                var quizDistractors = quiz.distractors.Where(x => x.questionId == question.questionId).ToArray();
                if (!quizDistractors.Any())
                    continue;
                foreach (var quizDistractor in quizDistractors)
                {
                    distractors.Add(new OfflineDistractorDTO()
                    {
                        questionId = quizDistractor.questionId,
                        distractor = quizDistractor.distractor,
                        distractorOrder = quizDistractor.distractorOrder,
                        distractorId = quizDistractor.distractorId,
                        distractorType = quizDistractor.distractorType
                    });
                }
                questions.Add(new OfflineQuestionDTO()
                {
                    question = question.question,
                    distractors = distractors.ToArray(),
                    questionId = question.questionId,
                    imageId = question.imageId,
                    questionTypeId = question.questionTypeId,
                    questionOrder = question.questionOrder,
                    randomizeAnswers = question.randomizeAnswers,
                    isMultipleChoice = question.isMultipleChoice,
                    restrictions = question.restrictions,
                    instruction = question.instruction,
                    imageVO = question.imageVO
                });
            }
            result.questions = questions.ToArray();
            result.description = quiz.quizVO.description;
            result.quizName = quiz.quizVO.quizName;
            //result.participant
            return result;
        }

        public OfflineQuizResultDTO SendAnswers(OfflineQuizAnswerDTO[] answers)
        {
            throw new NotImplementedException();
        }
    }
}
