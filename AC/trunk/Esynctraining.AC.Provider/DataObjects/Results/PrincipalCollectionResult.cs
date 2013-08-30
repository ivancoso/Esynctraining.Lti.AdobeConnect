using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class PrincipalCollectionResult : GenericCollectionResultBase<Principal>
    {
        public PrincipalCollectionResult(StatusInfo status) : base(status)
        {
        }

        public PrincipalCollectionResult(StatusInfo status, IEnumerable<Principal> values)
            : base(status, values)
        {
        }
    }
}
