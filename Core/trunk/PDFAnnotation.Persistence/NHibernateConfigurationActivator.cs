namespace PDFAnnotation.Persistence
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
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateConfigurationActivator"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="kernel">
        /// The kernel.
        /// </param>
        /// <param name="onCreation">
        /// The on creation.
        /// </param>
        /// <param name="onDestruction">
        /// The on destruction.
        /// </param>
        public NHibernateConfigurationActivator(
            ComponentModel model, 
            IKernelInternal kernel, 
            ComponentInstanceDelegate onCreation, 
            ComponentInstanceDelegate onDestruction)
            : base(model, kernel, onCreation, onDestruction)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create instance.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="constructor">
        /// The constructor.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
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

        #endregion
    }
}