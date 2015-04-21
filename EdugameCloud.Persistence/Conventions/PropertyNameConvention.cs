namespace EdugameCloud.Persistence.Conventions
{
    using System.Collections.Generic;
    using System.Globalization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    /// The property name convention.
    /// </summary>
    internal sealed class PropertyNameConvention : IPropertyConvention
    {
        #region Fields

        /// <summary>
        ///     The exceptions.
        /// </summary>
        private readonly List<string> exceptions = new List<string>
                                                       {
                                                           Lambda.Property<Module>(x => x.ModuleName), 
                                                           Lambda.Property<Quiz>(x => x.QuizName),
                                                           Lambda.Property<Survey>(x => x.SurveyName),
                                                           Lambda.Property<QuizFormat>(x => x.QuizFormatName),
                                                           Lambda.Property<SubModule>(x => x.SubModuleName),
                                                           Lambda.Property<Test>(x => x.TestName),
                                                           Lambda.Property<Theme>(x => x.ThemeName),
                                                           Lambda.Property<State>(x => x.StateName),
                                                           Lambda.Property<Company>(x => x.CompanyName),
                                                           Lambda.Property<File>(x => x.FileName),
                                                           
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