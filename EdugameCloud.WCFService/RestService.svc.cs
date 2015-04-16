// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using Resources;

    using File = EdugameCloud.Core.Domain.Entities.File;
    using WebException = WcfRestContrib.ServiceModel.Web.Exceptions.WebException;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RestService : BaseService, IRestService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The activate.
        /// </summary>
        /// <param name="activationCode">
        /// The activation code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Activate(string activationCode)
        {
            var arr = UserActivationModel.GetOneByCode(activationCode).Value;
            if (arr == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_USER_INACTIVE,
                    ErrorsTexts.UserActivationError_Subject,
                    ErrorsTexts.UserActivationError_InvalidActivationCode);
                this.LogError("Rest.Activate", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            if (arr.DateExpires < DateTime.Now)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_USER_INACTIVE,
                    ErrorsTexts.UserActivationError_Subject,
                    ErrorsTexts.UserActivationError_InvalidActivationCode);
                this.LogError("Rest.Activate", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            var user = arr.User;
            user.Status = UserStatus.Active;
            user.DateModified = DateTime.Now;
            UserActivationModel model = this.UserActivationModel;
            UserActivation userActivation;
            if ((userActivation = model.GetLatestByUser(user.Id).Value) == null)
            {
                userActivation = new UserActivation
                                     {
                                         User = user,
                                         ActivationCode = Guid.NewGuid().ToString(),
                                         DateExpires = DateTime.Now.AddDays(7)
                                     };
                model.RegisterSave(userActivation);
            }

            UserModel.RegisterSave(user);
            IoC.Resolve<RealTimeNotificationModel>()
                .NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
            UserActivationModel.RegisterDelete(arr);
            this.SendActivationLinkEmail(user.FirstName, user.Email, userActivation.ActivationCode);
            if (WebOperationContext.Current != null)
            {
                var response = WebOperationContext.Current.OutgoingResponse;
                response.StatusCode = HttpStatusCode.Redirect;
                response.Headers.Add(HttpResponseHeader.Location, (string)this.Settings.BasePath);
            }

            return null;
        }

        /// <summary>
        /// The countries list.
        /// </summary>
        /// <returns>
        /// The <see cref="CountryDTO"/>.
        /// </returns>
        public CountryDTO[] GetCountries()
        {
            return IoC.Resolve<CountryModel>().GetAll().Select(x => new CountryDTO(x)).ToArray();
        }

        /// <summary>
        /// The countries list.
        /// </summary>
        /// <returns>
        /// The <see cref="StateDTO"/>.
        /// </returns>
        public StateDTO[] GetStates()
        {
            return IoC.Resolve<StateModel>().GetAll().Select(x => new StateDTO(x)).ToArray();
        }

        /// <summary>
        /// Get file data as content disposition file.
        /// </summary>
        /// <param name="id">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public Stream GetFileStream(string id)
        {
            Error result;
            if (this.Validate(id, out result))
            {
                File file = this.FileModel.GetOneById(Guid.Parse(id)).Value;
                var buffer = this.FileModel.GetData(file);
                if (buffer != null)
                {
                    if (WebOperationContext.Current != null)
                    {
                        WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = "attachment; filename=" + file.FileName;
                        WebOperationContext.Current.OutgoingResponse.Headers["Content-Type"] = @"image\" + Path.GetExtension(file.FileName).If(x => !string.IsNullOrEmpty(x), x => x.Substring(1));
                    }

                    return new MemoryStream(buffer);
                }
            }

            if (result != null)
            {
                this.LogError("Rest.GetFileStream", result);
                throw new FaultException<Error>(result, result.errorMessage);
            }

            throw new WebException(HttpStatusCode.NotFound, "File not found");
        }

        /// <summary>
        /// Get file data as File.
        /// </summary>
        /// <param name="id">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public Stream GetFile(string id)
        {
            Error result;
            if (this.Validate(id, out result))
            {
                File file = this.FileModel.GetOneById(Guid.Parse(id)).Value;
                var buffer = this.FileModel.GetData(file);
                if (buffer != null)
                {
                    var contentType = file.FileName.GetContentTypeByExtension();
                    if (WebOperationContext.Current != null && !string.IsNullOrWhiteSpace(contentType))
                    {
                        WebOperationContext.Current.OutgoingResponse.Headers["Content-Type"] = contentType;
                    }

                    return new MemoryStream(buffer);
                }
            }

            if (result != null)
            {
                this.LogError("Rest.GetFile", result);
                throw new FaultException<Error>(result, result.errorMessage);
            }

            throw new WebException(HttpStatusCode.NotFound, "File not found");
        }

        #endregion

        #region Methods

        /// <summary>
        /// The is valid.
        /// </summary>
        /// <param name="imageId">
        /// The file id.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool Validate(string imageId, out Error result)
        {
            result = this.Validate(imageId);
            return result == null;
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="imageId">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="Error"/>.
        /// </returns>
        private Error Validate(string imageId)
        {
            if (string.IsNullOrWhiteSpace(imageId))
            {
               return new Error(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "FileIdValidation", "Provided fileId is empty");
            }

            Guid id;
            if (!Guid.TryParse(imageId, out id))
            {
                return new Error(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "FileIdValidation", "Provided fileId is not UUID");
            }

            return null;
        }

        #endregion
    }
}