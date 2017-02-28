using Esynctraining.Core.Providers;
using Esynctraining.Core.Providers.Mailer.Models;

namespace EdugameCloud.WCFService.Mail.Models
{
    public class EventQuizResultUnsuccessModel : BaseTemplateModel
    {
        public EventQuizResultUnsuccessModel(ApplicationSettingsProvider settings) : base(settings)
        {
        }

        public string Name { get; set; }

        public string EventName { get; set; }

    }
}