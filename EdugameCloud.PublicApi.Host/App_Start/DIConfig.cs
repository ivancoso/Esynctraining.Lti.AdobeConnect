using System;
using System.Configuration;
using System.Reflection;
using System.Web.Configuration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdugameCloud.Core;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Persistence;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Providers;

namespace EdugameCloud.PublicApi
{
    public class DIConfig
    {
        public static void RegisterComponents(IWindsorContainer container)
        {
            Type egcCoremodelsType = typeof(ACSessionModel);
            Assembly egcCoreAssembly = egcCoremodelsType.Assembly;

            //container.Register(Component.For<AuthenticationModel>().LifeStyle.PerWcfOperation());
            
            container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.PerWebRequest);
            
            container.Install(new CoreWindsorInstaller());
            container.Install(new NHibernateWindsorInstaller());
            
            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                    .DynamicParameters((k, d) => d.Add("collection", WebConfigurationManager.AppSettings))
                    .DynamicParameters((k, d) => d.Add("globalizationSection", ConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection)).LifeStyle.Singleton);

            //container.Register(Component.For<HttpServerUtilityBase>().ImplementedBy<HttpServerUtilityWrapper>()
            //        .DynamicParameters((k, d) => d.Insert("httpServerUtility", HttpContext.Current.Server))
            //        .LifeStyle.Transient);
            
            container.Register(Classes.FromAssembly(egcCoreAssembly).Pick()
                    .If(Component.IsInNamespace(egcCoremodelsType.Namespace))
                    .WithService.Self()
                    .Configure(c => c.LifestyleTransient()));
            
            container.Install(new LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());
        }

    }

}
