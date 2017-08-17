using Castle.Core.Resource;
using Castle.Windsor;
using EdugameCloud.Persistence;
using Esynctraining.CastleLog4Net;
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
        }

        private static void RegisterLtiComponents(WindsorContainer container)
        {
            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Moodle/EdugameCloud.Lti.Moodle.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Desire2Learn/EdugameCloud.Lti.Desire2Learn.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Canvas/EdugameCloud.Lti.Canvas.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.AgilixBuzz/EdugameCloud.Lti.AgilixBuzz.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Blackboard/EdugameCloud.Lti.BlackBoard.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Sakai/EdugameCloud.Lti.Sakai.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Schoology/EdugameCloud.Lti.Schoology.Windsor.xml")),
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Haiku/EdugameCloud.Lti.Haiku.Windsor.xml"))
            );

            container.Install(new LtiWindsorInstaller());
        }

    }

}
