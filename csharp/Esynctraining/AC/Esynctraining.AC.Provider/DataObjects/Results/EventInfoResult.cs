namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using Esynctraining.AC.Provider.Entities;

    public class EventInfoResult : ResultBase
    {
        public EventInfo EventInfo { get; set; }
        public PrincipalPreferences Preferences { get; set; }

        public override bool Success
        {
            get { return base.Success && EventInfo != null; }
        }


        public EventInfoResult(StatusInfo status) : base(status) { }

        public EventInfoResult(StatusInfo status, EventInfo eventInfo, PrincipalPreferences preferences)
            : base(status)
        {
            EventInfo = eventInfo;
            Preferences = preferences;
        }
    }
}
