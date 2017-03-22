using System;
using System.Web;
using System.Web.Mvc;
using Castle.Core.Logging;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti
{
    internal class LmsAuthorizeBaseAttribute : ActionFilterAttribute
    {
        private static readonly string HeaderName = "Authorization";
        private static readonly string ltiAuthScheme = "lti ";
        private static readonly string apiAuthScheme = "ltiapi ";

        private readonly LmsUserSessionModel _userSessionModel;
        private readonly ILogger _logger;

        private LanguageModel LanguageModel
        {
            get { return IoC.Resolve<LanguageModel>(); }
        }

        
        public LmsAuthorizeBaseAttribute()
        {
            _userSessionModel = IoC.Resolve<LmsUserSessionModel>();
            _logger = IoC.Resolve<ILogger>();
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string mode;
            Guid sessionKey = FetchToken(filterContext.HttpContext.Request, out mode);

            if (sessionKey != Guid.Empty)
            {
                LmsUserSession session = GetReadOnlySession(sessionKey);
                if (session == null)
                {
                    filterContext.Result = new JsonNetResult
                    {
                        Data = OperationResult.Error(Resources.Messages.SessionTimeOut),
                    };
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
                        filterContext.ActionParameters["session"] = session;
                    }
                }
            }
            else
            {
                filterContext.Result = new JsonNetResult
                {
                    Data = OperationResult.Error("Necessary arguments were not provided."),
                };
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

        private static Guid FetchToken(HttpRequestBase req, out string mode)
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

    internal sealed class LmsAuthorizeAttribute : LmsAuthorizeBaseAttribute
    {
        private readonly LmsRoleService _lmsRoleService;


        public LmsAuthorizeAttribute()
        {
            _lmsRoleService = IoC.Resolve<LmsRoleService>();
        }


        protected override bool IsAllowed(LmsUserSession session, out ActionResult notAllowedResult)
        {
            var isTeacher = _lmsRoleService.IsTeacher(session.LtiSession.LtiParam);

            if (!isTeacher)
            {
                notAllowedResult = new JsonNetResult
                {
                    Data = OperationResult.Error("Operation is not enabled."),
                };
                return false;
            }
            else
            {
                notAllowedResult = null;
                return true;
            }
        }

    }

}