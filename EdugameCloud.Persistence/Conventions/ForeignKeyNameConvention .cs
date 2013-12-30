namespace EdugameCloud.Persistence.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Utils;

    using FluentNHibernate;
    using FluentNHibernate.Conventions;

    /// <summary>
    /// The foreign key name convention.
    /// </summary>
    public class ForeignKeyNameConvention : ForeignKeyConvention
    {
        #region Fields

        /// <summary>
        /// The exceptions.
        /// </summary>
        private readonly List<string> exceptions = new List<string>
                                                       {
                                                           Lambda.Property<Question>(x => x.CreatedBy), 
                                                           Lambda.Property<Question>(x => x.ModifiedBy)
                                                       };

        #endregion

        #region Methods

        /// <summary>
        /// The get key name.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetKeyName(Member property, Type type)
        {
            return property != null && this.exceptions.Contains(property.Name)
                       ? Inflector.Uncapitalize(property.Name)
                       : string.Format(
                           CultureInfo.InvariantCulture, 
                           FluentConfiguration.ForeignKeyNameTemplate, 
                           Inflector.Uncapitalize(type.Name));
        }

        #endregion
    }
}