namespace Esynctraining.PdfProcessor.Actions.Filters
{
    using System.Linq;

    /// <summary>
    /// The not filter.
    /// </summary>
    public class NotFilter : LogicalOperationFilter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFilter"/> class.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        public NotFilter(PdfFilter filter)
            : base(new[] { filter })
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Execute(int? page)
        {
            var filter = this.Filters.FirstOrDefault();
            if (filter != null)
            {
                return !filter.Execute(page);
            }

            return false;
        }

        #endregion
    }
}