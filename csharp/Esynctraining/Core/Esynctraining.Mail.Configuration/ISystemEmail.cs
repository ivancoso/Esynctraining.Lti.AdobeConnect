namespace Esynctraining.Mail.Configuration
{
    public interface ISystemEmail
    {
        string Token { get; }

        string Name { get; }

        string Email { get; }


        //MailAddress BuildMailAddress();

    }

}
