namespace Esynctraining.Persistence.Extensions
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.Registration.Lifestyle;

    using Esynctraining.Core.LifeStyle;

    /// <summary>
    ///     The life style extensions.
    /// </summary>
    public static class LifeStyleExtensions
    {
        #region Public Methods and Operators

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
        public static ComponentRegistration<TService> PerWcfOperationIncludingWebOrb<TService>(
            this LifestyleGroup<TService> @group) where TService : class
        {
            return group.Scoped<EsyncWcfOperationScopeAccessor>();
        }

        #endregion
    }
}