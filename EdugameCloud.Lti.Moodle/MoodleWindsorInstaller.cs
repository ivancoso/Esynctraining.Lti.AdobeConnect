using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Moodle
{
    public sealed class MoodleWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());

            container.Register(Component.For<EdugameCloud.Lti.API.Moodle.IMoodleApi>().ImplementedBy<MoodleApi>().Named("IMoodleAPI"));
            container.Register(Component.For<EdugameCloud.Lti.API.Moodle.IEGCEnabledMoodleApi>().ImplementedBy<EGCEnabledMoodleApi>().Named("IEGCEnabledMoodleAPI"));

            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<MoodleLmsUserService>().Named(LmsProviderEnum.Moodle.ToString()));
        }

    }

}
