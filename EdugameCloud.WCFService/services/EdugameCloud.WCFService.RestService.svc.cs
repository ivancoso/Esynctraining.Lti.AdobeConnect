﻿// ReSharper disable CheckNamespace
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
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using Resources;

    using File = EdugameCloud.Core.Domain.Entities.File;
    using WebException = WcfRestContrib.ServiceModel.Web.Exceptions.WebException;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RestService : BaseService, IRestService
    {
        #region Properties

        /// <summary>
        ///     Gets the authentication model.
        /// </summary>
        private AuthenticationModel AuthenticationModel
        {
            get
            {
                return IoC.Resolve<AuthenticationModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The activate.
        /// </summary>
        /// <param name="activationCode">
        /// The activation code.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<string> Activate(string activationCode)
        {
            var result = new ServiceResponse<string>();
            var arr = UserActivationModel.GetOneByCode(activationCode).Value;
            if (arr == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_USER_INACTIVE, ErrorsTexts.UserActivationError_Subject, ErrorsTexts.UserActivationError_InvalidActivationCode));
            }
            else if (arr.DateExpires < DateTime.Now)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_USER_INACTIVE, ErrorsTexts.UserActivationError_Subject, ErrorsTexts.UserActivationError_InvalidActivationCode));
            }
            else
            {
                var user = arr.User;
                user.Status = UserStatus.Active;
                user.DateModified = DateTime.Now;
                var newPassword = AuthenticationModel.CreateRandomPassword();
                user.SetPassword(newPassword);
                UserModel.RegisterSave(user);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
                UserActivationModel.RegisterDelete(arr);
                this.SendPasswordEmail(user.FirstName, user.Email, newPassword);
                if (WebOperationContext.Current != null)
                {
                    var response = WebOperationContext.Current.OutgoingResponse;
                    response.StatusCode = HttpStatusCode.Redirect;
                    response.Headers.Add(HttpResponseHeader.Location, (string)this.Settings.BasePath);
                }

                return null;
            }

            return result;
        }

        /// <summary>
        /// The countries list.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CountryDTO> GetCountries()
        {
            var result = new ServiceResponse<CountryDTO>();
            var arr = IoC.Resolve<CountryModel>();
            result.@objects = arr.GetAll().Select(x => new CountryDTO(x)).ToList();
            return result;
        }

        /// <summary>
        /// The countries list.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<StateDTO> GetStates()
        {
            var result = new ServiceResponse<StateDTO>();
            var arr = IoC.Resolve<StateModel>();
            result.@objects = arr.GetAll().Select(x => new StateDTO(x)).ToList();
            return result;
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
            ServiceResponse<string> result;
            if (this.Validate(id, out result))
            {
                File file = this.FileModel.GetOneById(int.Parse(id)).Value;
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
            ServiceResponse<string> result;
            if (this.Validate(id, out result))
            {
                File file = this.FileModel.GetOneById(int.Parse(id)).Value;
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
        private bool Validate(string imageId, out ServiceResponse<string> result)
        {
            result = new ServiceResponse<string>();
            result = (ServiceResponse<string>)this.Validate(imageId, result);
            return result.error == null;
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="imageId">
        /// The file id.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        private ServiceResponse Validate(string imageId, ServiceResponse result)
        {
            if (string.IsNullOrWhiteSpace(imageId))
            {
                result.error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "FileIdValidation", "Provided fileId is empty");
                return result;
            }

            int id;
            if (!int.TryParse(imageId, out id))
            {
                result.error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "FileIdValidation", "Provided fileId is not numeric");
                return result;
            }

            return result;
        }

        #endregion
    }
}