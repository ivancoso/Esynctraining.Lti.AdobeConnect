using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Moodle;
using Esynctraining.Lti.Lms.Moodle;

namespace EdugameCloud.Lti.Moodle
{
    public sealed class MoodleWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn(typeof(ILmsAPI)).WithServiceSelf().LifestyleTransient());

            container.Register(Component.For<MoodleApi>().ImplementedBy<MoodleApi>().Named("IMoodleAPI"));
            container.Register(Component.For<IEGCEnabledMoodleApi>().ImplementedBy<EGCEnabledMoodleApi>().Named("IEGCEnabledMoodleAPI"));

            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<MoodleLmsUserService>().Named(LmsProviderEnum.Moodle.ToString()));
        }

    }

}
