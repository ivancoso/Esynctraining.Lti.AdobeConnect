namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System;

    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The field result.
    /// </summary>
    public class FieldResult : ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public FieldResult(StatusInfo status)
            : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        public FieldResult(StatusInfo status, String fieldValue)
            : base(status)
        {
            this.FieldValue = fieldValue;
        }

        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        public String FieldValue { get; set; }
    }
}
