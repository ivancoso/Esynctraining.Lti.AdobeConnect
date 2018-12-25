namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using Esynctraining.AC.Provider.Entities;

    public class RegisterEventInfoResult : ResultBase
    {
        public Principal Principal { get; set; }

        public override bool Success
        {
            get { return base.Success && Principal != null; }
        }


        public RegisterEventInfoResult(StatusInfo status) : base(status) { }

        public RegisterEventInfoResult(StatusInfo status, Principal principal)
            : base(status)
        {
            Principal = principal;
        }
    }
}
