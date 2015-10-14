using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Canvas
{
    public sealed class CanvasWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());

            container.Register(Component.For<EdugameCloud.Lti.API.Canvas.ICanvasAPI>().ImplementedBy<CanvasAPI>().Named("ICanvasAPI"));
            container.Register(Component.For<EdugameCloud.Lti.API.Canvas.IEGCEnabledCanvasAPI>().ImplementedBy<EGCEnabledCanvasAPI>().Named("IEGCEnabledCanvasAPI"));

            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<CanvasLmsUserService>().Named(LmsProviderEnum.Canvas.ToString()));

        }

    }

}
