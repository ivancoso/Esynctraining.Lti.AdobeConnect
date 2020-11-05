using Esynctraining.Mail.Configuration;

namespace Esynctraining.Mail.TestConsole
{
    internal sealed class Application
    {
        SmtpSettings _smtp;
        INotificationsSettings _not;

        public Application(SmtpSettings smtp, INotificationsSettings not)
        {
            _smtp = smtp;
            _not = not;
        }


        public void Run()
        {
        }
        
    }

}
