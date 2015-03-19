namespace EdugameCloud.WCFService.ExtendedHttpBehavior
{
    using System;
    using System.ServiceModel.Dispatcher;

    /// <summary>
    /// The extended query string converter.
    /// </summary>
    public sealed class ExtendedQueryStringConverter : QueryStringConverter
    {
        #region Public Methods and Operators

        /// <summary>
        /// The can convert.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool CanConvert(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);

            return (underlyingType != null && base.CanConvert(underlyingType)) || type == typeof(string[]) || base.CanConvert(type);
        }

        /// <summary>
        /// The convert string to value.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="parameterType">
        /// The parameter type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object ConvertStringToValue(string parameter, Type parameterType)
        {
            Type underlyingType = Nullable.GetUnderlyingType(parameterType);

            // Handle nullable types
            if (underlyingType != null)
            {
                // Define a null value as being an empty or missing (null) string passed as the query parameter value
                return string.IsNullOrEmpty(parameter) ? null : base.ConvertStringToValue(parameter, underlyingType);
            }

            if (parameterType == typeof(string[]))
            {
                string[] parms = parameter.Split(',');
                return parms;
            }

            return base.ConvertStringToValue(parameter, parameterType);
        }

        public override string ConvertValueToString(object parameter, Type parameterType)
        {
            if (parameterType == typeof(string[]))
            {
                string valstring = string.Join(",", parameter as string[]);
                return valstring;
            }

            return base.ConvertValueToString(parameter, parameterType);
        }

        #endregion
    }
}