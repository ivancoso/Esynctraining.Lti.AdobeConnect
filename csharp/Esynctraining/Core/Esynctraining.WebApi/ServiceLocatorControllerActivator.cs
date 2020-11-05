using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Esynctraining.Core.Utils;

namespace Esynctraining.WebApi
{
    public class ServiceLocatorControllerActivator : IHttpControllerActivator
    {
        private readonly IServiceLocator container;


        public ServiceLocatorControllerActivator(IServiceLocator container)
        {
            this.container = container;
        }


        public IHttpController Create(
            HttpRequestMessage request,
            HttpControllerDescriptor controllerDescriptor,
            Type controllerType)
        {
            var controller =
                (IHttpController)this.container.GetInstance(controllerType);

            request.RegisterForDispose(
                new Release(
                    () => this.container.Release(controller)));

            return controller;
        }

        private class Release : IDisposable
        {
            private readonly Action release;

            public Release(Action release)
            {
                this.release = release;
            }

            public void Dispose()
            {
                this.release();
            }
        }

    }

}
