namespace PDFAnnotation.Persistence.Conventions
{
    using System.Collections.Generic;
    using System.Globalization;

    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The property name convention.
    /// </summary>
    public class PropertyNameConvention : IPropertyConvention
    {
        #region Fields

        /// <summary>
        ///     The exceptions.
        /// </summary>
        private readonly List<string> exceptions = new List<string>
                                                       {
                                                           Lambda.Property<State>(x => x.StateName),
                                                           Lambda.Property<File>(x => x.FileName),
                                                           Lambda.Property<Category>(x => x.CategoryName),
                                                           Lambda.Property<Company>(x => x.CompanyName),
                                                       };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The apply.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public void Apply(IPropertyInstance instance)
        {
            const string NamePattern = "Name";
            bool isEntityTypeName = !this.exceptions.Contains(instance.Property.Name)
                                    && instance.Property.Name.EndsWith(NamePattern)
                                    && instance.Property.Name == instance.EntityType.Name + NamePattern;
            string column = string.Format(
                CultureInfo.InvariantCulture, 
                FluentConfiguration.ColumnNameTemplate, 
                Inflector.Uncapitalize(isEntityTypeName ? instance.EntityType.Name : instance.Property.Name));
            instance.Column(column);
        }

        #endregion
    }
}