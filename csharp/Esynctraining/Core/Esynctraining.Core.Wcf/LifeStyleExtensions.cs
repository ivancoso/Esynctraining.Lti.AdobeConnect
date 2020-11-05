using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;

namespace Esynctraining.Core.Wcf
{
    /// <summary>
    /// The life style extensions.
    /// </summary>
    [Obsolete]
    public static class LifeStyleExtensions
    {
        /// <summary>
        /// The per wcf operation including web orb.
        /// </summary>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <typeparam name="TService">
        /// </typeparam>
        /// <returns>
        /// The <see cref="ComponentRegistration{TService}"/>.
        /// </returns>
        public static ComponentRegistration<TService> PerWcfOperationIncludingWebOrb<TService>(this LifestyleGroup<TService> @group)
            where TService : class
        {
            return group.Scoped<EsyncWcfOperationScopeAccessor>();
        }

    }

}
