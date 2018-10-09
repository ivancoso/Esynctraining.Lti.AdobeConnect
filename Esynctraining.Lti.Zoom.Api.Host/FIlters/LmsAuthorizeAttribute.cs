using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Api.Host.Controllers;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Common;
using Esynctraining.Lti.Zoom.Common.Services;

namespace Esynctraining.Lti.Zoom.Api.Host.FIlters
{
    public class LmsAuthorizeBaseAttribute : ActionFilterAttribute
    {
        private static readonly string HeaderName = "Authorization";
        private static readonly string ltiAuthScheme = "lti ";
        private static readonly string apiAuthScheme = "ltiapi ";

        public bool ApiCallEnabled { get; set; }

        public string FeatureName { get; set; }

        public LmsAuthorizeBaseAttribute(){ }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Guid token = FetchToken(context.HttpContext.Request, out string mode);
            if (token != Guid.Empty)
            {
                if (mode == ltiAuthScheme)
                {
                    // TODO: try\catch?
                    var service = (UserSessionService)context.HttpContext.RequestServices.GetService(typeof(UserSessionService));
                    LmsUserSession session = await GetReadOnlySession(service, token);
                    
                    
                    if (session == null)
                    {
                        context.Result = new JsonResult(OperationResult.Error("SessionTimeOut"));
                    }
                    else
                    {
                        var allowed = IsAllowed(session, out ActionResult notAllowedResult);
                        if (!allowed)
                        {
                            context.Result = notAllowedResult;
                        }
                        else
                        {
                            var api = context.Controller as BaseApiController;
                            
                            var licenseService = (ILmsLicenseService)context.HttpContext.RequestServices.GetService(typeof(ILmsLicenseService));
                            var lmsUserServiceFactory = (LmsUserServiceFactory)context.HttpContext.RequestServices.GetService(typeof(LmsUserServiceFactory));

                            api.Session = session;
                            api.LmsLicense = await licenseService.GetLicense(session.LicenseKey);
                            api.CourseId = session.CourseId;
                            api.User = await GetLmsUser(api, lmsUserServiceFactory);
                            api.Param = GetParam(session.SessionData, context.HttpContext);
                        }
                    }
                }
            }
            else
            {
                context.Result = new JsonResult(OperationResult.Error("Necessary Authorization arguments were not provided."));
            }

            await base.OnActionExecutionAsync(context, next);
        }

        private async Task<LmsUserDTO> GetLmsUser(BaseApiController controller, LmsUserServiceFactory lmsUserServiceFactory)
        {
            var lmsUserService = lmsUserServiceFactory.GetUserService(controller.LmsLicense.ProductId);
            var user = await lmsUserService.GetUser(controller.LmsLicense.GetLMSSettings(controller.Session), controller.Session.LmsUserId, controller.CourseId);
            return user.Data;
        }

        private LtiParamDTO GetParam(string sessionData, HttpContext httpContext)
        {
            if (string.IsNullOrEmpty(sessionData))
                return null;

            var deserializer = (IJsonDeserializer)httpContext.RequestServices.GetService(typeof(IJsonDeserializer));
            return deserializer.JsonDeserialize<LtiParamDTO>(sessionData);
        }

        protected virtual bool IsAllowed(LmsUserSession session, out ActionResult notAllowedResult)
        {
            notAllowedResult = null;
            return true;
        }

        protected async Task<LmsUserSession> GetReadOnlySession(UserSessionService service, Guid key)
        {
            var session = await service.GetSession(key);
            return session;
        }

        private static Guid FetchToken(HttpRequest req, out string mode)
        {
            mode = null;
            string authHeader = req.Headers[HeaderName];

            if ((authHeader != null) && authHeader.StartsWith(ltiAuthScheme, StringComparison.OrdinalIgnoreCase))
            {
                string token = authHeader.Substring(ltiAuthScheme.Length).Trim();

                if (Guid.TryParse(token, out Guid uid))
                {
                    mode = ltiAuthScheme;
                    return uid;
                }
            }

            if ((authHeader != null) && authHeader.StartsWith(apiAuthScheme, StringComparison.OrdinalIgnoreCase))
            {
                var parts = authHeader.Substring(apiAuthScheme.Length).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    return Guid.Empty;

                string token = parts[0];
                if (Guid.TryParse(token, out Guid uid))
                {
                    mode = apiAuthScheme;
                    return uid;
                }
            }

            return Guid.Empty;
        }

    }

    public class QueryStringLmsAuthorizeAttribute : LmsAuthorizeBaseAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Query.TryGetValue("session", out StringValues token))
            {
                filterContext.HttpContext.Request.Headers.Add("Authorization", $"lti {token}");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
