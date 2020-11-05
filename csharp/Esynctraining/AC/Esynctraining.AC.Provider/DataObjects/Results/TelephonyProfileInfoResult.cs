using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class TelephonyProfileInfoResult : ResultBase
    {
        public TelephonyProfile TelephonyProfile { get; set; }

        /// <summary>
        /// TRICK: returned by "telephony-profile-info" request!
        /// </summary>
        public IDictionary<string, string> TelephonyProfileFields { get; set; }

        public override bool Success
        {
            get { return base.Success && this.TelephonyProfile != null; }
        }


        public TelephonyProfileInfoResult(StatusInfo status) : base(status)
        {
        }

        public TelephonyProfileInfoResult(StatusInfo status, TelephonyProfile telephonyProfile)
            : base(status)
        {
            TelephonyProfile = telephonyProfile;
        }

        public TelephonyProfileInfoResult(StatusInfo status, TelephonyProfile telephonyProfile, IDictionary<string, string> telephonyProfileFields)
            : base(status)
        {
            TelephonyProfile = telephonyProfile;
            TelephonyProfileFields = telephonyProfileFields;
        }
        
    }

}