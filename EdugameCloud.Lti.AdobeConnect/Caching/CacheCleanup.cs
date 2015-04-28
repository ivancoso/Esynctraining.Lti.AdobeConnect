using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EdugameCloud.Lti.AdobeConnect.Caching
{
    internal sealed class CacheCleanup
    {
        public static void CleanAllPrincipalCache()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "TRUNCATE TABLE AcCachePrincipal";

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }

}
