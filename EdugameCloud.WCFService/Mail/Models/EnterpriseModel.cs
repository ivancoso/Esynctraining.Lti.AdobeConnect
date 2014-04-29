namespace EdugameCloud.WCFService.Mail.Models
{
    using Esynctraining.Core.Providers;

    public class EnterpriseModel : TrialModel
    {
        public EnterpriseModel(ApplicationSettingsProvider settings)
            : base(settings)
        {
        }

        public string ExpirationDate { get; set; }
    }
}