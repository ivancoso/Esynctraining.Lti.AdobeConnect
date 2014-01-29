namespace PDFAnnotation.Persistence
{
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.ComponentActivator;
    using Castle.MicroKernel.Context;

    using Esynctraining.Core.FullText;
    using Esynctraining.Core.Utils;
    using NHibernate.Cfg;

    /// <summary>
    /// The n hibernate session factory activator.
    /// </summary>
    public class NHibernateSessionFactoryActivator : DefaultComponentActivator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateSessionFactoryActivator"/> class.
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
        public NHibernateSessionFactoryActivator(
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
        protected override object CreateInstance(CreationContext context, ConstructorCandidate constructor, object[] arguments)
        {
            var cfg = IoC.Resolve<Configuration>();
            cfg.SetListener(NHibernate.Event.ListenerType.PostUpdate, new LuceneFTIndexEventListener());
            cfg.SetListener(NHibernate.Event.ListenerType.PostInsert, new LuceneFTIndexEventListener());
            cfg.SetListener(NHibernate.Event.ListenerType.PostDelete, new LuceneFTIndexEventListener());
            return cfg.BuildSessionFactory();
        }

        #endregion
    }
}