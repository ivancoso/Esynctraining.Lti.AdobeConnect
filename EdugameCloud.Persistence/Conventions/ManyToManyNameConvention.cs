namespace EdugameCloud.Persistence.Conventions
{
    using System.Globalization;

    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    using Persistence;

    internal sealed class ManyToManyNameConvention : IHasManyToManyConvention
	{
		public void Apply(IManyToManyCollectionInstance instance)
		{
			var parentName = instance.EntityType.Name;
			var childName = instance.ChildType.Name;

			if (parentName.CompareTo(childName) < 0)
			{
				instance.Table(
					string.Format(
						CultureInfo.InvariantCulture,
						FluentConfiguration.TableNameTemplate,
						string.Format(
							CultureInfo.InvariantCulture,
							FluentConfiguration.ManyToManyTemplate,
							Inflector.Pluralize(parentName),
							Inflector.Pluralize(childName))));
			}
			else
			{
				instance.Table(
					string.Format(
						CultureInfo.InvariantCulture,
						FluentConfiguration.TableNameTemplate,
						string.Format(
							CultureInfo.InvariantCulture,
							FluentConfiguration.ManyToManyTemplate,
							Inflector.Pluralize(childName),
							Inflector.Pluralize(parentName))));
			}
		}
	}
}