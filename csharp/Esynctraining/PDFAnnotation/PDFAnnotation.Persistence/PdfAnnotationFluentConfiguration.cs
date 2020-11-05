namespace PDFAnnotation.Persistence
{
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
        static PdfAnnotationFluentConfiguration()
        {
            Esynctraining.Persistence.Conventions.PropertyNameConvention.Exceptions = PropertyNameConvention.Exceptions;
        }

        protected override void Configure(PersistenceModel persistenceModel, Configuration cfg)
        {
            var executing = Assembly.GetExecutingAssembly();
            cfg.AddAssembly(executing);
            persistenceModel.AddMappingsFromSource(new NameSpaceTypeSource(executing, typeof(FileMap).Namespace));
#if ADD_COMPANY_CONTACT_MAPPING
            persistenceModel.AddMappingsFromSource(new NameSpaceTypeSource(executing, typeof(CompanyMap).Namespace));
#endif
            this.AddConventions(persistenceModel.Conventions);
            persistenceModel.Configure(cfg);
        }

    }

}