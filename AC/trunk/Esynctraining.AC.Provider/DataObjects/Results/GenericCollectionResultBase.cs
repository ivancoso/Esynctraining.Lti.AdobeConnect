using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class GenericCollectionResultBase<T> : ResultBase
    {
        public GenericCollectionResultBase(StatusInfo status) : base(status)
        {
        }

        public GenericCollectionResultBase(StatusInfo status, IEnumerable<T> values) : base(status)
        {
            Values = values;
        }

        public IEnumerable<T> Values { get; private set; }
    }
}
