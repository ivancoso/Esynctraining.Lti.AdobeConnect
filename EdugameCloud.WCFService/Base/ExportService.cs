namespace EdugameCloud.WCFService.Base
{
    using EdugameCloud.Core.Business.Models;
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    /// <summary>
    /// The export service.
    /// </summary>
    public abstract class ExportService : BaseService
    {
        #region Properties

        /// <summary>
        /// Gets the question type model.
        /// </summary>
        protected QuestionTypeModel QuestionTypeModel
        {
            get { return IoC.Resolve<QuestionTypeModel>(); }
        }

        /// <summary>
        /// Gets the question model.
        /// </summary>
        protected QuestionModel QuestionModel
        {
            get { return IoC.Resolve<QuestionModel>(); }
        }

        /// <summary>
        /// Gets the distractor model.
        /// </summary>
        protected DistractorModel DistractorModel
        {
            get { return IoC.Resolve<DistractorModel>(); }
        }

        /// <summary>
        /// Gets the storage path.
        /// </summary>
        protected string StoragePath
        {
            get { return Settings.FileStorage as string ?? string.Empty; }
        }

        /// <summary>
        /// Gets the export path.
        /// </summary>
        protected string ExportPath
        {
            get { return Path.Combine(StoragePath, Settings.ExportSubPath as string ?? string.Empty); }
        }

        /// <summary>
        /// Gets the import path.
        /// </summary>
        protected string ImportPath
        {
            get { return Path.Combine(StoragePath, Settings.ImportSubPath as string ?? string.Empty); }
        }

        /// <summary>
        /// Gets the schemas path.
        /// </summary>
        protected string SchemasPath
        {
            get { return Path.Combine(StoragePath, Settings.SchemasSubPath as string ?? string.Empty); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets questions.
        /// </summary>
        /// <param name="questions">Questions from stored procedure.</param>
        /// <param name="distractors">Distractors from stored procedure.</param>
        /// <param name="isInvalid">Indicates whether questions were retrieved.</param>
        /// <returns>List of questions.</returns>
        protected List<EdugameQuestion> GetEdugameQuestions(IEnumerable<Question> questions, IEnumerable<Distractor> distractors, out bool isInvalid)
        {
            var questionTypes = this.QuestionTypeModel.GetAllActive();
            Func<int, QuestionType> getQuestionTypeById = id => questionTypes.FirstOrDefault(type => type.Id == id);
            Func<Core.Domain.Entities.File, string> getImageByFile = file =>
            {
                string result = string.Empty;
                if (file != null)
                {
                    var raw = this.FileModel.GetData(file);
                    if (raw != null)
                    {
                        result = Convert.ToBase64String(raw);
                    }
                }

                return result;
            };
            
            Func<Core.Domain.Entities.File, string> getImageNameByFile = file =>
            {
                string result = string.Empty;
                if (file != null)
                {
                    result = file.FileName;
                }

                return result;
            };

            var edugameQuestions = questions.Select(question => EdugameConverter.Convert(question, getQuestionTypeById(question.QuestionType.Id), distractors.Where(x => x.Question.Id == question.Id), getImageByFile, getImageNameByFile)).ToList();
            isInvalid = edugameQuestions.Any(question => question == null || question.Type == null || question.Distractors.Any(distractor => distractor == null));

            return edugameQuestions;
        }

        /// <summary>
        /// Export questions to string.
        /// </summary>
        /// <param name="smiId">SubModule item Id.</param>
        /// <param name="questionIds">Question Ids.</param>
        /// <returns>Serialized string.</returns>
        protected string ExportToString(int smiId, List<int> questionIds = null)
        {
            var result = string.Empty;
            var questions = this.QuestionModel.GetByQuestionIdsAndSmiID(smiId, questionIds).ToList();
            var distractors = this.DistractorModel.GetAllByQuestionsIds(questions.Select(x => x.Id).ToList()).ToList();
            bool isInvalid;
            var edugameQuestions = this.GetEdugameQuestions(questions, distractors, out isInvalid);

            if (!isInvalid)
            {
                if (edugameQuestions.Any())
                {
                    try
                    {
                        var list = new EdugameQuestions(smiId, edugameQuestions);
                        result = list.Serialize();
                    }
                    catch
                    {
                        // It will be handled later.
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Export entity by sub module item Id.
        /// </summary>
        /// <param name="smiId">Sub module item id.</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        protected ServiceResponse<string> Export(int smiId)
        {
            return this.Export(smiId, null);
        }

        /// <summary>
        /// Export questions by sub module item Id.
        /// </summary>
        /// <param name="smiId">SubModule item id.</param>
        /// <param name="questionIds">Question Ids.</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        protected ServiceResponse<string> Export(int smiId, List<int> questionIds)
        {
            var result = new ServiceResponse<string>();
            try
            {
                var data = this.ExportToString(smiId, questionIds);
                if (string.IsNullOrEmpty(data))
                {
                    throw new ArgumentException();
                }

                var id = Guid.NewGuid().ToString();
                var fileName = id + ".xml";
                var filePath = Path.Combine(this.ExportPath, fileName);

                System.IO.File.WriteAllText(filePath, data);

                result.@object = id;
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
            }
            catch (ArgumentException)
            {
                this.LogError(ErrorsTexts.ExportError_Subject, result);
                result.SetError(new Error(Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, ErrorsTexts.ExportError_Subject, ErrorsTexts.ExportError_NoData));
            }
            catch
            {
                this.LogError(ErrorsTexts.ExportError_Subject, result);
                result.SetError(new Error(Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, ErrorsTexts.ExportError_Subject, ErrorsTexts.ExportError_InvalidFormat));
            }

            return result;
        }

        /// <summary>
        /// Import questions from string.
        /// </summary>
        /// <param name="data">String for importing.</param>
        /// <param name="format">Questions format.</param>
        /// <returns>Questions collection.</returns>
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
                        var questionTypes = this.QuestionTypeModel.GetAllActive().ToList();
                        var pool = data.Deserialize<WebExPool>();
                        result = WebExConverter.Convert(pool, questionTypes);
                        break;
                }
            }
            catch
            {
                // It will be handled later.
            }

            return result;
        }

        /// <summary>
        /// Get questions from imported file.
        /// </summary>
        /// <param name="fileId">Imported file id.</param>
        /// <param name="smiId">Sub module item id.</param>
        /// <param name="format">Questions format.</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        protected ServiceResponse Import(string fileId, int smiId, FormatsEnum format)
        {
            var result = new ServiceResponse();
            try
            {
                var subModuleItem = this.SubModuleItemModel.GetOneById(smiId).Value;
                var fileName = fileId + ".xml";
                var filePath = Path.Combine(this.ImportPath, fileName);
                var schemaPath = Path.Combine(this.SchemasPath, format + ".xsd");

                if (subModuleItem == null)
                {
                    throw new ArgumentException();
                }

                if (!System.IO.File.Exists(filePath) || !System.IO.File.Exists(schemaPath))
                {
                    throw new FileNotFoundException();
                }

                var rawData = System.IO.File.ReadAllText(filePath);
                var data = format == FormatsEnum.WebEx ? HttpUtility.HtmlDecode(rawData) : rawData;

                string validationError;
                if (!XsdValidator.ValidateXmlAgainsXsd(data, schemaPath, out validationError))
                {
                    throw new SerializationException(validationError);
                }
                
                var questions = this.ImportFromString(data, format);

                if (string.IsNullOrEmpty(data) || questions == null || questions.Questions == null || questions.Questions.Count == 0)
                {
                    throw new ArgumentException();
                }

                var questionModel = this.QuestionModel;
                var distractorModel = this.DistractorModel;

                Func<string, string, Core.Domain.Entities.File> saveImage = (imageName, imageData) =>
                {
                    if (string.IsNullOrEmpty(imageName) || string.IsNullOrEmpty(imageData))
                    {
                        return null;
                    }

                    var fileModel = this.FileModel;
                    var file = fileModel.CreateFile(subModuleItem.CreatedBy, imageName, default(DateTime), null, null, null, null);
                    var byteData = Convert.FromBase64String(imageData);
                    fileModel.SetData(file, byteData);

                    var permanentFileName = fileModel.PermanentFileName(file);
                    using (var stream = System.IO.File.OpenWrite(permanentFileName))
                    {
                        fileModel.ResizeImage(byteData, fileModel.PermanentFileName(file), stream);
                    }
                    
                    return file;
                };
                
                foreach (var question in questions.Questions)
                {
                    var convertedQuestion = EdugameConverter.Convert(question);
                    convertedQuestion.SubModuleItem = subModuleItem;
                    if (convertedQuestion.CreatedBy == null)
                    {
                        convertedQuestion.CreatedBy = subModuleItem.CreatedBy;
                    }

                    if (convertedQuestion.ModifiedBy == null)
                    {
                        convertedQuestion.ModifiedBy = convertedQuestion.CreatedBy;
                    }

                    var questionImage = saveImage(question.ImageName, question.Image);
                    if (questionImage != null)
                    {
                        convertedQuestion.Image = questionImage;
                    }

                    questionModel.RegisterSave(convertedQuestion);
                    foreach (var distractor in question.Distractors)
                    {
                        var convertedDistractor = EdugameConverter.Convert(distractor, convertedQuestion);
                        var distractorImage = saveImage(distractor.ImageName, distractor.Image);
                        if (distractorImage != null)
                        {
                            convertedDistractor.Image = distractorImage;
                        }

                        distractorModel.RegisterSave(convertedDistractor);
                    }
                }

                System.IO.File.Delete(filePath);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
            }
            catch (ArgumentException)
            {
                this.LogError(ErrorsTexts.ImportError_Subject, result);
                result.SetError(new Error(Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, ErrorsTexts.ImportError_Subject, ErrorsTexts.ImportError_NoData));
            }
            catch (SerializationException)
            {
                this.LogError(ErrorsTexts.ImportError_Subject, result);
                result.SetError(new Error(Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, ErrorsTexts.ImportError_Subject, ErrorsTexts.ImportError_InvalidFormat));
            }
            catch (FileNotFoundException)
            {
                this.LogError(ErrorsTexts.ImportError_Subject, result);
                result.SetError(new Error(Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, ErrorsTexts.ImportError_Subject, ErrorsTexts.ImportError_NotFound));
            }
            catch
            {
                this.LogError(ErrorsTexts.ImportError_Subject, result);
                result.SetError(new Error(Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, ErrorsTexts.ImportError_Subject, ErrorsTexts.ImportError_Unknown));
            }

            return result;
        }

        #endregion
    }
}