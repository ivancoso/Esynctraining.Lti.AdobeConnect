namespace Esynctraining.PdfProcessor.Actions
{
    /// <summary>
    /// The PDF action interface.
    /// </summary>
    public interface IPdfAction
    {
        #region Public Methods and Operators

        /// <summary>
        /// Executes the action for a certain page.
        /// </summary>
        /// <param name="page">
        /// Page number to invoke action.
        /// </param>
        /// <returns>
        /// True if the action has been executed, false otherwise.
        /// </returns>
        bool Execute(int? page);

        #endregion
    }
}