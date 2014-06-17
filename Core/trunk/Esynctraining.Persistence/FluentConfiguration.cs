namespace Esynctraining.Persistence
{
    using System.Reflection;

    using Esynctraining.Persistence.Conventions;
    using Esynctraining.Persistence.Mappings;

    using FluentNHibernate;
    using FluentNHibernate.Conventions;

    using NHibernate.Cfg;

    /// <summary>
    /// The fluent configuration.
    /// </summary>
    public class FluentConfiguration
    {
        #region Constants

        /// <summary>
        /// The column name template.
        /// </summary>
        public const string ColumnNameTemplate = "[{0}]";

        /// <summary>
        /// The foreign key name template.
        /// </summary>
        public const string ForeignKeyNameTemplate = "{0}Id";

        /// <summary>
        /// The foreign key template.
        /// </summary>
        public const string ForeignKeyTemplate = "FK_{0}_{1}";

        /// <summary>
        /// The many to many template.
        /// </summary>
        public const string ManyToManyTemplate = "{0}To{1}";

        /// <summary>
        /// The reference name template.
        /// </summary>
        public const string ReferenceNameTemplate = "{0}Id";

        /// <summary>
        /// The table name template.
        /// </summary>
        public const string TableNameTemplate = "{0}";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The configured.
        /// </summary>
        /// <param name="cfg">
        /// The config.
        /// </param>
        public virtual void Configured(Configuration cfg)
        {
            var persistenceModel = new PersistenceModel();
            this.Configure(persistenceModel, cfg);
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
        protected virtual void Configure(PersistenceModel persistenceModel, Configuration cfg)
        {
            Assembly executing = Assembly.GetExecutingAssembly();
            cfg.AddAssembly(executing);
            persistenceModel.AddMappingsFromSource(new AssemblyTypeSource(Assembly.GetCallingAssembly()));
            AddConventions(persistenceModel.Conventions);
            persistenceModel.Configure(cfg);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add conventions.
        /// </summary>
        /// <param name="conventions">
        /// The conventions.
        /// </param>
        private static void AddConventions(IConventionFinder conventions)
        {
            // uncomment lines if needed 
            conventions.Add(new ForeignKeyConstraintHasManyNameConvention());
            conventions.Add(new ForeignKeyConstraintReferenceNameConvention());
            conventions.Add(new ForeignKeyNameConvention());
            conventions.Add(new ManyToManyNameConvention());
            conventions.Add(new ReferenceNameConvention());
            conventions.Add(new ForeignKeyConstraintHasManyToManyNameConvention());
            conventions.Add(new TableNameConvention());
            conventions.Add(new PrimaryKeyNameConvention());
            conventions.Add(new PropertyNameConvention());
            conventions.Add(new EnumConvention());
        }

        #endregion
    }
}