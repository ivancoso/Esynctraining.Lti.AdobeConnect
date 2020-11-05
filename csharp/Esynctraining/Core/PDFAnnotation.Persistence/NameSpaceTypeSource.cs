namespace PDFAnnotation.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using FluentNHibernate;
    using FluentNHibernate.Diagnostics;

    /// <summary>
    /// The name space type source.
    /// </summary>
    public class NameSpaceTypeSource : ITypeSource
    {
        #region Fields

        /// <summary>
        /// The mapping path.
        /// </summary>
        private readonly string mappingPath;

        /// <summary>
        /// The source.
        /// </summary>
        private readonly Assembly source;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NameSpaceTypeSource"/> class. 
        /// The assembly name space type source.
        /// </summary>
        /// <param name="source">
        /// Assembly with mappings
        /// </param>
        /// <param name="mappingPath">
        /// namespace
        /// </param>
        public NameSpaceTypeSource(Assembly source, string mappingPath)
        {
            this.source = source;
            this.mappingPath = mappingPath;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetIdentifier()
        {
            return this.source.GetName().FullName + this.mappingPath;
        }

        /// <summary>
        /// The get types.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<Type> GetTypes()
        {
            return from x in this.source.GetExportedTypes()
                   orderby x.FullName
                   where x.Namespace == this.mappingPath
                   select x;
        }

        /// <summary>
        /// The log source.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public void LogSource(IDiagnosticLogger logger)
        {
            logger.LoadedFluentMappingsFromSource(this);
        }

        #endregion
    }
}