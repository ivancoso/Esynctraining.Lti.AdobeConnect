namespace Esynctraining.Mail.Configuration
{
    public interface INotificationsSettings
    {
        ISystemEmailCollection SystemEmails { get; }

        IEmailRecipientSettingsCollection RecipientSettings { get; }

    }

}
