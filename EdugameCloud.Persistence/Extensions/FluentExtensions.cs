namespace EdugameCloud.Persistence.Extensions
{
    using FluentNHibernate.Mapping;

    public static class FluentExtensions
    {
        #region Public Methods and Operators

        public static PropertyPart Default<T>(this PropertyPart propertyPart)
        {
            return propertyPart.Default(default(T).ToString());
        }

        #endregion
    }
}