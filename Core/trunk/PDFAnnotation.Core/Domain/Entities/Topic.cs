namespace PDFAnnotation.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The Topic.
    /// </summary>
    public class Topic : Entity
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets Contact
        /// </summary>
        public virtual Category Category { get; set; }

        /// <summary>
        ///     Gets or sets FirstName
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets LastName
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        ///     Gets or sets LastName
        /// </summary>
        public virtual string FullName
        {
            get
            {
                var res = this.FirstName.Return(x => x, string.Empty) + " " + this.LastName.Return(x => x, string.Empty);
                res = res.Trim();
                return string.IsNullOrEmpty(res) ? this.FirmName : res;
            }
            set
            {
            }
        }

        /// <summary>
        ///     Gets or sets CompanyName
        /// </summary>
        public virtual string FirmName { get; set; }

        #endregion
    }
}