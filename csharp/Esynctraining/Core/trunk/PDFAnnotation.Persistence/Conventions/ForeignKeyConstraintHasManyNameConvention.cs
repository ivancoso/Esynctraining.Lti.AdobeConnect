namespace PDFAnnotation.Persistence.Conventions
{
    using System.Globalization;

    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    ///     The foreign key constraint has many name convention.
    /// </summary>
    public class ForeignKeyConstraintHasManyNameConvention : IHasManyConvention
    {
        #region Public Methods and Operators

        /// <summary>
        /// The apply.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Key.ForeignKey(
                string.Format(
                    CultureInfo.InvariantCulture, 
                    FluentConfiguration.ForeignKeyTemplate, 
                    instance.EntityType.Name, 
                    Inflector.Pluralize(instance.Member.Name)));
        }

        #endregion
    }
}