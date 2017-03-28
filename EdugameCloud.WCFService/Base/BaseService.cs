namespace EdugameCloud.WCFService.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Mail;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Text.RegularExpressions;
    using System.Web;
    using Esynctraining.Core.Logging;
    using Castle.MicroKernel;
    using EdugameCloud.Core.Authentication;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Converters;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Providers.Mailer.Models;
    using EdugameCloud.WCFService.Mail.Models;
    using Esynctraining.Core.Comparers;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using FluentValidation;
    using FluentValidation.Results;
    using Resources;
    using Esynctraining.Mail;

    /// <summary>
    /// The base service.
    /// </summary>
    public abstract class BaseService
    {
        #region Fields
        
        private const string ACErrorMessageTitle = "Adobe Connect Error";


        private string authHeaderName;

        #endregion

        #region Properties
        
        private string AuthHeaderName
        {
            get
            {
                this.authHeaderName = this.authHeaderName ?? this.Settings.AuthHeaderName;
                return this.authHeaderName;
            }
        }

        private HttpRequestMessageProperty CurrentRequest
        {
            get
            {
                object httpRequestValue = null;
                if (OperationContext.Current.With(x => x.IncomingMessageProperties)
                    .With(x => x.TryGetValue("httpRequest", out httpRequestValue)) && httpRequestValue != null)
                {
                    return (HttpRequestMessageProperty)httpRequestValue;
                }

                return null;
            }
        }

        ///// <summary>
        ///// Gets the current user.
        ///// </summary>
        //protected Guid CurrentUserToken
        //{
        //    get
        //    {
        //        Guid token;
        //        foreach (
        //            object value in
        //                OperationContext.Current.With(x => x.IncomingMessageProperties)
        //                    .Return(x => x.Values, new List<object>()))
        //        {
        //            if (value is AmfMessage)
        //            {
        //                var message = (AmfMessage)value;
        //                object data = message.With(x => x.Data);
        //                if (data is AbstractMessage)
        //                {
        //                    var abstractMessage = (AbstractMessage)data;
        //                    IDictionary<string, object> headers = abstractMessage.With(x => x.Headers);

        //                    if (headers != null && headers.ContainsKey("DSEndpoint") && headers["DSEndpoint"] != null
        //                        && Guid.TryParse(headers["DSEndpoint"].ToString(), out token))
        //                    {
        //                        return token;
        //                    }
        //                }
        //            }
        //        }

        //        var authHeader = this.AuthHeaderName;
        //        if (this.CurrentRequest != null
        //            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        //            && this.CurrentRequest.Headers != null
        //            && this.CurrentRequest.Headers.HasKey(authHeader)
        //            && Guid.TryParse(this.CurrentRequest.Headers[authHeader], out token))
        //        {
        //            return token;
        //        }

        //        return Guid.Empty;
        //    }
        //}
        
        protected User CurrentUser
        {
            get
            {
                return HttpContext.Current.With(x => x.User).If(x => x.Identity is EdugameCloudIdentity, x => ((EdugameCloudIdentity)x.Identity).With(y => y.InternalEntity));
            }
        }
        
        protected FileModel FileModel
        {
            get { return IoC.Resolve<FileModel>(); }
        }
        
        protected ISmtpClientEngine MailModel
        {
            get { return IoC.Resolve<ISmtpClientEngine>(); }
        }
        
        protected ILogger Logger
        {
            get { return IoC.Resolve<ILogger>(); }
        }
        
        protected dynamic Settings
        {
            get { return IoC.Resolve<ApplicationSettingsProvider>(); }
        }
        
        protected UserModel UserModel
        {
            get { return IoC.Resolve<UserModel>(); }
        }
        
        protected UserActivationModel UserActivationModel
        {
            get { return IoC.Resolve<UserActivationModel>(); }
        }
        
        protected SubModuleItemModel SubModuleItemModel
        {
            get { return IoC.Resolve<SubModuleItemModel>(); }
        }
        
        protected ACSessionModel ACSessionModel
        {
            get { return IoC.Resolve<ACSessionModel>(); }
        }
        
        protected ITemplateProvider TemplateProvider
        {
            get { return IoC.Resolve<ITemplateProvider>(); }
        }
        
        protected EmailHistoryModel EmailHistoryModel
        {
            get { return IoC.Resolve<EmailHistoryModel>(); }
        }

        #endregion

        #region Public Methods and Operators
        
        protected Version ProcessVersion(string swfFolder, string buildSelector)
        {
            if (string.IsNullOrWhiteSpace(swfFolder))
                throw new ArgumentNullException("swfFolder");
            if (string.IsNullOrWhiteSpace(buildSelector))
                throw new ArgumentNullException("buildSelector");

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
            catch (ComponentNotFoundException ex)
            {
                Logger.Error("BaseService.IsValid", ex);

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
        /// <typeparam name="T">
        /// Type of entity
        /// </typeparam>
        /// <param name="expression">
        /// The method expressions.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        protected void UpdateCache<T>(Expression<Action<T>> expression, params object[] args)
        {
        }
        
        protected void SendActivation(User user)
        {
            UserActivationModel model = this.UserActivationModel;
            UserActivation userActivation;
            List<MailAddress> bcced = null;

            if ((userActivation = model.GetLatestByUser(user.Id).Value) == null)
            {
                userActivation = UserActivation.Build(user);
                model.RegisterSave(userActivation);
                bcced = GetBCCed(this.Settings.BCCNewEmail as string);
            }

            this.SendActivationEmail(user.FirstName, user.Email, user.Company,  userActivation.ActivationCode, bcced);
        }
        
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
                ExpirationDate = license.Return(x => x.ExpiryDate.ToShortDateString(), string.Empty),
            };
            var bcced = new List<MailAddress>
            {
                new MailAddress(this.Settings.TrialContactEmail),
                new MailAddress(Common.JacquieEmail, Common.JacquieName)
            };

            bool sentSuccessfully = this.MailModel.SendEmailSync(
                user.FirstName,
                user.Email,
                Emails.TrialSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail,
                bcced: bcced);

            this.SaveHistory(
                sentSuccessfully,
                user.FirstName,
                user.Email,
                Emails.TrialSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail,
                bcced: bcced);
        }
        
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

            bool sentSuccessfully = this.MailModel.SendEmailSync(
                user.FirstName,
                user.Email,
                Emails.TrialSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail,
                bcced: bcced);

            this.SaveHistory(
                sentSuccessfully,
                user.FirstName,
                user.Email,
                Emails.TrialSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail,
                bcced: bcced);
        }
        
        protected PagedRecentReportsDTO GetBaseRecentSplashScreenReports(int userId, int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedRecentReportsDTO
            {
                objects = this.SubModuleItemModel.GetRecentSplashScreenReportsPaged(userId, pageIndex, pageSize, out totalCount).ToArray(),
                totalCount = totalCount
            };
        }
        
        protected PagedReportsDTO GetBaseSplashScreenReports(int userId, int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedReportsDTO
            {
                objects = this.ACSessionModel.GetSplashScreenReportsPaged(userId, pageIndex, pageSize, out totalCount).ToArray(),
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
                PrimaryEmail = company.PrimaryContact.With(x => x.Email),
                PrimaryName = company.PrimaryContact.With(x => x.FullName),
                SeatsCount = license.With(x => x.TotalLicensesCount),
                IsTrial = license.Return(x => x.LicenseStatus == CompanyLicenseStatus.Trial, false),
                ExpirationDate = license.ExpiryDate.Date.ToShortDateString(),
            };

            bool sentSuccessfully = this.MailModel.SendEmailSync(
                "License Admin",
                (string)this.Settings.TrialContactEmail,
                Emails.LicenseUpgradeRequested,
                model,
                Common.AppEmailName,
                Common.AppEmail);

            this.SaveHistory(
                sentSuccessfully,
                "License Admin",
                (string)this.Settings.TrialContactEmail,
                Emails.LicenseUpgradeRequested,
                model,
                Common.AppEmailName,
                Common.AppEmail);
        }
        
        protected void SendPasswordEmail(string firstName, string email, string password)
        {
            var model = new ChangePasswordModel(this.Settings)
            {
                FirstName = firstName,
                Password = password,
                TrialContactEmail = this.Settings.TrialContactEmail,
            };

            bool sentSuccessfully = this.MailModel.SendEmailSync(
                firstName,
                email,
                Emails.ChangePasswordSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail);

            this.SaveHistory(
                sentSuccessfully,
                firstName,
                email,
                Emails.ChangePasswordSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail);
        }
        
        protected void SendActivationLinkEmail(string firstName, string email, string activationCode)
        {
            var model = new ActivationLinkModel(this.Settings)
            {
                FirstName = firstName,
                ActivationCode = activationCode,
                TrialContactEmail = this.Settings.TrialContactEmail,
            };

            bool sentSuccessfully = this.MailModel.SendEmailSync(
                firstName,
                email,
                Emails.ChangePasswordSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail);

            this.SaveHistory(
                sentSuccessfully,
                firstName,
                email,
                Emails.ChangePasswordSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail);
        }
        
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
                TrialContactEmail = this.Settings.TrialContactEmail,
                CompanyName = company.CompanyName,
            };

            bool sentSuccessfully = this.MailModel.SendEmailSync(
                firstName,
                email,
                Emails.ActivationSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail, 
                cced, 
                blindCopies);

            this.SaveHistory(
                sentSuccessfully,
                firstName,
                email,
                Emails.ActivationSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail, 
                cced, 
                blindCopies);
        }

        #endregion

        #region Methods
        
        protected void LogError(string methodName, Error error, string userName = null)
        {
            this.Logger.Error(
                string.Format(
                    "{4}. Error result for user: {0}, Error code: {1}, Error Message: {2}, Error Details: {3}",
                    userName ?? "Anonymous",
                    error.errorCode,
                    error.errorMessage,
                    error.errorDetail,
                    methodName));
        }
        
        protected Error GenerateValidationError(ValidationResult validationResult)
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

                        return new Error { errorCode = errorCode, errorMessage = errorDetails.ElementAtOrDefault(1) };
                    }

                    return new Error { errorCode = Errors.CODE_ERRORTYPE_GENERIC_ERROR, errorMessage = failure.ErrorMessage };
                }
            }

            return new Error { errorCode = Errors.CODE_ERRORTYPE_GENERIC_ERROR, errorMessage = "There were validation errors." };
        }
        
        protected List<string> UpdateResultToString(ValidationResult validationResult)
        {
            var list = new List<string>();
            if (validationResult != null)
            {
                list.AddRange(from failure in validationResult.Errors where !string.IsNullOrWhiteSpace(failure.ErrorMessage) select failure.ErrorMessage);
            }

            return list;
        }
        
        private void SaveHistory<TModel>(
            bool mailWasSentSuccessfully,
            string toName, string toEmail, 
            string subject,
            TModel model, 
            string fromName = null, string fromEmail = null,
            List<MailAddress> cced = null, 
            List<MailAddress> bcced = null)
        {
            string body = this.TemplateProvider.GetTemplate<TModel>().TransformTemplate(model), message = body;
            if (message != null)
            {
                message = Regex.Replace(message, "<[^>]*(>|$)", string.Empty);
                message = message.Replace("\r\n", "\n").Replace("\r", "\n").Replace("&nbsp;", " ").Replace("&#39;", @"'");
                message = Regex.Replace(message, @"[ ]{2,}", " ");
                message = message.Replace("\n ", "\n");
                message = Regex.Replace(message, @"[\n]{2,}", "\n");
                message = message.Replace("Enriching online interaction for meetings, training and education on Adobe Connect", string.Empty);
                message = message.TrimStart("\n".ToCharArray());
            }

            var emailHistory = new EmailHistory
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
                Message = message,
                Status = mailWasSentSuccessfully ? EmailHistory.StatusSent : EmailHistory.StatusFailed,
            };

            this.EmailHistoryModel.RegisterSave(emailHistory, true);

            if (!mailWasSentSuccessfully)
            {
                Logger.ErrorFormat("[BaseService.SaveHistory] '{0}' mail to '{1}' has not been sent.", subject, toEmail);

                var error = new Error { errorCode = 201, errorMessage = "Email has not been sent." };
                throw new FaultException<Error>(error, error.errorMessage);
            }
        }
        
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
        
        private static List<MailAddress> GetBCCed(string emails)
        {
            if (string.IsNullOrEmpty(emails))
            {
                return null;
            }

            var blindSeparator = new[] { ";" };
            var blindEmails = emails.Split(blindSeparator, StringSplitOptions.RemoveEmptyEntries);

            return blindEmails.Any() ? blindEmails.Select(email => new MailAddress(email)).ToList() : null;
        }

        #endregion

    }

}