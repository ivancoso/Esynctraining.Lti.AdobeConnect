using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class PermissionCollectionResult : GenericCollectionResultBase<PermissionInfo>
    {
        public PermissionCollectionResult(StatusInfo status) : base(status)
        {
        }

        public PermissionCollectionResult(StatusInfo status, IEnumerable<PermissionInfo> values) : base(status, values)
        {
        }
    }
}
