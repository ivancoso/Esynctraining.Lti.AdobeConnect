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
    public class PropertyNameConvention 
    {
        #region Fields

        /// <summary>
        ///     The exceptions.
        /// </summary>
        public static readonly List<string> Exceptions = new List<string>
                                                       {
                                                           "StateName",
                                                           "FileName",
                                                           "CategoryName",
                                                           "CompanyName",
                                                       };

        #endregion
    }
}