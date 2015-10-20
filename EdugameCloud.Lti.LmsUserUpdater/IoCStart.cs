using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Lti.AdobeConnect.Caching;
using EdugameCloud.Lti.Sakai;
using EdugameCloud.Lti.BlackBoard;
using EdugameCloud.Lti.BrainHoney;
using EdugameCloud.Lti.Canvas;
using EdugameCloud.Lti.Desire2Learn;
using EdugameCloud.Lti.Moodle;
using EdugameCloud.Persistence;
using Esynctraining.Core.Utils;
using Esynctraining.CastleLog4Net;

namespace EdugameCloud.Lti.LmsUserUpdater
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
            container.Install(new MoodleWindsorInstaller());
            container.Install(new CanvasWindsorInstaller());
            container.Install(new BlackboardWindsorInstaller());
            container.Install(new BrainHoneyWindsorInstaller());
            container.Install(new Desire2LearnWindsorInstaller());
            container.Install(new SakaiWindsorInstaller());
            container.Install(new LtiWindsorInstaller());
            
            container.Register(Component.For<EdugameCloud.Lti.API.AdobeConnect.IPrincipalCache>().ImplementedBy<PrincipalCache>());            
        }

    }

}
