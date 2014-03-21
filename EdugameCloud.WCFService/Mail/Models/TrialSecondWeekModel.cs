using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Providers.Mailer.Models;

namespace EdugameCloud.WCFService.Mail.Models
{
	/// <summary>
	/// The trial model.
	/// </summary>
	public class TrialSecondWeekModel : BaseTemplateModel
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TrialSecondWeekModel"/> class. 
		/// </summary>
		/// <param name="settings">
		/// The settings.
		/// </param>
		public TrialSecondWeekModel(ApplicationSettingsProvider settings)
			: base(settings)
		{
		}

		/// <summary>
		/// Gets or sets the user name.
		/// </summary>
		public string FirstName { get; set; }

		#endregion
	}
}