namespace PDFAnnotation.Persistence
{
    using System.Linq;
    using System.Reflection;

    using Esynctraining.Persistence;

    using FluentNHibernate;
    using NHibernate.Cfg;

    using PDFAnnotation.Persistence.Conventions;
    using PDFAnnotation.Persistence.Mappings;

    /// <summary>
    /// The fluent configuration.
    /// </summary>
    public class PdfAnnotationFluentConfiguration : Esynctraining.Persistence.FluentConfiguration
    {
        public static object @locker = new object();

        /// <summary>
        /// Initializes static members of the <see cref="PdfAnnotationFluentConfiguration"/> class.
        /// </summary>
        public PdfAnnotationFluentConfiguration()
        {
            lock (locker)
            {
                if (Esynctraining.Persistence.Conventions.PropertyNameConvention.Exceptions == null
                    || Esynctraining.Persistence.Conventions.PropertyNameConvention.Exceptions.Count() == 0)
                {
                    lock (locker)
                    {
                        Esynctraining.Persistence.Conventions.PropertyNameConvention.Exceptions = PropertyNameConvention.Exceptions;
                    }
                }
            }
        }

        /// <summary>
        /// The configured.
        /// </summary>
        /// <param name="persistenceModel">
        /// The persistence Model.
        /// </param>
        /// <param name="cfg">
        /// The config.
        /// </param>
        protected override void Configure(PersistenceModel persistenceModel, Configuration cfg)
        {
            var executing = Assembly.GetExecutingAssembly();
            cfg.AddAssembly(executing);
            persistenceModel.AddMappingsFromSource(new NameSpaceTypeSource(executing, typeof(ContactTypeMap).Namespace));
            this.AddConventions(persistenceModel.Conventions);
            persistenceModel.Configure(cfg);
        }

    }
}