namespace Esynctraining.Persistence
{
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.ComponentActivator;
    using Castle.MicroKernel.Context;

    using Esynctraining.Core.Utils;

    using NHibernate;

    /// <summary>
    /// The n hibernate session activator.
    /// </summary>
    public class NHibernateSessionActivator : DefaultComponentActivator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateSessionActivator"/> class.
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
        public NHibernateSessionActivator(
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
            var sessionFactory = IoC.Resolve<ISessionFactory>();
            return sessionFactory.OpenSession();
        }

        #endregion
    }
}