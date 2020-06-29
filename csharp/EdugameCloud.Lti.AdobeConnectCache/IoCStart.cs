using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Lti.AdobeConnect.Caching;
using EdugameCloud.Persistence;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.AdobeConnectCache
{
    internal static class IoCStart
    {
        public static void Init()
        {
            var container = new WindsorContainer();
            IoC.Initialize(container);
            container.RegisterComponents(console: true);
            container.Install(new LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());
            RegisterLtiComponents(container);

            container.Register(Component.For<ILog>().ImplementedBy<ConsoleLog>());
        }


        private static void RegisterLtiComponents(WindsorContainer container)
        {
            container.Install(new LtiWindsorInstaller());
        }

    }

}
