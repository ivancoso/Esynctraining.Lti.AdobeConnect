namespace Esynctraining.Core.Extensions
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The build version extensions.
    /// </summary>
    public static class BuildVersionExtensions
    {
        #region Static Fields

        /// <summary>
        /// The regex.
        /// </summary>
        private static readonly Regex regex = new Regex(@"(\d+\.)+", RegexOptions.Compiled);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get build version.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static Version GetBuildVersion(this string fileName)
        {
            string result = string.Empty;
            int res;
            try
            {
                var firstMatch = regex.Match(fileName);
                result = firstMatch.Success ? firstMatch.Value.TrimEnd(".".ToCharArray()) : "1.0";
            }
            catch
            {
            }

            try
            {
                return new Version(result);
            }
            catch (Exception)
            {
                return new Version(1, 0);
            }
        }

        #endregion
    }
}