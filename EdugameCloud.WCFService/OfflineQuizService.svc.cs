using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Castle.Components.DictionaryAdapter;
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
using Esynctraining.Core.Providers;
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

        protected dynamic Settings
        {
            get { return IoC.Resolve<ApplicationSettingsProvider>(); }
        }

        private CompanyEventQuizMappingModel CompanyEventQuizMappingModel
        {
            get { return IoC.Resolve<CompanyEventQuizMappingModel>(); }
        }

        private QuizModel QuizModel
        {
            get { return IoC.Resolve<QuizModel>(); }
        }

        private ACSessionModel ACSessionModel
        {
            get { return IoC.Resolve<ACSessionModel>(); }
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

            var mapping = EventQuizMappingModel.GetOneById(quizResult.EventQuizMapping.Id).Value;

            var postQuizId = mapping.PostQuiz.Id;
            var postQuizResults = QuizResultModel.GetQuizResultsByQuizIds(new[] {postQuizId}.ToList());
            if (postQuizResults.Any(x => x.ACEmail == quizResult.ACEmail))
            {
                return new OfflineQuizDTO()
                {
                    errorMessage = "You have already passed this Quiz!"
                };
            }

            var questions = new List<OfflineQuestionDTO>();
            var result = new OfflineQuizDTO();
            //var quiz = QuizModel.GetOneById(quizResult.Quiz.Id).Value;
            var postQuiz = QuizModel.getQuizDataByQuizID(mapping.PostQuiz.Id);
            if (postQuiz.questions == null || !postQuiz.questions.Any())
                return result;
            foreach (var question in postQuiz.questions)
            {
                var distractors = new List<OfflineDistractorDTO>();
                var quizDistractors = postQuiz.distractors.Where(x => x.questionId == question.questionId).ToArray();
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
            result.description = postQuiz.quizVO.description;
            result.quizName = postQuiz.quizVO.quizName;
            //var quizResultObj = QuizResultModel.GetOneById(postQuiz.quizVO.quizId).Value;

            result.participant = new ParticipantDTO()
            {
                email = quizResult.ACEmail,
                participantName = quizResult.ParticipantName
            };
            return result;
        }

        public OfflineQuizResultDTO SendAnswers(OfflineQuizAnswerContainerDTO answerContainer)
        {
            var quizResultGuid = answerContainer.quizResultGuid;
            var quizResult = QuizResultModel.GetOneByGuid(quizResultGuid).Value;
            var mapping = EventQuizMappingModel.GetOneById(quizResult.EventQuizMapping.Id).Value;
            var postQuizData = QuizModel.getQuizDataByQuizID(mapping.PostQuiz.Id);
            DateTime startTime, endTime;
            var isDateCorrect = DateTime.TryParse(answerContainer.startTime, out startTime);
            isDateCorrect = isDateCorrect && DateTime.TryParse(answerContainer.endTime, out endTime);

            if (!isDateCorrect)
            {
                throw new InvalidOperationException("Date is not in correct format");
            }

            var existingPostQuizResult = QuizResultModel.GetQuizResultsByQuizIds(new List<int> {quizResult.Quiz.Id});
            var existing = existingPostQuizResult.FirstOrDefault(x => x.ACEmail == quizResult.ACEmail && x.Quiz.IsPostQuiz);
            if (existing!=null)
                return new OfflineQuizResultDTO() { errorMessage = "You have already passed this Quiz!" };

            var acSession = ACSessionModel.GetOneById(quizResult.ACSessionId);
            var quizEventMapping = EventQuizMappingModel.GetOneById(quizResult.EventQuizMapping.Id).Value;
            if (quizEventMapping.PostQuiz == null)
                throw new InvalidOperationException("Post quiz can't be null in mapping");
            var postQuizId = quizEventMapping.PostQuiz.Id;
            var postQuizResult = new QuizResult
            {
                isCompleted = true,
                StartTime = startTime,
                EndTime = startTime,
                ACEmail = quizResult.ACEmail,
                Email = quizResult.Email,
                ACSessionId = quizResult.ACSessionId,
                ParticipantName = quizResult.ParticipantName,
                Quiz = QuizModel.GetOneById(postQuizId).Value,
                EventQuizMapping = quizEventMapping,
                DateCreated = DateTime.Now,
                Guid = Guid.NewGuid()
            };

            var quizPassingScoreInPercents = (float)postQuizResult.Quiz.PassingScore / 100;
            var totalQuestions = postQuizData.questions.Length;
            postQuizResult.Score = CalcScoreAndSaveQuestionResult(answerContainer.answers, postQuizData, quizResult);
            var scoreInPercents = (float)postQuizResult.Score / totalQuestions;

            QuizResultModel.RegisterSave(postQuizResult);

            var isSuccess = scoreInPercents >= quizPassingScoreInPercents;
            var resultDto = new OfflineQuizResultDTO()
            {
                score = (int)(scoreInPercents * 100),
                isSuccess = isSuccess,
            };
            if (isSuccess)
            {
                resultDto.certificatePreviewUrl = $"{Settings.CertificatesUrl}/QuizCertificate/Preview?quizResultGuid={postQuizResult.Guid}";
                resultDto.certificateDownloadUrl = $"{Settings.CertificatesUrl}/QuizCertificate/Download?quizResultGuid={postQuizResult.Guid}";
            }

            return resultDto;
        }

        private int CalcScoreAndSaveQuestionResult(OfflineQuizAnswerDTO[] answers, QuizDataDTO quizData, QuizResult quizResult)
        {
            var score = 0;
            foreach (var answer in answers)
            {
                var questionResult = new QuizQuestionResult();
                var question = quizData.questions.First(x => x.questionId == answer.questionId);
                //var distractors = quizData.questions.Where(x => x.questionId == answer.questionId).ToList();
                var distractors = DistractorModel.GetAllByQuestionId(answer.questionId).ToList();
                questionResult.Question = question.question;
                var questionObj = QuestionModel.GetOneById(question.questionId).Value;
                questionResult.QuestionRef = questionObj;
                questionResult.QuestionType = questionObj.QuestionType;
                questionResult.QuizResult = quizResult;

                if (answer.trueFalseAnswer != null)
                {
                    var shouldBeOneDistractor = distractors.FirstOrDefault();
                    if (shouldBeOneDistractor == null)
                        throw new InvalidOperationException("There should be a distractor for true/false question");
                    var isCorrect = answer.trueFalseAnswer.answer == (shouldBeOneDistractor.IsCorrect ?? false);
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
                    var distractor = distractors.FirstOrDefault(x => x.Id == quizDistractorAnswer.Id);
                    if (distractor == null)
                        throw new InvalidOperationException("How come answered distractor id is not present in question distractors???");
                    var isCorrect = distractor.IsCorrect ?? false;
                    if (isCorrect)
                    {
                        score++;
                    }
                    questionResult.IsCorrect = isCorrect;

                    QuizQuestionResultModel.RegisterSave(questionResult, true);
                    QuizQuestionResultAnswerModel.RegisterSave(quizQuestionResultAnswer);
                }

                if (answer.multiChoiceAnswer != null)
                {
                    var correctDistractors = distractors.Where(x => x.IsCorrect ?? false).Select(x => x.Id);
                    var isCorrect = correctDistractors.SequenceEqual(answer.multiChoiceAnswer.answeredDistractorIds);
                    if (isCorrect)
                    {
                        score++;
                    }
                    questionResult.IsCorrect = isCorrect;

                    QuizQuestionResultModel.RegisterSave(questionResult, true);

                    foreach (var answeredDistractorId in answer.multiChoiceAnswer.answeredDistractorIds)
                    {
                        var distrator = distractors.First(x => x.Id == answeredDistractorId);
                        var quizQuestionResultAnswer = new QuizQuestionResultAnswer()
                        {
                            QuizQuestionResult = questionResult,
                            Value = distrator.DistractorName,
                            QuizDistractorAnswer = distrator
                        };
                        QuizQuestionResultAnswerModel.RegisterSave(quizQuestionResultAnswer);
                    }
                }
            }

            return score;
        }
    }
}
