namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using Esynctraining.AC.Provider.Entities;

    public class RegisterEventInfoResult : ResultBase
    {
        // NOTE: Principal can be null if user was new for AC. So you need to call RegisterToEvent second time. 
        public Principal Principal { get; set; }


        public RegisterEventInfoResult(StatusInfo status) : base(status) { }

        public RegisterEventInfoResult(StatusInfo status, Principal principal)
            : base(status)
        {
            Principal = principal;
        }
    }
}
