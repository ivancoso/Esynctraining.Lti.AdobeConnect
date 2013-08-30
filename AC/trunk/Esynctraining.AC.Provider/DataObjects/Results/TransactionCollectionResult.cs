using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class TransactionCollectionResult : GenericCollectionResultBase<TransactionInfo>
    {
        public TransactionCollectionResult(StatusInfo status) : base(status)
        {
        }

        public TransactionCollectionResult(StatusInfo status, IEnumerable<TransactionInfo> values)
            : base(status, values)
        {
        }
    }
}
