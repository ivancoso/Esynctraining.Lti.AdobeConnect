 namespace EdugameCloud.MVC.Attributes
{
    using System.Reflection;
    using System.Web.Mvc;

    /// <summary>
    /// The ajax attribute.
    /// </summary>
    public class AjaxAttribute : ActionMethodSelectorAttribute
    {
        /// <summary>
        /// The ajax.
        /// </summary>
        private readonly bool ajax;

        /// <summary>
        /// Initializes a new instance of the <see cref="AjaxAttribute"/> class. 
        /// </summary>
        /// <param name="ajax">
        /// Flag if ajax is allowed
        /// </param>
        public AjaxAttribute(bool ajax)
        {
            this.ajax = ajax;
        }

        #region Overrides of ActionMethodSelectorAttribute

        /// <summary>
        /// The is valid for request.
        /// </summary>
        /// <param name="controllerContext">
        /// The controller context.
        /// </param>
        /// <param name="methodInfo">
        /// The method info.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return this.ajax == controllerContext.HttpContext.Request.IsAjaxRequest();
        }

        #endregion
    }
}
