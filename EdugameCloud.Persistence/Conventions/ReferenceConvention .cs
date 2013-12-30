namespace EdugameCloud.Persistence.Conventions
{
    using System.Globalization;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    using Persistence;

    public class ReferenceNameConvention : IReferenceConvention
	{
		public void Apply(IManyToOneInstance instance)
		{
			instance.Column(string.Format(
				CultureInfo.InvariantCulture,
				FluentConfiguration.ReferenceNameTemplate,
				instance.Property.PropertyType.Name));
		}
	}
}