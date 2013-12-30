namespace EdugameCloud.Web.Providers
{
    using System.Web;

    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.ComponentActivator;
    using Castle.MicroKernel.Context;

    /// <summary>
    /// The resource provider activator.
    /// </summary>
    public class ResourceProviderActivator : DefaultComponentActivator
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
                return HttpContext.Current.Session[ResourceProvideActivatorKey]
                       ?? (HttpContext.Current.Session[ResourceProvideActivatorKey] =
                           new EGCResourceProvider());
            }
            return new EGCResourceProvider();
        }

        #endregion
    }
}