namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.AC.Provider.Entities;

    public class FieldCollectionResult : CollectionResult<Field>
    {
        public FieldCollectionResult(StatusInfo status)
            : base(status)
        {
        }

        public FieldCollectionResult(StatusInfo status, IEnumerable<Field> values)
            : base(status, values)
        {
        }

    }

}
