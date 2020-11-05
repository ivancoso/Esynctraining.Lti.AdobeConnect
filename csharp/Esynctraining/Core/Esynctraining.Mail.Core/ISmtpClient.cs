using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Mail.Configuration;

namespace Esynctraining.Mail
{
    public interface ISmtpClient
    {
        Task<bool> SendEmailAsync<TModel>(
            ISystemEmail from,
            IEnumerable<ISystemEmail> to,
            string subject,
            TModel model,
            IEnumerable<ISystemEmail> cced = null,
            IEnumerable<ISystemEmail> bcced = null,
            IEnumerable<Attachment> attachments = null);

        Task<bool> SendEmailAsync(
            ISystemEmail from,
            IEnumerable<ISystemEmail> to,
            string subject,
            string body,
            IEnumerable<ISystemEmail> cced = null,
            IEnumerable<ISystemEmail> bcced = null,
            IEnumerable<Attachment> attachments = null);
    }

}
