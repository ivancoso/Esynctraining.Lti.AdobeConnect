using Esynctraining.Core.Providers;
using Esynctraining.Core.Providers.Mailer.Models;

namespace EdugameCloud.WCFService.Mail.Models
{
    public class TeacherCertificateEmailModel : BaseTemplateModel
    {
        public TeacherCertificateEmailModel(ApplicationSettingsProvider settings) : base(settings)
        {
        }

        public string ParticipantName { get; set; }
        public string CertificateLink { get; set; }
        public string TeacherName { get; set; }
    }
}