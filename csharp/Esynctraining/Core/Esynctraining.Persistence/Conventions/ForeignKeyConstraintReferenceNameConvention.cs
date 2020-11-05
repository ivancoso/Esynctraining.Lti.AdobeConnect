namespace Esynctraining.Persistence.Conventions
{
    using System.Globalization;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    /// The foreign key constraint reference name convention.
    /// </summary>
    public class ForeignKeyConstraintReferenceNameConvention : IReferenceConvention
    {
        #region Public Methods and Operators

        /// <summary>
        /// The apply.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public void Apply(IManyToOneInstance instance)
        {
            instance.ForeignKey(
                string.Format(
                    CultureInfo.InvariantCulture, 
                    FluentConfiguration.ForeignKeyTemplate, 
                    instance.EntityType.Name, 
                    instance.Name));
        }

        #endregion
    }
}