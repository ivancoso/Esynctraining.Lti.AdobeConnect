using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class RecordingCollectionResult : CollectionResult<Recording>
    {
        public RecordingCollectionResult(StatusInfo status)
            : base(status)
        {
        }

        public RecordingCollectionResult(StatusInfo status, IEnumerable<Recording> recorings)
            : base(status, recorings)
        {
        }

    }

}
