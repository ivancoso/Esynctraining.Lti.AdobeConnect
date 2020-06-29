namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Web.Mvc;

    using EdugameCloud.MVC.Wrappers;
    
    [HandleError]
    public partial class ErrorController : Controller
    {
        [HttpGet]
        public virtual ActionResult Index(Exception exc)
        {
            return this.View(
                EdugameCloudT4.Shared.Views.Error,
                new HandleErrorInfoWrapper(exc, EdugameCloudT4.Error.Index()));
        }
        
    }

}