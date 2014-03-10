﻿namespace EdugameCloud.WCFService.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Mail;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
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

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
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

        #endregion

        #region Public Methods and Operators

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
        protected void SendTrialEmail(User user, string password, Company company)
        {
            var license = company.Licenses.FirstOrDefault();
            var days = (int)Math.Round(license.Return(x => x.ExpiryDate.Subtract(DateTime.Today), new TimeSpan(45, 0, 0, 0)).TotalDays);
            
            this.MailModel.SendEmail(
                user.FirstName,
                user.Email,
                Emails.TrialSubject,
                new TrialModel(this.Settings)
                    {
                        CompanyName = company.CompanyName,
                        MailSubject = Emails.TrialSubject,
                        TrialContactEmail = (string)this.Settings.TrialContactEmail,
                        TrialDays = days,
                        UserName = user.Email,
                        Password = password
                    },
                Common.AppEmailName,
                Common.AppEmail,
                new List<MailAddress> { new MailAddress(this.Settings.TrialContactEmail) });
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
            this.MailModel.SendEmail(
                "License Admin",
                (string)this.Settings.TrialContactEmail,
                Emails.LicenseUpgradeRequested,
                new LicenseUpgradeModel(this.Settings)
                {
                    CompanyName = company.CompanyName,
                    MailSubject = Emails.LicenseUpgradeRequested,
                    PrimaryEmail = company.PrimaryContact.With(x => x.Email),
                    PrimaryName = company.PrimaryContact.With(x => x.FullName),
                    SeatsCount = license.With(x => x.TotalLicensesCount),
                    IsTrial = license.Return(x => x.LicenseStatus == CompanyLicenseStatus.Trial, false),
                    ExpirationDate = license.With(x => x.ExpiryDate.ToEst() + " EST")
                },
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
            this.MailModel.SendEmail(
                firstName,
                email,
                Emails.ChangePasswordSubject,
                new ChangePasswordModel(this.Settings) { FirstName = firstName, Password = password, TrialContactEmail = this.Settings.TrialContactEmail },
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

            var bccedProduction = T4MVCHelpers.IsProduction() ? GetBCCed(Settings.BCCActivationEmail as string) : null;
            if (bccedProduction != null)
            {
                blindCopies.AddRange(bccedProduction);
            }

            this.MailModel.SendEmail(
                firstName,
                email,
                Emails.ActivationSubject,
                new ActivationInvitationModel(this.Settings) { FirstName = firstName, ActivationCode = activationCode, TrialContactEmail = this.Settings.TrialContactEmail, CompanyName = company.CompanyName },
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

        #endregion
    }
}