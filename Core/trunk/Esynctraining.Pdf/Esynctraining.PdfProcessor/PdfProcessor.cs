namespace Esynctraining.PdfProcessor
{
    using System.Collections.Generic;
    using System.Linq;

    using Esynctraining.PdfProcessor.Actions;

    /// <summary>
    /// The PDF processor.
    /// </summary>
    public class PdfProcessor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfProcessor"/> class.
        /// </summary>
        /// <param name="actions">
        /// The actions.
        /// </param>
        public PdfProcessor(IList<IPdfAction> actions)
        {
            this.Actions = actions;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the actions.
        /// </summary>
        protected IList<IPdfAction> Actions { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Executes list of actions for certain page.
        /// </summary>
        /// <param name="page">
        /// Page to execute
        /// </param>
        /// <returns>
        /// True if at least one action has been executed.
        /// </returns>
        public virtual bool Execute(int? page)
        {
            return this.Actions.Aggregate(false, (current, pdfAction) => current | pdfAction.Execute(page));
        }

        #endregion
    }
}