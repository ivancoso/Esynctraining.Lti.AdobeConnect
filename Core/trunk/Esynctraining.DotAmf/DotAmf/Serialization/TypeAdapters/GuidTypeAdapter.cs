namespace DotAmf.Serialization.TypeAdapters
{
    using System;

    public sealed class GuidTypeAdapter : BaseTypeAdapter<Guid, Guid?>
    {
        public override object Adapt(RuntimeTypeHandle type, object value)
        {
            if (typeof(Guid).TypeHandle.Equals(type))
            {
                var convertedValue = Convert(value);
                return convertedValue.HasValue ? convertedValue.Value : value;
            }

            if (typeof(Guid?).TypeHandle.Equals(type))
            {
                return Convert(value);
            }

            return null;
        }

        private static Guid? Convert(object value)
        {
            if (value != null)
            {
                string str = value.ToString();
                if (string.IsNullOrWhiteSpace(str))
                    return null;

                Guid result;
                if (Guid.TryParse(str, out result))
                {
                    return result;
                }
            }

            return null;
        }

    }

}
