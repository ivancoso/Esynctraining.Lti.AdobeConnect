namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System.Collections.Generic;

    using Esynctraining.AC.Provider.Entities;

    public class TelephonyProviderCollectionResult : GenericCollectionResultBase<TelephonyProvider>
    {
        public TelephonyProviderCollectionResult(StatusInfo status)
            : base(status)
        {
        }
        
        public TelephonyProviderCollectionResult(StatusInfo status, IEnumerable<TelephonyProvider> values)
            : base(status, values)
        {
        }

    }

}