using System;
using Castle.Windsor;
using Esynctraining.Core.Utils;

namespace Esynctraining.Windsor
{
    /// <summary>
    /// Adapts the behavior of the Windsor container to the common
    /// IServiceLocator
    /// </summary>
    public class WindsorServiceLocator : IServiceLocator
    {
        private readonly IWindsorContainer container;


        public WindsorServiceLocator(IWindsorContainer container)
        {
            this.container = container;
        }


        public object GetInstance(Type serviceType)
        {
            return container.Resolve(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return container.Resolve(serviceType, key);
        }

        public TService GetInstance<TService>()
        {
            return container.Resolve<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return container.Resolve<TService>(key);
        }

    }

}
