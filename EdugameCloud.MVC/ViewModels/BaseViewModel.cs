namespace EdugameCloud.MVC.ViewModels
{
    using System;
    using Esynctraining.Core.Extensions;
    using BaseController = EdugameCloud.MVC.Controllers.BaseController;

    /// <summary>
    /// The base view model.
    /// </summary>
    public abstract class BaseViewModel
    {
        private BaseController _controller;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        protected BaseViewModel(BaseController controller)
        {
            SetController(controller);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
        /// </summary>
        protected BaseViewModel()
        {
        }

        #endregion

        #region Public Properties

        public string ActionName
        {
            get { return _controller.With(x => x.ActionName); }
        }

        public string ControllerName
        {
            get { return _controller.With(x => x.ControllerName); }
        }

        public dynamic Settings
        {
            get { return _controller.With(x => x.Settings); }
        }

        public string HtmlBaseUrl
        {
            get
            {
                if (Settings == null)
                    return string.Empty;

                var protoHeader = _controller.Request.Headers["X-Forwarded-Proto"];
                bool isHttps = (protoHeader != null) && protoHeader.IndexOf("https", StringComparison.OrdinalIgnoreCase) != -1;

                string schema = _controller.Request.Url.Scheme;
                if (isHttps)
                    schema = "https";
                var uriBuilder = new UriBuilder(Settings.BasePath)
                {
                    Scheme = schema,
                    Port = -1, // TRICK: don't add port to url
                };
                return uriBuilder.ToString();
            }
        }

        #endregion

        public virtual void SetController(BaseController baseController)
        {
            _controller = baseController;
        }

    }

}