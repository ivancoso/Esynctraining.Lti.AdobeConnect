using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.BlackBoard
{
    public sealed class BlackboardWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());

            container.Register(Component.For<EdugameCloud.Lti.API.BlackBoard.IBlackBoardApi>().ImplementedBy<SoapBlackBoardApi>().Named("IBlackBoardAPI"));
            container.Register(Component.For<EdugameCloud.Lti.API.BlackBoard.IEGCEnabledBlackBoardApi>().ImplementedBy<EGCEnabledBlackboardApi>().Named("IEGCEnabledBlackBoardAPI"));

            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<BlackboardLmsUserService>().Named(LmsProviderEnum.Blackboard.ToString()));
        }

    }

}
