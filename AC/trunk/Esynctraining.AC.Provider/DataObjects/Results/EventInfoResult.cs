namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using Esynctraining.AC.Provider.Entities;

    public class EventInfoResult : ResultBase
    {
        public EventInfo ScoInfo { get; set; }

        public override bool Success
        {
            get { return base.Success && ScoInfo != null; }
        }


        public EventInfoResult(StatusInfo status) : base(status) { }

        public EventInfoResult(StatusInfo status, EventInfo scoInfo)
            : base(status)
        {
            ScoInfo = scoInfo;
        }
    }
}
