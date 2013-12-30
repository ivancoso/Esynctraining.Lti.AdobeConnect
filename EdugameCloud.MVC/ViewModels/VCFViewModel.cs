namespace EdugameCloud.MVC.ViewModels
{
    using EdugameCloud.MVC.Controllers;

    /// <summary>
    ///     The vcf view model.
    /// </summary>
    public class VCFViewModel : BaseViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VCFViewModel"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public VCFViewModel(BaseController controller)
            : base(controller)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VCFViewModel"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        public VCFViewModel(BaseController controller, int? page)
            : base(controller, page)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VCFViewModel"/> class.
        /// </summary>
        public VCFViewModel()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the xml profile.
        /// </summary>
        public string XmlProfile { get; set; }

        #endregion
    }
}