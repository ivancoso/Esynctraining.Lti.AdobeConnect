namespace EdugameCloud.Persistence.Conventions
{
    using System.Globalization;

    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    using Persistence;

    public class ForeignKeyConstraintHasManyToManyNameConvention : IHasManyToManyConvention
	{
		public void Apply(IManyToManyCollectionInstance instance)
		{
			instance.Key.ForeignKey(
				string.Format(
					CultureInfo.InvariantCulture,
					FluentConfiguration.ForeignKeyTemplate,
					Inflector.Pluralize(instance.EntityType.Name),
					Inflector.Pluralize(instance.Member.Name)));
		}
	}
}