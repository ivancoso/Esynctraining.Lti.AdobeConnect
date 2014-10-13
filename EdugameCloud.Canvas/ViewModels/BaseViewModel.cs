namespace EdugameCloud.Canvas.ViewModels
{
    using System.Threading;
    using System.Web.Mvc;

    using Esynctraining.Core.Extensions;

    using BaseController = EdugameCloud.Canvas.Controllers.BaseController;

    /// <summary>
    /// The base view model.
    /// </summary>
    public abstract class BaseViewModel
    {
        #region Fields

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        protected int? page;

        /// <summary>
        /// The controller.
        /// </summary>
        private BaseController controller;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        protected BaseViewModel(BaseController controller)
        {
            this.SetController(controller);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        protected BaseViewModel(BaseController controller, int? page)
        {
            this.SetController(controller, page);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
        /// </summary>
        protected BaseViewModel()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the action name.
        /// </summary>
        public string ActionName
        {
            get
            {
                return this.controller.With(x => x.ActionName);
            }
        }

        /// <summary>
        /// Gets or sets the back url.
        /// </summary>
        public string BackUrl { get; set; }

        /// <summary>
        /// Gets the controller name.
        /// </summary>
        public string ControllerName
        {
            get
            {
                return this.controller.With(x => x.ControllerName);
            }
        }

        /// <summary>
        /// Gets the controller settings.
        /// </summary>
        public dynamic Settings
        {
            get
            {
                return this.controller.With(x => x.Settings);
            }
        }

        /// <summary>
        /// Gets the html base url.
        /// </summary>
        public string HtmlBaseUrl
        {
            get
            {
                return this.Settings == null ? string.Empty : this.Settings.BasePath;
            }
        }

        /// <summary>
        /// Gets the model state.
        /// </summary>
        public virtual ModelStateDictionary ModelState
        {
            get
            {
                return this.controller.With(x => x.ModelState);
            }
        }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        public virtual int Page
        {
            get
            {
                return this.page ?? 1;
            }

            set
            {
                this.page = value;
            }
        }

        public virtual string SelectedLanguage
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture.Name;
            }

            set
            {
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The set controller.
        /// </summary>
        /// <param name="baseController">
        /// The base controller.
        /// </param>
        public virtual void SetController(BaseController baseController)
        {
            this.controller = baseController;
        }

        /// <summary>
        /// The set controller.
        /// </summary>
        /// <param name="baseController">
        /// The base controller.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        public virtual void SetController(BaseController baseController, int? page)
        {
            this.controller = baseController;
            this.page = page;
        }

        #endregion
    }
}