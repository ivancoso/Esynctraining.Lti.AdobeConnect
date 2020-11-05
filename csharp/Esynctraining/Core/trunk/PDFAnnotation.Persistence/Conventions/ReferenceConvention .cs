namespace PDFAnnotation.Persistence.Conventions
{
    using System.Globalization;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    /// The reference name convention.
    /// </summary>
    public class ReferenceNameConvention : IReferenceConvention
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
            instance.Column(
                string.Format(
                    CultureInfo.InvariantCulture, 
                    FluentConfiguration.ReferenceNameTemplate, 
                    instance.Property.PropertyType.Name));
        }

        #endregion
    }
}