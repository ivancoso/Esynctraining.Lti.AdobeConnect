namespace Esynctraining.Persistence.Mappings
{
    using Esynctraining.Core.Domain.Entities;

    using FluentNHibernate.Mapping;

    /// <summary>
    /// The base class map GUID.
    /// </summary>
    /// <typeparam name="T">
    /// Type of entity
    /// </typeparam>
    public abstract class BaseClassMapGuid<T> : ClassMap<T>
        where T : EntityGuid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClassMapGuid{T}" /> class.
        /// </summary>
        protected BaseClassMapGuid()
        {
            this.Id(x => x.Id).GeneratedBy.GuidComb();
        }

    }

}