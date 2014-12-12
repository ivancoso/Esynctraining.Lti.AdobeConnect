namespace EdugameCloud.WCFService.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.EntityParsing;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using Newtonsoft.Json;

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
        /// Gets the survey model.
        /// </summary>
        private SurveyModel SurveyModel
        {
            get
            {
                return IoC.Resolve<SurveyModel>();
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
        /// Gets the question for rate model.
        /// </summary>
        private QuestionForRateModel QuestionForRateModel
        {
            get
            {
                return IoC.Resolve<QuestionForRateModel>();
            }
        }

        /// <summary>
        /// Gets the question for open answer model.
        /// </summary>
        private QuestionForOpenAnswerModel QuestionForOpenAnswerModel
        {
            get
            {
                return IoC.Resolve<QuestionForOpenAnswerModel>();
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
        /// Gets the survey grouping type model.
        /// </summary>
        private SurveyGroupingTypeModel SurveyGroupingTypeModel
        {
            get
            {
                return IoC.Resolve<SurveyGroupingTypeModel>();
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
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <param name="companyLmsId">
        /// The company Lms Id.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<int, int> ConvertQuizzes(IEnumerable<LmsQuizDTO> quizzes, User user, bool isSurvey, int companyLmsId)
        {
            var submoduleQuiz = new Dictionary<int, int>();
            
            foreach (var quiz in quizzes)
            {
                if (quiz.questions.Length > 0)
                {
                    var submoduleCategory = this.ProcessSubModuleCategory(quiz, user, companyLmsId);
                    var item = this.ConvertQuiz(quiz, user, submoduleCategory, isSurvey, companyLmsId);
                    if (!submoduleQuiz.ContainsKey(item.Item1))
                    {
                        submoduleQuiz.Add(item.Item1, item.Item2);
                    }
                }
            }

            return submoduleQuiz;
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
        /// <param name="subModuleCategory">
        /// The sub Module Category.
        /// </param>
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <param name="companyLmsId">
        /// The company lms Id.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        private Tuple<int, int> ConvertQuiz(LmsQuizDTO quiz, User user, SubModuleCategory subModuleCategory, bool isSurvey, int companyLmsId)
        {
            SubModuleItem submodule;
            Tuple<int, int> result;

            if (isSurvey)
            {
                var egcSurvey = this.SurveyModel.GetOneByLmsSurveyId(user.Id, quiz.id, companyLmsId).Value
                    ?? new Survey();


                submodule = this.ProcessSubModule(user, subModuleCategory, egcSurvey.IsTransient() ? null : egcSurvey.SubModuleItem, quiz);

                result = this.ProcessSurveyData(quiz, egcSurvey, submodule);
            }
            else
            {
                var egcQuiz = this.QuizModel.GetOneByLmsQuizId(user.Id, quiz.id, companyLmsId).Value
                    ?? new Quiz();


                submodule = this.ProcessSubModule(user, subModuleCategory, egcQuiz.IsTransient() ? null : egcQuiz.SubModuleItem, quiz);

                result = this.ProcessQuizData(quiz, egcQuiz, submodule);
            }

            this.ProcessQuizQuestions(quiz, user, submodule, isSurvey, companyLmsId);

            return result;
        }

        /// <summary>
        /// The process sub module category.
        /// </summary>
        /// <param name="quiz">
        /// The quiz.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="companyLmsId">
        /// The company Lms Id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategory"/>.
        /// </returns>
        private SubModuleCategory ProcessSubModuleCategory(LmsQuizDTO quiz, User user, int companyLmsId)
        {
            var subModuleCategoryModel = this.SubModuleCategoryModel;
            var subModuleCategory = subModuleCategoryModel.GetOneByLmsCourseIdAndCompanyLms(quiz.course, companyLmsId).Value
                                          ?? new SubModuleCategory
                                          {
                                              CompanyLmsId = this.CompanyLmsModel.GetOneById(companyLmsId).Value.With(x => x.Id),
                                              CategoryName = quiz.courseName,
                                              LmsCourseId = quiz.course,
                                              User = user,
                                              DateModified = DateTime.Now,
                                              IsActive = true,
                                              ModifiedBy = user,
                                              SubModule = this.SubModuleModel.GetOneById((int)SubModuleItemType.Quiz).Value
                                          };
            if (subModuleCategory.IsTransient())
            {
                subModuleCategoryModel.RegisterSave(subModuleCategory);
            }

            return subModuleCategory;
        }

        /// <summary>
        /// The process sub module.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="subModuleCategory">
        /// The sub Module Category.
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
        private SubModuleItem ProcessSubModule(User user, SubModuleCategory subModuleCategory, SubModuleItem item, LmsQuizDTO lmsQuiz)
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
            submodule.SubModuleCategory = subModuleCategory;

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
            egcQuiz.QuizName = this.ClearName(quiz.title);
            egcQuiz.SubModuleItem = submoduleItem;
            egcQuiz.Description = this.ClearName(Regex.Replace(quiz.description, "<[^>]*(>|$)", string.Empty));
            egcQuiz.ScoreType = this.ScoreTypeModel.GetOneById(1).Value;
            egcQuiz.QuizFormat = this.QuizFormatModel.GetOneById(1).Value;
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
        /// The process survey data.
        /// </summary>
        /// <param name="quiz">
        /// The quiz.
        /// </param>
        /// <param name="egcSurvey">
        /// The egc survey.
        /// </param>
        /// <param name="submoduleItem">
        /// The submodule item.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        private Tuple<int, int> ProcessSurveyData(LmsQuizDTO quiz, Survey egcSurvey, SubModuleItem submoduleItem)
        {
            egcSurvey.LmsSurveyId = quiz.id;
            egcSurvey.SurveyName = this.ClearName(quiz.title);
            egcSurvey.SubModuleItem = submoduleItem;
            egcSurvey.Description = this.ClearName(Regex.Replace(quiz.description, "<[^>]*(>|$)", string.Empty));
            egcSurvey.SubModuleItem.IsShared = true;
            egcSurvey.SurveyGroupingType = this.SurveyGroupingTypeModel.GetOneById(1).Value;

            this.SurveyModel.RegisterSave(egcSurvey, true);
            var result = new Tuple<int, int>(submoduleItem.Id, egcSurvey.Id);
            if (!egcSurvey.IsTransient())
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
        /// The clear name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ClearName(string name)
        {
            return Regex.Replace(name ?? string.Empty, "<[^>]*(>|$)", string.Empty).Replace("&nbsp;", " ");
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
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <param name="companyLmsId">
        /// The company Lms Id.
        /// </param>
        private void ProcessQuizQuestions(LmsQuizDTO quiz, User user, SubModuleItem submodule, bool isSurvey, int companyLmsId)
        {
            var companyLms = this.CompanyLmsModel.GetOneById(companyLmsId).Value;
            var qtypes = this.LmsQuestionTypeModel.GetAllByProvider(companyLms.LmsProvider.Id).ToList();

            foreach (var quizQuestion in quiz.questions.Where(qs => qs.question_type != null))
            {
                bool isNumeric = false;
                var questionType = qtypes.FirstOrDefault(qt => qt.LmsQuestionTypeName.Equals(quizQuestion.question_type));
                if (questionType == null)
                {
                    continue;
                }

                if (isSurvey && (companyLms.LmsProvider.Id == (int)LmsProviderEnum.Moodle))
                {
                    var separatorIndex = quizQuestion.presentation.IndexOf(">>>>>", System.StringComparison.Ordinal);
                    string text = quizQuestion.question_text ?? quizQuestion.question_name,
                        answers = separatorIndex > 0
                            ? quizQuestion.presentation.Substring(separatorIndex + 5)
                            : string.Empty,
                        type = separatorIndex > 0 ? quizQuestion.presentation.Substring(0, separatorIndex) : string.Empty;
                    if (type.Equals("d"))
                    {
                        questionType = new LmsQuestionType()
                                           {
                                               QuestionTypeId = (int)QuestionTypeEnum.Rate,
                                               LmsQuestionTypeName = string.Empty
                                           };
                            
                    }
                    if (questionType.QuestionTypeId == (int)QuestionTypeEnum.Numerical)
                    {
                        isNumeric = true;
                        questionType = new LmsQuestionType()
                        {
                            QuestionTypeId = (int)QuestionTypeEnum.OpenAnswerSingleLine,
                            LmsQuestionTypeName = string.Empty
                        };
                    }

                    quizQuestion.is_single = !type.Equals("c");

                    quizQuestion.question_text = this.ClearName(text);
                    quizQuestion.answers = answers.Split('|').Select((a, i) => new AnswerDTO()
                    {
                        text = a.IndexOf("####", System.StringComparison.Ordinal) > -1 ? a.Substring(a.IndexOf("####", System.StringComparison.Ordinal) + 4) : a,
                        id = i
                    }).ToList();
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

                string questionText = null;

                if (questionType.QuestionTypeId == (int)QuestionTypeEnum.MultipleDropdowns
                    || questionType.QuestionTypeId == (int)QuestionTypeEnum.FillInTheBlank)
                {
                    questionText = this.ProcessFillInTheBlankQuestionText(quizQuestion, (LmsProviderEnum)companyLms.LmsProvider.Id);
                }
                else if (questionType.QuestionTypeId == (int)QuestionTypeEnum.Calculated
                    || questionType.QuestionTypeId == (int)QuestionTypeEnum.CalculatedMultichoice)
                {
                    questionText = this.ProcessCalculatedQuestionText(quizQuestion, (LmsProviderEnum)companyLms.LmsProvider.Id);
                }
                else if (questionType.QuestionTypeId == (int)QuestionTypeEnum.OpenAnswerSingleLine && isNumeric)
                {
                    questionText = this.ClearName(quizQuestion.question_text ?? quizQuestion.question_name);
                    if (quizQuestion.presentation != null && quizQuestion.presentation.IndexOf("|") > -1)
                    {
                        var presentationIndex = quizQuestion.presentation.IndexOf("|");
                        questionText = string.Format(
                            "{0} ({1}-{2})", 
                            questionText, 
                            quizQuestion.presentation.Substring(0, presentationIndex),
                            quizQuestion.presentation.Substring(presentationIndex + 1));
                    }
                }
                else
                {
                    questionText = this.ClearName(quizQuestion.question_text ?? quizQuestion.question_name);
                }

                question.IsMoodleSingle = !questionType.LmsQuestionTypeName.Equals("multiple_answers_question"); 
                question.QuestionName = questionText;
                question.QuestionType = this.QuestionTypeModel.GetOneById(questionType.QuestionTypeId).Value;
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
                                new QuestionForTrueFalse { Question = question, IsMandatory = quizQuestion.is_mandatory });
                            break;
                        case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                            this.QuestionForSingleMultipleChoiceModel.RegisterSave(
                                new QuestionForSingleMultipleChoice
                                    {
                                        Question = question, 
                                        Restrictions = !quizQuestion.is_single ? "multi_choice" : null,
                                        IsMandatory = quizQuestion.is_mandatory
                                    });
                            break;
                        case (int)QuestionTypeEnum.OpenAnswerSingleLine:
                        case (int)QuestionTypeEnum.OpenAnswerMultiLine:
                            this.QuestionForOpenAnswerModel.RegisterSave(
                                new QuestionForOpenAnswer
                                {
                                    Question = question,
                                    IsMandatory = quizQuestion.is_mandatory
                                });
                            break;
                        case (int)QuestionTypeEnum.Rate:
                            this.QuestionForRateModel.RegisterSave(
                                new QuestionForRate
                                    {
                                        Question = question, 
                                        Restrictions = quizQuestion.is_single ? "single_choice" : string.Empty, 
                                        AllowOther = false, 
                                        PageNumber = 0,
                                        IsAlwaysRateDropdown = true,
                                        IsMandatory = quizQuestion.is_mandatory
                                    });
                            break;
                    }
                }

                this.ProcessDistractors(user, questionType.QuestionTypeId, quizQuestion, question, (LmsProviderEnum)companyLms.LmsProvider.Id);

                if (isSurvey)
                {
                    if (question.QuestionType.Id == (int)QuestionTypeEnum.Calculated
                        || question.QuestionType.Id == (int)QuestionTypeEnum.Numerical)
                    {
                        question.QuestionType =
                            this.QuestionTypeModel.GetOneById((int)QuestionTypeEnum.OpenAnswerSingleLine).Value;
                        this.QuestionModel.RegisterSave(question);
                    }
                    if (question.QuestionType.Id == (int)QuestionTypeEnum.Essay)
                    {
                        question.QuestionType =
                            this.QuestionTypeModel.GetOneById((int)QuestionTypeEnum.OpenAnswerMultiLine).Value;
                        this.QuestionModel.RegisterSave(question);
                    }
                    if (question.QuestionType.Id == (int)QuestionTypeEnum.ShortAnswer)
                    {
                        question.QuestionType =
                            this.QuestionTypeModel.GetOneById((int)QuestionTypeEnum.OpenAnswerMultiLine).Value;
                        this.QuestionModel.RegisterSave(question);
                    }
                }
            }
        }

        /// <summary>
        /// The process distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="qtypeId">
        /// The question type Id.
        /// </param>
        /// <param name="q">
        /// The question.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="lmsProvider">
        /// The lms Provider.
        /// </param>
        private void ProcessDistractors(User user, int qtypeId, QuizQuestionDTO q, Question question, LmsProviderEnum lmsProvider)
        {
            var disctarctorModel = this.DistractorModel;
            switch (qtypeId)
            {
                case (int)QuestionTypeEnum.ShortAnswer:
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                case (int)QuestionTypeEnum.Rate:
                    {
                        this.ProcessSingleMultipleChoiceTextDistractors(user, q, question, disctarctorModel);
                        break;
                    }

                case (int)QuestionTypeEnum.MultipleDropdowns:
                    {
                        this.ProcessFillInTheBlankDistractors(user, q, question, true, lmsProvider);
                        break;
                    }
                case (int)QuestionTypeEnum.FillInTheBlank:
                    {
                        this.ProcessFillInTheBlankDistractors(user, q, question, false, lmsProvider);
                        break;
                    }
                case (int)QuestionTypeEnum.Matching:
                    {
                        this.ProcessMatchingDistractors(user, q, question);
                        break;
                    }
                case (int)QuestionTypeEnum.TrueFalse:
                    {
                        this.ProcessQuestionForTrueFalseDistractors(user, q, question, disctarctorModel);
                        break;
                    }
                case (int)QuestionTypeEnum.Numerical:
                    {
                        this.ProcessNumericalDistractors(user, q, question);
                        break;
                    }
                case (int)QuestionTypeEnum.CalculatedMultichoice:
                case (int)QuestionTypeEnum.Calculated:
                    {
                        this.ProcessCalculatedDistractors(user, q, question, lmsProvider);
                        break;
                    }
            }
        }

        /// <summary>
        /// The process fill in the blank question text.
        /// </summary>
        /// <param name="q">
        /// The q.
        /// </param>
        /// <param name="lmsProvider">
        /// The lms provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ProcessFillInTheBlankQuestionText(QuizQuestionDTO q, LmsProviderEnum lmsProvider)
        {
            switch (lmsProvider)
            {
                case LmsProviderEnum.Canvas:
                return this.ProcessFillInTheBlankQuestionTextCanvas(q);
                case LmsProviderEnum.Moodle:
                return this.ProcessFillInTheBlankQuestionTextMoodle(q);
            }
            return string.Empty;
        }

        /// <summary>
        /// The process fill in the blank question text moodle.
        /// </summary>
        /// <param name="q">
        /// The q.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ProcessFillInTheBlankQuestionTextMoodle(QuizQuestionDTO q)
        {
            var i = 1;
            var questionText = this.ClearName(q.question_text);
            foreach (var a in q.answers)
            {
                if (a.question_text == null) continue;
                var index = a.question_text.IndexOf(":=");
                if (index < 0) continue;
                var text = a.question_text.Substring(index + 2, a.question_text.Length - index - 3);
                questionText = questionText.Replace("{#" + (i++) + "}", text);
            }

            return questionText;
        }

        /// <summary>
        /// The process fill in the blank question text.
        /// </summary>
        /// <param name="q">
        /// The q.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ProcessFillInTheBlankQuestionTextCanvas(QuizQuestionDTO q)
        {
            var questionText = this.ClearName(q.question_text);
            foreach (var a in q.answers)
            {
                if (a.weight < 100)
                {
                    continue;
                }

                var placeholder = string.Format("[{0}]", a.blank_id);
                questionText = questionText.Replace(placeholder, a.text);
            }

            return questionText;
        }

        /// <summary>
        /// The process fill in the blank distractors.
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
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="lmsProvider">
        /// The lms provider.
        /// </param>
        private void ProcessFillInTheBlankDistractors(
            User user,
            QuizQuestionDTO q,
            Question question,
            bool option,
            LmsProviderEnum lmsProvider)
        {
            switch (lmsProvider)
            {
                case LmsProviderEnum.Canvas:
                    this.ProcessFillInTheBlankDistractorsCanvas(user, q, question, option);
                    break;
                case LmsProviderEnum.Moodle:
                    this.ProcessFillInTheBlankDistractorsMoodle(user, q, question, option);
                    break;
            }
        }

        /// <summary>
        /// The process fill in the blank distractors moodle.
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
        /// <param name="option">
        /// The option.
        /// </param>
        private void ProcessFillInTheBlankDistractorsMoodle(
            User user,
            QuizQuestionDTO q,
            Question question,
            bool option)
        {
            string blankTrue = "<text id=\"0\" isBlank=\"true\">",
                blankFalse = "<text id=\"0\" isBlank=\"false\">",
                closing = "</text>";
            var distractorText = this.ClearName(q.question_text);
            if (!distractorText.StartsWith("{#"))
            {
                distractorText = blankTrue + distractorText;
            }

            distractorText = distractorText.Replace("{#", closing + blankTrue)
                .Replace("}", closing + blankFalse);
            if (distractorText.StartsWith(closing))
            {
                distractorText = "<data>" + distractorText.Substring(closing.Length);
            }
            else
            {
                distractorText = "<data>" + blankFalse + distractorText;
            }

            var i = 1;
            foreach (var a in q.answers)
            {
                if (a.question_text == null) continue;
                var index = a.question_text.IndexOf(":=");
                if (index < 0) continue;
                var text = a.question_text.Substring(index + 2, a.question_text.Length - index - 3);
                text = blankTrue + text + closing;
                distractorText = distractorText.Replace(blankTrue + (i++) + closing, text);
            }
            if (distractorText.EndsWith(blankFalse))
            {
                distractorText = distractorText.Substring(0, distractorText.Length - blankFalse.Length);
            }
            if (distractorText.EndsWith(blankTrue))
            {
                distractorText = distractorText.Substring(0, distractorText.Length - blankTrue.Length);
            }

            if (!distractorText.EndsWith(closing))
            {
                distractorText = distractorText + closing;
            }

            distractorText = distractorText + "</data>";

            var lmsId = q.id;

            var distractor = this.DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
            distractor.DateModified = DateTime.Now;
            distractor.ModifiedBy = user;
            distractor.DistractorName = distractorText;
            distractor.IsActive = true;
            distractor.DistractorType = null;
            distractor.IsCorrect = true;

            this.DistractorModel.RegisterSave(distractor);
        }

        /// <summary>
        /// The process fill in the blank distractors.
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
        /// <param name="option">
        /// The option.
        /// </param>
        private void ProcessFillInTheBlankDistractorsCanvas(
            User user,
            QuizQuestionDTO q,
            Question question,
            bool option)
        {
            var splitText = new List<string>();
            var questionText = this.ClearName(q.question_text);
            while (questionText.Length > 0)
            {
                int nextPlaceholderStart = questionText.IndexOf("["), nextPlaceholderEnd = questionText.IndexOf("]");
                if (nextPlaceholderEnd < 0 || nextPlaceholderStart < 0)
                {
                    splitText.Add(questionText);
                    break;
                }
                if (nextPlaceholderStart > 0)
                {
                    splitText.Add(questionText.Substring(0, nextPlaceholderStart));
                    questionText = questionText.Substring(nextPlaceholderStart);
                }
                splitText.Add(questionText.Substring(0, nextPlaceholderEnd - nextPlaceholderStart + 1));
                questionText = questionText.Substring(nextPlaceholderEnd - nextPlaceholderStart + 1);
            }

            var distractorText = new StringBuilder("<data>");
            var lmsText = new StringBuilder();
            var keys = new Dictionary<string, int>();
            int textId = 0, optionId = 0;
            
            foreach (var textPart in splitText)
            {
                if (textPart.StartsWith("[") && textPart.EndsWith("]"))
                {
                    var blank = textPart.Substring(1, textPart.Length - 2);
                    var options = q.answers.Where(a => a.blank_id != null && a.blank_id.Equals(blank)).ToList();
                    var correct = options.FirstOrDefault(o => o.weight == 100);
                    if (correct == null)
                    {
                        correct = options.FirstOrDefault();
                    }
                    if (correct != null)
                    {
                        distractorText.AppendFormat(
                            "<text id=\"{0}\" isBlank=\"true\">{1}</text>",
                            textId,
                            correct.text);
                    }

                    if (option)
                    {    
                        distractorText.AppendFormat("<options id=\"{0}\">", textId);
                        foreach (var o in options)
                        {
                            distractorText.AppendFormat(
                                "<option name=\"{0}\" lmsid=\"{1}\" />",
                                o.text,
                                o.id);
                        }
                        distractorText.Append("</options>");
                    }
                    keys.Add(blank, optionId++);

                    textId++;
                }
                else
                {
                    if (option)
                    {
                        distractorText.AppendFormat("<options id=\"{0}\" />", textId);
                    }
                    distractorText.AppendFormat("<text id=\"{0}\" isBlank=\"false\">{1}</text>", textId, textPart);
                    textId++;
                }
            }
            distractorText.Append("</data>");

            var distractor = this.DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, question.LmsQuestionId.GetValueOrDefault()).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = question.LmsQuestionId.GetValueOrDefault()
                    };
            distractor.DateModified = DateTime.Now;
            distractor.ModifiedBy = user;
            distractor.DistractorName = distractorText.ToString();
            distractor.IsActive = true;
            distractor.DistractorType = null;
            distractor.IsCorrect = true;
            distractor.LmsAnswer = JsonConvert.SerializeObject(keys);

            this.DistractorModel.RegisterSave(distractor);
            this.DistractorModel.Flush();
        }

        /// <summary>
        /// The process numerical distractors.
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
        private void ProcessNumericalDistractors(
            User user,
            QuizQuestionDTO q,
            Question question)
        {
            foreach (var a in q.answers)
            {
                var lmsId = a.id;
                bool isRange = a.numerical_answer_type != null && a.numerical_answer_type.Contains("range");
                var name = string.Format(
                    "{{\"min\":{0}, \"max\": {1}, \"error\":{2},\"type\":\"{3}\"}}",
                    isRange ? a.start.ToString() : (string.IsNullOrWhiteSpace(a.text) ? a.exact.ToString() : a.text),
                    string.IsNullOrWhiteSpace(a.text) ? a.end.ToString() : a.text,
                    a.margin,
                    isRange ? "Range" : "Exact");
                var distractor = this.DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
                distractor.DateModified = DateTime.Now;
                distractor.ModifiedBy = user;
                distractor.DistractorName = name;
                distractor.IsActive = true;
                distractor.DistractorType = 1;
                distractor.IsCorrect = a.weight == 100;

                this.DistractorModel.RegisterSave(distractor);
            }
        }

        /// <summary>
        /// The process calculated distractors.
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
        /// <param name="lmsProvider">
        /// The lms provider.
        /// </param>
        private void ProcessCalculatedDistractors(User user, QuizQuestionDTO q, Question question, LmsProviderEnum lmsProvider)
        {
            switch (lmsProvider)
            {
                case LmsProviderEnum.Canvas:
                    this.ProcessCalculatedDistractorsCanvas(user, q, question);
                break;
                case LmsProviderEnum.Moodle:
                    this.ProcessCalculatedDistractorsMoodle(user, q, question);
                break;
            }
        }

        /// <summary>
        /// The process calculated distractors moodle.
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
        private void ProcessCalculatedDistractorsMoodle(User user, QuizQuestionDTO q, Question question)
        {
            var answerNumber = question.IsMoodleSingle.GetValueOrDefault() ? 0 : 1; // singlechoice starts from 0, multichoice from 1
            foreach (var a in q.answers)
            {
                var expression = new MoodleExpressionParser(this.ClearName(a.text));
                foreach (var ds in q.datasets)
                {
                    var variable = ds.Name;
                    var value = ds.Items.Count > 0 ? ds.Items.First().Value : double.Parse(ds.Max ?? "0");
                    expression.SetValue(variable, value);
                }

                var result = expression.Calculate();

                var name = question.QuestionType.Id == (int)QuestionTypeEnum.CalculatedMultichoice
                               ? result.ToString()
                               : string.Format("{{\"val\":{0},\"error\":{1} }}", result, a.margin);

                var lmsId = a.id;
                var distractor = this.DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
                distractor.DateModified = DateTime.Now;
                distractor.ModifiedBy = user;
                distractor.DistractorName = name;
                distractor.IsActive = true;
                distractor.DistractorType = 1;
                distractor.IsCorrect = a.weight == 100;
                distractor.LmsAnswer = (answerNumber++).ToString();

                this.DistractorModel.RegisterSave(distractor);
            }
        }

        /// <summary>
        /// The process calculated distractors canvas.
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
        private void ProcessCalculatedDistractorsCanvas(
            User user,
            QuizQuestionDTO q,
            Question question)
        {
            var a = q.answers.FirstOrDefault();
            if (a == null)
            {
                return;
            }

            var lmsId = a.id;
            var name = string.Format(
                "{{\"val\":{0}, \"error\":{1} }}",
                a.answer,
                a.margin);
            var distractor = this.DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                new Distractor
                {
                    DateCreated = DateTime.Now,
                    CreatedBy = user,
                    Question = question,
                    LmsAnswerId = lmsId
                };
            distractor.DateModified = DateTime.Now;
            distractor.ModifiedBy = user;
            distractor.DistractorName = name;
            distractor.IsActive = true;
            distractor.DistractorType = 1;
            distractor.IsCorrect = a.weight == 100;

            this.DistractorModel.RegisterSave(distractor);
        }

        /// <summary>
        /// The process calculated question text.
        /// </summary>
        /// <param name="q">
        /// The q.
        /// </param>
        /// <param name="lmsProvider">
        /// The lms Provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ProcessCalculatedQuestionText(QuizQuestionDTO q, LmsProviderEnum lmsProvider)
        {
            var questionText = this.ClearName(q.question_text);
            switch (lmsProvider)
            {
                case LmsProviderEnum.Canvas:
                    var values = q.answers.FirstOrDefault();
                    if (values == null)
                    {
                        return questionText;
                    }
            
                    foreach (var variable in values.variables)
                    {
                        questionText = questionText.Replace("[" + variable.name + "]", variable.value);
                    }
                    break;
                case LmsProviderEnum.Moodle:
                    var vals = new Dictionary<string, double>();
                    foreach (var a in q.datasets)
                    {
                        var value = a.Items.Count > 0 ? a.Items.First().Value : double.Parse(a.Max ?? "0");
                        if (!vals.ContainsKey(a.Name))
                            vals.Add(a.Name, value);
                        var name = a.Name;
                        questionText = questionText.Replace("{" + name + "}", value.ToString());
                    }

                    questionText = MoodleExpressionParser.SimplifyExpression(questionText, vals);

                    break;
            }
            
            return questionText;
        }

        /// <summary>
        /// The process matching distractors.
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
        private void ProcessMatchingDistractors(
            User user,
            QuizQuestionDTO q,
            Question question)
        {
            foreach (var a in q.answers)
            {
                var lmsId = a.id;
                var name = a.right != null ? 
                    string.Format("{0}$${1}", this.ClearName(a.text), this.ClearName(a.right))
                    : string.Format("{0}$${1}", this.ClearName(a.question_text), this.ClearName(a.text));
                var distractor = this.DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
                distractor.LmsAnswer = a.match_id;
                distractor.DateModified = DateTime.Now;
                distractor.ModifiedBy = user;
                distractor.DistractorName = name;
                distractor.IsActive = true;
                distractor.DistractorType = 1;
                distractor.IsCorrect = true;
                
                this.DistractorModel.RegisterSave(distractor);
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
                distractor.DistractorName = this.ClearName(a.text);
                distractor.IsActive = true;
                distractor.DistractorType = 1;
                distractor.IsCorrect = a.weight > 0;
                distractor.LmsAnswer = a.id.ToString();
                distractor.LmsAnswerId = a.id;

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
                if (a.text != null && a.text.ToLower().Equals("true"))
                {
                    distractor.LmsAnswer = a.id.ToString();
                }
                else
                {
                    distractor.LmsAnswerId = a.id;
                }

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
