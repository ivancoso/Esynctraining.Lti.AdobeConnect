using Esynctraining.LogViewer.MvcWeb.Models;

namespace Esynctraining.LogViewer.MvcWeb.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// A connection string name from Web.config.
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        public static string ToFriendlyLogDatabaseName(this string connectionStringName)
        {
            if (string.IsNullOrWhiteSpace(connectionStringName))
            {
                return null;
            }

            return connectionStringName.Replace(LogDatabase.ConnectionStringPrefix, string.Empty);
        }

    }

}