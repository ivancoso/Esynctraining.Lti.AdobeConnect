using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Persistence;
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
            RegisterLtiComponents(container);

            container.Register(Component.For<ILog>().ImplementedBy<Log>());
        }

        private static void RegisterLtiComponents(WindsorContainer container)
        {
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti.Core").Pick()
                .If(Component.IsInNamespace("EdugameCloud.Lti.Core.Business.Models")).WithService.Self().Configure(c => c.LifestyleTransient()));

            container.Register(Component.For<MeetingSetup>().ImplementedBy<MeetingSetup>());
            container.Register(Component.For<UsersSetup>().ImplementedBy<UsersSetup>());
        }

    }

}
