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
using FluentValidation.Results;

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

        private CompanyEventQuizMappingModel EventQuizMappingModel
        {
            get { return IoC.Resolve<CompanyEventQuizMappingModel>(); }
        }

        private QuizResultModel QuizResultModel
        {
            get { return IoC.Resolve<QuizResultModel>(); }
        }

        private DistractorModel DistractorModel
        {
            get { return IoC.Resolve<DistractorModel>(); }
        }

        private QuizQuestionResultModel QuizQuestionResultModel
        {
            get { return IoC.Resolve<QuizQuestionResultModel>(); }
        }

        private QuizQuestionResultAnswerModel QuizQuestionResultAnswerModel
        {
            get { return IoC.Resolve<QuizQuestionResultAnswerModel>(); }
        }

        private QuestionModel QuestionModel
        {
            get { return IoC.Resolve<QuestionModel>(); }
        }

        private ILogger Logger
        {
            get { return IoC.Resolve<ILogger>(); }
        }

        public OfflineQuizDTO GetQuizByKey(string key)
        {
            var guid = Guid.Parse(key);
            var quizResult = QuizResultModel.GetOneByGuid(guid).Value;

            var questions = new List<OfflineQuestionDTO>();
            var result = new OfflineQuizDTO();
            //var quiz = QuizModel.GetOneById(quizResult.Quiz.Id).Value;
            var quiz = QuizModel.getQuizDataByQuizGuid(quizResult.Quiz.Guid);
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

        public OfflineQuizResultDTO SendAnswers(OfflineQuizAnswerContainerDTO answerContainer)
        {
            var quizResultGuid = answerContainer.quizResultGuid;
            var quizResult = QuizResultModel.GetOneByGuid(quizResultGuid).Value;
            var quizData = QuizModel.getQuizDataByQuizGuid(quizResult.Quiz.Guid);
            var quizEventMapping = EventQuizMappingModel.GetOneById(quizResult.EventQuizMapping.Id).Value;
            var postQuizId = quizEventMapping.PostQuiz.Id;
            var postQuizResult = new QuizResult
            {
                isCompleted = true,
                StartTime = answerContainer.startTime,
                EndTime = answerContainer.endTime,
                ACEmail = quizResult.ACEmail,
                Email = quizResult.Email,
                ACSessionId = quizResult.ACSessionId,
                ParticipantName = quizResult.ParticipantName,
                Quiz = QuizModel.GetOneById(postQuizId).Value
            };

            postQuizResult.Score = CalcScoreAndSaveQuestionResult(answerContainer.answers, quizData);
            QuizResultModel.RegisterSave(postQuizResult);

            return new OfflineQuizResultDTO()
            {
                score = postQuizResult.Score,
                certificateUrl = "no one yet"
            };
        }

        private int CalcScoreAndSaveQuestionResult(OfflineQuizAnswerDTO[] answers, QuizDataDTO quizData)
        {
            var score = 0;
            foreach (var answer in answers)
            {
                var questionResult = new QuizQuestionResult();
                var question = quizData.questions.First(x => x.questionId == answer.questionId);
                //var distractors = quizData.questions.Where(x => x.questionId == answer.questionId).ToList();
                var distractors = DistractorModel.GetAllByQuestionId(answer.questionId).ToList();
                if (answer.trueFalseAnswer != null)
                {
                    var shouldBeOneDistrctor = distractors.FirstOrDefault();
                    if (shouldBeOneDistrctor == null)
                        throw new InvalidOperationException("There should be a distractor for true/false question");
                    var isCorrect = answer.trueFalseAnswer.answer == (shouldBeOneDistrctor.IsCorrect ?? false);
                    if (isCorrect)
                    {
                        score++;
                    }
                    questionResult.IsCorrect = isCorrect;
                }

                if (answer.singleChoiceAnswer != null)
                {
                    var quizDistractorAnswer = DistractorModel.GetOneById(answer.singleChoiceAnswer.answeredDistractorId).Value;
                    var quizQuestionResultAnswer = new QuizQuestionResultAnswer()
                    {
                        QuizQuestionResult = questionResult,
                        Value = quizDistractorAnswer.DistractorName,
                        QuizDistractorAnswer = quizDistractorAnswer
                    };
                    questionResult.IsCorrect = distractors.First(x => x.Id == quizDistractorAnswer.Id).IsCorrect ?? false;
                    QuizQuestionResultAnswerModel.RegisterSave(quizQuestionResultAnswer);
                }

                questionResult.Question = question.question;
                var questionObj = QuestionModel.GetOneById(question.questionId).Value;
                questionResult.QuestionRef = questionObj;
                questionResult.QuestionType = questionObj.QuestionType;
                QuizQuestionResultModel.RegisterSave(questionResult, true);
            }

            return score;
        }
    }
}
