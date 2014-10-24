namespace EdugameCloud.Lti.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Utils;

    using NHibernate.Mapping;

    /// <summary>
    /// The quiz converter.
    /// </summary>
    public class QuizConverter
    {
        /// <summary>
        ///     Gets the quiz model.
        /// </summary>
        private QuizModel QuizModel
        {
            get
            {
                return IoC.Resolve<QuizModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private QuestionForSingleMultipleChoiceModel QuestionForSingleMultipleChoiceModel
        {
            get
            {
                return IoC.Resolve<QuestionForSingleMultipleChoiceModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private SubModuleCategoryModel SubModuleCategoryModel
        {
            get
            {
                return IoC.Resolve<SubModuleCategoryModel>();
            }
        }

        /// <summary>
        /// Gets the lms provider model.
        /// </summary>
        private LmsProviderModel LmsProviderModel
        {
            get
            {
                return IoC.Resolve<LmsProviderModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private SubModuleModel SubModuleModel
        {
            get
            {
                return IoC.Resolve<SubModuleModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private SubModuleItemModel SubModuleItemModel
        {
            get
            {
                return IoC.Resolve<SubModuleItemModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private QuestionForTrueFalseModel QuestionForTrueFalseModel
        {
            get
            {
                return IoC.Resolve<QuestionForTrueFalseModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private LmsQuestionTypeModel LmsQuestionTypeModel
        {
            get
            {
                return IoC.Resolve<LmsQuestionTypeModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private QuestionModel QuestionModel
        {
            get
            {
                return IoC.Resolve<QuestionModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private QuestionTypeModel QuestionTypeModel
        {
            get
            {
                return IoC.Resolve<QuestionTypeModel>();
            }
        }

        /// <summary>
        /// Gets the quiz format model.
        /// </summary>
        private QuizFormatModel QuizFormatModel
        {
            get
            {
                return IoC.Resolve<QuizFormatModel>();
            }
        }

        /// <summary>
        /// Gets the score type model.
        /// </summary>
        private ScoreTypeModel ScoreTypeModel
        {
            get
            {
                return IoC.Resolve<ScoreTypeModel>();
            }
        }

        /// <summary>
        /// Gets the course api.
        /// </summary>
        private CourseAPI CourseAPI
        {
            get
            {
                return IoC.Resolve<CourseAPI>();
            }
        }

        /// <summary>
        /// Gets the distractor model.
        /// </summary>
        private DistractorModel DistractorModel
        {
            get
            {
                return IoC.Resolve<DistractorModel>();
            }
        }

        /// <summary>
        /// Gets the user model.
        /// </summary>
        private UserModel UserModel
        {
            get
            {
                return IoC.Resolve<UserModel>();
            }
        }

        /// <summary>
        /// Gets the company lms model.
        /// </summary>
        private CompanyLmsModel CompanyLmsModel
        {
            get
            {
                return IoC.Resolve<CompanyLmsModel>();
            }
        }

        /// <summary>
        /// The convert quizzes.
        /// </summary>
        /// <param name="quizzes">
        /// The quizzes.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        public void ConvertQuizzes(IEnumerable<LmsQuizDTO> quizzes, User user)
        {
            foreach (var quiz in quizzes)
            {
                if (quiz.questions.Length > 0)
                {
                    this.ConvertQuiz(quiz, user);
                }
            }
        }

        /// <summary>
        /// The convert quiz.
        /// </summary>
        /// <param name="quiz">
        /// The quiz.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        private void ConvertQuiz(LmsQuizDTO quiz, User user)
        {
            var egcQuiz = this.QuizModel.GetOneByLmsQuizId(user.Id, quiz.id, (int)LmsProviderEnum.Canvas).Value
                      ?? new Quiz();

            var submodule = this.ProcessSubModule(user, egcQuiz.IsTransient() ? null : egcQuiz.SubModuleItem, quiz);

            var result = this.ProcessQuizData(quiz, egcQuiz, submodule);

            this.ProcessQuizQuestions(quiz, user, submodule);
        }

        /// <summary>
        /// The process sub module.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="lmsQuiz">
        /// The lms quiz.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItem"/>.
        /// </returns>
        private SubModuleItem ProcessSubModule(User user, SubModuleItem item, LmsQuizDTO lmsQuiz)
        {
            var submodule = item ??
                new SubModuleItem()
                {
                    DateCreated = DateTime.Now,
                    CreatedBy = user
                };

            submodule.IsActive = true;
            submodule.IsShared = true;
            submodule.DateModified = DateTime.Now;
            submodule.ModifiedBy = user;

            var subModuleCategoryModel = this.SubModuleCategoryModel;
            submodule.SubModuleCategory = subModuleCategoryModel.GetOneByLmsCourseId(lmsQuiz.id).Value
                                          ?? new SubModuleCategory
                                          {
                                              CategoryName = lmsQuiz.title,
                                              LmsCourseId = lmsQuiz.id,
                                              User = user,
                                              DateModified = DateTime.Now,
                                              IsActive = true,
                                              ModifiedBy = user,
                                              SubModule = SubModuleModel.GetOneById((int)SubModuleItemType.Quiz).Value
                                          };
            if (submodule.SubModuleCategory.IsTransient())
            {
                subModuleCategoryModel.RegisterSave(submodule.SubModuleCategory);
            }

            this.SubModuleItemModel.RegisterSave(submodule);
            return submodule;
        }

        /// <summary>
        /// The process quiz data.
        /// </summary>
        /// <param name="quiz">
        /// The quiz.
        /// </param>
        /// <param name="egcQuiz">
        /// The egc quiz.
        /// </param>
        /// <param name="submoduleItem">
        /// The submodule item.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        private Tuple<int, int> ProcessQuizData(LmsQuizDTO quiz, Quiz egcQuiz, SubModuleItem submoduleItem)
        {
            egcQuiz.LmsQuizId = quiz.id;
            egcQuiz.QuizName = quiz.title;
            egcQuiz.SubModuleItem = submoduleItem;
            egcQuiz.Description = Regex.Replace(quiz.description, "<[^>]*(>|$)", string.Empty);
            egcQuiz.ScoreType = this.ScoreTypeModel.GetOneById(1).Value;
            egcQuiz.QuizFormat = this.QuizFormatModel.GetOneById(1).Value;
            egcQuiz.LmsProvider = LmsProviderModel.GetOneById((int)LmsProviderEnum.Canvas).Value;
            egcQuiz.SubModuleItem.IsShared = true;

            this.QuizModel.RegisterSave(egcQuiz, true);
            var result = new Tuple<int, int>(submoduleItem.Id, egcQuiz.Id);
            if (!egcQuiz.IsTransient())
            {
                var questionData = this.QuestionModel.GetAllBySubModuleItemId(submoduleItem.Id);
                foreach (var question in questionData)
                {
                    question.IsActive = false;
                    this.QuestionModel.RegisterSave(question);
                    var questionsDistractors = this.DistractorModel.GetAllByQuestionId(question.Id);
                    foreach (var d in questionsDistractors)
                    {
                        d.IsActive = false;
                        this.DistractorModel.RegisterSave(d);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The process quiz questions.
        /// </summary>
        /// <param name="quiz">
        /// The quiz.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="submodule">
        /// The submodule.
        /// </param>
        private void ProcessQuizQuestions(LmsQuizDTO quiz, User user, SubModuleItem submodule)
        {
            var qtypes = this.LmsQuestionTypeModel.GetAllByProvider((int)LmsProviderEnum.Canvas);

            foreach (var quizQuestion in quiz.questions.Where(qs => qs.question_type != null))
            {
                var questionType = qtypes.FirstOrDefault(qt => qt.LmsQuestionTypeName.Equals(quizQuestion.question_type));
                if (questionType == null)
                {
                    continue;
                }

                var lmsQuestionId = quizQuestion.id;

                var question = this.QuestionModel.GetOneBySubmoduleItemIdAndLmsId(submodule.Id, lmsQuestionId).Value ??
                        new Question
                        {
                            SubModuleItem = submodule,
                            DateCreated = DateTime.Now,
                            CreatedBy = user,
                            LmsQuestionId = lmsQuestionId
                        };
                question.QuestionName = quizQuestion.question_text;
                question.QuestionType = questionType.QuestionType;
                question.DateModified = DateTime.Now;
                question.ModifiedBy = user;
                question.IsActive = true;
                var isTransient = question.Id == 0;

                this.QuestionModel.RegisterSave(question);

                if (isTransient)
                {
                    switch (question.QuestionType.Id)
                    {
                        case (int)QuestionTypeEnum.TrueFalse:
                            this.QuestionForTrueFalseModel.RegisterSave(
                                new QuestionForTrueFalse { Question = question });
                            break;
                        case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                            this.QuestionForSingleMultipleChoiceModel.RegisterSave(
                                new QuestionForSingleMultipleChoice { Question = question });
                            break;
                    }
                }

                this.ProcessDistractors(user, questionType.QuestionType, quizQuestion, question);
            }
        }

        /// <summary>
        /// The process distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="qtype">
        /// The qtype.
        /// </param>
        /// <param name="q">
        /// The q.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        private void ProcessDistractors(User user, QuestionType qtype, QuizQuestionDTO q, Question question)
        {
            var disctarctorModel = this.DistractorModel;
            switch (qtype.Id)
            {
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                    {
                        this.ProcessSingleMultipleChoiceTextDistractors(user, q, question, disctarctorModel);
                        break;
                    }

                case (int)QuestionTypeEnum.TrueFalse:
                    {
                        this.ProcessQuestionForTrueFalseDistractors(user, q, question, disctarctorModel);
                        break;
                    }

            }
        }

        /// <summary>
        /// The process single multiple choice text distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="q">
        /// The q.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="distractorModel">
        /// The distractor model.
        /// </param>
        private void ProcessSingleMultipleChoiceTextDistractors(
            User user,
            QuizQuestionDTO q,
            Question question,
            DistractorModel distractorModel)
        {
            var answerNumber = question.IsMoodleSingle.GetValueOrDefault() ? 0 : 1; // singlechoice starts from 0, multichoice from 1
            foreach (var a in q.answers)
            {
                var lmsId = a.id;

                var distractor = distractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
                distractor.DateModified = DateTime.Now;
                distractor.ModifiedBy = user;
                distractor.DistractorName = a.text;
                distractor.IsActive = true;
                distractor.DistractorType = 1;
                distractor.IsCorrect = a.weight > 0;
                distractor.LmsAnswer = (answerNumber++).ToString();

                distractorModel.RegisterSave(distractor);
            }
        }

        /// <summary>
        /// The process question for true false distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="q">
        /// The q.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="distractorModel">
        /// The distractor model.
        /// </param>
        private void ProcessQuestionForTrueFalseDistractors(
            User user,
            QuizQuestionDTO q,
            Question question,
            DistractorModel distractorModel)
        {
            var lmsId = q.id;
            var distractor = distractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
            distractor.DateModified = DateTime.Now;
            distractor.ModifiedBy = user;
            distractor.DistractorName = "truefalse";
            distractor.IsActive = true;
            distractor.DistractorType = 1;

            var isCorrect = false;
            foreach (var a in q.answers)
            {
                var isCorrectAnswer = a.weight > 0;
                if (a.text != null && a.text.ToLower().Equals("true") && isCorrectAnswer)
                {
                    isCorrect = true;
                }
            }

            distractor.IsCorrect = isCorrect;
            distractorModel.RegisterSave(distractor);
        }
    }
}
