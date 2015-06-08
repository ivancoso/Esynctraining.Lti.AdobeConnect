using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Lti.AdobeConnect.Caching;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.API.Common;
using EdugameCloud.Lti.API.Desire2Learn;
using EdugameCloud.Lti.API.Sakai;
using EdugameCloud.Lti.BlackBoard;
using EdugameCloud.Lti.BrainHoney;
using EdugameCloud.Lti.Canvas;
using EdugameCloud.Lti.Desire2Learn;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Moodle;
using EdugameCloud.Persistence;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.LmsUserUpdater
{
    internal static class IoCStart
    {
        public static void Init()
        {
            var container = new WindsorContainer();
            IoC.Initialize(container);
            container.RegisterComponents(console: true);
            RegisterLtiComponents(container);

            container.Register(Component.For<ILog>().ImplementedBy<ConsoleLog>());
        }

        private static void RegisterLtiComponents(WindsorContainer container)
        {
            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti.Core").Pick()
                .If(Component.IsInNamespace("EdugameCloud.Lti.Core.Business.Models")).WithService.Self().Configure(c => c.LifestyleTransient()));
            container.Register(Component.For<IMeetingSetup>().ImplementedBy<MeetingSetup>());
            container.Register(Component.For<UsersSetup>().ImplementedBy<UsersSetup>());
            container.Register(Component.For<IDesire2LearnApiService>().ImplementedBy<Desire2LearnApiService>().LifestyleTransient());

            container.Register(Component.For<EdugameCloud.Lti.API.BrainHoney.IBrainHoneyScheduling>().ImplementedBy<ShedulingHelper>());
            container.Register(Component.For<EdugameCloud.Lti.API.BrainHoney.IBrainHoneyApi>().ImplementedBy<DlapAPI>().Named("IBrainHoneyApi"));

            container.Register(Component.For<EdugameCloud.Lti.API.Canvas.ICanvasAPI>().ImplementedBy<CanvasAPI>().Named("ICanvasAPI"));
            container.Register(Component.For<EdugameCloud.Lti.API.Canvas.IEGCEnabledCanvasAPI>().ImplementedBy<EGCEnabledCanvasAPI>().Named("IEGCEnabledCanvasAPI"));

            container.Register(Component.For<EdugameCloud.Lti.API.Moodle.IMoodleApi>().ImplementedBy<MoodleApi>().Named("IMoodleAPI"));
            container.Register(Component.For<EdugameCloud.Lti.API.Moodle.IEGCEnabledMoodleApi>().ImplementedBy<EGCEnabledMoodleApi>().Named("IEGCEnabledMoodleAPI"));

            container.Register(Component.For<EdugameCloud.Lti.API.BlackBoard.IBlackBoardApi>().ImplementedBy<SoapBlackBoardApi>().Named("IBlackBoardAPI"));
            container.Register(Component.For<EdugameCloud.Lti.API.BlackBoard.IEGCEnabledBlackBoardApi>().ImplementedBy<EGCEnabledBlackboardApi>().Named("IEGCEnabledBlackBoardAPI"));
            container.Register(Component.For<LTI2Api>().ImplementedBy<LTI2Api>().Named("SakaiAPI"));

            container.Register(Component.For<EdugameCloud.Lti.API.AdobeConnect.IPrincipalCache>().ImplementedBy<PrincipalCache>());

            container.Register(Component.For<LmsFactory>().ImplementedBy<LmsFactory>());
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<BlackboardLmsUserService>().Named(LmsProviderEnum.Blackboard.ToString()));
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<BrainHoneyLmsUserService>().Named(LmsProviderEnum.BrainHoney.ToString()));
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<CanvasLmsUserService>().Named(LmsProviderEnum.Canvas.ToString()));
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<Desire2LearnLmsUserService>().Named(LmsProviderEnum.Desire2Learn.ToString()));
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<MoodleLmsUserService>().Named(LmsProviderEnum.Moodle.ToString()));
            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<SakaiLmsUserService>().Named(LmsProviderEnum.Sakai.ToString()));
            container.Register(Component.For<IAdobeConnectUserService>().ImplementedBy<AdobeConnectUserService>());
            container.Register(Component.For<ISynchronizationUserService>().ImplementedBy<SynchronizationUserService>());
            container.Register(Component.For<IAdobeConnectAccountService>().ImplementedBy<AdobeConnectAccountService>());
        }

    }

}
