using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdugameCloud.Lti.Persistence
{
    using System.Reflection;

    using EdugameCloud.Lti.Persistence.Mappings;

    using Esynctraining.Persistence;

    using FluentNHibernate;

    using NHibernate.Cfg;

    /// <summary>
    /// The fluent configuration.
    /// </summary>
    public class FluentConfiguration : Esynctraining.Persistence.FluentConfiguration
    {
        #region Public Methods and Operators

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="persistenceModel">
        /// The persistence model.
        /// </param>
        /// <param name="cfg">
        /// The config.
        /// </param>
        protected override void Configure(PersistenceModel persistenceModel, Configuration cfg)
        {
            var executing = Assembly.GetAssembly(typeof(FluentConfiguration));
            cfg.AddAssembly(executing);
            persistenceModel.AddMappingsFromSource(new NameSpaceTypeSource(executing, typeof(LmsUserMap).Namespace));
            base.Configure(persistenceModel, cfg);
        }

        #endregion
    }
}
