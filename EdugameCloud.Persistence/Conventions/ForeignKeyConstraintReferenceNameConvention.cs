namespace EdugameCloud.Persistence.Conventions
{
    using System.Globalization;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    using Persistence;

    public class ForeignKeyConstraintReferenceNameConvention : IReferenceConvention
	{
		public void Apply(IManyToOneInstance instance)
		{
			instance.ForeignKey(string.Format(CultureInfo.InvariantCulture, FluentConfiguration.ForeignKeyTemplate, instance.EntityType.Name, instance.Name));
		}
	}
}