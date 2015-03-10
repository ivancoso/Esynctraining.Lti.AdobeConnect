namespace EdugameCloud.MailSender
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Castle.Core.Logging;
    using Castle.Facilities.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Persistence;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer;
    using Esynctraining.Core.Utils;

    using global::MailSender.Mail.Models;

    using NHibernate;

    using Configuration = NHibernate.Cfg.Configuration;

    /// <summary>
    ///     The trial weeks.
    /// </summary>
    public enum TrialWeeks
    {
        /// <summary>
        /// The first.
        /// </summary>
        First = 1, 

        /// <summary>
        /// The second.
        /// </summary>
        Second = 7, 

        /// <summary>
        /// The third.
        /// </summary>
        Third = 14, 

        /// <summary>
        /// The fourth.
        /// </summary>
        Fourth = 22, 

        /// <summary>
        /// The fifth.
        /// </summary>
        Fifth = 28
    }

    /// <summary>
    /// The trial mails sender.
    /// </summary>
    public class TrialMailsSender
    {
        #region Constants

        /// <summary>
        /// The email from.
        /// </summary>
        private const string EmailFrom = "jbeck@esynctraining.com";

        /// <summary>
        /// The name from.
        /// </summary>
        private const string NameFrom = "Jacquie";

        /// <summary>
        /// The subject.
        /// </summary>
        private const string Subject = "Welcome to EduGame Cloud!";

        #endregion

        #region Properties

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
        ///     Gets the company model.
        /// </summary>
        private CompanyLicenseModel CompanyLicenseModel
        {
            get
            {
                return IoC.Resolve<CompanyLicenseModel>();
            }
        }

        /// <summary>
        ///     Gets the email history model.
        /// </summary>
        protected EmailHistoryModel EmailHistoryModel
        {
            get
            {
                return IoC.Resolve<EmailHistoryModel>();
            }
        }

        /// <summary>
        ///     Gets the template provider.
        /// </summary>
        protected ITemplateProvider TemplateProvider
        {
            get
            {
                return IoC.Resolve<ITemplateProvider>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private ILogger Logger
        {
            get
            {
                return IoC.Resolve<ILogger>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The main.
        /// </summary>
        public static void Main()
        {
            InitializeContainer();
            dynamic settings = IoC.Resolve<ApplicationSettingsProvider>();
            var imagesFolder = (string)settings.ImagesFolderToLink;
            var trialMailsSender = new TrialMailsSender();
            foreach (TrialWeeks trialWeek in Enum.GetValues(typeof(TrialWeeks)))
            {
                trialMailsSender.SendMails(imagesFolder, trialWeek);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The initialize container.
        /// </summary>
        private static void InitializeContainer()
        {
            var container = new WindsorContainer();
            IoC.Initialize(container);

            container.Register(Component.For<FluentConfiguration>().LifeStyle.Singleton);
            container.Register(
                Component.For<Configuration>().LifeStyle.Singleton.Activator<NHibernateConfigurationActivator>());
            container.Register(
                Component.For<ISessionFactory>().LifeStyle.Singleton.Activator<NHibernateSessionFactoryActivator>());
            container.Register(
                Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.Transient);
            container.Register(
                Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(Repository<,>)).LifeStyle.Transient);
            container.Register(
                Component.For<ApplicationSettingsProvider>()
                    .ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", ConfigurationManager.AppSettings))
                    .DynamicParameters((k, d) => d.Add("globalizationSection", null))
                    .LifeStyle.Singleton);

            container.Register(Component.For<ITemplateProvider>().ImplementedBy<TemplateProvider>().LifeStyle.Transient);
            container.Register(Component.For<IAttachmentsProvider>().ImplementedBy<AttachmentsProvider>().LifeStyle.Transient);
            container.Register(Component.For<MailModel>().ImplementedBy<MailModel>().LifeStyle.Transient);
            container.Register(
                Classes.FromAssemblyNamed("EdugameCloud.Core")
                    .Pick()
                    .If(Component.IsInNamespace("EdugameCloud.Core.Business.Models"))
                    .WithService.Self()
                    .Configure(c => c.LifestyleTransient()));
            container.AddFacility(new LoggingFacility(LoggerImplementation.Log4net, "log4net.cfg.xml"));
        }

        /// <summary>
        /// The send mails.
        /// </summary>
        /// <param name="imagesFolder">
        /// The images Folder.
        /// </param>
        /// <param name="trialWeek">
        /// The trial week.
        /// </param>
        private void SendMails(string imagesFolder, TrialWeeks trialWeek)
        {
            var trialLicenses = this.CompanyLicenseModel.GetAllTrial().ToList();
            var usedEmails = new HashSet<string>();
            foreach (CompanyLicense trialLicense in trialLicenses.Where(l => l.DateStart.AddDays((int)trialWeek).Date == DateTime.Today.Date).ToList())
            {
                if (trialLicense.Company == null) continue;

                var users = trialLicense.Company.PrimaryContact != null
                    ? new List<User> { trialLicense.Company.PrimaryContact }
                    : trialLicense.Company.Users.Where(u => u.IsAdministrator()).ToList();

                foreach (var user in users)
                {
                    if (usedEmails.Contains(user.Email))
                    {
                        continue;
                    }

                    usedEmails.Add(user.Email);

                    var firstName = user.FirstName;
                    var email = user.Email;

                    Console.WriteLine("Sending notification to {0}: {1}", firstName, email);
                    this.Logger.Error(string.Format("Sending notification to {0}: {1}", firstName, email));

                    switch (trialWeek)
                    {
                        case TrialWeeks.First:
                            var model1 = new TrialFirstWeekModel(this.Settings)
                                        {
                                            FirstName = firstName,
                                            MailSubject = Subject,
                                            CompanyName = trialLicense.Company.CompanyName
                                        };
                            this.MailModel.SendEmail(
                                firstName,
                                email,
                                Subject,
                                model1,
                                NameFrom,
                                EmailFrom);
                            this.SaveHistory(firstName, user, Subject, model1, NameFrom, EmailFrom);
                            break;
                        case TrialWeeks.Second:
                            var model2 = new TrialSecondWeekModel(this.Settings)
                                        {
                                            FirstName = firstName,
                                            MailSubject = Subject
                                        };
                            this.MailModel.SendEmail(
                                firstName,
                                email,
                                Subject,
                                model2, 
                            NameFrom,
                            EmailFrom);
                            this.SaveHistory(firstName, user, Subject, model2, NameFrom, EmailFrom);
                        break;
                    case TrialWeeks.Third:
                        var imagelink = new LinkedResource(Path.Combine(imagesFolder, "img.png"), "image/png")
                        {
                            ContentId = "TrialThirdWeek",
                            TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                        };
                        var model3 = new TrialThirdWeekModel(this.Settings)
                                {
                                    FirstName = firstName,
                                    MailSubject = Subject
                                };
                        this.MailModel.SendEmail(
                            firstName,
                            email, 
                            Subject, 
                            model3,
                                NameFrom,
                                EmailFrom,
                                linkedResources: new List<LinkedResource> { imagelink });
                        this.SaveHistory(firstName, user, Subject, model3, NameFrom, EmailFrom);
                            break;
                        case TrialWeeks.Fourth:
                            var model4 = new TrialFourthWeekModel(this.Settings)
                                    {
                                        FirstName = firstName,
                                        MailSubject = Subject
                                    };
                            this.MailModel.SendEmail(
                                firstName,
                                email,
                                Subject,
                                model4,
                                NameFrom,
                                EmailFrom);
                            this.SaveHistory(firstName, user, Subject, model4, NameFrom, EmailFrom);
                            break;
                    }
                }
                Thread.Sleep(3000);
            }
        }

        private void SaveHistory<TModel>(string toName, User user, string subject, TModel model, string fromName = null, string fromEmail = null, List<MailAddress> cced = null, List<MailAddress> bcced = null)
        {
            try
            {

                string body = this.TemplateProvider.GetTemplate<TModel>().TransformTemplate(model), message = body;
                if (message != null)
                {
                    message = Regex.Replace(message, "<[^>]*(>|$)", "");
                    message = message.Replace("\r\n", "\n")
                        .Replace("\r", "\n")
                        .Replace("&nbsp;", " ")
                        .Replace("&#39;", @"'");
                    message = Regex.Replace(message, @"[ ]{2,}", " ");
                    message = message.Replace("\n ", "\n");
                    message = Regex.Replace(message, @"[\n]{2,}", "\n");
                    while (message.StartsWith("\n")) message = message.Remove(0, 1);
                }

                var emailHistory = new EmailHistory()
                                   {
                                       SentTo = user.Email,
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
                                       User = user,
                                       Body = body,
                                       Message = message
                                   };

                this.EmailHistoryModel.RegisterSave(emailHistory, true);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        #endregion
    }
}