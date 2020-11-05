namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using Esynctraining.AC.Provider.Entities;

    public class EventLoginInfoResult : ResultBase
    {
        public EventLoginInfo LoginInfo { get; set; }

        public override bool Success
        {
            get { return base.Success && LoginInfo != null; }
        }


        public EventLoginInfoResult(StatusInfo status) : base(status) { }

        public EventLoginInfoResult(StatusInfo status, EventLoginInfo loginInfo)
            : base(status)
        {
            LoginInfo = loginInfo;
        }
    }
}
