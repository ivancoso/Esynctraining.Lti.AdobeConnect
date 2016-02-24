using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class TelephonyProfileInfoResult : ResultBase
    {
        public TelephonyProfileInfoResult(StatusInfo status) : base(status)
        {
        }

        public TelephonyProfileInfoResult(StatusInfo status, TelephonyProfile telephonyProfile)
            : base(status)
        {
            TelephonyProfile = telephonyProfile;
        }

        public TelephonyProfile TelephonyProfile { get; set; }

        public override bool Success
        {
            get
            {
                return base.Success && this.TelephonyProfile != null;
            }
        }
    }
}