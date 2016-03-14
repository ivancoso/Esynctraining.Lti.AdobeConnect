using System.Collections.Generic;
using Esynctraining.AC.Provider.DataObjects.Results;

namespace Esynctraining.AC.Provider.Entities
{
    public class SeminarLicensesCollectionResult : GenericCollectionResultBase<SeminarLicenseSco>
    {
        public SeminarLicensesCollectionResult(StatusInfo status) : base(status)
        {
        }

        public SeminarLicensesCollectionResult(StatusInfo status, IEnumerable<SeminarLicenseSco> values)
            : base(status, values)
        {
        }
    }
}