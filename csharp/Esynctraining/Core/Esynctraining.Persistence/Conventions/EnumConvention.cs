namespace Esynctraining.Persistence.Conventions
{
    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.AcceptanceCriteria;
    using FluentNHibernate.Conventions.Inspections;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    /// The convention.
    /// </summary>
    public class EnumConvention : IUserTypeConvention
    {
        #region Public Methods and Operators

        /// <summary>
        /// The accept.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Property.PropertyType.IsEnum);
        }

        /// <summary>
        /// The apply.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        public void Apply(IPropertyInstance target)
        {
            target.CustomType(target.Property.PropertyType);
        }

        #endregion
    }
}