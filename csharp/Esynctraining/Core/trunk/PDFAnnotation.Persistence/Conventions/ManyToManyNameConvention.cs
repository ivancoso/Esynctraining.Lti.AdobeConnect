namespace PDFAnnotation.Persistence.Conventions
{
    using System.Globalization;

    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    /// The many to many name convention.
    /// </summary>
    public class ManyToManyNameConvention : IHasManyToManyConvention
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
            string parentName = instance.EntityType.Name;
            string childName = instance.ChildType.Name;

            if (parentName.CompareTo(childName) < 0)
            {
                instance.Table(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        FluentConfiguration.TableNameTemplate, 
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            FluentConfiguration.ManyToManyTemplate, 
                            Inflector.Pluralize(parentName), 
                            Inflector.Pluralize(childName))));
            }
            else
            {
                instance.Table(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        FluentConfiguration.TableNameTemplate, 
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            FluentConfiguration.ManyToManyTemplate, 
                            Inflector.Pluralize(childName), 
                            Inflector.Pluralize(parentName))));
            }
        }

        #endregion
    }
}