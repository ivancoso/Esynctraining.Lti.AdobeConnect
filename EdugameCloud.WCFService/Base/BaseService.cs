namespace EdugameCloud.WCFService.Base
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Mail;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Text.RegularExpressions;
    using System.Web;

    using Castle.Core.Logging;
    using Castle.MicroKernel;

    using EdugameCloud.Core.Authentication;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Converters;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Providers.Mailer.Models;
    using EdugameCloud.WCFService.Mail.Models;

    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Comparers;
    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer;
    using Esynctraining.Core.Utils;
    using Esynctraining.Core.Weborb.WCFExtension;

    using FluentValidation;
    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The base service.
    /// </summary>
    public abstract class BaseService
    {
        #region Fields

        /// <summary>
        ///     The error message title.
        /// </summary>
        protected const string ACErrorMessageTitle = "Adobe Connect Error";

        /// <summary>
        ///     The authentication header name.
        /// </summary>
        private string authHeaderName;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the authentication header name.
        /// </summary>
        protected string AuthHeaderName
        {
            get
            {
                this.authHeaderName = this.authHeaderName ?? this.Settings.AuthHeaderName;
                return this.authHeaderName;
            }
        }

        /// <summary>
        ///     Gets the current request.
        /// </summary>
        protected HttpRequestMessageProperty CurrentRequest
        {
            get
            {
                object authValue;
                OperationContext.Current.IncomingMessageProperties.TryGetValue("httpRequest", out authValue);
                if (authValue != null)
                {
                    return (HttpRequestMessageProperty)authValue;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the current user.
        /// </summary>
        protected User CurrentUser
        {
            get
            {
                return HttpContext.Current.With(x => x.User).If(x => x.Identity is EdugameCloudIdentity, x => ((EdugameCloudIdentity)x.Identity).With(y => y.InternalEntity));
            }
        }

        /// <summary>
        /// Gets the file model.
        /// </summary>
        protected FileModel FileModel
        {
            get
            {
                return IoC.Resolve<FileModel>();
            }
        }

        /// <summary>
        ///     Gets the mail model.
        /// </summary>
        protected MailModel MailModel
        {
            get
            {
                return IoC.Resolve<MailModel>();
            }
        }

        /// <summary>
        ///     Gets the Logger.
        /// </summary>
        protected ILogger Logger
        {
            get
            {
                return IoC.Resolve<ILogger>();
            }
        }

        /// <summary>
        ///     Gets the settings.
        /// </summary>
        protected dynamic Settings
        {
            get
            {
                return IoC.Resolve<ApplicationSettingsProvider>();
            }
        }

        /// <summary>
        ///     Gets the user model.
        /// </summary>
        protected UserModel UserModel
        {
            get
            {
                return IoC.Resolve<UserModel>();
            }
        }

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        protected ILogger logger
        {
            get
            {
                return IoC.Resolve<ILogger>();
            }
        }

        /// <summary>
        ///     Gets the UserActivation model.
        /// </summary>
        protected UserActivationModel UserActivationModel
        {
            get
            {
                return IoC.Resolve<UserActivationModel>();
            }
        }

        /// <summary>
        ///     Gets the SubModuleItem model.
        /// </summary>
        protected SubModuleItemModel SubModuleItemModel
        {
            get
            {
                return IoC.Resolve<SubModuleItemModel>();
            }
        }

        /// <summary>
        ///     Gets the ACSession model.
        /// </summary>
        protected ACSessionModel ACSessionModel
        {
            get
            {
                return IoC.Resolve<ACSessionModel>();
            }
        }

        /// <summary>
        ///     Gets the ACSession model.
        /// </summary>
        protected ITemplateProvider TemplateProvider
        {
            get
            {
                return IoC.Resolve<ITemplateProvider>();
            }
        }

        /// <summary>
        ///     Gets the ACSession model.
        /// </summary>
        protected EmailHistoryModel EmailHistoryModel
        {
            get
            {
                return IoC.Resolve<EmailHistoryModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The process version.
        /// </summary>
        /// <param name="swfFolder">
        /// The SWF folder.
        /// </param>
        /// <param name="buildSelector">
        /// The build Selector.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected Version ProcessVersion(string swfFolder, string buildSelector)
        {
            if (Directory.Exists(swfFolder))
            {
                var versions = new List<KeyValuePair<Version, string>>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var file in Directory.GetFiles(swfFolder, buildSelector))
                {
                    var fileName = Path.GetFileName(file);
                    var version = fileName.GetBuildVersion();
                    versions.Add(new KeyValuePair<Version, string>(version, fileName));
                }

                versions.Sort(new BuildVersionComparer());
                return versions.FirstOrDefault().With(x => x.Key);
            }

            return null;
        }

        /// <summary>
        /// The format error from AC.
        /// </summary>
        /// <param name="res">
        /// The res.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Reviewed. Suppression is OK here.")]
        protected ServiceResponse FormatErrorFromAC(ResultBase res)
        {
            return this.FormatErrorFromAC(res, new ServiceResponse());
        }

        /// <summary>
        /// The format error from ac.
        /// </summary>
        /// <param name="res">
        /// The res.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <typeparam name="T">
        /// The service response
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Reviewed. Suppression is OK here.")]
        protected T FormatErrorFromAC<T>(ResultBase res, T result) where T : ServiceResponse
        {
            if (res != null && res.Status != null)
            {
                string field = res.Status.InvalidField;
                StatusCodes errorCode = res.Status.Code;
                StatusSubCodes errorSubCode = res.Status.SubCode;
                string message = string.Empty;
                if (field == "login" && errorCode == StatusCodes.invalid && errorSubCode == StatusSubCodes.duplicate)
                {
                    message = "User already exists in Adobe Connect.";
                }
                else if (res is LoginResult && !res.Success && string.IsNullOrWhiteSpace(field) && errorCode == StatusCodes.no_data)
                {
                    message = "Login failed";
                }
                else
                {
                    message = string.Format(
                        "Adobe Connect error: {0}{1}{2}",
                        field,
                        errorCode == StatusCodes.not_set ? string.Empty : " is " + errorCode,
                        errorSubCode == StatusSubCodes.not_set
                            ? string.Empty
                            : string.Format(" (reason : {0})", errorSubCode));
                }
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ACErrorMessageTitle, message));
            }

            return result;
        }

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
        protected bool IsValid<T>(T obj, out ValidationResult validationResult)
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
        /// The convert.
        /// </summary>
        /// <param name="transferObject">
        /// The t 1.
        /// </param>
        /// <param name="instance">
        /// The t 2.
        /// </param>
        /// <param name="flush">
        /// The flush.
        /// </param>
        /// <typeparam name="TDataTransferObject">
        /// Data transfer object instance
        /// </typeparam>
        /// <typeparam name="TObjectInstance">
        /// Object instance
        /// </typeparam>
        /// <returns>
        /// The <see cref="TObjectInstance"/> instance.
        /// </returns>
        protected TObjectInstance Convert<TDataTransferObject, TObjectInstance>(TDataTransferObject transferObject, TObjectInstance instance, bool flush = false)
        {
            return IoC.Resolve<BaseConverter<TDataTransferObject, TObjectInstance>>().Convert(transferObject, instance, flush);
        }

        /// <summary>
        /// The update cache.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        protected void UpdateCache(MethodInfo method, params object[] args)
        {
            WCFUtil.InvalidateCache(this, method, args);
        }

        /// <summary>
        /// The update cache.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="methodName">
        /// The method Name.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        protected void UpdateCache<T>(T target, string methodName, params object[] args)
        {
            WCFUtil.InvalidateCache(target, typeof(T).GetMethod(methodName), args);
        }

        /// <summary>
        /// The update cache.
        /// </summary>
        /// <param name="expression">
        /// The method expressions.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        protected void UpdateCache<T>(Expression<Action<T>> expression, params object[] args)
        {
            WCFUtil.InvalidateCache(this, Lambda.MethodInfo(expression), args);
        }

        /// <summary>
        /// The send activation.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        protected void SendActivation(User user)
        {
            UserActivationModel model = this.UserActivationModel;
            UserActivation userActivation;
            List<MailAddress> bcced = null;

            if ((userActivation = model.GetLatestByUser(user.Id).Value) == null)
            {
                userActivation = new UserActivation
                {
                    User = user,
                    ActivationCode = Guid.NewGuid().ToString(),
                    DateExpires = DateTime.Now.AddDays(7)
                };
                model.RegisterSave(userActivation);
                bcced = GetBCCed(Settings.BCCNewEmail as string);
            }

            this.SendActivationEmail(user.FirstName, user.Email, user.Company,  userActivation.ActivationCode, bcced);
        }

        /// <summary>
        /// The send trial email.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="company">
        /// The company.
        /// </param>
        protected void SendEnterpriseEmail(User user, string activationCode, Company company)
        {
            var license = company.Licenses.FirstOrDefault();
            var days = (int)Math.Round(license.Return(x => x.ExpiryDate.Subtract(DateTime.Today), new TimeSpan(45, 0, 0, 0)).TotalDays);

            var model = new EnterpriseModel(this.Settings)
                        {
                            CompanyName = company.CompanyName,
                            MailSubject = Emails.TrialSubject,
                            TrialContactEmail = (string)this.Settings.TrialContactEmail,
                            TrialDays = days,
                            UserName = user.Email,
                            FirstName = user.FirstName,
                            ActivationCode = activationCode,
                            ExpirationDate =
                                license.Return(
                                    x => x.ExpiryDate.ToShortDateString(),
                                    string.Empty)
                        };
            var bcced = new List<MailAddress>
                        {
                            new MailAddress(this.Settings.TrialContactEmail),
                            new MailAddress(Common.JacquieEmail, Common.JacquieName)
                        };

            this.MailModel.SendEmail(
                user.FirstName,
                user.Email,
                Emails.TrialSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail,
                bcced: bcced);

            this.SaveHistory(
                user.FirstName,
                user.Email,
                Emails.TrialSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail,
                bcced: bcced);
        }

        /// <summary>
        /// The get license status.
        /// </summary>
        /// <param name="licenseVo">
        /// The license vo.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLicenseStatus"/>.
        /// </returns>
        protected CompanyLicenseStatus GetLicenseStatus(CompanyLicenseDTO licenseVo)
        {
            if (licenseVo.isTrial)
            {
                return CompanyLicenseStatus.Trial;
            }

            if (licenseVo.isEnterprise)
            {
                return CompanyLicenseStatus.Enterprise;
            }

            return CompanyLicenseStatus.Pro;
        }

        /// <summary>
        /// The send trial email.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="company">
        /// The company.
        /// </param>
        protected void SendTrialEmail(User user, string activationCode, Company company)
        {
            var license = company.Licenses.FirstOrDefault();
            var days = (int)Math.Round(license.Return(x => x.ExpiryDate.Subtract(DateTime.Today), new TimeSpan(45, 0, 0, 0)).TotalDays);
            
            var model = new TrialModel(this.Settings)
                        {
                            CompanyName = company.CompanyName,
                            MailSubject = Emails.TrialSubject,
                            TrialContactEmail = (string)this.Settings.TrialContactEmail,
                            TrialDays = days,
                            UserName = user.Email,
                            ActivationCode = activationCode,
                            FirstName = user.FirstName
                        };
            var bcced = new List<MailAddress>
                        {
                            new MailAddress(this.Settings.TrialContactEmail),
                            new MailAddress(Common.JacquieEmail, Common.JacquieName)
                        };

            this.MailModel.SendEmail(
                user.FirstName,
                user.Email,
                Emails.TrialSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail,
                bcced: bcced);

            this.SaveHistory(
                user.FirstName,
                user.Email,
                Emails.TrialSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail,
                bcced: bcced);
        }

        /// <summary>
        /// The get base recent splash screen reports.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        protected ServiceResponse<RecentReportDTO> GetBaseRecentSplashScreenReports(int userId, int pageIndex, int pageSize)
        {
            int totalCount;
            return new ServiceResponse<RecentReportDTO>
            {
                objects = this.SubModuleItemModel.GetRecentSplashScreenReportsPaged(userId, pageIndex, pageSize, out totalCount).ToList(),
                totalCount = totalCount
            };
        }

        /// <summary>
        /// The get base splash screen reports.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        protected ServiceResponse<ReportDTO> GetBaseSplashScreenReports(int userId, int pageIndex, int pageSize)
        {
            int totalCount;
            return new ServiceResponse<ReportDTO>
            {
                objects = this.ACSessionModel.GetSplashScreenReportsPaged(userId, pageIndex, pageSize, out totalCount).ToList(),
                totalCount = totalCount
            };
        }

        /// <summary>
        /// The send trial email.
        /// </summary>
        /// <param name="company">
        /// The company.
        /// </param>
        protected void SendLicenseUpgradeEmail(Company company)
        {
            var license = company.Licenses.FirstOrDefault();
            var model = new LicenseUpgradeModel(this.Settings)
                        {
                            CompanyName = company.CompanyName,
                            MailSubject = Emails.LicenseUpgradeRequested,
                            PrimaryEmail =
                                company.PrimaryContact.With(x => x.Email),
                            PrimaryName =
                                company.PrimaryContact.With(x => x.FullName),
                            SeatsCount = license.With(x => x.TotalLicensesCount),
                            IsTrial =
                                license.Return(
                                    x =>
                                x.LicenseStatus == CompanyLicenseStatus.Trial,
                                    false),
                            ExpirationDate =
                                license.With(x => x.ExpiryDate.ToEst() + " EST")
                        };

            this.MailModel.SendEmail(
                "License Admin",
                (string)this.Settings.TrialContactEmail,
                Emails.LicenseUpgradeRequested,
                model,
                Common.AppEmailName,
                Common.AppEmail);

            this.SaveHistory(
                "License Admin",
                (string)this.Settings.TrialContactEmail,
                Emails.LicenseUpgradeRequested,
                model,
                Common.AppEmailName,
                Common.AppEmail);
        }

        /// <summary>
        /// The send password email.
        /// </summary>
        /// <param name="firstName">
        /// The first name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        protected void SendPasswordEmail(string firstName, string email, string password)
        {
            var model = new ChangePasswordModel(this.Settings)
                        {
                            FirstName = firstName,
                            Password = password,
                            TrialContactEmail = this.Settings.TrialContactEmail
                        };

            this.MailModel.SendEmail(
                firstName,
                email,
                Emails.ChangePasswordSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail);

            this.SaveHistory(
                firstName,
                email,
                Emails.ChangePasswordSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail);
        }

        /// <summary>
        /// The send password email.
        /// </summary>
        /// <param name="firstName">
        /// The first name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        protected void SendActivationLinkEmail(string firstName, string email, string activationCode)
        {
            var model = new ActivationLinkModel(this.Settings)
            {
                FirstName = firstName,
                ActivationCode = activationCode,
                TrialContactEmail = this.Settings.TrialContactEmail
            };

            this.MailModel.SendEmail(
                firstName,
                email,
                Emails.ChangePasswordSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail);

            this.SaveHistory(
                firstName,
                email,
                Emails.ChangePasswordSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail);
        }


        /// <summary>
        /// The send activation email.
        /// </summary>
        /// <param name="firstName">
        /// The first name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="activationCode">
        /// The activation code.
        /// </param>
        protected void SendActivationEmail(string firstName, string email, Company company, string activationCode, List<MailAddress> bcced = null)
        {
            var cced = GetCCed(email, company);
            var blindCopies = new List<MailAddress>();

            if (bcced != null)
            {
                blindCopies.AddRange(bcced);
            }

            var bccedProduction = HttpContext.Current != null && !HttpContext.Current.IsDebuggingEnabled ? GetBCCed(this.Settings.BCCActivationEmail as string) : null;
            if (bccedProduction != null)
            {
                blindCopies.AddRange(bccedProduction);
            }

            var model = new ActivationInvitationModel(this.Settings)
                           {
                               FirstName = firstName,
                               UserName = email,
                               ActivationCode = activationCode,
                               TrialContactEmail =
                                   this.Settings.TrialContactEmail,
                               CompanyName = company.CompanyName
                           };               

            this.MailModel.SendEmail(
                firstName,
                email,
                Emails.ActivationSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail, cced, blindCopies);

            this.SaveHistory(
                firstName,
                email,
                Emails.ActivationSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail, cced, blindCopies);
            
        }

        /// <summary>
        /// The get c ced.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="company">
        /// The company.
        /// </param>
        /// <returns>
        /// The <see cref="List{MailAddress}"/>.
        /// </returns>
        private static List<MailAddress> GetCCed(string email, Company company)
        {
            if (company != null && company.PrimaryContact != null && !email.Equals(company.PrimaryContact.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                return new List<MailAddress>
                                     {
                                         new MailAddress(company.PrimaryContact.Email, company.PrimaryContact.FullName)
                                     };
            }

            return null;
        }

        /// <summary>
        /// The get bc ced.
        /// </summary>
        /// <param name="emails">
        /// The emails.
        /// </param>
        /// <returns>
        /// The <see cref="List{MailAddress}"/>.
        /// </returns>
        private static List<MailAddress> GetBCCed(string emails)
        {
            if (string.IsNullOrEmpty(emails))
            {
                return null;
            }

            var blindSeparator = new [] { ";" };
            var blindEmails = emails.Split(blindSeparator, StringSplitOptions.RemoveEmptyEntries);

            return blindEmails.Any() ? blindEmails.Select(email => new MailAddress(email)).ToList() : null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The log error.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        protected void LogError(string methodName, ServiceResponse result, User currentUser = null)
        {
            this.LogError(methodName, result, currentUser.With(x => x.FullName));
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
        protected void LogError(string methodName, ServiceResponse result, string userName)
        {
            if (result != null && result.error != null)
            {
                this.logger.Error(
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
        protected T UpdateResult<T>(T result, ValidationResult validationResult) where T : ServiceResponse
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
        /// The update result to string.
        /// </summary>
        /// <param name="validationResult">
        /// The validation result.
        /// </param>
        /// <returns>
        /// The <see cref="List{String}"/>.
        /// </returns>
        protected List<string> UpdateResultToString(ValidationResult validationResult)
        {
            var list = new List<string>();
            if (validationResult != null)
            {
                list.AddRange(from failure in validationResult.Errors where !string.IsNullOrWhiteSpace(failure.ErrorMessage) select failure.ErrorMessage);
            }

            return list;
        }

        private void SaveHistory<TModel>(string toName, string toEmail, string subject, TModel model, string fromName = null, string fromEmail = null, List<MailAddress> cced = null, List<MailAddress> bcced = null)
        {
            string body = this.TemplateProvider.GetTemplate<TModel>().TransformTemplate(model),
                message = body;
            if (message != null)
            {
                message = Regex.Replace(message, "<[^>]*(>|$)", "");
                message = message.Replace("\r\n", "\n").Replace("\r", "\n").Replace("&nbsp;", " ").Replace("&#39;", @"'");
                message = Regex.Replace(message, @"[ ]{2,}", " ");
                message = message.Replace("\n ", "\n");
                message = Regex.Replace(message, @"[\n]{2,}", "\n");
                message =
                    message.Replace(
                        "Enriching online interaction for meetings, training and education on Adobe Connect",
                        "");
                while (message.StartsWith("\n")) message = message.Remove(0, 1);
            }

            var emailHistory = new EmailHistory()
                               {
                                   SentTo = toEmail,
                                   SentToName = toName,
                                   SentFrom = fromEmail,
                                   SentFromName = fromName,
                                   Date = DateTime.Now,
                                   SentBcc =
                                       bcced != null
                                           ? bcced.Select(ma => ma.Address)
                                       .Aggregate((a1, a2) => a1 + ";" + a2)
                                           : null,
                                   SentCc =
                                       cced != null
                                           ? cced.Select(ma => ma.Address)
                                       .Aggregate((a1, a2) => a1 + ";" + a2)
                                           : null,
                                   Subject = subject,
                                   User = UserModel.GetOneByEmail(toEmail).Value,
                                   Body = body,
                                   Message = message
                               };

            this.EmailHistoryModel.RegisterSave(emailHistory, true);
        }

        #endregion
    }
}