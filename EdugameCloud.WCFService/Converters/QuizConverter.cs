using System.Text.RegularExpressions;
using Esynctraining.Core.Providers;

namespace EdugameCloud.WCFService.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Core.Constants;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using EdugameCloud.Lti.Moodle;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;
    using Newtonsoft.Json;
    using RestSharp;
    using EdugameCloud.Lti.Core;

    /// <summary>
    /// The quiz converter.
    /// </summary>
    public sealed class QuizConverter
    {
        /// <summary>
        ///   Gets the settings.
        /// </summary>
        public dynamic Settings
        {
            get { return IoC.Resolve<ApplicationSettingsProvider>(); }
        }

        private QuizModel QuizModel
        {
            get { return IoC.Resolve<QuizModel>(); }
        }

        private SurveyModel SurveyModel
        {
            get { return IoC.Resolve<SurveyModel>(); }
        }

        private QuestionForSingleMultipleChoiceModel QuestionForSingleMultipleChoiceModel
        {
            get { return IoC.Resolve<QuestionForSingleMultipleChoiceModel>(); }
        }

        private QuestionForRateModel QuestionForRateModel
        {
            get { return IoC.Resolve<QuestionForRateModel>(); }
        }

        private QuestionForOpenAnswerModel QuestionForOpenAnswerModel
        {
            get { return IoC.Resolve<QuestionForOpenAnswerModel>(); }
        }

        private SubModuleCategoryModel SubModuleCategoryModel
        {
            get { return IoC.Resolve<SubModuleCategoryModel>(); }
        }

        private SubModuleModel SubModuleModel
        {
            get { return IoC.Resolve<SubModuleModel>(); }
        }

        private SubModuleItemModel SubModuleItemModel
        {
            get { return IoC.Resolve<SubModuleItemModel>(); }
        }

        private QuestionForTrueFalseModel QuestionForTrueFalseModel
        {
            get { return IoC.Resolve<QuestionForTrueFalseModel>(); }
        }

        private LmsQuestionTypeModel LmsQuestionTypeModel
        {
            get { return IoC.Resolve<LmsQuestionTypeModel>(); }
        }

        private QuestionModel QuestionModel
        {
            get { return IoC.Resolve<QuestionModel>(); }
        }

        private QuestionTypeModel QuestionTypeModel
        {
            get { return IoC.Resolve<QuestionTypeModel>(); }
        }

        private QuizFormatModel QuizFormatModel
        {
            get { return IoC.Resolve<QuizFormatModel>(); }
        }

        private FileModel FileModel
        {
            get { return IoC.Resolve<FileModel>(); }
        }

        private ScoreTypeModel ScoreTypeModel
        {
            get { return IoC.Resolve<ScoreTypeModel>(); }
        }

        private SurveyGroupingTypeModel SurveyGroupingTypeModel
        {
            get { return IoC.Resolve<SurveyGroupingTypeModel>(); }
        }

        private DistractorModel DistractorModel
        {
            get { return IoC.Resolve<DistractorModel>(); }
        }

        private LmsCompanyModel LmsCompanyModel
        {
            get { return IoC.Resolve<LmsCompanyModel>(); }
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
                    SubModule = this.SubModuleModel.GetOneById(isSurvey ? (int)SubModuleItemType.Survey : (int)SubModuleItemType.Quiz).Value,
                };
            if (subModuleCategory.IsTransient())
            {
                subModuleCategoryModel.RegisterSave(subModuleCategory);
            }

            return subModuleCategory;
        }

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

        private Tuple<int, int> ProcessQuizData(LmsQuizDTO quiz, Quiz egcQuiz, SubModuleItem submoduleItem)
        {
            egcQuiz.LmsQuizId = quiz.id;
            egcQuiz.QuizName = quiz.title;
            egcQuiz.SubModuleItem = submoduleItem;
            egcQuiz.IsPostQuiz = egcQuiz.IsPostQuiz;
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

        private void ProcessQuizQuestions(LmsQuizDTO quiz, User user, SubModuleItem submodule, bool isSurvey, int companyLmsId)
        {
            var companyLms = this.LmsCompanyModel.GetOneById(companyLmsId).Value;
            var qtypes = this.LmsQuestionTypeModel.GetAllByProvider(companyLms.LmsProviderId).ToList();

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
                    questionText = this.ProcessFillInTheBlankQuestionText(quizQuestion, (LmsProviderEnum)companyLms.LmsProviderId);
                }
                else if (questionType.QuestionTypeId == (int)QuestionTypeEnum.Calculated
                    || questionType.QuestionTypeId == (int)QuestionTypeEnum.CalculatedMultichoice)
                {
                    questionText = this.ProcessCalculatedQuestionText(quizQuestion, (LmsProviderEnum)companyLms.LmsProviderId);
                }

                var htmlText = ReplaceImageConstant(quiz, user, quizQuestion, questionText);

                question.HtmlText = quizQuestion.htmlText;
                question.QuestionName = htmlText;
                question.QuestionType = this.QuestionTypeModel.GetOneById(questionType.QuestionTypeId).Value;
                question.DateModified = DateTime.Now;
                question.ModifiedBy = user;
                question.IsActive = true;
                question.QuestionOrder = questionOrder++;
                question.Rows = quizQuestion.rows;

                var isTransient = question.Id == 0;
                if (quizQuestion.files.Count == 1 && !string.IsNullOrEmpty(quizQuestion.files[0].base64Content))
                {
                    var imageName = quizQuestion.files[0].fileName;
                    var imageBytes = Convert.FromBase64String(quizQuestion.files[0].base64Content);
                    var file = question.Image != null
                        ? FileModel.GetOneById(question.Image.Id).Value
                        : FileModel.CreateFile(user, imageName, DateTime.Now, null, null, null, null);
                    file.FileName = imageName.IndexOf('.') > -1 ? imageName : $"{imageName}.jpg"; // file without extension is not downloadable in our application
                    FileModel.SetData(file, imageBytes);
                    question.Image = file;
                }
                question.RandomizeAnswers = quizQuestion.randomizeAnswers ?? (question.QuestionType.Id == (int)QuestionTypeEnum.Sequence
                                            ? (bool?)true
                                            : null);

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
                        case (int)QuestionTypeEnum.MultipleAnswer:
                            this.QuestionForSingleMultipleChoiceModel.RegisterSave(
                                new QuestionForSingleMultipleChoice
                                {
                                    Question = question,
                                    Restrictions = !quizQuestion.is_single && questionType.LmsQuestionTypeName != "Opinion Scale" ? "multi_choice" : null,
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
                        case (int)QuestionTypeEnum.Sequence:
                            if (!question.RandomizeAnswers.GetValueOrDefault() &&
                                !quizQuestion.randomizeAnswers.HasValue)
                            {

                            }
                            break;
                    }
                }

                this.ProcessDistractors(user, companyLms, questionType.QuestionTypeId, quizQuestion, question, (LmsProviderEnum)companyLms.LmsProviderId, quiz);
            }
        }

        private string ReplaceImageConstant(LmsQuizDTO quiz, User user, LmsQuestionDTO quizQuestion, string questionText)
        {
            if (string.IsNullOrEmpty(quizQuestion.htmlText))
                return questionText;
            var pattern = @"@X@EmbeddedFile\.requestUrlStub@X@[A-Za-z\/\-_\+\d\.]+";
            var regex = new Regex(pattern);

            if (regex.IsMatch(quizQuestion.htmlText))
            {
                var match = regex.Matches(quizQuestion.htmlText);
                var titles = match.Cast<Match>().Select(x => x.Value).Distinct();
                foreach (var title in titles)
                {
                    //var fileName = title.Replace(@"@X@EmbeddedFile.requestUrlStub@X@", String.Empty);
                    string imageBase64;
                    var result = quiz.Images.TryGetValue(title, out imageBase64);
                    if (!result) continue;
                    var theFile = FileModel.GetOneByUniqueName(title).Value;
                    var imageBinary = Convert.FromBase64String(imageBase64);
                    var newOrUpdatedFile = theFile != null
                        ? FileModel.SetData(theFile, imageBinary)
                        : FileModel.CreateFile(user, title, DateTime.Now, null, null, null, null);
                    //var newFileUrl = Settings.BaseServiceUrl.ToString().TrimEnd('/') + "/file/get?id=" + newOrUpdatedFile.Id;
                    var newFileUrl = "/file/get?id=" + newOrUpdatedFile.Id;
                    questionText = questionText.Replace(title, newFileUrl);
                }
            }
            return questionText;
        }

        private void ProcessDistractors(User user, LmsCompany lmsCompany, int qtypeId, LmsQuestionDTO q, Question question, LmsProviderEnum lmsProvider, LmsQuizDTO quiz)
        {
            var disctarctorModel = this.DistractorModel;
            switch (qtypeId)
            {
                case (int)QuestionTypeEnum.ShortAnswer:
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                case (int)QuestionTypeEnum.MultipleAnswer:
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
                        this.ProcessFillInTheBlankDistractors(user, q, question, true, lmsProvider, quiz);
                        break;
                    }
                case (int)QuestionTypeEnum.FillInTheBlank:
                    {
                        this.ProcessFillInTheBlankDistractors(user, q, question, false, lmsProvider, quiz);
                        break;
                    }
                case (int)QuestionTypeEnum.Matching:
                    {
                        this.ProcessMatchingDistractors(user, q, question, quiz);
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

            //image processing is done for hotspot separately
            if (qtypeId != (int)QuestionTypeEnum.Hotspot)
            {
                ProcessImagesInDistractors(user, lmsCompany, q, question, quiz);
            }
        }

        private string ProcessFillInTheBlankQuestionText(LmsQuestionDTO q, LmsProviderEnum lmsProvider)
        {
            switch (lmsProvider)
            {
                case LmsProviderEnum.Canvas:
                case LmsProviderEnum.Blackboard:
                case LmsProviderEnum.Sakai:
                    return this.ProcessFillInTheBlankQuestionTextCanvas(q);
                case LmsProviderEnum.Moodle:
                    return this.ProcessFillInTheBlankQuestionTextMoodle(q);
            }
            return string.Empty;
        }

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

        private void ProcessFillInTheBlankDistractors(User user, LmsQuestionDTO q, Question question, bool option, LmsProviderEnum lmsProvider, LmsQuizDTO quiz)
        {
            switch (lmsProvider)
            {
                case LmsProviderEnum.Canvas:
                    if (!q.answers.Any())
                    {
                        throw new WarningMessageException("Unable to import. Fill In Multiple Blanks questions should have possible answers.");
                    }
                    this.ProcessFillInTheBlankDistractorsCanvas(user, q, question, option, quiz);
                    break;

                case LmsProviderEnum.Blackboard:
                    this.ProcessFillInTheBlankDistractorsCanvas(user, q, question, option, quiz);
                    break;

                case LmsProviderEnum.Moodle:
                    this.ProcessFillInTheBlankDistractorsMoodle(user, q, question, option);
                    break;
                case LmsProviderEnum.Sakai:
                    this.ProcessFillInTheBlankDistractorsCanvas(user, q, question, option, quiz);
                    break;

            }
        }

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

        private void ProcessFillInTheBlankDistractorsCanvas(User user, LmsQuestionDTO q, Question question, bool option, LmsQuizDTO quiz)
        {
            var splitText = new List<string>();
            var questionText = ReplaceImageConstant(quiz, user, q, q.question_text.ClearName());

            while (questionText.Length > 0)
            {
                int nextPlaceholderStart = questionText.IndexOf("[");
                int nextPlaceholderEnd = questionText.IndexOf("]");
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
                    var correct = options.FirstOrDefault(o => o.weight == 100) ?? options.FirstOrDefault();
                    string subType = null;
                    if (correct != null && correct.question_type != null)
                    {
                        subType = string.Format(" subType=\"{0}\"", correct.question_type);
                    }
                    distractorText.AppendFormat(
                        "<text id=\"{0}\" isBlank=\"true\"{2}{3}>{1}</text>",
                        textId,
                        correct != null ? correct.text : string.Empty,
                        subType,
                        q.caseSensitive ? " caseSensitive=\"true\"" : string.Empty);

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
                    if (!keys.ContainsKey(blank))
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

                byte[] imageBytes;
                if (string.IsNullOrEmpty(a.fileData))
                {
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
                    if (lmsCompany.AdminUser != null)
                    {
                        restRequest.AddParameter("user_id", lmsCompany.AdminUser.Username);
                        restRequest.AddParameter("password", lmsCompany.AdminUser.Password);
                    }
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

                    imageBytes = restClient.Execute(restRequest).RawBytes;

                }
                else
                {
                    imageBytes = Convert.FromBase64String(a.fileData);
                }

                var imageName =
                    a.question_text.Substring(
                        a.question_text.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase) + 1);

                var file = distractor.Image != null
                    ? FileModel.GetOneById(distractor.Image.Id).Value
                    : FileModel.CreateFile(user, imageName, DateTime.Now, null, null, null, null);
                FileModel.SetData(file, imageBytes);
                file.FileName = imageName;
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

        private void ProcessImagesInDistractors(User user, LmsCompany lmsCompany, LmsQuestionDTO lmsQuestionDto, Question question, LmsQuizDTO quiz)
        {
            for (var i = 0; i < lmsQuestionDto.answers.Count; i++)
            {
                var answer = lmsQuestionDto.answers[i];
                var lmsId = answer.id;
                var distractor = this.DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value;
                if (distractor == null || lmsQuestionDto.answersImageLinks == null || lmsQuestionDto.answersImageLinks.Count <= i)
                    continue;

                var imageLink = lmsQuestionDto.answersImageLinks[i];
                if (string.IsNullOrEmpty(imageLink))
                    continue;
                string imageBase64;
                var getResult = quiz.Images.TryGetValue(imageLink, out imageBase64);

                if (!getResult)
                    continue;
                if (imageBase64 == null)
                    continue;

                var imageBytes = Convert.FromBase64String(imageBase64);

                var imageName = string.IsNullOrEmpty(imageLink) ?
                    answer.question_text.Substring(
                        answer.question_text.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase) + 1) : imageLink;

                var file = distractor.Image != null
                    ? FileModel.GetOneById(distractor.Image.Id).Value
                    : FileModel.CreateFile(user, imageName, DateTime.Now, null, null, null, null);
                FileModel.SetData(file, imageBytes);
                file.FileName = imageName;
                distractor.Image = file;

                this.DistractorModel.RegisterSave(distractor);
            }
        }

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

        private void ProcessNumericalDistractors(
            User user,
            LmsQuestionDTO q,
            Question question)
        {
            foreach (var a in q.answers)
            {
                // TODO: http://jira.esynctraining.com/browse/EDUGAMECLOUD-1529?filter=-1
                //хотя.вадим грил что точно такой же вопрос созданный в канвасе (а не импортированный) -типо работает.
                //Чинить ХЗ когда буду -времени на баги Майк не выдает.
                //Ну и нужно будет смотреть чего там с муддла летит для такого случая - не понятно в каком месте правильнее чинить.. )
                var lmsId = a.id;
                bool isRange = a.numerical_answer_type != null && a.numerical_answer_type.Contains("range");

                // http://jira.esynctraining.com/browse/EDUGAMECLOUD-1529?filter=-1
                bool isImportedCanvasExact = a.numerical_answer_type == "exact_answer" && a.text == "answer_text";

                var name = string.Format(
                    "{{\"min\":{0}, \"max\": {1}, \"error\":{2},\"type\":\"{3}\"}}",

                    isRange
                    ? a.start.ToString()
                    : ((string.IsNullOrWhiteSpace(a.text) || isImportedCanvasExact) ? a.exact.ToString() : a.text),

                    (string.IsNullOrWhiteSpace(a.text) || isImportedCanvasExact) ? a.end.ToString() : a.text,
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

        private string ProcessCalculatedQuestionText(LmsQuestionDTO q, LmsProviderEnum lmsProvider)
        {
            var questionText = q.question_text;
            switch (lmsProvider)
            {
                case LmsProviderEnum.Canvas:
                case LmsProviderEnum.Blackboard:
                case LmsProviderEnum.Sakai:
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

        private void ProcessMatchingDistractors(
            User user,
            LmsQuestionDTO q,
            Question question, LmsQuizDTO quiz)
        {
            foreach (var answerDto in q.answers)
            {
                var lmsId = answerDto.id;
                var name = answerDto.right != null ?
                    string.Format("{0}$${1}", answerDto.text, answerDto.right)
                    : string.Format("{0}$${1}", answerDto.question_text, answerDto.text);
                var distractor = this.DistractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
                distractor.LmsAnswer = answerDto.match_id;
                distractor.DateModified = DateTime.Now;
                distractor.ModifiedBy = user;
                distractor.DistractorName = name;
                distractor.IsActive = true;
                distractor.DistractorType = 1;
                distractor.IsCorrect = true;

                if (!string.IsNullOrEmpty(answerDto.leftImageName))
                {
                    var fileName = answerDto.leftImageName;
                    var leftImg = MatchingSetImage(user, quiz, fileName, distractor);
                    distractor.LeftImage = leftImg;
                }
                if (!string.IsNullOrEmpty(answerDto.rightImageName))
                {
                    var fileName = answerDto.rightImageName;
                    var rightImg = MatchingSetImage(user, quiz, fileName, distractor);
                    distractor.RightImage = rightImg;
                }

                this.DistractorModel.RegisterSave(distractor);
            }
        }

        private File MatchingSetImage(User user, LmsQuizDTO quiz, string fileName, Distractor distractor)
        {
            string imageBase64;
            var hasImage = quiz.Images.TryGetValue(fileName, out imageBase64);
            if (hasImage)
            {
                var binary = Convert.FromBase64String(imageBase64);
                var file = distractor.Image != null
                    ? FileModel.GetOneById(distractor.Image.Id).Value
                    : FileModel.CreateFile(user, fileName, DateTime.Now, null, null, null, null);
                FileModel.SetData(file, binary);
                file.FileName = fileName;
                return file;
            }
            return null;
        }

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

