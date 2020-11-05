namespace Esynctraining.PdfProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Esynctraining.PdfProcessor.Actions;

    public class PdfProcessor
    {
        protected IList<IPdfAction> Actions { get; set; }


        public PdfProcessor(IList<IPdfAction> actions)
        {
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }


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

    }

}