namespace EdugameCloud.Persistence
{
    using System;
    using System.IO;
    using System.Xml;

    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.ComponentActivator;
    using Castle.MicroKernel.Context;

    using Esynctraining.Core.Utils;

    using NHibernate.Cfg;

    /// <summary>
    /// The n hibernate configuration activator.
    /// </summary>
    public class NHibernateConfigurationActivator : DefaultComponentActivator
    {
        public NHibernateConfigurationActivator(
            ComponentModel model, 
            IKernelInternal kernel, 
            ComponentInstanceDelegate onCreation, 
            ComponentInstanceDelegate onDestruction)
            : base(model, kernel, onCreation, onDestruction)
        {
        }


        protected override object CreateInstance(
            CreationContext context, 
            ConstructorCandidate constructor, 
            object[] arguments)
        {
            var cfg = new Configuration();

            string hibernateConfig = "nhibernate.cfg.xml";

            // if not rooted, assume path from base directory
            if (Path.IsPathRooted(hibernateConfig) == false)
            {
                hibernateConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, hibernateConfig);
            }

            if (File.Exists(hibernateConfig))
            {
                cfg.Configure(new XmlTextReader(hibernateConfig));
            }

            IoC.Resolve<FluentConfiguration>().Configured(cfg);

            return cfg;
        }

    }

}