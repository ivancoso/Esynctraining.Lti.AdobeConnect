namespace Esynctraining.Mail.Configuration.Json
{
    public class NotificationsSettings : INotificationsSettings
    {
        public SystemEmailCollection SystemEmails { get; set; }

        public EmailRecipientSettingsCollection RecipientSettings { get; set; }

        ISystemEmailCollection INotificationsSettings.SystemEmails => SystemEmails;

        IEmailRecipientSettingsCollection INotificationsSettings.RecipientSettings => RecipientSettings;

    }
    
}
