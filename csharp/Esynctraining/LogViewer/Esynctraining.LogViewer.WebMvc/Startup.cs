using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Esynctraining.LogViewer.WebMvc.Startup))]
namespace Esynctraining.LogViewer.WebMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

    }

}
