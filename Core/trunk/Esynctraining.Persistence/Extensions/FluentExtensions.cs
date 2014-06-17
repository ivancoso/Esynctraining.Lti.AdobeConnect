namespace Esynctraining.Persistence.Extensions
{
    using FluentNHibernate.Mapping;

    /// <summary>
    /// The fluent extensions.
    /// </summary>
    public static class FluentExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The default.
        /// </summary>
        /// <param name="propertyPart">
        /// The property part.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="PropertyPart"/>.
        /// </returns>
        public static PropertyPart Default<T>(this PropertyPart propertyPart)
        {
            return propertyPart.Default(default(T).ToString());
        }

        #endregion
    }
}