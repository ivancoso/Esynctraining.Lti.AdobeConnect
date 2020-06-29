using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EdugameCloud.BuildManager.Startup))]
namespace EdugameCloud.BuildManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
