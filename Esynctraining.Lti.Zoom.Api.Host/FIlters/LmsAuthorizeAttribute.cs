using System;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Api.Host.Controllers;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
//using EdugameCloud.Lti.Api.Controllers;

namespace Esynctraining.Lti.Zoom.Api.Host.FIlters
{
    public class LmsAuthorizeBaseAttribute : ActionFilterAttribute, Zoom.Api.Host.FIlters.IApiEnableAttribute
    {
        private static readonly string HeaderName = "Authorization";
        private static readonly string ltiAuthScheme = "lti ";
        private static readonly string apiAuthScheme = "ltiapi ";

        public bool ApiCallEnabled { get; set; }

        public string FeatureName { get; set; }

        // TODO: DI
        public LmsAuthorizeBaseAttribute()
        {

        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string mode;
            Guid token = FetchToken(filterContext.HttpContext.Request, out mode);
            if (token != Guid.Empty)
            {
                if (mode == ltiAuthScheme)
                {
                    // TODO: try\catch?
                    var service = (UserSessionService)filterContext.HttpContext.RequestServices.GetService(typeof(UserSessionService));
                    LmsUserSession session = Task.Run(async () => await GetReadOnlySession(service, token)).Result;

                    if (session == null)
                    {
                        filterContext.Result = new JsonResult(OperationResult.Error("SessionTimeOut"));
                    }
                    //else if (!string.IsNullOrWhiteSpace(FeatureName) && !session.LmsLicense.GetSetting<bool>(FeatureName))
                    //{
                    //    filterContext.Result = new ObjectResult(OperationResult.Error("Operation is not enabled."));
                    //}
                    else
                    {
                        ActionResult notAllowedResult;
                        var allowed = IsAllowed(session, out notAllowedResult);
                        if (!allowed)
                        {
                            filterContext.Result = notAllowedResult;
                        }
                        else
                        {
                            var api = filterContext.Controller as BaseApiController;
                            api.Session = session;
                            var licenseService = (ILmsLicenseService)filterContext.HttpContext.RequestServices.GetService(typeof(ILmsLicenseService));
                            api.LmsLicense = licenseService.GetLicense(session.LicenseId);
                            api.CourseId = session.CourseId;
                            if (!string.IsNullOrEmpty(session.SessionData))
                            {
                                var deserializer = (IJsonDeserializer)filterContext.HttpContext.RequestServices.GetService(typeof(IJsonDeserializer));
                                api.Param = deserializer.JsonDeserialize<LtiParamDTO>(session.SessionData);
                            }
                        }
                    }
                }
                //else
                //{
                //    if (!ApiCallEnabled)
                //    {
                //        filterContext.Result = new JsonResult(OperationResult.Error("External calls are not permitted"));
                //    }
                //    else
                //    {
                //        var service = filterContext.HttpContext.RequestServices.GetService(typeof(UserSessionService));
                //        LmsLicense license = GetLicense(token);
                //        if (license == null)
                //        {
                //            // TODO: better msg
                //            filterContext.Result = new JsonResult(OperationResult.Error("SessionTimeOut"));
                //        }
                //        else if (!string.IsNullOrWhiteSpace(FeatureName) && !license.GetSetting<bool>(FeatureName))
                //        {
                //            filterContext.Result = new ObjectResult(OperationResult.Error("Operation is not enabled."));
                //        }
                //        else
                //        {
                //            //ActionResult notAllowedResult;
                //            //var allowed = IsAllowed(session, out notAllowedResult);

                //            //if (!allowed)
                //            //{
                //            //    filterContext.Result = notAllowedResult;
                //            //}
                //            //else
                //            {
                //                var api = filterContext.Controller as BaseApiController;
                //                api.LmsCompany = license;
                //                api.CourseId = FetchApiCourseId(filterContext.HttpContext.Request);
                //            }
                //        }
                //    }
                //}
            }
            else
            {
                filterContext.Result = new JsonResult(OperationResult.Error("Necessary Authorization arguments were not provided."));
            }
            base.OnActionExecuting(filterContext);
        }


        protected virtual bool IsAllowed(LmsUserSession session, out ActionResult notAllowedResult)
        {
            notAllowedResult = null;
            return true;
        }

        protected async Task<LmsUserSession> GetReadOnlySession(UserSessionService service, Guid key)
        {
            var session = await service.GetSession(key);
            if (session == null)
            {
                //Logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                return null;
            }

            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(session.LmsCompany.LanguageId).TwoLetterCode);

            return session;
        }

        //private LmsLicense GetLicense(int id)
        //{
        //    var license = LicenseModel.GetOneByConsumerKey(key.ToString()).Value;
        //    if (license == null)
        //    {
        //        Logger.WarnFormat("LmsCompany not found. Key: {0}.", key);
        //        return null;
        //    }

        //    //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(license.LanguageId).TwoLetterCode);

        //    return license;
        //}


        private static Guid FetchToken(HttpRequest req, out string mode)
        {
            mode = null;
            string authHeader = req.Headers[HeaderName];

            if ((authHeader != null) && authHeader.StartsWith(ltiAuthScheme, StringComparison.OrdinalIgnoreCase))
            {
                string token = authHeader.Substring(ltiAuthScheme.Length).Trim();

                Guid uid;
                if (Guid.TryParse(token, out uid))
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

                Guid uid;
                if (Guid.TryParse(token, out uid))
                {
                    mode = apiAuthScheme;
                    return uid;
                }
            }

            return Guid.Empty;
        }

        private static string FetchApiCourseId(HttpRequest req)
        {
            string authHeader = req.Headers[HeaderName];
            string token = authHeader.Substring(apiAuthScheme.Length)
                    .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)[1];

            return token;

            //return SakaiCourseNumberTrick.GetHashCode(token);

            //string ltiUrl = (IoC.Resolve<ApplicationSettingsProvider>() as dynamic).LtiHostUrl as string;
            //var url = new Uri(new Uri(new Uri(ltiUrl), "hash/"), WebUtility.UrlEncode(token));
            //try
            //{
            //    string value;
            //    using (var web = new WebClient())
            //    {
            //        value = web.DownloadString(url);
            //    }
            //    return int.Parse(value);
            //}
            //catch (Exception ex)
            //{
            //    throw new InvalidOperationException("Error fetching GetHashCode for Sakai course id", ex);
            //}
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
