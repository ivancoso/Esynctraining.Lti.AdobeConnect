namespace EdugameCloud.WCFService.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Constants;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Core.Constants;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.EntityParsing;
    using EdugameCloud.Lti.Extensions;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;
    using Newtonsoft.Json;
    using RestSharp;

    /// <summary>
    /// The quiz converter.
    /// </summary>
    public sealed class QuizConverter
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
        /// Gets the file model.
        /// </summary>
        private FileModel FileModel
        {
            get
            {
                return IoC.Resolve<FileModel>();
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
        /// Gets the company lms model.
        /// </summary>
        private LmsCompanyModel LmsCompanyModel
        {
            get
            {
                return IoC.Resolve<LmsCompanyModel>();
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

            foreach (var quiz in quizzes.Where(x => x.question_list != null && x.question_list.Length > 0))
            {
                SubModuleCategory submoduleCategory = this.ProcessSubModuleCategory(quiz, user, companyLmsId, isSurvey);

                var item = this.ConvertQuiz(quiz, user, submoduleCategory, isSurvey, companyLmsId);
                if (!submoduleQuiz.ContainsKey(item.Item1))
                {
                    submoduleQuiz.Add(item.Item1, item.Item2);
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
            SubModuleItem subModuleItem;
            Tuple<int, int> result;

            if (isSurvey)
            {
                var egcSurvey = this.SurveyModel.GetOneByLmsSurveyId(user.Id, quiz.id, companyLmsId).Value
                    ?? new Survey();

                subModuleItem = this.ProcessSubModule(user, subModuleCategory, egcSurvey.IsTransient() ? null : egcSurvey.SubModuleItem, quiz);

                result = this.ProcessSurveyData(quiz, egcSurvey, subModuleItem);
            }
            else
            {
                var egcQuiz = this.QuizModel.GetOneByLmsQuizId(user.Id, quiz.id, companyLmsId).Value
                    ?? new Quiz();


                subModuleItem = this.ProcessSubModule(user, subModuleCategory, egcQuiz.IsTransient() ? null : egcQuiz.SubModuleItem, quiz);

                result = this.ProcessQuizData(quiz, egcQuiz, subModuleItem);
            }

            this.ProcessQuizQuestions(quiz, user, subModuleItem, isSurvey, companyLmsId);

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
        private SubModuleCategory ProcessSubModuleCategory(LmsQuizDTO quiz, User user, int companyLmsId, bool isSurvey)
        {
            var subModuleCategoryModel = this.SubModuleCategoryModel;
            var subModuleCategory = subModuleCategoryModel.GetOneByLmsCourseIdAndCompanyLms(quiz.course, companyLmsId).Value
                ?? new SubModuleCategory
                {
                    CompanyLmsId = this.LmsCompanyModel.GetOneById(companyLmsId).Value.With(x => x.Id),
                    CategoryName = quiz.courseName,
                    LmsCourseId = quiz.course,
                    User = user,
                    DateModified = DateTime.Now,
                    IsActive = true,
                    ModifiedBy = user,
                    SubModule = this.SubModuleModel.GetOneById(isSurvey ? (int)SubModuleItemType.Survey :(int)SubModuleItemType.Quiz).Value,
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
                    CreatedBy = user,
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
            egcQuiz.QuizName = quiz.title;
            egcQuiz.SubModuleItem = submoduleItem;
            egcQuiz.Description = quiz.description == null ? null : quiz.description.ClearName();
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
            egcSurvey.SurveyName = quiz.title;
            egcSurvey.SubModuleItem = submoduleItem;
            egcSurvey.Description = quiz.description;
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
            var companyLms = this.LmsCompanyModel.GetOneById(companyLmsId).Value;
            var qtypes = this.LmsQuestionTypeModel.GetAllByProvider(companyLms.LmsProvider.Id).ToList();

            int subModuleId = isSurvey ? (int)SubModuleItemType.Survey : (int)SubModuleItemType.Quiz;

            int questionOrder = 0;

            foreach (var quizQuestion in quiz.question_list.Where(qs => qs.question_type != null))
            {
                var questionType = qtypes.FirstOrDefault(qt => qt.LmsQuestionTypeName.Equals(quizQuestion.question_type) && (qt.SubModuleId == null || qt.SubModuleId.GetValueOrDefault() == subModuleId));
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

                string questionText = quizQuestion.question_text;

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

                question.QuestionName = questionText;
                question.QuestionType = this.QuestionTypeModel.GetOneById(questionType.QuestionTypeId).Value;
                question.DateModified = DateTime.Now;
                question.ModifiedBy = user;
                question.IsActive = true;
                question.QuestionOrder = questionOrder++;
                
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

                this.ProcessDistractors(user, companyLms, questionType.QuestionTypeId, quizQuestion, question, (LmsProviderEnum)companyLms.LmsProvider.Id);
            }
        }

        /// <summary>
        /// The process distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="companyLms">
        /// The company Lms.
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
        private void ProcessDistractors(User user, LmsCompany lmsCompany, int qtypeId, LmsQuestionDTO q, Question question, LmsProviderEnum lmsProvider)
        {
            var disctarctorModel = this.DistractorModel;
            switch (qtypeId)
            {
                case (int)QuestionTypeEnum.ShortAnswer:
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                case (int)QuestionTypeEnum.Rate:
                    {
                        this.ProcessSingleMultipleChoiceTextDistractors(user, q, question, 1);
                        break;
                    }
                case (int)QuestionTypeEnum.RateScaleLikert:
                    {
                        this.ProcessSingleMultipleChoiceTextDistractors(user, q, question, 2);
                        this.AddLikertQuestionDistractor(user, q, question);
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
                case (int)QuestionTypeEnum.Sequence:
                    {
                        this.ProcessSequenceDistractors(user, q, question);
                        break;
                    }
                case (int)QuestionTypeEnum.Hotspot:
                    {
                        this.ProcessHotSpotDistractors(user, lmsCompany, q, question);
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
        private string ProcessFillInTheBlankQuestionText(LmsQuestionDTO q, LmsProviderEnum lmsProvider)
        {
            switch (lmsProvider)
            {
                case LmsProviderEnum.Canvas:
                case LmsProviderEnum.Blackboard:
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
        private string ProcessFillInTheBlankQuestionTextMoodle(LmsQuestionDTO q)
        {
            var i = 1;
            var questionText = q.question_text.ClearName();
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
        private string ProcessFillInTheBlankQuestionTextCanvas(LmsQuestionDTO q)
        {
            var questionText = q.question_text;
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
            LmsQuestionDTO q,
            Question question,
            bool option,
            LmsProviderEnum lmsProvider)
        {
            switch (lmsProvider)
            {
                case LmsProviderEnum.Canvas:
                case LmsProviderEnum.Blackboard:
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
            LmsQuestionDTO q,
            Question question,
            bool option)
        {
            string blankTrue = "<text id=\"0\" isBlank=\"true\">",
                blankFalse = "<text id=\"0\" isBlank=\"false\">",
                closing = "</text>";
            var distractorText = q.question_text.ClearName();
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
            LmsQuestionDTO q,
            Question question,
            bool option)
        {
            var splitText = new List<string>();
            var questionText = q.question_text.ClearName();
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
                    distractorText.AppendFormat(
                        "<text id=\"{0}\" isBlank=\"true\">{1}</text>",
                        textId,
                        correct != null ? correct.text : string.Empty);
                    
                    if (option)
                    {    
                        distractorText.AppendFormat("<options id=\"{0}\">", textId);
                        foreach (var o in options)
                        {
                            distractorText.AppendFormat(
                                "<option name=\"{0}\" lmsid=\"{1}\" />",
                                o.text,
                                o.match_id ?? o.id.ToString());
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
        /// The process sequence distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="companyLms">
        /// The company Lms.
        /// </param>
        /// <param name="q">
        /// The q.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        private void ProcessHotSpotDistractors(
            User user,
            LmsCompany lmsCompany,
            LmsQuestionDTO q,
            Question question)
        {
            foreach (var a in q.answers)
            {
                var lmsId = a.id;
                var distractor = this.DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
                distractor.DistractorOrder = a.order + 1;
                distractor.DateModified = DateTime.Now;
                distractor.ModifiedBy = user;
                distractor.IsActive = true;
                distractor.DistractorType = 1;
                distractor.IsCorrect = true;

                var domain = string.Format(
                        "{0}{1}",
                        lmsCompany.UseSSL.GetValueOrDefault() ? HttpScheme.Https : HttpScheme.Http,
                        lmsCompany.LmsDomain.EndsWith("/")
                            ? lmsCompany.LmsDomain.Substring(0, lmsCompany.LmsDomain.Length - 1)
                            : lmsCompany.LmsDomain);
                var restClient = new RestClient(domain);

                var restRequest = new RestRequest(string.Empty, Method.GET);
                var cookie1 = restClient.Execute(restRequest);

                restRequest = new RestRequest("/webapps/login/", Method.POST);
                restRequest.AddParameter("user_id", lmsCompany.AdminUser.Username);
                restRequest.AddParameter("password", lmsCompany.AdminUser.Password);
                restRequest.AddParameter("action", "login");
                restRequest.AddParameter("login", "Login");
                foreach (var c in cookie1.Cookies)
                {
                    restRequest.AddCookie(c.Name, c.Value);
                }

                var cookie2 = restClient.Execute(restRequest);
                
                if (cookie2.Cookies.Count == 1)
                {
                    cookie2 = restClient.Execute(restRequest);
                }

                if (cookie2.Cookies.Count == 1)
                {
                    cookie2 = restClient.Execute(restRequest);
                }


                restRequest = new RestRequest(a.question_text, Method.GET);

                foreach (var c in cookie2.Cookies)
                {
                    restRequest.AddCookie(c.Name, c.Value);
                }

                var image = restClient.Execute(restRequest);
                var imageName = a.question_text.Substring(a.question_text.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase) + 1);

                var file = distractor.Image != null
                               ? FileModel.GetOneById(distractor.Image.Id).Value
                               : FileModel.CreateFile(user, imageName, DateTime.Now, null, null, null, null);
                FileModel.SetData(file, image.RawBytes);
                distractor.Image = file;

                var coord = a.text.Split(',').Select(z => int.Parse(z.Trim())).ToArray();
                if (coord.Length > 3)
                {
                    int x = coord[0], y = coord[1], width = coord[2] - coord[0], height = coord[3] - coord[1];
                    distractor.DistractorName = string.Format("<data><hotspot x=\"{0}\" y=\"{1}\" width=\"{2}\" height=\"{3}\" isCorrect=\"true\" hasBorder=\"true\" type=\"rectangle\" hasBG=\"true\"/></data>",
                        x,
                        y,
                        width,
                        height);
                    distractor.LmsAnswer = string.Format("{0}, {1}", x, y);
                }
                else
                {
                    distractor.DistractorName = string.Empty;
                }


                this.DistractorModel.RegisterSave(distractor);
            }
        }

        /// <summary>
        /// The process sequence distractors.
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
        private void ProcessSequenceDistractors(
            User user,
            LmsQuestionDTO q,
            Question question)
        {
            foreach (var a in q.answers)
            {
                var lmsId = a.id;
                var distractor = this.DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
                distractor.DistractorOrder = a.order + 1;
                distractor.DateModified = DateTime.Now;
                distractor.ModifiedBy = user;
                distractor.DistractorName = a.text;
                distractor.IsActive = true;
                distractor.DistractorType = 1;
                distractor.IsCorrect = a.weight == 100;

                this.DistractorModel.RegisterSave(distractor);
            }
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
            LmsQuestionDTO q,
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
        private void ProcessCalculatedDistractors(User user, LmsQuestionDTO q, Question question, LmsProviderEnum lmsProvider)
        {
            switch (lmsProvider)
            {
                case LmsProviderEnum.Canvas:
                case LmsProviderEnum.Blackboard:
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
        private void ProcessCalculatedDistractorsMoodle(User user, LmsQuestionDTO q, Question question)
        {
            var answerNumber = 1;//question.IsMoodleSingle.GetValueOrDefault() ? 0 : 1; // singlechoice starts from 0, multichoice from 1
            foreach (var a in q.answers)
            {
                var expression = new MoodleExpressionParser(a.text.ClearName());
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
            LmsQuestionDTO q,
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
            distractor.LmsAnswer = q.answers.Any() ? q.answers.First().question_text : string.Empty;
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
        private string ProcessCalculatedQuestionText(LmsQuestionDTO q, LmsProviderEnum lmsProvider)
        {
            var questionText = q.question_text;
            switch (lmsProvider)
            {
                case LmsProviderEnum.Canvas:
                case LmsProviderEnum.Blackboard:
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
            LmsQuestionDTO q,
            Question question)
        {
            foreach (var a in q.answers)
            {
                var lmsId = a.id;
                var name = a.right != null ? 
                    string.Format("{0}$${1}", a.text, a.right)
                    : string.Format("{0}$${1}", a.question_text, a.text);
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
            LmsQuestionDTO q,
            Question question,
            int distractorType)
        {
            foreach (var a in q.answers)
            {
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
                distractor.DistractorName = a.text;
                distractor.IsActive = true;
                distractor.DistractorType = distractorType;
                distractor.IsCorrect = a.weight > 0;
                distractor.LmsAnswer = a.id.ToString();
                distractor.LmsAnswerId = a.id;

                this.DistractorModel.RegisterSave(distractor);
            }
        }

        /// <summary>
        /// The add likert question distractor.
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
        private void AddLikertQuestionDistractor(
            User user,
            LmsQuestionDTO q,
            Question question)
        {
            var lmsId = -1;

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
            distractor.DistractorName = q.question_text;
            distractor.IsActive = true;
            distractor.DistractorType = 1;
            distractor.LmsAnswerId = lmsId;

            this.DistractorModel.RegisterSave(distractor);
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
            LmsQuestionDTO q,
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

                if (a.question_text != null)
                {
                    distractor.DistractorName = a.question_text;
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
