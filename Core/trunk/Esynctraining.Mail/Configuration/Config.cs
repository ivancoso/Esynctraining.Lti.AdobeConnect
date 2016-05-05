using System.Configuration;

namespace Esynctraining.Mail.Configuration
{
    public static class Config
    {
        // TODO: implement generic method with 'name' parameter
        public static INotificationsSettings GetConfig()
        {
            var result = (INotificationsSettings)ConfigurationManager.GetSection("mailerSettings");

            if (result == null)
                throw new ConfigurationErrorsException("Configuration section 'mailerSettings' was not found.");

            return result;
        }

    }

}
