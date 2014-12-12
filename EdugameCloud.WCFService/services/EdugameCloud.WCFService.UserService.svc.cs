// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.Lti.Business.Models;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using EdugameCloud.WCFService.DTO;
    using EdugameCloud.WCFService.ViewModels;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class UserService : BaseService, IUserService
    {
        #region Properties

        /// <summary>
        ///     Gets the user login history model.
        /// </summary>
        protected UserLoginHistoryModel UserLoginHistoryModel
        {
            get
            {
                return IoC.Resolve<UserLoginHistoryModel>();
            }
        }

        /// <summary>
        ///     Gets the language model.
        /// </summary>
        private LanguageModel LanguageModel
        {
            get
            {
                return IoC.Resolve<LanguageModel>();
            }
        }

        /// <summary>
        ///     Gets the time zone model.
        /// </summary>
        private TimeZoneModel TimeZoneModel
        {
            get
            {
                return IoC.Resolve<TimeZoneModel>();
            }
        }

        /// <summary>
        ///     Gets the UserRole model.
        /// </summary>
        private UserRoleModel UserRoleModel
        {
            get
            {
                return IoC.Resolve<UserRoleModel>();
            }
        }

        /// <summary>
        ///     Gets the CompanyLicense model.
        /// </summary>
        private CompanyLicenseModel CompanyLicenseModel
        {
            get
            {
                return IoC.Resolve<CompanyLicenseModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private CompanyModel CompanyModel
        {
            get
            {
                return IoC.Resolve<CompanyModel>();
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
        ///     Gets the social user tokens
        /// </summary>
        protected SocialUserTokensModel SocialUserTokensModel
        {
            get
            {
                return IoC.Resolve<SocialUserTokensModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SocialUserTokensDTO> GetSocialUserTokens(string key)
        {
            var result = new ServiceResponse<SocialUserTokensDTO>();
            SocialUserTokens user;
            if ((user = this.SocialUserTokensModel.GetOneByKey(key).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.GetById_NoUserExists));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SocialUserTokensDTO(user);
            }

            return result;
        }

        /// <summary>
        /// The activate by id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="logoId">
        /// The logo Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse UpdateLogo(int userId, Guid logoId)
        {
            var result = new ServiceResponse();
            var model = this.UserModel;
            User user;
            File file;
            if ((user = model.GetOneById(userId).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists));
            }
            else if ((file = this.FileModel.GetOneById(logoId).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_NotFound,
                        ErrorsTexts.ImageUploadFailed_FileNotFound));
            }
            else
            {
                user.Logo = file;
                model.RegisterSave(user, true);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
            }

            return result;
        }

        /// <summary>
        /// The activate by id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse ActivateById(int userId)
        {
            var result = new ServiceResponse();
            UserModel model = this.UserModel;
            User user;
            if ((user = model.GetOneById(userId).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.DeleteById_NoUserExists));
            }
            else
            {
                user.Status = UserStatus.Active;
                model.RegisterSave(user, true);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
            }

            return result;
        }

        /// <summary>
        /// The deactivate by id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse DeactivateById(int userId)
        {
            var result = new ServiceResponse();
            UserModel model = this.UserModel;
            User user;
            if ((user = model.GetOneById(userId).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.DeleteById_NoUserExists));
            }
            else
            {
                user.Status = UserStatus.Inactive;
                model.RegisterSave(user, true);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
            }

            return result;
        }

        /// <summary>
        /// The deactivate by id.
        /// </summary>
        /// <param name="userIds">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse ActivateByIds(List<int> userIds)
        {
            var result = new ServiceResponse();
            UserModel model = this.UserModel;
            IEnumerable<User> users;
            if (!(users = model.GetAllByIds(userIds)).Any())
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists));
            }
            else
            {
                foreach (var user in users)
                {
                    user.Status = UserStatus.Active;
                    model.RegisterSave(user, true);
                    IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
                }
            }

            return result;
        }

        /// <summary>
        /// The deactivate by id.
        /// </summary>
        /// <param name="userIds">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse DeactivateByIds(List<int> userIds)
        {
            var result = new ServiceResponse();
            UserModel model = this.UserModel;
            IEnumerable<User> users;
            if (!(users = model.GetAllByIds(userIds)).Any())
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists));
            }
            else
            {
                foreach (var user in users)
                {
                    user.Status = UserStatus.Inactive;
                    model.RegisterSave(user, true);
                    IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
                }

                model.Flush();
            }

            return result;
        }

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<int> DeleteById(int id)
        {
            var result = new ServiceResponse<int>();
            UserModel model = this.UserModel;
            User user;
            if ((user = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists));
            }
            else
            {
                model.RegisterDelete(user, true);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Delete, user.Company.Id, user.Id);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
        }

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="userIds">
        /// The user Ids.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<int> DeleteByIds(List<int> userIds)
        {
            var result = new ServiceResponse<int>();
            var res = new List<int>();
            UserModel model = this.UserModel;
            IEnumerable<User> users;
            if (!(users = model.GetAllByIds(userIds)).Any())
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists));
            }
            else
            {
                foreach (var user in users)
                {
                    model.RegisterDelete(user, true);
                    IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
                    res.Add(user.Id);
                }
            }

            result.objects = res;
            return result;
        }

        /// <summary>
        /// The forgot password.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse ForgotPassword(string email)
        {
            var result = new ServiceResponse();
            ValidationResult validationResult;
            if (this.IsValid(new ForgetPasswordViewModel { Email = email }, out validationResult))
            {
                User user = this.UserModel.GetOneByEmail(email).Value;
                
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
                
                user.Status = UserStatus.Active;
                this.UserModel.RegisterSave(user);
                this.SendActivationLinkEmail(user.FirstName, user.Email, userActivation.ActivationCode);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError("ForgotPassword", result, email);
            return result;
        }

        /// <summary>
        ///     All users test.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<UserDTO> GetAll()
        {
            return new ServiceResponse<UserDTO>
                       {
                           objects = this.UserModel.GetAll().Select(x => new UserDTO(x)).ToList()
                       };
        }

        /// <summary>
        /// The get all for company.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<UserWithLoginHistoryDTO> GetAllForCompany(int companyId)
        {
            return new ServiceResponse<UserWithLoginHistoryDTO> 
                       {
                           objects = this.UserModel.GetAllForCompany(companyId).ToList()
                       };
        }

        /// <summary>
        /// The get login history for contact.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<UserLoginHistoryDTO> GetLoginHistoryForCompany(int companyId)
        {
            var result = new ServiceResponse<UserLoginHistoryDTO>();
            Company company;
            if ((company = this.CompanyModel.GetOneById(companyId).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_USER, ErrorsTexts.AccessError_Subject, ErrorsTexts.GetById_NoUserExists));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                var history = this.UserLoginHistoryModel.GetAllForUsers(company.Users.Select(x => x.Id).ToList()).ToList();
                result.objects = history.Select(x => new UserLoginHistoryDTO(x, company)).ToList();
            }

            return result;
        }

        /// <summary>
        /// The get login history for contact.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<UserLoginHistoryDTO> GetLoginHistoryForUser(int userId)
        {
            var result = new ServiceResponse<UserLoginHistoryDTO>();
            if (this.UserModel.GetOneById(userId).Value == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_USER, ErrorsTexts.AccessError_Subject, ErrorsTexts.GetById_NoUserExists));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                var history = this.UserLoginHistoryModel.GetAllForUser(userId).ToList();
                var companies = this.CompanyModel.GetAllByUsers(history.Select(x => x.User.Id).Distinct().ToList());
                result.objects = history.Select(x => new UserLoginHistoryDTO(x, companies.FirstOrDefault(c => c.Id == x.User.Company.Id))).ToList();
            }

            return result;
        }

        /// <summary>
        /// The get login history paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<UserLoginHistoryDTO> GetLoginHistoryPaged(int pageIndex, int pageSize)
        {
            var result = new ServiceResponse<UserLoginHistoryDTO>();
            int totalCount;
            var history = this.UserLoginHistoryModel.GetAllPaged(pageIndex, pageSize, out totalCount).ToList();
            var companies = this.CompanyModel.GetAllByUsers(history.Select(x => x.User.Id).Distinct().ToList());
            result.objects = history.Select(x => new UserLoginHistoryDTO(x, companies.FirstOrDefault(c => c.Id == x.User.Company.Id))).ToList();
            result.totalCount = totalCount;
            return result;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<UserDTO> GetById(int id)
        {
            var result = new ServiceResponse<UserDTO>();
            User user;
            if ((user = this.UserModel.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.GetById_NoUserExists));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new UserDTO(user);
            }

            return result;
        }

        /// <summary>
        /// The request session token.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SessionDTO> RequestSessionToken(int userId)
        {
            var result = new ServiceResponse<SessionDTO>();
            User user;
            if ((user = this.UserModel.GetOneById(userId).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.AccessError_NoUserExistsWithTheGivenEmail));
            }
            else if (user.Status != UserStatus.Active)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_USER_INACTIVE,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.AccessError_UserIsInactive));
            }
            else
            {
                result.@object = this.GetOrSetANewSession(user);
            }

            return result;
        }

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<UserWithSplashScreenDTO> Login(LoginWithHistoryDTO dto)
        {
            var result = new ServiceResponse<UserWithSplashScreenDTO>();
            User user;
            if ((user = this.UserModel.GetOneByEmail(dto.email).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.AccessError_NoUserExistsWithTheGivenEmail));
            }
            else if (user.Status != UserStatus.Active)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_USER_INACTIVE, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.AccessError_UserIsInactive));
            }
            else if (user.Company.Status != CompanyStatus.Active)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_USER_INACTIVE,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.AccessError_CompanyIsInactive));
            }
            else if (user.Company.CurrentLicense == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_EXPIRED_LICENSE,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.AccessError_CompanyLicenseIsExpired));
            }
            else if (!user.ValidatePasswordHash(dto.passwordHash) && !user.ValidatePassword(dto.passwordHash))
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.AccessError_InvalidPassword));
            }
            else
            {
                result.@object = new UserWithSplashScreenDTO(user);
                var userHistory = new UserLoginHistory
                                      {
                                          Application = dto.application,
                                          FromIP = dto.fromIP,
                                          User = user,
                                          DateCreated = DateTime.Now
                                      };

                this.UserLoginHistoryModel.RegisterSave(userHistory);

                result.@object.session = this.GetOrSetANewSession(user);
                if (dto.sendSplashScreen)
                {
                    result.@object.splashScreen = new SplashScreenDTO
                        {
                            recentReports = this.GetBaseRecentSplashScreenReports(user.Id, 1, 10).objects.ToList(),
                            reports = this.GetBaseSplashScreenReports(user.Id, 1, 5).objects.ToList()
                        };
                }

                var companyLms = CompanyLmsModel.GetAllByCompanyId(user.Company.Id);
                result.@object.companyLms = companyLms.Select(c => new CompanyLmsDTO(c)).ToArray();
            }

            return result;
        }

        /// <summary>
        /// The upload batch users.
        /// </summary>
        /// <param name="batch">
        /// The batch.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<UserDTO> UploadBatchUsers(BatchUsersDTO batch)
        {
            var result = new ServiceResponse<UserDTO>();
            ValidationResult validationResult;
            if (this.IsValid(batch, out validationResult))
            {
                var company = this.CompanyModel.GetOneById(batch.companyId).Value;
                List<string> failedRows;
                string error;
                var results = this.UserModel.UploadBatchOfUsers(company, batch.csvOrExcelContent, batch.type, out failedRows, out error, this.SendActivation).ToList();
                if (results.Any())
                {
                    result.objects = results.Select(x => new UserDTO(x)).ToList();
                }

                if (error != null || failedRows != null)
                {
                    result.SetError(new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, error, this.CombineFailed(failedRows)));
                    result.status = results.Any() ? Errors.CODE_RESULTTYPE_PARTIALSUCCESS : Errors.CODE_RESULTTYPE_ERROR;
                }
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError("Upload batch users", result);
            return result;
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<int> GetCompanyIdByEmail(string email)
        {
            var result = new ServiceResponse<int>();
            User user;
            if ((user = this.UserModel.GetOneByEmail(email).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.@object = user.Company.With(x => x.Id);
            }

            return result;
        }

        /// <summary>
        /// The registration.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1515:SingleLineCommentMustBePrecededByBlankLine", Justification = "Reviewed. Suppression is OK here.")]
        public ServiceResponse<UserDTO> Save(UserDTO user)
        {
            var result = new ServiceResponse<UserDTO>();
            ValidationResult validationResult;
            if (this.IsValid(user, out validationResult))
            {
                UserModel userModel = this.UserModel;
                bool isTransient = user.userId == 0;
                User userInstance = !isTransient
                                        ? userModel.GetOneById(user.userId).Value
                                        : userModel.GetOneByEmail(user.email).Value;
                if (isTransient && userInstance == null)
                {
                    var license = this.CompanyLicenseModel.GetOneByCompanyId(user.companyId).Value;
                    var currentUsersCount = this.UserModel.GetCountForCompany(user.companyId).Value;
                    if (license != null && license.TotalLicensesCount < currentUsersCount + 1)
                    {
                        result.SetError(
                            new Error(
                                Errors.TOO_MANY_USERS,
                                ErrorsTexts.UserCreateUpdateError_Subject,
                                ErrorsTexts.EntityCreationError_Subject,
                                ErrorsTexts.UserCreationError_LicenseError));
                        return result;
                    }
                }

                if (!isTransient && userInstance != null && userInstance.Status != UserStatus.Active)
                {
                    result.SetError(
                        new Error(
                            Errors.CODE_ERRORTYPE_USER_INACTIVE, 
                            ErrorsTexts.UserCreateUpdateError_Subject, 
                            ErrorsTexts.UserCreateUpdateError_NotActivated, 
                            ErrorsTexts.UserCreateUpdateError_NotActivated_Details));
                }
                else if (userInstance == null || !isTransient)
                {
                    if (isTransient && string.IsNullOrWhiteSpace(user.password))
                    {
                        user.password = AuthenticationModel.CreateRandomPassword();
                    }

                    // bool passwordChanged = false, 
                    bool emailChanged;
                    userInstance = this.ConvertDto(user, userInstance, out emailChanged);
                    if (isTransient
                        || (!string.IsNullOrWhiteSpace(user.password) && !userInstance.ValidatePassword(user.password)))
                    {
                        // passwordChanged = true;
                        userInstance.SetPassword(user.password);
                    }

                    userModel.RegisterSave(userInstance, true);
                    IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, userInstance.Company.Id, userInstance.Id);

                    if (isTransient)
                    {
                        this.SendActivation(userInstance);
                    }
////                    else if (passwordChanged || emailChanged)
////                    {
////                        this.SendPasswordEmail(user.firstName, user.email, user.password);
////                    }

                    result.@object = new UserDTO(userInstance);
                }
                else
                {
                    result.SetError(
                        new Error(
                            Errors.CODE_ERRORTYPE_USER_EXISTING, 
                            ErrorsTexts.UserCreationError_Subject, 
                            ErrorsTexts.UserCreationError_AlreadyExists, 
                            ErrorsTexts.UserCreationError_AlreadyExists_Details));
                }

                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.UserCreationError_Subject, result, user.With(x => x.email));
            return result;
        }

        /// <summary>
        /// The send activation.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<string> SendActivation(string email)
        {
            var result = new ServiceResponse<string>();
            return result;
        }

        /// <summary>
        /// The update password.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="newPassword">
        /// The new password.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse UpdatePasswordByCode(string code, string newPassword)
        {
            UserActivation userActivation;
            var result = new ServiceResponse();
            if ((userActivation = this.UserActivationModel.GetOneByCode(code).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.UserActivationError_InvalidActivationCode));
            }
            else if (userActivation.User.Status != UserStatus.Activating)
            {
                result.SetError(
                     new Error(
                         Errors.CODE_ERRORTYPE_USER_INACTIVE,
                         ErrorsTexts.AccessError_Subject,
                         ErrorsTexts.UserActivationError_Subject));
            }
            else
            {
                var contact = userActivation.User;
                contact.SetPassword(newPassword);
                contact.Status = UserStatus.Active;
                this.UserModel.RegisterSave(contact, true);
                var activations = this.UserActivationModel.GetAllByUser(contact.Id);
                foreach (var passwordActivation in activations)
                {
                    this.UserActivationModel.RegisterDelete(passwordActivation);
                }

                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, contact.Company.Id, contact.Id);
                this.SendPasswordEmail(contact.FirstName, contact.Email, newPassword);
            }

            this.LogError("UpdatePasswordByCode", result);
            return result;
        }

        /// <summary>
        /// The activate by id.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="Esynctraining.Core.Domain.Contracts.ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<bool> ActivateByCode(string code)
        {
            var result = new ServiceResponse<bool>();
            var passwordActivation = this.UserActivationModel.GetOneByCode(code).Value;
            var contact = passwordActivation.With(x => x.User);
            if (contact != null)
            {
                contact.Status = UserStatus.Activating;
                this.UserModel.RegisterSave(contact, true);
                result.@object = true;
            }
            else
            {
                result.@object = false;
                result.SetError(
                   new Error(
                       Errors.CODE_ERRORTYPE_INVALID_USER,
                       ErrorsTexts.AccessError_Subject,
                       ErrorsTexts.DeleteById_NoUserExists));
            }

            return result;
        }

        /// <summary>
        /// The update password.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="oldPasswordHash">
        /// The old password.
        /// </param>
        /// <param name="newPassword">
        /// The new password.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse UpdatePassword(string email, string oldPasswordHash, string newPassword)
        {
            User user;
            var result = new ServiceResponse();
            if ((user = this.UserModel.GetOneByEmail(email).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.AccessError_NoUserExistsWithTheGivenEmail));
            }
            else if (!user.ValidatePasswordHash(oldPasswordHash) && !user.ValidatePassword(oldPasswordHash))
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.AccessError_InvalidPassword));
            }
            else
            {
                user.SetPassword(newPassword);
                this.UserModel.RegisterSave(user, true);
                this.SendPasswordEmail(user.FirstName, user.Email, newPassword);
            }

            this.LogError("UpdatePassword", result, email);
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The set new session.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="SessionDTO"/>.
        /// </returns>
        private SessionDTO SetNewSession(User user)
        {
            user.SessionToken = AuthenticationModel.CreateRandomPassword(45);
            var userModel = this.UserModel;
            while (userModel.GetOneByToken(user.SessionToken).Value != null)
            {
                user.SessionToken = AuthenticationModel.CreateRandomPassword(45);
            }

            user.SessionTokenExpirationDate = DateTime.Now.AddDays(1);
            this.UserModel.RegisterSave(user);

            return new SessionDTO
            {
                expiration = user.SessionTokenExpirationDate.Value,
                session = user.SessionToken
            };
        }

        /// <summary>
        /// The get or set a new session.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="SessionDTO"/>.
        /// </returns>
        private SessionDTO GetOrSetANewSession(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.SessionToken) && user.SessionTokenExpirationDate.HasValue
                && user.SessionTokenExpirationDate.Value > DateTime.Now)
            {
                return new SessionDTO
                {
                    expiration = user.SessionTokenExpirationDate.Value,
                    session = user.SessionToken
                };
            }

            return this.SetNewSession(user);
        }

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="emailChanged">
        /// The email Changed.
        /// </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        private User ConvertDto(UserDTO user, User instance, out bool emailChanged)
        {
            emailChanged = instance != null && instance.Email.ToLower() != user.email.ToLower();
            instance = instance ?? new User();
            instance.Email = user.email;
            instance.FirstName = user.firstName;
            instance.LastName = user.lastName;
            instance.Company = this.CompanyModel.GetOneById(user.companyId).Value;
            instance.DateCreated = user.dateCreated == DateTime.MinValue ? DateTime.Now : user.dateCreated;
            instance.DateModified = user.dateModified == DateTime.MinValue ? DateTime.Now : user.dateModified;
            instance.Status = instance.Id != 0 ? instance.Status : UserStatus.Inactive;
            instance.Language = this.LanguageModel.GetOneById(user.languageId).Value;
            instance.TimeZone = this.TimeZoneModel.GetOneById(user.timeZoneId).Value;
            instance.UserRole = this.UserRoleModel.GetOneById(user.userRoleId).Value;
            instance.CreatedBy = user.createdBy.HasValue ? this.UserModel.GetOneById(user.createdBy.Value).Value : null;
            instance.ModifiedBy = user.modifiedBy.HasValue
                                      ? this.UserModel.GetOneById(user.modifiedBy.Value).Value
                                      : null;
instance.Logo = user.logoId.HasValue
                                      ? this.FileModel.GetOneById(user.logoId.Value).Value
                                      : null;
            return instance;
        }

        /// <summary>
        /// The combine failed.
        /// </summary>
        /// <param name="failedRows">
        /// The failed rows.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string CombineFailed(IEnumerable<string> failedRows)
        {
            return failedRows.Where(failedRow => !string.IsNullOrWhiteSpace(failedRow)).Aggregate(string.Empty, (current, failedRow) => current + (failedRow + Environment.NewLine));
        }

        #endregion
    }
}