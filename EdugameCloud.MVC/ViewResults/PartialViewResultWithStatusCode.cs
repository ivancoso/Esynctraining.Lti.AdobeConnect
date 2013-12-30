namespace EdugameCloud.MVC.ViewResults
{
    using System;
    using System.Net;
    using System.Web.Mvc;

    /// <summary>
    /// The partial view result with status code.
    /// </summary>
    public class PartialViewResultWithStatusCode : PartialViewResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialViewResultWithStatusCode"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Web.Mvc.HttpStatusCodeResult"/> class using a status code.
        /// </summary>
        /// <param name="statusCode">
        /// The status code.
        /// </param>
        public PartialViewResultWithStatusCode(int statusCode)
            : this(statusCode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialViewResultWithStatusCode"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Web.Mvc.HttpStatusCodeResult"/> class using a status code.
        /// </summary>
        /// <param name="statusCode">
        /// The status code.
        /// </param>
        public PartialViewResultWithStatusCode(HttpStatusCode statusCode)
            : this(statusCode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialViewResultWithStatusCode"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Web.Mvc.HttpStatusCodeResult"/> class using a status code
        ///     and status description.
        /// </summary>
        /// <param name="statusCode">
        /// The status code.
        /// </param>
        /// <param name="statusDescription">
        /// The status description.
        /// </param>
        public PartialViewResultWithStatusCode(HttpStatusCode statusCode, string statusDescription)
            : this((int)statusCode, statusDescription)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialViewResultWithStatusCode"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Web.Mvc.HttpStatusCodeResult"/> class using a status code
        ///     and status description.
        /// </summary>
        /// <param name="statusCode">
        /// The status code.
        /// </param>
        /// <param name="statusDescription">
        /// The status description.
        /// </param>
        public PartialViewResultWithStatusCode(int statusCode, string statusDescription)
        {
            this.StatusCode = statusCode;
            this.StatusDescription = statusDescription;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the HTTP status code.
        /// </summary>
        /// <returns>
        ///     The HTTP status code.
        /// </returns>
        public int StatusCode { get; private set; }

        /// <summary>
        ///     Gets the HTTP status description.
        /// </summary>
        /// <returns>
        ///     the HTTP status description.
        /// </returns>
        public string StatusDescription { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from the
        ///     <see cref="T:System.Web.Mvc.ActionResult"/> class.
        /// </summary>
        /// <param name="context">
        /// The context in which the result is executed. The context information includes the controller,
        ///     HTTP content, request context, and route data.
        /// </param>
        public override void ExecuteResult(ControllerContext context)
        {
            base.ExecuteResult(context);
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.HttpContext.Response.StatusCode = this.StatusCode;
            if (this.StatusDescription == null)
            {
                return;
            }

            context.HttpContext.Response.StatusDescription = this.StatusDescription;
        }

        #endregion
    }
}