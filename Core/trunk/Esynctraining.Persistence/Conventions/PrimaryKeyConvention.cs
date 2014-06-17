namespace Esynctraining.Persistence.Conventions
{
    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    /// The primary key name convention.
    /// </summary>
    public class PrimaryKeyNameConvention : IIdConvention
    {
        #region Public Methods and Operators

        /// <summary>
        /// The apply.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public void Apply(IIdentityInstance instance)
        {
            instance.Column(Inflector.Uncapitalize(instance.EntityType.Name) + "Id");
        }

        #endregion
    }
}