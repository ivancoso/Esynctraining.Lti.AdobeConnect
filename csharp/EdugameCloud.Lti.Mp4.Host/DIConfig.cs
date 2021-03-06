﻿using System.Collections.Specialized;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Persistence;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Providers;
using Esynctraining.Mp4Service.Tasks.Client;
using Esynctraining.Windsor;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace EdugameCloud.Lti.Mp4.Host
{
    internal static class DIConfig
    {
        public static WindsorContainer ConfigureWindsor(IConfiguration Configuration)
        {
            var container = new WindsorContainer();
            
            container
                .Register(Component.For<ISessionSource>().ImplementedBy<AspNetCoreNHibernateSessionSource>().LifeStyle.Scoped())
                .RegisterComponents();

            WindsorIoC.Initialize(container);

            var configurationSection = Configuration.GetSection("AppSettings");
            var collection = new NameValueCollection();
            foreach (var appSetting in configurationSection.GetChildren())
            {
                collection.Add(appSetting.Key, appSetting.Value);
            }

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                .DynamicParameters((k, d) => d.Add("collection", collection))
                .LifeStyle.Singleton);

            container.Install(
                new LoggerWindsorInstaller(),
                new EdugameCloud.Core.Logging.LoggerWindsorInstaller());

            RegisterLtiComponents(container);
            RegisterLocalComponents(container);

            return container;
        }
        
        private static void RegisterLtiComponents(WindsorContainer container)
        {
            container.Install(
                new LtiWindsorInstaller(),
                new TelephonyWindsorInstaller());

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti")
                .BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());

        }

        private static void RegisterLocalComponents(IWindsorContainer container)
        {
            dynamic settings = Esynctraining.Core.Utils.IoC.Resolve<ApplicationSettingsProvider>() as dynamic;

            string mp4ApiBaseAddress = settings.MP4ApiBaseAddress as string;
            container.Register(Component.For<TaskClient>()
                   .ImplementedBy<TaskClient>()
                   .DependsOn(Dependency.OnValue("baseApiAddress", mp4ApiBaseAddress))
                   .LifeStyle.Singleton);

            container.Register(Classes.FromAssemblyNamed("Esynctraining.Mp4Service.Tasks.Client").Pick()
                .If(type => type.Name.EndsWith("Model"))
                .WithService.Self().Configure(c => c.LifestyleTransient()));

            container.Register(Component.For<IMp4LinkBuilder>()
                .UsingFactoryMethod(() => new AdobeConnectFileAccessLinkBuilder(settings.BaseUrl.TrimEnd('/') + "/" + settings.Mp4FileAccess_Mp4.TrimStart('/')))
                .LifestyleSingleton());

            container.Register(Component.For<IVttLinkBuilder>()
                .UsingFactoryMethod(() => new AdobeConnectFileAccessLinkBuilder(settings.BaseUrl.TrimEnd('/') + "/" + settings.Mp4FileAccess_Vtt.TrimStart('/')))
                .LifestyleSingleton());
        }

    }

}
