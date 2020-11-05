namespace PDFAnnotation.Persistence.Conventions
{
    using System.Globalization;

    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    ///     The foreign key constraint has many to many name convention.
    /// </summary>
    public class ForeignKeyConstraintHasManyToManyNameConvention : IHasManyToManyConvention
    {
        #region Public Methods and Operators

        /// <summary>
        /// The apply.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Key.ForeignKey(
                string.Format(
                    CultureInfo.InvariantCulture, 
                    FluentConfiguration.ForeignKeyTemplate, 
                    Inflector.Pluralize(instance.EntityType.Name), 
                    Inflector.Pluralize(instance.Member.Name)));
        }

        #endregion
    }
}