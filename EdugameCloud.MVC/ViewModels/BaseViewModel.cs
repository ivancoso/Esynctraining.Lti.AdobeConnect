namespace EdugameCloud.MVC.ViewModels
{
    using System;
    using System.Threading;
    using System.Web.Mvc;
    using EdugameCloud.MVC.Attributes;
    using Esynctraining.Core.Extensions;
    using BaseController = EdugameCloud.MVC.Controllers.BaseController;

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
                if (this.Settings == null)
                    return string.Empty;

                var uriBuilder = new UriBuilder(this.Settings.BasePath)
                {
                    Scheme = controller.Request.Url.Scheme,
                    Port = -1, // TRICK: don't add port to url
                };
                return uriBuilder.ToString();
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

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        [LocalizedDisplayName("Language", ResourceName = "Shared")]
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