namespace EdugameCloud.MVC.Wrappers
{
    using System;
    using System.Web.Mvc;

    public class HandleErrorInfoWrapper
    {
        public string ActionName { get; }

        public string ControllerName { get; }

        public Exception Exception { get; }


        public HandleErrorInfoWrapper(Exception exception, ActionResult actionResult)
        {
            var callInfo = actionResult.GetT4MVCResult();
            Exception = exception;
            ControllerName = callInfo.Controller;
            ActionName = callInfo.Action;
        }

    }

}
