using Esynctraining.Core.Providers;
using Esynctraining.Core.Providers.Mailer.Models;

namespace EdugameCloud.WCFService.Mail.Models
{
    public class CertificateEmailModel : BaseTemplateModel
    {
        public CertificateEmailModel(ApplicationSettingsProvider settings) : base(settings)
        {
        }

        public string ParticipantName { get; set; }
        public string CertificateLink { get; set; }
    }
}