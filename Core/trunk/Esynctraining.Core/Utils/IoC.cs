namespace Esynctraining.Core.Utils
{
    using System;
    using System.Collections.Generic;

    using Castle.Windsor;

    /// <summary>
    /// Global and Local container wrapper
    /// </summary>
    public static class IoC
    {
        #region Constants

        /// <summary>
        /// The container not initialized message.
        /// </summary>
        private const string ContainerNotInitializedMessage =
            "The container has not been initialized! Please call IoC.Initialize(container) before using it.";

        #endregion

        #region Static Fields

        /// <summary>
        /// The local container key.
        /// </summary>
        private static readonly object LocalContainerKey = new object();

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
                IWindsorContainer container = LocalContainer ?? GlobalContainer;
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
                return (LocalContainer ?? GlobalContainer) != null;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the global container.
        /// </summary>
        internal static IWindsorContainer GlobalContainer { get; set; }

        /// <summary>
        /// Gets the local container.
        /// </summary>
        private static IWindsorContainer LocalContainer
        {
            get
            {
                if (LocalContainerStack.Count == 0)
                {
                    return null;
                }

                return LocalContainerStack.Peek();
            }
        }

        /// <summary>
        /// Gets the local container stack.
        /// </summary>
        private static Stack<IWindsorContainer> LocalContainerStack
        {
            get
            {
                var stack = Local.Data[LocalContainerKey] as Stack<IWindsorContainer>;
                if (stack == null)
                {
                    Local.Data[LocalContainerKey] = stack = new Stack<IWindsorContainer>();
                }

                return stack;
            }
        }

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
        }

        /// <summary>
        /// The reset.
        /// </summary>
        public static void Reset()
        {
            Reset(LocalContainer ?? GlobalContainer);
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
                if (ReferenceEquals(LocalContainer, containerToReset))
                {
                    LocalContainerStack.Pop();
                    if (LocalContainerStack.Count == 0)
                    {
                        Local.Data[LocalContainerKey] = null;
                    }
                }
                else if (ReferenceEquals(GlobalContainer, containerToReset))
                {
                    GlobalContainer = null;
                }
            }
        }

        /// <summary>
        /// Resolve type.
        /// </summary>
        /// <typeparam name="T">
        /// Type to be resolved
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/> resolved instance.
        /// </returns>
        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        /// <summary>
        /// Resolve type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="Object"/> resolved instance.
        /// </returns>
        public static object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        /// <summary>
        /// The use local container.
        /// </summary>
        /// <param name="localContainer">
        /// The local container.
        /// </param>
        /// <returns>
        /// The <see cref="IDisposable"/>.
        /// </returns>
        public static IDisposable UseLocalContainer(IWindsorContainer localContainer)
        {
            LocalContainerStack.Push(localContainer);
            return new DisposableAction(() => Reset(localContainer));
        }

        #endregion
    }
}