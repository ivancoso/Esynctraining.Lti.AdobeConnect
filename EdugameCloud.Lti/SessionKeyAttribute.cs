//using System;
//using System.Web.Mvc;
//using Castle.Core.Logging;
//using EdugameCloud.Core.Business.Models;
//using EdugameCloud.Lti.Core.Business.Models;
//using EdugameCloud.Lti.Core.Constants;
//using EdugameCloud.Lti.Domain.Entities;
//using Esynctraining.Core.Domain;
//using Esynctraining.Core.Utils;

//namespace EdugameCloud.Lti
//{
//    public class SessionKeyAttribute : ActionFilterAttribute
//    {
//        private readonly LmsUserSessionModel _userSessionModel;
//        private readonly ILogger _logger;


//        private LanguageModel LanguageModel
//        {
//            get { return IoC.Resolve<LanguageModel>(); }
//        }


//        public SessionKeyAttribute()
//        {
//            _userSessionModel = IoC.Resolve<LmsUserSessionModel>();
//            _logger = IoC.Resolve<ILogger>();
//        }


//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            var sessionKey = filterContext.RouteData.Values["lmsProviderName"] as string;
//            if (sessionKey != null)
//            {
//                //int id;
//                //SlugIds.TryGetValue(slug, out id);
//                //filterContext.ActionParameters["id"] = id;
//                LmsCompany lmsCompany = null;
//                var session = GetReadOnlySession(sessionKey);

//                if (session == null)
//                {
//                    filterContext.Result = new JsonNetResult
//                    {
//                        Data = OperationResult.Error(Resources.Messages.SessionTimeOut),
//                    };
//                }
//                else
//                {
//                    lmsCompany = session.LmsCompany;

//                    if (!lmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
//                    {
//                        filterContext.Result = new JsonNetResult
//                        {
//                            Data = OperationResult.Error("Operation is not enabled."),
//                        };
//                    }
//                    else
//                    {
//                        filterContext.ActionParameters["session"] = session;
//                    }
//                }
//            }
//            base.OnActionExecuting(filterContext);
//        }

//        protected LmsUserSession GetReadOnlySession(string key)
//        {
//            Guid uid;
//            var session = Guid.TryParse(key, out uid) ? _userSessionModel.GetByIdWithRelated(uid).Value : null;

//            if (session == null)
//            {
//                _logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
//                return null;
//            }

//            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(session.LmsCompany.LanguageId).TwoLetterCode);

//            return session;
//        }

//    }

//}
