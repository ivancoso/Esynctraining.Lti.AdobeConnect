using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Lti.AdobeConnect.Caching;
using EdugameCloud.Persistence;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Utils;
using Esynctraining.Windsor;

namespace EdugameCloud.Lti.LmsUserUpdater
{
    internal static class IoCStart
    {
        public static void Init()
        {
            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);
            container.RegisterComponents();
            container.RegisterComponentsConsole();
            container.Install(new LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());
            RegisterLtiComponents(container);

            container.Register(Component.For<ILog>().ImplementedBy<ConsoleLog>());
        }

        private static void RegisterLtiComponents(WindsorContainer container)
        {
            //container.Install(new MoodleWindsorInstaller());
            //container.Install(new CanvasWindsorInstaller());
            //container.Install(new BlackboardWindsorInstaller());
            //container.Install(new BrainHoneyWindsorInstaller());
            //container.Install(new Desire2LearnWindsorInstaller());
            //container.Install(new SakaiWindsorInstaller());

            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Moodle/EdugameCloud.Lti.Moodle.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Desire2Learn/EdugameCloud.Lti.Desire2Learn.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Canvas/EdugameCloud.Lti.Canvas.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.BrainHoney/EdugameCloud.Lti.BrainHoney.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Blackboard/EdugameCloud.Lti.BlackBoard.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Sakai/EdugameCloud.Lti.Sakai.Windsor.xml"))
            );

            container.Install(new LtiWindsorInstaller());
            
            container.Register(Component.For<EdugameCloud.Lti.API.AdobeConnect.IPrincipalCache>().ImplementedBy<PrincipalCache>());            
        }

    }

}
