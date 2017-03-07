using System;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Providers.Mailer.Models;

namespace EdugameCloud.WCFService.Mail.Models
{
    public class EventQuizRegistrationModel : BaseTemplateModel
    {
        public EventQuizRegistrationModel(ApplicationSettingsProvider settings) : base(settings)
        {
            
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EventName { get; set; }

        public DateTime EventStartDate { get; set; }

        public DateTime EventEndDate { get; set; }

        public string MeetingUrl { get; set; }
    }
}