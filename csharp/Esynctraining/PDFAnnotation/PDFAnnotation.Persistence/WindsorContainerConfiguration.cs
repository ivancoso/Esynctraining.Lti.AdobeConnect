namespace PDFAnnotation.Persistence
{
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Esynctraining.Core.FullText;
    using Esynctraining.NHibernate;
    using Esynctraining.Persistence;
    using NHibernate;
    using PDFAnnotation.Core.Utils;
    using Configuration = NHibernate.Cfg.Configuration;

    /// <summary>
    /// The windsor container configuration.
    /// </summary>
    public static class WindsorContainerConfiguration
    {
        public static void RegisterComponents(this IWindsorContainer container, bool wcf = false, bool web = false)
        {
            if (wcf)
            {
                container.AddFacility<WcfFacility>();
            }

            container.Register(Component.For<Configuration>().LifeStyle.Singleton.Activator<NHibernateConfigurationActivator>());
            container.Register(Component.For<ISessionFactory>().LifeStyle.Singleton.Activator<Esynctraining.Persistence.NHibernateSessionFactoryActivator>());

            if (wcf)
            {
                container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionSource>().LifeStyle.PerWcfOperation());   
            }
            else if (web)
            {
                container.Register(Component.For<ISessionSource>().ImplementedBy<NHibernateSessionWebSource>().LifeStyle.Scoped());
            }

            container.Register(Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(Repository<,>)).LifeStyle.Transient);

            container.Register(Component.For<FullTextModel>().LifeStyle.Singleton);
            container.Register(Component.For<Pdf2SwfConverter>().LifeStyle.Singleton);
            container.Register(Types.FromAssemblyNamed("PDFAnnotation.Core").Pick().If(Component.IsInNamespace("PDFAnnotation.Core.Business.Models", true)).WithService.Self().Configure(c => c.LifestyleTransient()));
            container.Register(Types.FromAssemblyNamed("Esynctraining.Core").Pick().If(Component.IsInNamespace("Esynctraining.Core.Business.Models"))
                .Unless(type => /*type == typeof(AuthenticationModel) ||*/ type == typeof(FullTextModel)).WithService.Self().Configure(c => c.LifestyleTransient()));
            
        }

    }

}