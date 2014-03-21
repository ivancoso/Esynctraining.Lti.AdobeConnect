using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.Persistence;
using EdugameCloud.WCFService.Mail.Models;
using EdugameCloud.WCFService.Providers;
using Esynctraining.Core.Business.Models;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Providers.Mailer.Models;
using Esynctraining.Core.Utils;
using Resources;
using File = EdugameCloud.Core.Domain.Entities.File;

namespace EdugameCloud.MailSender
{
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
		/// Gets the company model.
		/// </summary>
		private CompanyLicenseModel CompanyLicenseModel
		{
			get
			{
				return IoC.Resolve<CompanyLicenseModel>();
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
		///     Gets the settings.
		/// </summary>
		protected dynamic Settings
		{
			get
			{
				return IoC.Resolve<ApplicationSettingsProvider>();
			}
		}

		private static readonly string subject = "Welcome to EduGame Cloud!";
		private static readonly string nameFrom = "Jacquie";
		private static readonly string emailFrom = "jbeck@esynctraining.com";

		private void SendMails(TrialWeeks trialWeek)
		{
			var trialLicenses = CompanyLicenseModel.GetAll().Where(l => l.LicenseStatus == CompanyLicenseStatus.Trial);
			foreach (var trialLicense in trialLicenses.Where(l => l.DateCreated.AddDays((int)trialWeek+1).Date == DateTime.Today.Date).ToList())
			{
				switch (trialWeek)
				{
					case TrialWeeks.First:
						this.MailModel.SendEmail(
							trialLicense.CreatedBy.FirstName,
							trialLicense.CreatedBy.Email,
							subject,
							new TrialFirstWeekModel(this.Settings)
							{
								FirstName = trialLicense.CreatedBy.FirstName,
								MailSubject = subject
							},
							nameFrom,
							emailFrom);
						break;
					case TrialWeeks.Second:
						this.MailModel.SendEmail(
							trialLicense.CreatedBy.FirstName,
							trialLicense.CreatedBy.Email,
							subject,
							new TrialSecondWeekModel(this.Settings)
							{
								FirstName = trialLicense.CreatedBy.FirstName,
								MailSubject = subject
							},
							nameFrom,
							emailFrom);
						break;
					case TrialWeeks.Third:
						this.MailModel.SendEmail(
							trialLicense.CreatedBy.FirstName,
							trialLicense.CreatedBy.Email,
							subject,
							new TrialThirdWeekModel(this.Settings)
							{
								FirstName = trialLicense.CreatedBy.FirstName,
								MailSubject = subject
							},
							nameFrom,
							emailFrom);
						break;
					case TrialWeeks.Fourth:
						this.MailModel.SendEmail(
							trialLicense.CreatedBy.FirstName,
							trialLicense.CreatedBy.Email,
							subject,
							new TrialFourthWeekModel(this.Settings)
							{
								FirstName = trialLicense.CreatedBy.FirstName,
								MailSubject = subject
							},
							nameFrom,
							emailFrom);
						break;
				}
				Thread.Sleep(3000);
			}
		}

		static void Main()
		{
			IoC.Initialize(new WindsorContainer());
			IoC.Container.RegisterComponents(console:true);
			TrialMailsSender trialMailsSender = new TrialMailsSender();
			foreach (TrialWeeks trialWeek in Enum.GetValues(typeof(TrialWeeks)))
			{
				trialMailsSender.SendMails(trialWeek);
			}
		}
	}
}
