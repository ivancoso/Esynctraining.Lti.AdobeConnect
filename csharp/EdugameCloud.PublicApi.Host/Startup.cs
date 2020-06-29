using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(EdugameCloud.PublicApi.Host.Startup))]

namespace EdugameCloud.PublicApi.Host
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}