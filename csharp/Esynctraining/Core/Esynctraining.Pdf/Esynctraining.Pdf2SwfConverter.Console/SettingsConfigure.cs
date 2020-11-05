using Microsoft.Extensions.Configuration;

namespace Esynctraining.Pdf2SwfConverter.Console
{
    public static class SettingsConfigure
    {
        public static void SetupAppSettingsAndLogging(this IConfigurationBuilder config)
        {
            config
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.logging.json", optional: false, reloadOnChange: true);

        }
    }
}
