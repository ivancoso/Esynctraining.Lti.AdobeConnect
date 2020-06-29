namespace EdugameCloud.Persistence.Conventions
{
    using System.Globalization;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.AcceptanceCriteria;
    using FluentNHibernate.Conventions.Inspections;
    using FluentNHibernate.Conventions.Instances;

    using Persistence;

    internal sealed class TableNameConvention : IClassConvention, IClassConventionAcceptance
	{
		public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
		{
			criteria.Expect(x => x.TableName, Is.Not.Set);
		}

		public void Apply(IClassInstance instance)
		{
			instance.Table(
				string.Format(
					CultureInfo.InvariantCulture,
					FluentConfiguration.TableNameTemplate,
					instance.EntityType.Name));
		}
	}
}