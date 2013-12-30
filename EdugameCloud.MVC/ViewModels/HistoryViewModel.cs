namespace EdugameCloud.MVC.ViewModels
{
    using BaseController = EdugameCloud.MVC.Controllers.BaseController;

    /// <summary>
    /// The history view model.
    /// </summary>
    public class HistoryViewModel : BaseViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryViewModel"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public HistoryViewModel(BaseController controller)
            : base(controller)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryViewModel"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        public HistoryViewModel(BaseController controller, int? page)
            : base(controller, page)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryViewModel"/> class.
        /// </summary>
        public HistoryViewModel()
        {
        }

        #endregion
    }
}