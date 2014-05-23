namespace EdugameCloud.MailSender
{
    using System;
    using System.Linq;
    using System.Threading;

    using Castle.Windsor;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Persistence;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Providers.Mailer;
    using Esynctraining.Core.Utils;

    using global::MailSender.Mail.Models;

    /// <summary>
    /// The trial weeks.
    /// </summary>
    public enum TrialWeeks
    {
        First = 0,
        Second = 7,
        Third = 14,
        Fourth = 21
    }

    public class TrialMailsSender
    {
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
        /// Gets the company model.
        /// </summary>
        private CompanyLicenseModel CompanyLicenseModel
        {
            get
            {
                return IoC.Resolve<CompanyLicenseModel>();
            }
        }

        private const string Subject = "Welcome to EduGame Cloud!";

        private const string NameFrom = "Jacquie";

        private const string EmailFrom = "jbeck@esynctraining.com";

        private void SendMails(TrialWeeks trialWeek)
        {
            var trialLicenses = CompanyLicenseModel.GetAll().Where(l => l.LicenseStatus == CompanyLicenseStatus.Trial);
            foreach (var trialLicense in trialLicenses.Where(l => l.DateCreated.AddDays((int)trialWeek + 1).Date == DateTime.Today.Date).ToList())
            {
                switch (trialWeek)
                {
                    case TrialWeeks.First:
                        this.MailModel.SendEmail(
                            trialLicense.CreatedBy.FirstName,
                            trialLicense.CreatedBy.Email,
                            Subject,
                            new TrialFirstWeekModel(this.Settings)
                            {
                                FirstName = trialLicense.CreatedBy.FirstName,
                                MailSubject = Subject
                            },
                            NameFrom,
                            EmailFrom);
                        break;
                    case TrialWeeks.Second:
                        this.MailModel.SendEmail(
                            trialLicense.CreatedBy.FirstName,
                            trialLicense.CreatedBy.Email,
                            Subject,
                            new TrialSecondWeekModel(this.Settings)
                            {
                                FirstName = trialLicense.CreatedBy.FirstName,
                                MailSubject = Subject
                            },
                            NameFrom,
                            EmailFrom);
                        break;
                    case TrialWeeks.Third:
                        this.MailModel.SendEmail(
                            trialLicense.CreatedBy.FirstName,
                            trialLicense.CreatedBy.Email,
                            Subject,
                            new TrialThirdWeekModel(this.Settings)
                            {
                                FirstName = trialLicense.CreatedBy.FirstName,
                                MailSubject = Subject
                            },
                            NameFrom,
                            EmailFrom);
                        break;
                    case TrialWeeks.Fourth:
                        this.MailModel.SendEmail(
                            trialLicense.CreatedBy.FirstName,
                            trialLicense.CreatedBy.Email,
                            Subject,
                            new TrialFourthWeekModel(this.Settings)
                            {
                                FirstName = trialLicense.CreatedBy.FirstName,
                                MailSubject = Subject
                            },
                            NameFrom,
                            EmailFrom);
                        break;
                }
                Thread.Sleep(3000);
            }
        }

        public static void Main()
        {
            IoC.Initialize(new WindsorContainer());
            IoC.Container.RegisterComponents(console: true);
            var trialMailsSender = new TrialMailsSender();
            foreach (TrialWeeks trialWeek in Enum.GetValues(typeof(TrialWeeks)))
            {
                trialMailsSender.SendMails(trialWeek);
            }
        }
    }
}
