using System.Net.Mail;

namespace Esynctraining.Mail.Configuration
{
    public static class SystemEmailExtensions
    {
        public static MailAddress BuildMailAddress(this ISystemEmail email)
        {
            return new MailAddress(email.Email, email.Name);
        }

    }

}
