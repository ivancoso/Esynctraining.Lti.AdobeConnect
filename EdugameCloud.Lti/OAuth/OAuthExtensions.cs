namespace EdugameCloud.Lti.Core.OAuth
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    /// <summary>
    /// The OAuth extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    internal static class OAuthExtensions
    {
        private static readonly string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        #region Public Methods and Operators

        /// <summary>
        /// The OAuth url encode.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public static string OAuthUrlEncode(this string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            var result = new StringBuilder();

            // Per spec, all values are utf-8 encoded first
            foreach (char symbol in Encoding.UTF8.GetBytes(value))
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.AppendFormat("%{0:X2}", (int)symbol);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// The normalize request parameters.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string NormalizeRequestParameters(this IList<QueryParameter> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            var sb = new StringBuilder();
            for (int i = 0; i < parameters.Count; i++)
            {
                QueryParameter queryParameter = parameters[i];
                sb.AppendFormat("{0}={1}", queryParameter.Name, queryParameter.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }


        #endregion

    }

}