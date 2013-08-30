using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public abstract class GenericCollectionResultBase<T> : ResultBase
    {
        protected GenericCollectionResultBase(StatusInfo status) : base(status)
        {
        }

        protected GenericCollectionResultBase(StatusInfo status, IEnumerable<T> values) : base(status)
        {
            Values = values;
        }

        public IEnumerable<T> Values { get; set; }
    }
}
