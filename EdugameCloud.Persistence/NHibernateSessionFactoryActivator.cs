namespace EdugameCloud.Persistence
{
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.ComponentActivator;
    using Castle.MicroKernel.Context;

    using Esynctraining.Core.Utils;

    using NHibernate.Cfg;

    /// <summary>
    /// The n hibernate session factory activator.
    /// </summary>
    public class NHibernateSessionFactoryActivator : DefaultComponentActivator
    {
        public NHibernateSessionFactoryActivator(
            ComponentModel model, 
            IKernelInternal kernel, 
            ComponentInstanceDelegate onCreation, 
            ComponentInstanceDelegate onDestruction)
            : base(model, kernel, onCreation, onDestruction)
        {
        }

        protected override object CreateInstance(
            CreationContext context, ConstructorCandidate constructor, object[] arguments)
        {
            var cfg = IoC.Resolve<Configuration>();

            return cfg.BuildSessionFactory();
        }

    }

}