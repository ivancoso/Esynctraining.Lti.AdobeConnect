namespace EdugameCloud.MVC.Wrappers
{
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// The handle error info wrapper.
    /// </summary>
    public class HandleErrorInfoWrapper
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HandleErrorInfoWrapper"/> class.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="actionResult">
        /// The action result.
        /// </param>
        public HandleErrorInfoWrapper(Exception exception, ActionResult actionResult)
        {
            var callInfo = actionResult.GetT4MVCResult();
            this.Exception = exception;
            this.ControllerName = callInfo.Controller;
            this.ActionName = callInfo.Action;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the action name.
        /// </summary>
        public string ActionName { get; private set; }

        /// <summary>
        /// Gets the controller name.
        /// </summary>
        public string ControllerName { get; private set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Exception { get; private set; }

        #endregion
    }
}
