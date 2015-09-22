namespace EdugameCloud.MVC.ViewModels
{
    using EdugameCloud.MVC.Controllers;

    /// <summary>
    ///     The import questions view model.
    /// </summary>
    public class ImportQuestionsViewModel : BaseViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportQuestionsViewModel"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public ImportQuestionsViewModel(BaseController controller)
            : base(controller)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportQuestionsViewModel"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        public ImportQuestionsViewModel(BaseController controller, int? page)
            : base(controller, page)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportQuestionsViewModel"/> class.
        /// </summary>
        public ImportQuestionsViewModel()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the xml profile.
        /// </summary>
        public string XmlToImport { get; set; }

        #endregion
    }
}