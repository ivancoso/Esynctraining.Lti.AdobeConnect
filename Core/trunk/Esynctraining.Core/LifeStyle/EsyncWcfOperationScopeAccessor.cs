//namespace Esynctraining.Core.LifeStyle
//{
//    using System;
//    using System.ComponentModel;
//    using System.ServiceModel;

//    using Castle.Core.Internal;
//    //using Castle.Facilities.WcfIntegration.Lifestyles;
//    using Castle.MicroKernel.Context;
//    using Castle.MicroKernel.Lifestyle.Scoped;

//    /// <summary>
//    /// The wcf operation scope ancestor.
//    /// </summary>
//    [Description("per WCF operation")]
//    public class EsyncWcfOperationScopeAccessor : IScopeAccessor, IDisposable
//    {
//        #region Fields

//        /// <summary>
//        /// The disposed.
//        /// </summary>
//        private ThreadSafeFlag disposed;

//        #endregion

//        #region Public Methods and Operators

//        /// <summary>
//        /// The dispose.
//        /// </summary>
//        public void Dispose()
//        {
//            if (!this.disposed.Signal())
//            {
//                return;
//            }

//            OperationContext scopeHolder = GetScopeHolder();
//            if (scopeHolder == null)
//            {
//                return;
//            }

//            var operationScopeHolder = scopeHolder.Extensions.Find<WcfOperationScopeHolder>();
//            if (operationScopeHolder == null || !scopeHolder.Extensions.Remove(operationScopeHolder))
//            {
//                return;
//            }

//            operationScopeHolder.Dispose();
//        }

//        /// <summary>
//        /// The get scope.
//        /// </summary>
//        /// <param name="context">
//        /// The context.
//        /// </param>
//        /// <returns>
//        /// The <see cref="ILifetimeScope"/>.
//        /// </returns>
//        public ILifetimeScope GetScope(CreationContext context)
//        {
//            return GetScope(GetScopeHolder());
//        }

//        #endregion

//        #region Methods

//        /// <summary>
//        /// The get scope.
//        /// </summary>
//        /// <param name="scopeHolder">
//        /// The scope holder.
//        /// </param>
//        /// <returns>
//        /// The <see cref="ILifetimeScope"/>.
//        /// </returns>
//        private static ILifetimeScope GetScope(IExtensibleObject<OperationContext> scopeHolder)
//        {
//            if (scopeHolder == null)
//            {
//                return new DefaultLifetimeScope();
//            }

//            var operationScopeHolder = scopeHolder.Extensions.Find<WcfOperationScopeHolder>();
//            if (operationScopeHolder == null)
//            {
//                operationScopeHolder = new WcfOperationScopeHolder(new DefaultLifetimeScope());
//                scopeHolder.Extensions.Add(operationScopeHolder);
//            }

//            return operationScopeHolder.Scope;
//        }

//        /// <summary>
//        /// The get scope holder.
//        /// </summary>
//        /// <returns>
//        /// The <see cref="OperationContext"/>.
//        /// </returns>
//        private static OperationContext GetScopeHolder()
//        {
//            return OperationContext.Current;
//        }

//        #endregion
//    }
//}