namespace EdugameCloud.MVC.ViewResults
{
    using System;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    ///     Represents a user-defined content type that is the result of an action method.
    /// </summary>
    public class ContentResultWithStatusCode : ActionResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentResultWithStatusCode"/> class.
        ///     Initializes a new instance of the <see cref="T:System.Web.Mvc.HttpStatusCodeResult"/> class using a status code.
        /// </summary>
        /// <param name="statusCode">
        /// The status code.
        /// </param>
        public ContentResultWithStatusCode(int statusCode)
            : this(statusCode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentResultWithStatusCode"/> class.
        ///     Initializes a new instance of the <see cref="T:System.Web.Mvc.HttpStatusCodeResult"/> class using a status code.
        /// </summary>
        /// <param name="statusCode">
        /// The status code.
        /// </param>
        public ContentResultWithStatusCode(HttpStatusCode statusCode)
            : this(statusCode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentResultWithStatusCode"/> class.
        ///     Initializes a new instance of the <see cref="T:System.Web.Mvc.HttpStatusCodeResult"/> class using a status code
        ///     and status description.
        /// </summary>
        /// <param name="statusCode">
        /// The status code.
        /// </param>
        /// <param name="statusDescription">
        /// The status description.
        /// </param>
        public ContentResultWithStatusCode(HttpStatusCode statusCode, string statusDescription)
            : this((int)statusCode, statusDescription)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentResultWithStatusCode"/> class.
        ///     Initializes a new instance of the <see cref="T:System.Web.Mvc.HttpStatusCodeResult"/> class using a status code
        ///     and status description.
        /// </summary>
        /// <param name="statusCode">
        /// The status code.
        /// </param>
        /// <param name="statusDescription">
        /// The status description.
        /// </param>
        public ContentResultWithStatusCode(int statusCode, string statusDescription)
        {
            this.StatusCode = statusCode;
            this.StatusDescription = statusDescription;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the content.
        /// </summary>
        /// <returns>
        ///     The content.
        /// </returns>
        public string Content { get; set; }

        /// <summary>
        ///     Gets or sets the content encoding.
        /// </summary>
        /// <returns>
        ///     The content encoding.
        /// </returns>
        public Encoding ContentEncoding { get; set; }

        /// <summary>
        ///     Gets or sets the type of the content.
        /// </summary>
        /// <returns>
        ///     The type of the content.
        /// </returns>
        public string ContentType { get; set; }

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
        /// The context within which the result is executed.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="context"/> parameter is null.
        /// </exception>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
            {
                response.ContentType = this.ContentType;
            }

            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            context.HttpContext.Response.StatusCode = this.StatusCode;

            if (this.StatusDescription != null)
            {
                context.HttpContext.Response.StatusDescription = this.StatusDescription;
            }

            if (this.Content == null)
            {
                return;
            }

            response.Write(this.Content);
        }

        #endregion
    }
}