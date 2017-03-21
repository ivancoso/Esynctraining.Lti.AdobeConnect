using System;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.Api.Controllers;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EdugameCloud.Lti.Api.Filters
{
    internal class LmsAuthorizeBaseAttribute : ActionFilterAttribute
    {
        private static readonly string HeaderName = "Authorization";
        private static readonly string ltiAuthScheme = "lti ";
        private static readonly string apiAuthScheme = "ltiapi ";


        private readonly LmsUserSessionModel _userSessionModel;
        private readonly LmsCompanyModel _licenseModel;
        private readonly LmsRoleService _lmsRoleService;
        private readonly ILogger _logger;

        private LanguageModel LanguageModel => IoC.Resolve<LanguageModel>();


        public LmsAuthorizeBaseAttribute()
        {
            //_userSessionModel = IoC.Resolve<LmsUserSessionModel>();
            //_lmsRoleService = IoC.Resolve<LmsRoleService>();
            //_licenseModel = IoC.Resolve<LmsCompanyModel>();
            //_logger = IoC.Resolve<ILogger>();
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string mode;
            Guid sessionKey = FetchToken(filterContext.HttpContext.Request, out mode);
            if (sessionKey != Guid.Empty)
            {
                if (mode == ltiAuthScheme)
                {
                    // TODO: try\catch?
                    LmsUserSession session = GetReadOnlySession(sessionKey);

                    if (session == null)
                    {
                        filterContext.Result = new JsonResult(OperationResult.Error(Resources.Messages.SessionTimeOut));
                    }
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
                            api.LmsCompany = session.LmsCompany;
                            api.CourseId = session.LmsCourseId;
                        }
                    }
                }
                else
                {
                    LmsCompany license = GetLicense(sessionKey);

                    if (license == null)
                    {
                        // TODO: better msg
                        filterContext.Result = new JsonResult(OperationResult.Error(Resources.Messages.SessionTimeOut));
                    }
                    else
                    {
                        // TODO: ENABLED API FLAG!! company license level + action level

                        //ActionResult notAllowedResult;
                        //var allowed = IsAllowed(session, out notAllowedResult);

                        //if (!allowed)
                        //{
                        //    filterContext.Result = notAllowedResult;
                        //}
                        //else
                        {
                            var api = filterContext.Controller as BaseApiController;
                            api.LmsCompany = license;
                            // HACK another header??
                            //api.CourseId = session.LmsCourseId; // TODO: !!!
                        }
                    }
                }
            }
            else
            {
                filterContext.Result = new JsonResult(OperationResult.Error("Necessary arguments were not provided."));
            }
            base.OnActionExecuting(filterContext);
        }

        
        protected virtual bool IsAllowed(LmsUserSession session, out ActionResult notAllowedResult)
        {
            notAllowedResult = null;
            return true;
        }

        protected LmsUserSession GetReadOnlySession(Guid key)
        {
            var session = _userSessionModel.GetByIdWithRelated(key).Value;
            if (session == null)
            {
                _logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                return null;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(session.LmsCompany.LanguageId).TwoLetterCode);

            return session;
        }

        protected LmsCompany GetLicense(Guid key)
        {
            var license = _licenseModel.GetOneByConsumerKey(key.ToString()).Value;
            if (license == null)
            {
                _logger.WarnFormat("LmsCompany not found. Key: {0}.", key);
                return null;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(license.LanguageId).TwoLetterCode);

            return license;
        }


        private static Guid FetchToken(HttpRequest req, out string mode)
        {
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
                string token = authHeader.Substring(apiAuthScheme.Length).Trim();

                Guid uid;
                if (Guid.TryParse(token, out uid))
                {
                    mode = apiAuthScheme;
                    return uid;
                }
            }

            mode = null;
            return Guid.Empty;
        }

    }

}
