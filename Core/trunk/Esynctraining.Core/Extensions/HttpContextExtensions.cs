namespace Esynctraining.Core.Extensions
{
    using System.Web;

    /// <summary>
    /// The http context extensions.
    /// </summary>
    public static class HttpContextExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get IP address.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetIPAddress()
        {
            HttpContext context = HttpContext.Current;
            return GetIPAddress(context);
        }

        /// <summary>
        /// The get IP address.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetIPAddress(this HttpContext context)
        {
            string address = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(address))
            {
                string[] addresses = address.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        #endregion
    }
}