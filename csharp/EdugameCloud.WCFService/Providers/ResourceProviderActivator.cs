namespace EdugameCloud.WCFService.Providers
{
    using System.Web;

    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.ComponentActivator;
    using Castle.MicroKernel.Context;

    /// <summary>
    /// The resource provider activator.
    /// </summary>
    public sealed class ResourceProviderActivator : DefaultComponentActivator
    {
        #region Constants

        /// <summary>
        /// The resource provide activator key.
        /// </summary>
        public const string ResourceProvideActivatorKey = "ResourceProvideActivator";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceProviderActivator"/> class.
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
        public ResourceProviderActivator(
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
            CreationContext context, ConstructorCandidate constructor, object[] arguments)
        {
            if (HttpContext.Current != null && HttpContext.Current.User != null
                && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.Session[ResourceProvideActivatorKey] == null)
                {
                    HttpContext.Current.Session[ResourceProvideActivatorKey] = new WcfResourceProvider();
                }

                return HttpContext.Current.Session[ResourceProvideActivatorKey];
            }
            
            return new WcfResourceProvider();
        }

        #endregion
    }
}