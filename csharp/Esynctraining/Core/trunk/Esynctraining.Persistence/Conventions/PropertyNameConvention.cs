namespace Esynctraining.Persistence.Conventions
{
    using System.Collections.Generic;
    using System.Globalization;

    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    /// The property name convention.
    /// </summary>
    public class PropertyNameConvention : IPropertyConvention
    {
        #region Fields

        /// <summary>
        ///     The exceptions.
        /// </summary>
        public static List<string> Exceptions = new List<string>
                                                       {
//                                                         Lambda.Property<State>(x => x.StateName),
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
            bool isEntityTypeName = !Exceptions.Contains(instance.Property.Name)
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