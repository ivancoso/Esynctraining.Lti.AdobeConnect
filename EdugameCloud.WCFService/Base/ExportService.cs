namespace EdugameCloud.WCFService.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Domain.Formats;
    using EdugameCloud.Core.Domain.Formats.Edugame;
    using EdugameCloud.Core.Domain.Formats.WebEx;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using Resources;

    using File = EdugameCloud.Core.Domain.Entities.File;

    /// <summary>
    ///     The export service.
    /// </summary>
    public abstract class ExportService : BaseService
    {
        #region Properties

        /// <summary>
        ///     Gets the distractor model.
        /// </summary>
        protected DistractorModel DistractorModel
        {
            get
            {
                return IoC.Resolve<DistractorModel>();
            }
        }

        /// <summary>
        ///     Gets the export path.
        /// </summary>
        protected string ExportPath
        {
            get
            {
                return Path.Combine(this.StoragePath, this.Settings.ExportSubPath as string ?? string.Empty);
            }
        }

        /// <summary>
        ///     Gets the import path.
        /// </summary>
        protected string ImportPath
        {
            get
            {
                return Path.Combine(this.StoragePath, this.Settings.ImportSubPath as string ?? string.Empty);
            }
        }

        /// <summary>
        ///     Gets the question model.
        /// </summary>
        protected QuestionForLikertModel QuestionForLikertModel
        {
            get
            {
                return IoC.Resolve<QuestionForLikertModel>();
            }
        }

        /// <summary>
        ///     Gets the question model.
        /// </summary>
protected QuestionForTrueFalseModel QuestionForTrueFalseModel
        {
            get
            {
                return IoC.Resolve<QuestionForTrueFalseModel>();
            }
        }

        /// <summary>
        ///     Gets the question model.
        /// </summary>
        protected QuestionForOpenAnswerModel QuestionForOpenAnswerModel
        {
            get
            {
                return IoC.Resolve<QuestionForOpenAnswerModel>();
            }
        }

        /// <summary>
        ///     Gets the question model.
        /// </summary>
        protected QuestionForRateModel QuestionForRateModel
        {
            get
            {
                return IoC.Resolve<QuestionForRateModel>();
            }
        }

        /// <summary>
        ///     Gets the question model.
        /// </summary>
        protected QuestionForSingleMultipleChoiceModel QuestionForSingleMultipleChoiceModel
        {
            get
            {
                return IoC.Resolve<QuestionForSingleMultipleChoiceModel>();
            }
        }

        /// <summary>
        ///     Gets the question model.
        /// </summary>
        protected QuestionForWeightBucketModel QuestionForWeightBucketModel
        {
            get
            {
                return IoC.Resolve<QuestionForWeightBucketModel>();
            }
        }

        /// <summary>
        ///     Gets the question model.
        /// </summary>
        protected QuestionModel QuestionModel
        {
            get
            {
                return IoC.Resolve<QuestionModel>();
            }
        }

        /// <summary>
        ///     Gets the question type model.
        /// </summary>
        protected QuestionTypeModel QuestionTypeModel
        {
            get
            {
                return IoC.Resolve<QuestionTypeModel>();
            }
        }

        /// <summary>
        ///     Gets the schemas path.
        /// </summary>
        protected string SchemasPath
        {
            get
            {
                return Path.Combine(this.StoragePath, this.Settings.SchemasSubPath as string ?? string.Empty);
            }
        }

        /// <summary>
        ///     Gets the storage path.
        /// </summary>
        protected string StoragePath
        {
            get
            {
                return this.Settings.FileStorage as string ?? string.Empty;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Export entity by sub module item Id.
        /// </summary>
        /// <param name="smiId">
        /// Sub module item id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        protected ServiceResponse<string> Export(int smiId)
        {
            return this.Export(smiId, null);
        }

        /// <summary>
        /// Export questions by sub module item Id.
        /// </summary>
        /// <param name="smiId">
        /// SubModule item id.
        /// </param>
        /// <param name="questionIds">
        /// Question Ids.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        protected ServiceResponse<string> Export(int smiId, List<int> questionIds)
        {
            var result = new ServiceResponse<string>();
            try
            {
                SubModuleItem smi = this.SubModuleItemModel.GetOneById(smiId).Value;
                string data = this.ExportToString(smi, questionIds);
                if (string.IsNullOrEmpty(data))
                {
                    throw new ArgumentException();
                }

                string id = Guid.NewGuid().ToString();
                string fileName = id + ".xml";
                string filePath = Path.Combine(this.ExportPath, fileName);

                System.IO.File.WriteAllText(filePath, data);

                result.@object = id;
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
            }
            catch (ArgumentException)
            {
                this.LogError(ErrorsTexts.ExportError_Subject, result);
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, 
                        ErrorsTexts.ExportError_Subject, 
                        ErrorsTexts.ExportError_NoData));
            }
            catch
            {
                this.LogError(ErrorsTexts.ExportError_Subject, result);
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, 
                        ErrorsTexts.ExportError_Subject, 
                        ErrorsTexts.ExportError_InvalidFormat));
            }

            return result;
        }

        /// <summary>
        /// Export questions to string.
        /// </summary>
        /// <param name="smi">
        /// The sub module item.
        /// </param>
        /// <param name="questionIds">
        /// Question Ids.
        /// </param>
        /// <returns>
        /// Serialized string.
        /// </returns>
        protected string ExportToString(SubModuleItem smi, List<int> questionIds = null)
        {
            string result = string.Empty;
            List<Question> questionsSource = this.QuestionModel.GetByQuestionIdsAndSmiID(smi.Id, questionIds).ToList();
            List<QuestionFor> customQuestions =
                this.QuestionModel.GetCustomQuestionsByQuestionIdsWithTypes(
                    questionsSource.Select(x => new KeyValuePair<int, int>(x.Id, x.QuestionType.Id))).ToList();

            List<Distractor> distractors =
                this.DistractorModel.GetAllByQuestionsIds(questionsSource.Select(x => x.Id).ToList()).ToList();
            bool isInvalid;
            List<EdugameQuestion> edugameQuestions = this.GetEdugameQuestions(
                questionsSource, 
                customQuestions, 
                distractors, 
                out isInvalid);

            if (!isInvalid)
            {
                if (edugameQuestions.Any())
                {
                    try
                    {
                        var list = new EdugameQuestions(smi.SubModuleCategory.SubModule.Id, edugameQuestions);
                        result = list.Serialize();
                    }
                        
                        // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                        // It will be handled later.
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets questions.
        /// </summary>
        /// <param name="questions">
        /// Questions from stored procedure.
        /// </param>
        /// <param name="customQuestions">
        /// The custom Questions.
        /// </param>
        /// <param name="distractors">
        /// Distractors from stored procedure.
        /// </param>
        /// <param name="isInvalid">
        /// Indicates whether questions were retrieved.
        /// </param>
        /// <returns>
        /// List of questions.
        /// </returns>
        protected List<EdugameQuestion> GetEdugameQuestions(
            List<Question> questions, 
            List<QuestionFor> customQuestions, 
            List<Distractor> distractors, 
            out bool isInvalid)
        {
            // ReSharper disable once InconsistentNaming
            IEnumerable<QuestionDTO> questionsDTOs =
                questions.Select(x => new QuestionDTO(x, this.SelectCustomTypeFromList(x, customQuestions)));
            IEnumerable<QuestionType> questionTypes = this.QuestionTypeModel.GetAllActive();
            Func<int, QuestionType> getQuestionTypeById = id => questionTypes.FirstOrDefault(type => type.Id == id);
            Func<File, string> getImageByFile = file =>
                {
                    string result = string.Empty;
                    if (file != null)
                    {
                        byte[] raw = this.FileModel.GetData(file);
                        if (raw != null)
                        {
                            result = Convert.ToBase64String(raw);
                        }
                    }

                    return result;
                };

            Func<File, string> getImageNameByFile = file =>
                {
                    string result = string.Empty;
                    if (file != null)
                    {
                        result = file.FileName;
                    }

                    return result;
                };

            List<EdugameQuestion> edugameQuestions =
                questionsDTOs.Select(
                    question =>
                    EdugameConverter.Convert(
                        question, 
                        questions.FirstOrDefault(x => x.Id == question.questionId).With(x => x.Image), 
                        getQuestionTypeById(question.questionTypeId), 
                        distractors.Where(x => x.Question.Id == question.questionId), 
                        getImageByFile, 
                        getImageNameByFile)).ToList();
            isInvalid =
                edugameQuestions.Any(
                    question =>
                    question == null || question.Type == null
                    || question.Distractors.Any(distractor => distractor == null));

            return edugameQuestions;
        }

        /// <summary>
        /// Get questions from imported file.
        /// </summary>
        /// <param name="fileId">
        /// Imported file id.
        /// </param>
        /// <param name="smiId">
        /// Sub module item id.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="format">
        /// Questions format.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        protected ServiceResponse<QuestionDTO> Import(string fileId, int? smiId, int? userId, FormatsEnum format)
        {
            var result = new ServiceResponse<QuestionDTO>();
            var questionDtos = new List<QuestionDTO>();
            try
            {
                SubModuleItem subModuleItem = smiId.HasValue
                                                  ? this.SubModuleItemModel.GetOneById(smiId.Value).Value
                                                  : null;
                User creator = userId.HasValue ? this.UserModel.GetOneById(userId.Value).Value : null;
                string fileName = fileId + ".xml";
                string filePath = Path.Combine(this.ImportPath, fileName);
                string schemaPath = Path.Combine(this.SchemasPath, format + ".xsd");

                if (subModuleItem == null && creator == null)
                {
                    throw new ArgumentException();
                }

                EdugameQuestions questions = this.DeserializeEdugameQuestions(format, filePath, schemaPath);

                QuestionModel questionModel = this.QuestionModel;
                DistractorModel distractorModel = this.DistractorModel;

                Func<string, string, File> saveImage = this.GetSaveImageRutine(subModuleItem, creator);

                List<QuestionType> questionTypes = this.QuestionTypeModel.GetAllActive().ToList();

                foreach (EdugameQuestion question in questions.Questions)
                {
                    Question convertedQuestion = EdugameConverter.Convert(question, questionTypes);
                    convertedQuestion.SubModuleItem = subModuleItem;
                    if (convertedQuestion.CreatedBy == null)
                    {
                        convertedQuestion.CreatedBy = subModuleItem.Return(x => x.CreatedBy, creator);
                    }

                    if (convertedQuestion.ModifiedBy == null)
                    {
                        convertedQuestion.ModifiedBy = convertedQuestion.CreatedBy;
                    }

                    File questionImage = saveImage(question.ImageName, question.Image);
                    if (questionImage != null)
                    {
                        convertedQuestion.Image = questionImage;
                    }

                    if (subModuleItem != null)
                    {
                        questionModel.RegisterSave(convertedQuestion);
                    }

                    QuestionFor customQuestion = this.ProcessCustomQuestionType(convertedQuestion, question);

                    ProcessDistractors(question, convertedQuestion, saveImage, subModuleItem, creator, distractorModel);

                    questionDtos.Add(new QuestionDTO(convertedQuestion, customQuestion));
                }

                System.IO.File.Delete(filePath);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
            }
            catch (ArgumentException)
            {
                this.LogError(ErrorsTexts.ImportError_Subject, result);
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, 
                        ErrorsTexts.ImportError_Subject, 
                        ErrorsTexts.ImportError_NoData));
            }
            catch (SerializationException)
            {
                this.LogError(ErrorsTexts.ImportError_Subject, result);
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, 
                        ErrorsTexts.ImportError_Subject, 
                        ErrorsTexts.ImportError_InvalidFormat));
            }
            catch (FileNotFoundException)
            {
                this.LogError(ErrorsTexts.ImportError_Subject, result);
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, 
                        ErrorsTexts.ImportError_Subject, 
                        ErrorsTexts.ImportError_NotFound));
            }
            catch
            {
                this.LogError(ErrorsTexts.ImportError_Subject, result);
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, 
                        ErrorsTexts.ImportError_Subject, 
                        ErrorsTexts.ImportError_Unknown));
            }

            result.objects = questionDtos;

            return result;
        }

        /// <summary>
        /// Import questions from string.
        /// </summary>
        /// <param name="data">
        /// String for importing.
        /// </param>
        /// <param name="format">
        /// Questions format.
        /// </param>
        /// <returns>
        /// Questions collection.
        /// </returns>
        protected EdugameQuestions ImportFromString(string data, FormatsEnum format)
        {
            var result = new EdugameQuestions();
            try
            {
                switch (format)
                {
                    case FormatsEnum.Edugame:
                        result = data.Deserialize<EdugameQuestions>();
                        break;
                    case FormatsEnum.WebEx:
                        List<QuestionType> questionTypes = this.QuestionTypeModel.GetAllActive().ToList();
                        var pool = data.Deserialize<WebExPool>();
                        result = WebExConverter.Convert(pool, questionTypes);
                        break;
                }
            }
                
                // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // It will be handled later.
            }

            return result;
        }

        /// <summary>
        /// The process custom questions.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="QuestionFor"/>.
        /// </returns>
        protected QuestionFor ProcessCustomQuestionType(Question question, EdugameQuestion dto)
        {
            switch (question.QuestionType.Id)
            
 {
                case (int)QuestionTypeEnum.TrueFalse:
                    var qtf = new QuestionForTrueFalse
                    {
                        PageNumber = dto.PageNumber,
                        IsMandatory = dto.IsMandatory ?? false,
                        Question = question
                    };

                    if (question.SubModuleItem != null)
                    {
                        this.QuestionForTrueFalseModel.RegisterSave(qtf);
                    }

                    return qtf;
                case (int)QuestionTypeEnum.Rate:
                    var qr = new QuestionForRate
                                 {
                                     AllowOther = dto.AllowOther, 
                                     Restrictions = dto.Restrictions, 
                                     PageNumber = dto.PageNumber, 
                                     IsMandatory = dto.IsMandatory ?? false, 
                                     Question = question
                                 };
                    if (question.SubModuleItem != null)
                    {
                        this.QuestionForRateModel.RegisterSave(qr);
                    }

                    return qr;

                case (int)QuestionTypeEnum.OpenAnswerMultiLine:
                case (int)QuestionTypeEnum.OpenAnswerSingleLine:
                    var qoa = new QuestionForOpenAnswer();
                    qoa.Restrictions = dto.Restrictions;
                    qoa.PageNumber = dto.PageNumber;
                    qoa.IsMandatory = dto.IsMandatory ?? false;
                    qoa.Question = question;
                    if (question.SubModuleItem != null)
                    {
                        this.QuestionForOpenAnswerModel.RegisterSave(qoa);
                    }

                    return qoa;

                case (int)QuestionTypeEnum.RateScaleLikert:
                    var ql = new QuestionForLikert
                                 {
                                     AllowOther = dto.AllowOther, 
                                     PageNumber = dto.PageNumber, 
                                     IsMandatory = dto.IsMandatory ?? false, 
                                     Question = question
                                 };
                    if (question.SubModuleItem != null)
                    {
                        this.QuestionForLikertModel.RegisterSave(ql);
                    }

                    return ql;

                case (int)QuestionTypeEnum.SingleMultipleChoiceImage:
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                    var qc = new QuestionForSingleMultipleChoice
                                 {
                                     AllowOther = dto.AllowOther, 
                                     PageNumber = dto.PageNumber, 
                                     IsMandatory = dto.IsMandatory ?? false, 
                                     Restrictions = dto.Restrictions, 
                                     Question = question
                                 };
                    if (question.SubModuleItem != null)
                    {
                        this.QuestionForSingleMultipleChoiceModel.RegisterSave(qc);
                    }

                    return qc;

                case (int)QuestionTypeEnum.WeightedBucketRatio:
                    var qw = new QuestionForWeightBucket
                                 {
                                     AllowOther = dto.AllowOther, 
                                     PageNumber = dto.PageNumber, 
                                     TotalWeightBucket = dto.TotalWeightBucket, 
                                     WeightBucketType = dto.WeightBucketType, 
                                     IsMandatory = dto.IsMandatory ?? false, 
                                     Question = question
                                 };
                    if (question.SubModuleItem != null)
                    {
                        this.QuestionForWeightBucketModel.RegisterSave(qw);
                    }

                    return qw;
            }

            return null;
        }

        /// <summary>
        /// The select custom type from list.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="customQuestions">
        /// The custom questions.
        /// </param>
        /// <returns>
        /// The <see cref="QuestionFor"/>.
        /// </returns>
        protected QuestionFor SelectCustomTypeFromList(Question question, IEnumerable<QuestionFor> customQuestions)
        {
            return
                customQuestions.FirstOrDefault(
                    c =>
                    c.QuestionTypes.Contains((QuestionTypeEnum)question.QuestionType.Id) && c.Question.Id == question.Id);
        }

        /// <summary>
        /// The process distractors.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="convertedQuestion">
        /// The converted question.
        /// </param>
        /// <param name="saveImage">
        /// The save image.
        /// </param>
        /// <param name="subModuleItem">
        /// The sub module item.
        /// </param>
        /// <param name="creator">
        /// The creator.
        /// </param>
        /// <param name="distractorModel">
        /// The distractor model.
        /// </param>
        private static void ProcessDistractors(
            EdugameQuestion question, 
            Question convertedQuestion, 
            Func<string, string, File> saveImage, 
            SubModuleItem subModuleItem, 
            User creator, 
            DistractorModel distractorModel)
        {
            foreach (EdugameDistractor distractor in question.Distractors)
            {
                Distractor convertedDistractor = EdugameConverter.Convert(distractor, convertedQuestion);
                File distractorImage = saveImage(distractor.ImageName, distractor.Image);
                if (distractorImage != null)
                {
                    convertedDistractor.Image = distractorImage;
                }

                if (convertedDistractor.CreatedBy == null)
                {
                    convertedDistractor.CreatedBy = subModuleItem.Return(x => x.CreatedBy, creator);
                }

                if (convertedDistractor.ModifiedBy == null)
                {
                    convertedDistractor.ModifiedBy = convertedDistractor.CreatedBy;
                }

                if (subModuleItem != null)
                {
                    distractorModel.RegisterSave(convertedDistractor);
                }

                convertedQuestion.Distractors.Add(convertedDistractor);
            }
        }

        /// <summary>
        /// The deserialize edugame questions.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="schemaPath">
        /// The schema path.
        /// </param>
        /// <returns>
        /// The <see cref="EdugameQuestions"/>.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// </exception>
        /// <exception cref="SerializationException">
        /// </exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        private EdugameQuestions DeserializeEdugameQuestions(FormatsEnum format, string filePath, string schemaPath)
        {
            if (!System.IO.File.Exists(filePath) || !System.IO.File.Exists(schemaPath))
            {
                throw new FileNotFoundException();
            }

            string rawData = System.IO.File.ReadAllText(filePath);
            string data = format == FormatsEnum.WebEx ? HttpUtility.HtmlDecode(rawData) : rawData;

            string validationError;
            if (!XsdValidator.ValidateXmlAgainsXsd(data, schemaPath, out validationError))
            {
                throw new SerializationException(validationError);
            }

            EdugameQuestions questions = this.ImportFromString(data, format);

            if (string.IsNullOrEmpty(data) || questions == null || questions.Questions == null
                || questions.Questions.Count == 0)
            {
                throw new ArgumentException();
            }

            return questions;
        }

        /// <summary>
        /// The get save image rutine.
        /// </summary>
        /// <param name="subModuleItem">
        /// The sub module item.
        /// </param>
        /// <param name="creator">
        /// The creator.
        /// </param>
        /// <returns>
        /// The <see cref="Func"/>.
        /// </returns>
        private Func<string, string, File> GetSaveImageRutine(SubModuleItem subModuleItem, User creator)
        {
            Func<string, string, File> saveImage = (imageName, imageData) =>
                {
                    if (string.IsNullOrEmpty(imageName) || string.IsNullOrEmpty(imageData))
                    {
                        return null;
                    }

                    FileModel fileModel = this.FileModel;
                    File file = fileModel.CreateFile(
                        subModuleItem.Return(x => x.CreatedBy, creator), 
                        imageName, 
                        default(DateTime), 
                        null, 
                        null, 
                        null, 
                        null);
                    byte[] byteData = Convert.FromBase64String(imageData);
                    fileModel.SetData(file, byteData);

                    string permanentFileName = fileModel.PermanentFileName(file);
                    using (FileStream stream = System.IO.File.OpenWrite(permanentFileName))
                    {
                        fileModel.ResizeImage(byteData, fileModel.PermanentFileName(file), stream);
                    }

                    return file;
                };
            return saveImage;
        }

        #endregion
    }
}