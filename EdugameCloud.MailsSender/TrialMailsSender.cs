namespace EdugameCloud.MailSender
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net.Mail;
    using System.Reflection;
    using System.Threading;

    using Castle.Core.Logging;
    using Castle.Facilities.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.MailSender.Extensions;
    using EdugameCloud.Persistence;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer;
    using Esynctraining.Core.Utils;

    using global::MailSender.Mail.Models;

    using log4net.Repository.Hierarchy;

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

            var trialMailsSender = new TrialMailsSender();
            foreach (TrialWeeks trialWeek in Enum.GetValues(typeof(TrialWeeks)))
            {
                trialMailsSender.SendMails(trialWeek);
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
        /// <param name="trialWeek">
        /// The trial week.
        /// </param>
        private void SendMails(TrialWeeks trialWeek)
        {
            var imagelink = new LinkedResource(@"images/img.png", "image/png");
            imagelink.ContentId = "TrialThirdWeek";
            imagelink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;

            var bcced = new List<MailAddress> { new MailAddress(EmailFrom, NameFrom) };
            var trialLicenses = this.CompanyLicenseModel.GetAllTrial().ToList();
            
            foreach (CompanyLicense trialLicense in trialLicenses.Where(l => l.DateStart.AddDays((int)trialWeek).Date == DateTime.Today.Date).ToList())
            {
//                var trialLicense = trialLicenses.FirstOrDefault();
                var firstName = trialLicense.CreatedBy.FirstName;
                var email = trialLicense.CreatedBy.Email;
                Console.WriteLine("Sending notification to {0}: {1}", firstName, email);
                this.Logger.Error(string.Format("Sending notification to {0}: {1}", firstName, email));
                
                switch (trialWeek)
                {
                    case TrialWeeks.First:
                        this.MailModel.SendEmail(
                            firstName, 
                            email, 
                            Subject, 
                            new TrialFirstWeekModel(this.Settings)
                                {
                                    FirstName = firstName, 
                                    MailSubject = Subject
                                }, 
                            NameFrom,
                            EmailFrom, 
                            bcced: bcced);
                        break;
                    case TrialWeeks.Second:
                        this.MailModel.SendEmail(
                            firstName, 
                            email, 
                            Subject, 
                            new TrialSecondWeekModel(this.Settings)
                                {
                                    FirstName = firstName, 
                                    MailSubject = Subject
                                }, 
                            NameFrom,
                            EmailFrom, 
                            bcced: bcced);
                        break;
                    case TrialWeeks.Third:
                        this.MailModel.SendEmail(
                            firstName,
                            email, 
                            Subject, 
                            new TrialThirdWeekModel(this.Settings)
                                {
                                    FirstName = firstName, 
                                    MailSubject = Subject
                                }, 
                            NameFrom,
                            EmailFrom, 
                            bcced: bcced,
                            linkedResources: new List<LinkedResource> { imagelink });
                        break;
                    case TrialWeeks.Fourth:
                        this.MailModel.SendEmail(
                            firstName,
                            email, 
                            Subject, 
                            new TrialFourthWeekModel(this.Settings)
                                {
                                    FirstName = firstName, 
                                    MailSubject = Subject
                                }, 
                            NameFrom,
                            EmailFrom, 
                            bcced: bcced);
                        break;
                }

                Thread.Sleep(3000);
            }
        }

        #endregion
    }
}