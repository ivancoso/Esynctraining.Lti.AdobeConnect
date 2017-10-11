namespace EdugameCloud.Web.Providers
{
    using System.Web;

    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.ComponentActivator;
    using Castle.MicroKernel.Context;

    public class ResourceProviderActivator : DefaultComponentActivator
    {
        private const string ResourceProvideActivatorKey = "ResourceProvideActivator";


        public ResourceProviderActivator(
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
            var ctx = HttpContext.Current;
            if (ctx != null && ctx.User != null && ctx.User.Identity.IsAuthenticated)
            {
                return ctx.Session[ResourceProvideActivatorKey] ?? (ctx.Session[ResourceProvideActivatorKey] = new EGCResourceProvider());
            }
            return new EGCResourceProvider();
        }

    }

}