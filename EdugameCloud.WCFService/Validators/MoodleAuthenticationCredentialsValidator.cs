

namespace EdugameCloud.WCFService.Validators
{
    using System;
    using System.IdentityModel.Selectors;
    using System.ServiceModel;

    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using NHibernate.Cfg;

    public class MoodleAuthenticationCredentialsValidator : UserNamePasswordValidator
    {
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

        public override void Validate(string userName, string password)
        {
            if (null == userName || null == password)
            {
                throw new ArgumentNullException();
            }

            if (!(userName == Settings.MoodleUserName  && password == Settings.MoodlePassword))
            {
                throw new FaultException("No user with such credentials found");
            }

        }
    }
}