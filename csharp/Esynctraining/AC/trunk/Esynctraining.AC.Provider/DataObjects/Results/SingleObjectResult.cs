namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System;
    using Esynctraining.AC.Provider.Entities;

    public class SingleObjectResult<T> : ResultBase where T : class
    {
        public T Value { get; private set; }

        public override bool Success
        {
            get
            {
                return base.Success && Value != null;
            }
        }


        public SingleObjectResult(StatusInfo status) : base(status)
        {
        }

        public SingleObjectResult(StatusInfo status, T value)
            : base(status)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

    }

}
