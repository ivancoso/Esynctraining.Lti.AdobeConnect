namespace EdugameCloud.Persistence.Mappings
{
    using Esynctraining.Core.Domain.Entities;
    using FluentNHibernate.Mapping;

    /// <summary>
    /// The base class map.
    /// </summary>
    /// <typeparam name="T">
    /// Type of entity id
    /// </typeparam>
    public abstract class BaseClassMapGuid<T> : ClassMap<T>
        where T : EntityGuid
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClassMap{T}"/> class.
        /// </summary>
        protected BaseClassMapGuid()
        {
            this.Id(x => x.Id).GeneratedBy.GuidComb();
        }

        #endregion
    }
}