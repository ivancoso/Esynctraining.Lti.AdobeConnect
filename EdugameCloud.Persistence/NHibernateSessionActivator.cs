namespace EdugameCloud.Persistence
{
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.ComponentActivator;
    using Castle.MicroKernel.Context;

    using Esynctraining.Core.Utils;

    using NHibernate;

    public class NHibernateSessionActivator : DefaultComponentActivator
	{
		public NHibernateSessionActivator(ComponentModel model, IKernelInternal kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override object CreateInstance(CreationContext context, ConstructorCandidate constructor, object[] arguments)
		{
			var sessionFactory = IoC.Resolve<ISessionFactory>();
			return sessionFactory.OpenSession();
		}
	}
}