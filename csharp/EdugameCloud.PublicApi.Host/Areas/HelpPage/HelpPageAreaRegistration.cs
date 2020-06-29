using System.Web.Http;
using System.Web.Mvc;

namespace EdugameCloud.PublicApi.Host.Areas.HelpPage
{
    public class HelpPageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "HelpPage"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HelpPage_ApiTest",
                "help/api-test",
                new { controller = "ApiTest", action = "Index" });

            context.MapRoute(
                "HelpPage_Default",
                "help/{action}/{apiId}",
                new { controller = "Help", action = "Index", apiId = UrlParameter.Optional });
            
            HelpPageConfig.Register(GlobalConfiguration.Configuration);
        }

    }

}