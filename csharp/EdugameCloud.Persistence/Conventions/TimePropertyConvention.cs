namespace EdugameCloud.Persistence.Conventions
{
    using System;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    /// The time property convention.
    /// </summary>
    internal sealed class TimePropertyConvention : IPropertyConvention
    {
        #region Public Methods and Operators

        /// <summary>
        /// The apply.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public void Apply(IPropertyInstance instance)
        {
            if (instance.Property.PropertyType == typeof(TimeSpan) || instance.Property.PropertyType == typeof(TimeSpan?))
            {
                instance.CustomType("TimeAsTimeSpan");
            }
        }

        #endregion
    }
}