using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class CollectionResult<T> : ResultBase
    {
        public IEnumerable<T> Values { get; private set; }


        public CollectionResult(StatusInfo status) : base(status)
        {
        }

        public CollectionResult(StatusInfo status, IEnumerable<T> values) : base(status)
        {
            // TODO: check on null? review is required!

            Values = values;
        }
        
    }

}
