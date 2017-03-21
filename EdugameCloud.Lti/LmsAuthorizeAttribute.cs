using System;
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
            var sessionKey = filterContext.Controller.ValueProvider.GetValue("lmsProviderName")?.AttemptedValue;
            if (sessionKey != null)
            {
                //int id;
                //SlugIds.TryGetValue(slug, out id);
                //filterContext.ActionParameters["id"] = id;
                //LmsCompany lmsCompany = null;
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

        protected LmsUserSession GetReadOnlySession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? _userSessionModel.GetByIdWithRelated(uid).Value : null;

            if (session == null)
            {
                _logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                return null;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(session.LmsCompany.LanguageId).TwoLetterCode);

            return session;
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