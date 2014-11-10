namespace EdugameCloud.Lti.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Utils;

    using Newtonsoft.Json;

    using NHibernate.Mapping;

    using RestSharp;
    using RestSharp.Serializers;

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
        /// <param name="course">
        /// The course.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<int, int> ConvertQuizzes(IEnumerable<LmsQuizDTO> quizzes, CourseDTO course, User user, bool isSurvey)
        {
            var submoduleQuiz = new Dictionary<int, int>();
            var submoduleCategory = this.ProcessSubModuleCategory(course, user);

            foreach (var quiz in quizzes)
            {
                if (quiz.questions.Length > 0)
                {
                    var item = this.ConvertQuiz(quiz, user, submoduleCategory, isSurvey);
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
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        private Tuple<int, int> ConvertQuiz(LmsQuizDTO quiz, User user, SubModuleCategory subModuleCategory, bool isSurvey)
        {
            SubModuleItem submodule;
            Tuple<int, int> result;

            if (isSurvey)
            {
                var egcSurvey = this.SurveyModel.GetOneByLmsSurveyId(user.Id, quiz.id, (int)LmsProviderEnum.Canvas).Value
                    ?? new Survey();


                submodule = this.ProcessSubModule(user, subModuleCategory, egcSurvey.IsTransient() ? null : egcSurvey.SubModuleItem, quiz);

                result = this.ProcessSurveyData(quiz, egcSurvey, submodule);
            }
            else
            {
                var egcQuiz = this.QuizModel.GetOneByLmsQuizId(user.Id, quiz.id, (int)LmsProviderEnum.Canvas).Value
                    ?? new Quiz();


                submodule = this.ProcessSubModule(user, subModuleCategory, egcQuiz.IsTransient() ? null : egcQuiz.SubModuleItem, quiz);

                result = this.ProcessQuizData(quiz, egcQuiz, submodule);
            }
            
            this.ProcessQuizQuestions(quiz, user, submodule, isSurvey);

            return result;
        }

        /// <summary>
        /// The process sub module category.
        /// </summary>
        /// <param name="course">
        /// The course.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategory"/>.
        /// </returns>
        private SubModuleCategory ProcessSubModuleCategory(CourseDTO course, User user)
        {
            var subModuleCategoryModel = this.SubModuleCategoryModel;
            var subModuleCategory = subModuleCategoryModel.GetOneByLmsCourseIdAndProvider(course.id, (int)LmsProviderEnum.Canvas).Value
                                          ?? new SubModuleCategory
                                          {
                                              LmsProvider = LmsProviderModel.GetOneById((int)LmsProviderEnum.Canvas).Value,
                                              CategoryName = course.name,
                                              LmsCourseId = course.id,
                                              User = user,
                                              DateModified = DateTime.Now,
                                              IsActive = true,
                                              ModifiedBy = user,
                                              SubModule = SubModuleModel.GetOneById((int)SubModuleItemType.Quiz).Value
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
            egcSurvey.LmsProvider = LmsProviderModel.GetOneById((int)LmsProviderEnum.Canvas).Value;
            egcSurvey.SubModuleItem.IsShared = true;
            egcSurvey.SurveyGroupingType = SurveyGroupingTypeModel.GetOneById(1).Value;

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
            return Regex.Replace(name ?? string.Empty, "<[^>]*(>|$)", string.Empty);
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
        private void ProcessQuizQuestions(LmsQuizDTO quiz, User user, SubModuleItem submodule, bool isSurvey)
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

                string questionText = null;

                if (questionType.QuestionType.Id == (int)QuestionTypeEnum.MultipleDropdowns
                    || questionType.QuestionType.Id == (int)QuestionTypeEnum.FillInTheBlank)
                {
                    questionText = this.ProcessFillInTheBlankQuestionText(quizQuestion);
                }
                else if (questionType.QuestionType.Id == (int)QuestionTypeEnum.Calculated)
                {
                    questionText = this.ProcessCalculatedQuestionText(quizQuestion);
                }
                else
                {
                    questionText = this.ClearName(quizQuestion.question_text);
                }

                question.IsMoodleSingle = !questionType.LmsQuestionTypeName.Equals("multiple_answers_question"); 
                question.QuestionName = questionText;
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

                if (isSurvey)
                {
                    if (question.QuestionType.Id == (int)QuestionTypeEnum.Calculated
                        || question.QuestionType.Id == (int)QuestionTypeEnum.Numerical)
                    {
                        question.QuestionType =
                            QuestionTypeModel.GetOneById((int)QuestionTypeEnum.OpenAnswerSingleLine).Value;
                        this.QuestionModel.RegisterSave(question);
                    }
                    if (question.QuestionType.Id == (int)QuestionTypeEnum.Essay)
                    {
                        question.QuestionType =
                            QuestionTypeModel.GetOneById((int)QuestionTypeEnum.OpenAnswerMultiLine).Value;
                        this.QuestionModel.RegisterSave(question);
                    }
                    if (question.QuestionType.Id == (int)QuestionTypeEnum.ShortAnswer)
                    {
                        question.QuestionType =
                            QuestionTypeModel.GetOneById((int)QuestionTypeEnum.OpenAnswerMultiLine).Value;
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
                case (int)QuestionTypeEnum.ShortAnswer:
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                    {
                        this.ProcessSingleMultipleChoiceTextDistractors(user, q, question, disctarctorModel);
                        break;
                    }

                case (int)QuestionTypeEnum.MultipleDropdowns:
                    {
                        this.ProcessFillInTheBlankDistractors(user, q, question, true);
                        break;
                    }
                case (int)QuestionTypeEnum.FillInTheBlank:
                    {
                        this.ProcessFillInTheBlankDistractors(user, q, question, false);
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
                case (int)QuestionTypeEnum.Calculated:
                    {
                        this.ProcessCalculatedDistractors(user, q, question);
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
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ProcessFillInTheBlankQuestionText(QuizQuestionDTO q)
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
        private void ProcessFillInTheBlankDistractors(
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

            var distractor = DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, question.LmsQuestionId.GetValueOrDefault()).Value ??
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

            DistractorModel.RegisterSave(distractor);
            DistractorModel.Flush();
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
                    isRange ? a.start : a.exact,
                    a.end,
                    a.margin,
                    isRange ? "Range" : "Exact");
                var distractor = DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
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

                DistractorModel.RegisterSave(distractor);
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
        private void ProcessCalculatedDistractors(
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
            var distractor = DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
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

            DistractorModel.RegisterSave(distractor);
        }

        /// <summary>
        /// The process calculated question text.
        /// </summary>
        /// <param name="q">
        /// The q.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ProcessCalculatedQuestionText(QuizQuestionDTO q)
        {
            var questionText = ClearName(q.question_text);
            var values = q.answers.FirstOrDefault();
            if (values == null)
            {
                return questionText;
            }
            
            foreach (var variable in values.variables)
            {
                questionText = questionText.Replace("[" + variable.name + "]", variable.value);
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
                var name = string.Format("{0}$${1}", ClearName(a.text), ClearName(a.right));
                var distractor = DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
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
                
                DistractorModel.RegisterSave(distractor);
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
