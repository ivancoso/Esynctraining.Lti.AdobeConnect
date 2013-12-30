namespace Esynctraining.Core.Extensions
{
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
        private static readonly Regex regex = new Regex(@"\d", RegexOptions.Compiled);

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
        public static int GetBuildVersion(this string fileName)
        {
            string result = string.Empty;
            int res;
            try
            {
                result = regex.Matches(fileName).Cast<Match>().Aggregate(result, (current, m) => current + m.Value);
            }
            catch
            {
            }

            if (int.TryParse(result, out res))
            {
                return res;
            }

            return 0;
        }

        #endregion
    }
}