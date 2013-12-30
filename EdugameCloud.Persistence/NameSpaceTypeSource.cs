namespace EdugameCloud.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using FluentNHibernate;
    using FluentNHibernate.Diagnostics;

    public class NameSpaceTypeSource : ITypeSource
	{
		private readonly Assembly source;

		private readonly string mappingPath;

		/// <summary>
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

		public string GetIdentifier()
		{
			return this.source.GetName().FullName + this.mappingPath;
		}

		public IEnumerable<Type> GetTypes()
		{
			return from x in this.source.GetExportedTypes()
					orderby x.FullName
					where x.Namespace == this.mappingPath
					select x;
		}

		public void LogSource(IDiagnosticLogger logger)
		{
			logger.LoadedFluentMappingsFromSource(this);
		}
	}
}
