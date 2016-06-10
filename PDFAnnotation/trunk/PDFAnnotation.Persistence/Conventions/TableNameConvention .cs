namespace PDFAnnotation.Persistence.Conventions
{
    using System.Globalization;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.AcceptanceCriteria;
    using FluentNHibernate.Conventions.Inspections;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    /// The table name convention.
    /// </summary>
    public class TableNameConvention : IClassConvention, IClassConventionAcceptance
    {
        #region Public Methods and Operators

        /// <summary>
        /// The accept.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
        {
            criteria.Expect(x => x.TableName, Is.Not.Set);
        }

        /// <summary>
        /// The apply.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public void Apply(IClassInstance instance)
        {
            instance.Table(
                string.Format(
                    CultureInfo.InvariantCulture, 
                    FluentConfiguration.TableNameTemplate, 
                    instance.EntityType.Name));
        }

        #endregion
    }
}