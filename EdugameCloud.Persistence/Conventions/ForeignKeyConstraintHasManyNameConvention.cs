namespace EdugameCloud.Persistence.Conventions
{
    using System.Globalization;

    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    using Persistence;

    internal sealed class ForeignKeyConstraintHasManyNameConvention : IHasManyConvention
	{
		public void Apply(IOneToManyCollectionInstance instance)
		{
			instance.Key.ForeignKey(
				string.Format(
					CultureInfo.InvariantCulture,
					FluentConfiguration.ForeignKeyTemplate,
					instance.EntityType.Name,
					Inflector.Pluralize(instance.Member.Name)));
		}
	}
}