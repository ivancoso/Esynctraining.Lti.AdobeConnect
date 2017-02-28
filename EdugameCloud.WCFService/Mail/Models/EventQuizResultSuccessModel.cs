using Esynctraining.Core.Providers;
using Esynctraining.Core.Providers.Mailer.Models;

namespace EdugameCloud.WCFService.Mail.Models
{
    public class EventQuizResultSuccessModel : BaseTemplateModel
    {
        public EventQuizResultSuccessModel(ApplicationSettingsProvider settings) : base(settings)
        {
        }

        public string Name { get; set; }

        public string EventName { get; set; }

        public string PostQuizUrl { get; set; }
    }
}