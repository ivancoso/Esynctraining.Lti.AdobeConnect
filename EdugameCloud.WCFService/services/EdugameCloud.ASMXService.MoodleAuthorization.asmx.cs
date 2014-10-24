// ReSharper disable once CheckNamespace
namespace EdugameCloud.ASMXService
{
    using System;
    using System.Linq;
    using System.Web.Services;

    using Castle.Core.Logging;
    using Castle.MicroKernel;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;

    using FluentValidation;
    using FluentValidation.Results;

    using Resources;

    /// <summary>
    /// Moodle Authorization Service instance
    /// </summary>
    ////    [System.Web.Script.Services.ScriptService]
    [WebService(Namespace = "http://dev.edugamecloud.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class MoodleAuthorization : WebService
    {
        #region Properties

        /// <summary>
        ///     Gets the user parameters model.
        /// </summary>
        private LmsUserParametersModel LmsUserParametersModel
        {
            get
            {
                return IoC.Resolve<LmsUserParametersModel>();
            }
        }

        /// <summary>
        ///     Gets the user model.
        /// </summary>
        private LmsUserModel LmsUserModel
        {
            get
            {
                return IoC.Resolve<LmsUserModel>();
            }
        }

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        private ILogger Logger
        {
            get
            {
                return IoC.Resolve<ILogger>();
            }
        }

        #endregion

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="acId">
        /// The AC id.
        /// </param>
        /// <param name="course">
        /// The course.
        /// </param>
        /// <param name="domain">
        /// The domain.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="wstoken">
        /// The WS token.
        /// </param>
        /// <returns>
        /// The <see cref="LmsUserParametersDTO"/>.
        /// </returns>
        [WebMethod]
        public LmsUserParametersDTO Save(string acId, int course, string domain, string provider, string wstoken)
        {
            var dto = new LmsUserParametersDTO
                      {
                          acId = acId,
                          course = course,
                          domain = domain,
                          provider = provider,
                          wstoken = wstoken
                      };
            var result = new ServiceResponse<LmsUserParametersDTO>();
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                var param = this.LmsUserParametersModel.GetOneByAcId(dto.acId).Value;
                param = this.ConvertDto(dto, param);
                this.LmsUserParametersModel.RegisterSave(param, true);
                return new LmsUserParametersDTO(param);
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return new LmsUserParametersDTO { errorMessage = result.error.errorMessage, errorDetails = result.error.errorDetail };
        }

        #region Methods

        /// <summary>
        /// The is valid.
        /// </summary>
        /// <param name="obj">
        /// The object to check.
        /// </param>
        /// <param name="validationResult">
        /// The validation result.
        /// </param>
        /// <typeparam name="T">
        /// The type of object to check
        /// </typeparam>
        /// <returns>
        /// The validation result <see cref="bool"/>.
        /// </returns>
        private bool IsValid<T>(T obj, out ValidationResult validationResult)
        {
            validationResult = null;
            try
            {
                validationResult = IoC.Resolve<IValidator<T>>().Validate(obj);
                return validationResult.IsValid;
            }
            catch (ComponentNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// The update result.
        /// </summary>
        /// <typeparam name="T">
        /// The service response
        /// </typeparam>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="validationResult">
        /// The validation result.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        private T UpdateResult<T>(T result, ValidationResult validationResult) where T : ServiceResponse
        {
            if (validationResult != null)
            {
                foreach (ValidationFailure failure in validationResult.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(failure.ErrorMessage) && failure.ErrorMessage.Contains("#_#"))
                    {
                        int errorCode;
                        var errorDetails = failure.ErrorMessage.Split(new[] { "#_#" }, StringSplitOptions.RemoveEmptyEntries);
                        if (errorDetails.FirstOrDefault() == null || !int.TryParse(errorDetails.FirstOrDefault(), out errorCode))
                        {
                            errorCode = Errors.CODE_ERRORTYPE_GENERIC_ERROR;
                        }

                        result.status = Errors.CODE_RESULTTYPE_ERROR;
                        result.error = new Error { errorCode = errorCode, errorMessage = errorDetails.ElementAtOrDefault(1) };
                    }
                    else
                    {
                        result.status = Errors.CODE_RESULTTYPE_ERROR;
                        result.error = new Error { errorCode = Errors.CODE_ERRORTYPE_GENERIC_ERROR, errorMessage = failure.ErrorMessage };
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The log error.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        private void LogError(string methodName, ServiceResponse result, string userName)
        {
            if (result != null && result.error != null)
            {
                this.Logger.Error(
                    string.Format(
                        "{4}. Error result for user: {0}, Error code: {1}, Error Message: {2}, Error Details: {3}",
                        userName,
                        result.error.errorCode,
                        result.error.errorMessage,
                        result.error.errorDetail,
                        methodName));
            }
        }

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="q">
        /// The result DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="LmsUserParameters"/>.
        /// </returns>
        private LmsUserParameters ConvertDto(LmsUserParametersDTO q, LmsUserParameters instance)
        {
            instance = instance ?? new LmsUserParameters();
            instance.AcId = q.acId;
            instance.Course = q.course;
            instance.Wstoken = q.wstoken;
            instance.LmsUser = q.lmsUserId.HasValue && q.lmsUserId.Value != 0
                                   ? this.LmsUserModel.GetOneById(q.lmsUserId.Value).Value
                                   : null;

            return instance;
        }

        #endregion
    }
}
