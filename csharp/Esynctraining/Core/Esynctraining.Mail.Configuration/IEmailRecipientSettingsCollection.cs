namespace Esynctraining.Mail.Configuration
{
    public interface IEmailRecipientSettingsCollection
    {
        IEmailRecipientSettings GetByToken(string emailToken);

    }

}
