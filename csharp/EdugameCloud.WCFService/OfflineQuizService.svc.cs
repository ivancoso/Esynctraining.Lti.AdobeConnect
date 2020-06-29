using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.Core.Domain.DTO.OfflineQuiz;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.WCFService.Base;
using EdugameCloud.WCFService.Contracts;
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
        private QuizModel QuizModel => IoC.Resolve<QuizModel>();

        private CompanyEventQuizMappingModel EventQuizMappingModel => IoC.Resolve<CompanyEventQuizMappingModel>();

        private QuizResultModel QuizResultModel => IoC.Resolve<QuizResultModel>();

        private DistractorModel DistractorModel => IoC.Resolve<DistractorModel>();

        private QuizQuestionResultModel QuizQuestionResultModel => IoC.Resolve<QuizQuestionResultModel>();

        private QuizQuestionResultAnswerModel QuizQuestionResultAnswerModel => IoC.Resolve<QuizQuestionResultAnswerModel>();

        private QuestionModel QuestionModel => IoC.Resolve<QuestionModel>();


        public OfflineQuizDTO GetQuizByKey(string key)
        {
            var guid = Guid.Parse(key);
            var quizResult = QuizResultModel.GetOneByGuid(guid).Value;

            string usedEmail = !string.IsNullOrEmpty(quizResult.ACEmail) ? quizResult.ACEmail : quizResult.Email;

            var mapping = EventQuizMappingModel.GetOneById(quizResult.EventQuizMapping.Id).Value;

            var postQuizId = mapping.PostQuiz.Id;
            var postQuizResults = QuizResultModel.GetQuizResultsByQuizIds(new[] {postQuizId}.ToList());
            if (postQuizResults.Any(x => (x.ACEmail == usedEmail || x.Email == usedEmail) && x.Quiz.IsPostQuiz && x.EventQuizMapping.Id == mapping.Id))
            {
                return new OfflineQuizDTO
                {
                    errorMessage = "You have already passed this Quiz!",
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
                    distractors.Add(new OfflineDistractorDTO
                    {
                        questionId = quizDistractor.questionId,
                        distractor = quizDistractor.distractor,
                        distractorOrder = quizDistractor.distractorOrder,
                        distractorId = quizDistractor.distractorId,
                        distractorType = quizDistractor.distractorType,
                    });
                }
                questions.Add(new OfflineQuestionDTO
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
                    imageVO = question.imageVO,
                });
            }
            result.questions = questions.ToArray();
            result.description = postQuiz.quizVO.description;
            result.quizName = postQuiz.quizVO.quizName;
            //var quizResultObj = QuizResultModel.GetOneById(postQuiz.quizVO.quizId).Value;

            result.participant = new ParticipantDTO
            {
                email = usedEmail,
                participantName = quizResult.ParticipantName,
            };
            return result;
        }

        public OfflineQuizResultDTO SendAnswers(OfflineQuizAnswerContainerDTO answerContainer)
        {
            var quizResultGuid = answerContainer.quizResultGuid;
            var quizResult = QuizResultModel.GetOneByGuid(quizResultGuid).Value;
            var mapping = EventQuizMappingModel.GetOneById(quizResult.EventQuizMapping.Id).Value;
            var postQuizId = mapping.PostQuiz.Id;
            var postQuizData = QuizModel.getQuizDataByQuizID(postQuizId);

            var isDateCorrect = DateTime.TryParse(answerContainer.startTime, out DateTime startTime)
                && DateTime.TryParse(answerContainer.endTime, out DateTime endTime);
            if (!isDateCorrect)
            {
                throw new InvalidOperationException("Date is not in correct format");
            }

            var existingPostQuizResult = QuizResultModel.GetQuizResultsByQuizIds(new List<int> { postQuizId });
            var existing = existingPostQuizResult.FirstOrDefault(x => x.ACEmail == quizResult.ACEmail && x.Quiz.IsPostQuiz && x.EventQuizMapping.Id == mapping.Id);
            if (existing != null)
                return new OfflineQuizResultDTO { errorMessage = "You have already passed this Quiz!" };

            //var acSession = ACSessionModel.GetOneById(quizResult.ACSessionId);
            var quizEventMapping = EventQuizMappingModel.GetOneById(quizResult.EventQuizMapping.Id).Value;
            if (quizEventMapping.PostQuiz == null)
                throw new InvalidOperationException("Post quiz can't be null in mapping");
            var postQuizResult = new QuizResult
            {
                isCompleted = true,
                StartTime = startTime,
                EndTime = startTime,
                ACEmail = quizResult.ACEmail,
                Email = quizResult.Email,
                ACSessionId = quizResult.ACSessionId,
                ParticipantName = quizResult.ParticipantName,
                Quiz = mapping.PostQuiz,
                EventQuizMapping = quizEventMapping,
                DateCreated = DateTime.Now,
                Guid = Guid.NewGuid(),
            };

            var quizPassingScoreInPercents = (float)postQuizResult.Quiz.PassingScore / 100;
            var totalQuestions = postQuizData.questions.Length;
            postQuizResult.Score = CalcScore(answerContainer.answers);
            
            var scoreInPercents = (float)postQuizResult.Score / totalQuestions;

            QuizResultModel.RegisterSave(postQuizResult);
            SaveQuestionResult(answerContainer.answers, postQuizData, postQuizResult);

            var isSuccess = scoreInPercents >= quizPassingScoreInPercents;
            var resultDto = new OfflineQuizResultDTO
            {
                score = (int) Math.Round(scoreInPercents * 100),
                isSuccess = isSuccess,
            };
            if (isSuccess)
            {
                resultDto.certificatePreviewUrl = postQuizResult.BuildPreviewUrl(Settings.CertificatesUrl);
                resultDto.certificateDownloadUrl = postQuizResult.BuildDownloadUrl(Settings.CertificatesUrl);

                var emailService = new EmailService();
                emailService.SendCertificate(postQuizResult.Guid.ToString());
                //emailService.SendCertificateToTeacher(postQuizResult.Guid.ToString());
            }

            return resultDto;
        }

        private int CalcScore(OfflineQuizAnswerDTO[] answers)
        {
            var score = 0;
            foreach (var answer in answers)
            {
                var distractors = DistractorModel.GetAllByQuestionId(answer.questionId).ToList();

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
                }

                if (answer.singleChoiceAnswer != null)
                {
                    var quizDistractorAnswer = DistractorModel.GetOneById(answer.singleChoiceAnswer.answeredDistractorId).Value;
                    var distractor = distractors.FirstOrDefault(x => x.Id == quizDistractorAnswer.Id);
                    if (distractor == null)
                        throw new InvalidOperationException("How come answered distractor id is not present in question distractors???");
                    var isCorrect = distractor.IsCorrect ?? false;
                    if (isCorrect)
                    {
                        score++;
                    }
                }

                if (answer.multiChoiceAnswer != null)
                {
                    var correctDistractors = distractors.Where(x => x.IsCorrect ?? false).Select(x => x.Id);
                    var isCorrect = correctDistractors.SequenceEqual(answer.multiChoiceAnswer.answeredDistractorIds);
                    if (isCorrect)
                    {
                        score++;
                    }
                }
            }

            return score;

        }

        private int SaveQuestionResult(OfflineQuizAnswerDTO[] answers, QuizDataDTO quizData, QuizResult postQuizResult)
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
                questionResult.QuizResult = postQuizResult;

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

                    QuizQuestionResultModel.RegisterSave(questionResult, true);
                }

                if (answer.singleChoiceAnswer != null)
                {
                    var quizDistractorAnswer = DistractorModel.GetOneById(answer.singleChoiceAnswer.answeredDistractorId).Value;
                    var quizQuestionResultAnswer = new QuizQuestionResultAnswer
                    {
                        QuizQuestionResult = questionResult,
                        Value = quizDistractorAnswer.DistractorName,
                        QuizDistractorAnswer = quizDistractorAnswer,
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
                        var quizQuestionResultAnswer = new QuizQuestionResultAnswer
                        {
                            QuizQuestionResult = questionResult,
                            Value = distrator.DistractorName,
                            QuizDistractorAnswer = distrator,
                        };

                        QuizQuestionResultAnswerModel.RegisterSave(quizQuestionResultAnswer);
                    }
                }
            }

            return score;
        }

    }

}
