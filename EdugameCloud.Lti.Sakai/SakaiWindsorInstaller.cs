using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Sakai
{
    public sealed class SakaiWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());

            container.Register(Component.For<LTI2Api>().ImplementedBy<LTI2Api>().Named("SakaiAPI"));

            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<SakaiLmsUserService>().Named(LmsProviderEnum.Sakai.ToString()));
        }

    }

}
