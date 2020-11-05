namespace Esynctraining.PdfProcessor.Actions.Filters
{
    using System.Collections.Generic;

    /// <summary>
    ///     The logical operation filter.
    /// </summary>
    public class LogicalOperationFilter : PdfFilter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalOperationFilter"/> class.
        /// </summary>
        /// <param name="filters">
        /// The Filters.
        /// </param>
        public LogicalOperationFilter(params PdfFilter[] filters)
            : this()
        {
            this.Filters = filters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalOperationFilter"/> class.
        /// </summary>
        /// <param name="filters">
        /// The Filters.
        /// </param>
        public LogicalOperationFilter(IList<PdfFilter> filters)
            : this()
        {
            this.Filters = filters;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicalOperationFilter" /> class.
        /// </summary>
        public LogicalOperationFilter()
            : base(null)
        {
            this.Filters = new List<PdfFilter>();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the filters.
        /// </summary>
        protected IList<PdfFilter> Filters { get; private set; }

        #endregion
    }
}