﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Desire2Learn;

namespace EdugameCloud.Lti.Desire2Learn
{
    public sealed class Desire2LearnWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IDesire2LearnApiService>().ImplementedBy<Desire2LearnApiService>().LifestyleTransient());

            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<Desire2LearnLmsUserService>().Named(LmsProviderEnum.Brightspace.ToString()));

            container.Register(Component.For<LmsUserServiceBase>().ImplementedBy<Desire2LearnLmsUserServiceSync>().Named(LmsProviderEnum.Brightspace.ToString() + "_Sync"));
        }

    }
}
