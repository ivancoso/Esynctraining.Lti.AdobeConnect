namespace Esynctraining.PdfProcessor.Actions.Filters
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The or filter.
    /// </summary>
    public class OrFilter : LogicalOperationFilter
    {
        #region Fields

        /// <summary>
        /// The Filters.
        /// </summary>
        private readonly IList<PdfFilter> filters;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrFilter"/> class.
        /// </summary>
        /// <param name="filters">
        /// The Filters.
        /// </param>
        public OrFilter(params PdfFilter[] filters)
        {
            this.filters = filters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrFilter"/> class.
        /// </summary>
        /// <param name="filters">
        /// The Filters.
        /// </param>
        public OrFilter(IList<PdfFilter> filters)
        {
            this.filters = filters;
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
            return this.filters.Any(pdfFilter => pdfFilter.Execute(page));
        }

        #endregion
    }
}