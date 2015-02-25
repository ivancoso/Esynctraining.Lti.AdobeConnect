namespace DotAmf.Serialization.TypeAdapters
{
    using System;

    /// <summary>
    /// The decimal type adapter.
    /// </summary>
    public class GuidTypeAdapter : BaseTypeAdapter<Guid, Guid?>
    {
        public override object Adapt(Type type, object value)
        {
            if (type == typeof(Guid))
            {
                var convertedValue = this.Convert(value);
                return convertedValue.HasValue ? convertedValue.Value : value;
            }

            if (type == typeof(Guid?))
            {
                return this.Convert(value);
            }

            return null;
        }

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private Guid? Convert(object value)
        {
            if (value != null)
            {
                Guid result;
                if (Guid.TryParse(value.ToString(), out result))
                {
                    return result;
                }
            }

            return Guid.Empty;
        }
    }
}
