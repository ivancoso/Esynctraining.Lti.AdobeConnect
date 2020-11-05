using System;
using Castle.Windsor;
using Esynctraining.Core.Utils;

namespace Esynctraining.Windsor
{
    /// <summary>
    /// Global and Local container wrapper
    /// </summary>
    public static class WindsorIoC
    {
        #region Constants

        /// <summary>
        /// The container not initialized message.
        /// </summary>
        private const string ContainerNotInitializedMessage =
            "The container has not been initialized! Please call IoC.Initialize(container) before using it.";

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Container is not initialized
        /// </exception>
        public static IWindsorContainer Container
        {
            get
            {
                IWindsorContainer container = GlobalContainer;
                if (container == null)
                {
                    throw new InvalidOperationException(ContainerNotInitializedMessage);
                }

                return container;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is initialized.
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                return (GlobalContainer) != null;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the global container.
        /// </summary>
        private static IWindsorContainer GlobalContainer { get; set; }
        
        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="windsorContainer">
        /// The windsor container.
        /// </param>
        public static void Initialize(IWindsorContainer windsorContainer)
        {
            GlobalContainer = windsorContainer;
            IoC.Initialize(new WindsorServiceLocator(windsorContainer));
        }

        /// <summary>
        /// The reset.
        /// </summary>
        public static void Reset()
        {
            Reset(GlobalContainer);
        }

        /// <summary>
        /// The reset.
        /// </summary>
        /// <param name="containerToReset">
        /// The container to reset.
        /// </param>
        public static void Reset(IWindsorContainer containerToReset)
        {
            if (containerToReset != null)
            {
                if (ReferenceEquals(GlobalContainer, containerToReset))
                {
                    GlobalContainer = null;
                }
            }
        }
        
        #endregion

    }

}
