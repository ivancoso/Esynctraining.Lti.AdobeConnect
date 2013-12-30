namespace EdugameCloud.Persistence.Mappings
{
    using Esynctraining.Core.Domain.Entities;

    using FluentNHibernate.Mapping;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The base class map.
    /// </summary>
    /// <typeparam name="T">
    /// Type of entity id
    /// </typeparam>
    public abstract class BaseClassMap<T> : ClassMap<T>
        where T : Entity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClassMap{T}"/> class.
        /// </summary>
        protected BaseClassMap()
        {
            this.Id(x => x.Id).GeneratedBy.Identity();
        }

        #endregion
    }
}