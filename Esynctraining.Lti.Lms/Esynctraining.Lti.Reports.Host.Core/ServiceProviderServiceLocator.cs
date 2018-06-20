using Esynctraining.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Esynctraining.Lti.Reports.Host.Core
{
    internal sealed class ServiceProviderServiceLocator : IServiceLocator
    {
        private readonly IServiceProvider _container;


        public ServiceProviderServiceLocator(IServiceProvider serviceProvider)
        {
            _container = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }


        public object GetInstance(Type serviceType)
        {
            return _container.GetService(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<TService>()
        {
            return _container.GetService<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            throw new NotImplementedException();
        }

        public void Release(object controller)
        {
            throw new NotImplementedException();
        }

    }
}
