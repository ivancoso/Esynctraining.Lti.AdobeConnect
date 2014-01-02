namespace Esynctraining.PdfProcessor.Actions.Filters
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The and filter.
    /// </summary>
    public class AndFilter : LogicalOperationFilter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AndFilter"/> class.
        /// </summary>
        /// <param name="filters">
        /// The Filters.
        /// </param>
        public AndFilter(params PdfFilter[] filters) : base(filters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AndFilter"/> class.
        /// </summary>
        /// <param name="filters">
        /// The Filters.
        /// </param>
        public AndFilter(IList<PdfFilter> filters) : base(filters)
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
            return this.Filters.All(pdfFilter => pdfFilter.Execute(page));
        }

        #endregion
    }
}