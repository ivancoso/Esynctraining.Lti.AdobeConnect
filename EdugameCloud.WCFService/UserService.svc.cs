namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using EdugameCloud.WCFService.DTO;
    using EdugameCloud.WCFService.ViewModels;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;
    using FluentValidation.Results;
    using Resources;

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
        /// Gets the company LMS model.
        /// </summary>
        private LmsCompanyModel LmsCompanyModel
        {
            get
            {
                return IoC.Resolve<LmsCompanyModel>();
            }
        }

        private LmsUserParametersModel LmsUserParametersModel
        {
            get
            {
                return IoC.Resolve<LmsUserParametersModel>();
            }
        }

        /// <summary>
        ///     Gets the social user tokens
        /// </summary>
        private SocialUserTokensModel SocialUserTokensModel
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
        /// The <see cref="SocialUserTokensDTO"/>.
        /// </returns>
        public SocialUserTokensDTO GetSocialUserTokens(string key)
        {
            SocialUserTokens user;
            if ((user = this.SocialUserTokensModel.GetOneByKey(key).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_USER,
                    ErrorsTexts.AccessError_Subject,
                    ErrorsTexts.GetById_NoUserExists);
                this.LogError("User.GetSocialUserTokens", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SocialUserTokensDTO(user);
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
        public void UpdateLogo(int userId, Guid logoId)
        {
            var model = this.UserModel;
            User user;
            File file;
            if ((user = model.GetOneById(userId).Value) == null)
            {
                var error = new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists);
                this.LogError("User.UpdateLogo", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            if ((file = this.FileModel.GetOneById(logoId).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_NotFound,
                        ErrorsTexts.ImageUploadFailed_FileNotFound);
                this.LogError("User.UpdateLogo", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            user.Logo = file;
            model.RegisterSave(user, true);
            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
        }

        /// <summary>
        /// The activate by id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        public void ActivateById(int userId)
        {
            UserModel model = this.UserModel;
            User user;
            if ((user = model.GetOneById(userId).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_USER, ErrorsTexts.AccessError_Subject, ErrorsTexts.DeleteById_NoUserExists);
                this.LogError("User.ActivateById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            user.Status = UserStatus.Active;
            model.RegisterSave(user, true);
            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
        }

        /// <summary>
        /// The deactivate by id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        public void DeactivateById(int userId)
        {
            UserModel model = this.UserModel;
            User user;
            if ((user = model.GetOneById(userId).Value) == null)
            {
                var error = new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.DeleteById_NoUserExists);
                this.LogError("User.DeactivateById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            user.Status = UserStatus.Inactive;
            model.RegisterSave(user, true);
            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
        }

        /// <summary>
        /// The deactivate by id.
        /// </summary>
        /// <param name="userIds">
        /// The user id.
        /// </param>
        public void ActivateByIds(int[] userIds)
        {
            userIds = userIds ?? new int[] { };
            UserModel model = this.UserModel;
            IEnumerable<User> users;
            if (!(users = model.GetAllByIds(userIds.ToList())).Any())
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists);
                this.LogError("User.DeactivateById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            foreach (var user in users)
            {
                user.Status = UserStatus.Active;
                model.RegisterSave(user, true);
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
            }
        }

        /// <summary>
        /// The deactivate by id.
        /// </summary>
        /// <param name="userIds">
        /// The user id.
        /// </param>
        public void DeactivateByIds(int[] userIds)
        {
            userIds = userIds ?? new int[] { };
            UserModel model = this.UserModel;
            IEnumerable<User> users;
            if (!(users = model.GetAllByIds(userIds.ToList())).Any())
            {
                var error = new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists);
                this.LogError("User.DeactivateById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            foreach (var user in users)
            {
                user.Status = UserStatus.Inactive;
                model.RegisterSave(user, true);
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
            }

            model.Flush();
        }

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteById(int id)
        {
            UserModel model = this.UserModel;
            User user;
            if ((user = model.GetOneById(id).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists);
                this.LogError("User.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(user, true);
            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Delete, user.Company.Id, user.Id);
            return id;
        }

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="userIds">
        /// The user Ids.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int[] DeleteByIds(int[] userIds)
        {
            userIds = userIds ?? new int[] { };
            var res = new List<int>();
            UserModel model = this.UserModel;
            IEnumerable<User> users;
            if (!(users = model.GetAllByIds(userIds.ToList())).Any())
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.DeleteById_NoUserExists);
                this.LogError("User.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            foreach (var user in users)
            {
                model.RegisterDelete(user, true);
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, user.Company.Id, user.Id);
                res.Add(user.Id);
            }

            return res.ToArray();
        }

        /// <summary>
        /// The forgot password.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        public void ForgotPassword(string email)
        {
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
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("User.ForgotPassword", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        ///     All users test.
        /// </summary>
        /// <returns>
        ///     The <see cref="UserDTO" />.
        /// </returns>
        public UserDTO[] GetAll()
        {
            return this.UserModel.GetAll().Select(x => new UserDTO(x)).ToArray();
        }

        /// <summary>
        /// The get all for company.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="UserWithLoginHistoryDTO"/>.
        /// </returns>
        public UserWithLoginHistoryDTO[] GetAllForCompany(int companyId)
        {
            return this.UserModel.GetAllForCompany(companyId).ToArray();
        }

        /// <summary>
        /// The get login history for contact.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <returns>
        /// The <see cref="UserLoginHistoryDTO"/>.
        /// </returns>
        public UserLoginHistoryDTO[] GetLoginHistoryForCompany(int companyId)
        {
            Company company;
            if ((company = this.CompanyModel.GetOneById(companyId).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_USER, ErrorsTexts.AccessError_Subject, ErrorsTexts.GetById_NoUserExists);
                this.LogError("User.GetLoginHistoryForCompany", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            var history = this.UserLoginHistoryModel.GetAllForUsers(company.Users.Select(x => x.Id).ToList()).ToList();
            return history.Select(x => new UserLoginHistoryDTO(x, company)).ToArray();
        }

        /// <summary>
        /// The get login history for contact.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="UserLoginHistoryDTO"/>.
        /// </returns>
        public UserLoginHistoryDTO[] GetLoginHistoryForUser(int userId)
        {
            if (this.UserModel.GetOneById(userId).Value == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_USER, ErrorsTexts.AccessError_Subject, ErrorsTexts.GetById_NoUserExists);
                this.LogError("User.GetLoginHistoryForUser", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            var history = this.UserLoginHistoryModel.GetAllForUser(userId).ToList();
            var companies = this.CompanyModel.GetAllByUsers(history.Select(x => x.User.Id).Distinct().ToList());
            return history.Select(x => new UserLoginHistoryDTO(x, companies.FirstOrDefault(c => c.Id == x.User.Company.Id))).ToArray();
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
        /// The <see cref="UserLoginHistoryDTO"/>.
        /// </returns>
        public PagedUserLoginHistoryDTO GetLoginHistoryPaged(int pageIndex, int pageSize)
        {
            var result = new PagedUserLoginHistoryDTO();
            int totalCount;
            var history = this.UserLoginHistoryModel.GetAllPaged(pageIndex, pageSize, out totalCount).ToList();
            var companies = this.CompanyModel.GetAllByUsers(history.Select(x => x.User.Id).Distinct().ToList());
            result.objects =
                history.Select(
                    x => new UserLoginHistoryDTO(x, companies.FirstOrDefault(c => c.Id == x.User.Company.Id))).ToArray();
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
        /// The <see cref="UserDTO"/>.
        /// </returns>
        public UserDTO GetById(int id)
        {
            User user;
            if ((user = this.UserModel.GetOneById(id).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.GetById_NoUserExists);
                this.LogError("User.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new UserDTO(user);
        }

        /// <summary>
        /// The request session token.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SessionDTO"/>.
        /// </returns>
        public SessionDTO RequestSessionToken(int userId)
        {
            User user;
            if ((user = this.UserModel.GetOneById(userId).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.AccessError_NoUserExistsWithTheGivenEmail);
                this.LogError("User.RequestSessionToken", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            if (user.Status != UserStatus.Active)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_USER_INACTIVE,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.AccessError_UserIsInactive);
                this.LogError("User.RequestSessionToken", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return this.GetOrSetANewSession(user);
        }

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="UserWithSplashScreenDTO"/>.
        /// </returns>
        public UserWithSplashScreenDTO Login(LoginWithHistoryDTO dto)
        {
            User user;
            if ((user = this.UserModel.GetOneByEmail(dto.email).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.AccessError_NoUserExistsWithTheGivenEmail);
                this.LogError("User.Login", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            if (user.Status != UserStatus.Active)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_USER_INACTIVE, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.AccessError_UserIsInactive);
                this.LogError("User.Login", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            if (user.Company.Status != CompanyStatus.Active)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_USER_INACTIVE,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.AccessError_CompanyIsInactive);
                this.LogError("User.Login", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            if (user.Company.CurrentLicense == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_EXPIRED_LICENSE,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.AccessError_CompanyLicenseIsExpired);
                this.LogError("User.Login", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            if (!user.ValidatePasswordHash(dto.passwordHash) && !user.ValidatePassword(dto.passwordHash))
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.AccessError_InvalidPassword);
                this.LogError("User.Login", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            var companyLms = this.LmsCompanyModel.GetAllByCompanyId(user.Company.Id);

            if (dto.lmsUserParametersId != null)
            {
                var lmsUserParameters = this.LmsUserParametersModel.GetOneById(dto.lmsUserParametersId.Value).Value;
                if (lmsUserParameters != null && !companyLms.Any(c => c.Id == lmsUserParameters.CompanyLms.Id))
                {
                    var company = this.CompanyModel.GetOneById(lmsUserParameters.CompanyLms.CompanyId).Value;

                    var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_ACCESS,
                        ErrorsTexts.AccessError_Subject,
                        string.Format("This meeting belongs to company {0}.", company != null ? company.CompanyName : string.Empty));
                    this.LogError("User.Login", error);
                    throw new FaultException<Error>(error, error.errorMessage);
                }
            }

            var result = new UserWithSplashScreenDTO(user);
            var userHistory = new UserLoginHistory
                                  {
                                      Application = dto.application,
                                      FromIP = dto.fromIP,
                                      User = user,
                                      DateCreated = DateTime.Now
                                  };

            this.UserLoginHistoryModel.RegisterSave(userHistory);

            result.session = this.GetOrSetANewSession(user);
            if (dto.sendSplashScreen)
            {
                result.splashScreen = new SplashScreenDTO
                                          {
                                              recentReports = this.GetBaseRecentSplashScreenReports(user.Id, 1, 10).objects.ToArray(),
                                              reports = this.GetBaseSplashScreenReports(user.Id, 1, 5).objects.ToArray()
                                          };
            }

            result.companyLms = companyLms.Select(c => new CompanyLmsDTO(c)).ToArray();

            return result;
        }

        /// <summary>
        /// The upload batch users.
        /// </summary>
        /// <param name="batch">
        /// The batch.
        /// </param>
        /// <returns>
        /// The <see cref="UserDTO"/>.
        /// </returns>
        public UserDTO[] UploadBatchUsers(BatchUsersDTO batch)
        {
            ValidationResult validationResult;
            if (this.IsValid(batch, out validationResult))
            {
                var company = this.CompanyModel.GetOneById(batch.companyId).Value;
                List<string> failedRows;
                string errorString;
                var results = this.UserModel.UploadBatchOfUsers(company, batch.csvOrExcelContent, batch.type, out failedRows, out errorString, this.SendActivation).ToList();

                if (errorString != null || failedRows != null)
                {
                    var error = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, errorString, this.CombineFailed(failedRows));
                    this.LogError("User.UploadBatchUsers", error);
                    throw new FaultException<Error>(error, error.errorMessage);
                }

                return results.Select(x => new UserDTO(x)).ToArray();
            }

            var validationError = this.GenerateValidationError(validationResult);
            this.LogError("User.UploadBatchUsers", validationError);
            throw new FaultException<Error>(validationError, validationError.errorMessage);
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetCompanyIdByEmail(string email)
        {
            User user;
            if ((user = this.UserModel.GetOneByEmail(email).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound);
                this.LogError("User.GetCompanyIdByEmail", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }
                
            return user.Company.With(x => x.Id);
        }

        /// <summary>
        /// The registration.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="UserDTO"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1515:SingleLineCommentMustBePrecededByBlankLine", Justification = "Reviewed. Suppression is OK here.")]
        public UserDTO Save(UserDTO user)
        {
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
                        var error =
                            new Error(
                                Errors.TOO_MANY_USERS,
                                ErrorsTexts.UserCreateUpdateError_Subject,
                                ErrorsTexts.EntityCreationError_Subject,
                                ErrorsTexts.UserCreationError_LicenseError);
                        this.LogError("User.Save", error);
                        throw new FaultException<Error>(error, error.errorMessage);
                    }
                }

                if (!isTransient && userInstance != null && userInstance.Status != UserStatus.Active)
                {
                    var error =
                        new Error(
                            Errors.CODE_ERRORTYPE_USER_INACTIVE, 
                            ErrorsTexts.UserCreateUpdateError_Subject, 
                            ErrorsTexts.UserCreateUpdateError_NotActivated, 
                            ErrorsTexts.UserCreateUpdateError_NotActivated_Details);
                    this.LogError("User.Save", error);
                    throw new FaultException<Error>(error, error.errorMessage);
                }
                else
                {
                    if (userInstance == null || !isTransient)
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
                        IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, userInstance.Company.Id, userInstance.Id);

                        if (isTransient)
                        {
                            this.SendActivation(userInstance);
                        }
                        ////                    else if (passwordChanged || emailChanged)
                        ////                    {
                        ////                        this.SendPasswordEmail(user.firstName, user.email, user.password);
                        ////                    }

                        return new UserDTO(userInstance);
                    }

                    var error =
                        new Error(
                            Errors.CODE_ERRORTYPE_USER_EXISTING, 
                            ErrorsTexts.UserCreationError_Subject, 
                            ErrorsTexts.UserCreationError_AlreadyExists, 
                            ErrorsTexts.UserCreationError_AlreadyExists_Details);
                    this.LogError("User.Save", error);
                    throw new FaultException<Error>(error, error.errorMessage);
                }
            }

            var validationError = this.GenerateValidationError(validationResult);
            this.LogError("User.Save", validationError);
            throw new FaultException<Error>(validationError, validationError.errorMessage);
        }

        /// <summary>
        /// The send activation.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string SendActivation(string email)
        {
            return string.Empty;
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
        public void UpdatePasswordByCode(string code, string newPassword)
        {
            UserActivation userActivation;
            if ((userActivation = this.UserActivationModel.GetOneByCode(code).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.UserActivationError_InvalidActivationCode);
                this.LogError("User.UpdatePasswordByCode", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            if (userActivation.User.Status != UserStatus.Activating)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_USER_INACTIVE,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.UserActivationError_Subject);
                this.LogError("User.UpdatePasswordByCode", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            var contact = userActivation.User;
            contact.SetPassword(newPassword);
            contact.Status = UserStatus.Active;
            this.UserModel.RegisterSave(contact, true);
            var activations = this.UserActivationModel.GetAllByUser(contact.Id);
            foreach (var passwordActivation in activations)
            {
                this.UserActivationModel.RegisterDelete(passwordActivation);
            }

            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<User>(NotificationType.Update, contact.Company.Id, contact.Id);
            this.SendPasswordEmail(contact.FirstName, contact.Email, newPassword);
        }

        /// <summary>
        /// The activate by id.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ActivateByCode(string code)
        {
            var passwordActivation = this.UserActivationModel.GetOneByCode(code).Value;
            var contact = passwordActivation.With(x => x.User);
            if (contact != null)
            {
                contact.Status = UserStatus.Activating;
                this.UserModel.RegisterSave(contact, true);
                return true;
            }

            var error =
                new Error(
                    Errors.CODE_ERRORTYPE_INVALID_USER,
                    ErrorsTexts.AccessError_Subject,
                    ErrorsTexts.DeleteById_NoUserExists);
            this.LogError("User.ActivateByCode", error);
            throw new FaultException<Error>(error, error.errorMessage);
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
        public void UpdatePassword(string email, string oldPasswordHash, string newPassword)
        {
            User user;
            if ((user = this.UserModel.GetOneByEmail(email).Value) == null)
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.AccessError_NoUserExistsWithTheGivenEmail);
                this.LogError("User.UpdatePassword", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            if (!user.ValidatePasswordHash(oldPasswordHash) && !user.ValidatePassword(oldPasswordHash))
            {
                var error =
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER, 
                        ErrorsTexts.AccessError_Subject, 
                        ErrorsTexts.AccessError_InvalidPassword);
                this.LogError("User.UpdatePassword", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            user.SetPassword(newPassword);
            this.UserModel.RegisterSave(user, true);
            this.SendPasswordEmail(user.FirstName, user.Email, newPassword);
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
                expiration = user.SessionTokenExpirationDate.Value.ConvertToUnixTimestamp(),
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
                    expiration = user.SessionTokenExpirationDate.Value.ConvertToUnixTimestamp(),
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
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.DateModified = DateTime.Now;
            instance.Status = instance.Id != 0 ? instance.Status : UserStatus.Inactive;
            instance.Language = this.LanguageModel.GetOneById(user.languageId).Value;
            instance.TimeZone = this.TimeZoneModel.GetOneById(user.timeZoneId).Value;
            instance.UserRole = this.UserRoleModel.GetOneById(user.userRoleId).Value;
            instance.CreatedBy = user.createdBy.HasValue ? this.UserModel.GetOneById(user.createdBy.Value).Value : null;
            instance.ModifiedBy = user.modifiedBy.HasValue ? this.UserModel.GetOneById(user.modifiedBy.Value).Value: null;
            instance.Logo = user.logoId.HasValue ? this.FileModel.GetOneById(user.logoId.Value).Value : null;
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