namespace Esynctraining.Mail.Configuration
{
    public interface ISmtpSettings
    {
        string Host { get; }

        int Port { get; }

        string UserName { get; }

        string Password { get; }

    }

}
