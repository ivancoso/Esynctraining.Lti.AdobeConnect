using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Esynctraining.LogViewer.MvcWeb.Models;
using Esynctraining.LogViewer.MvcWeb.Extensions;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using System.Data.Common;

namespace Esynctraining.LogViewer.WebMvc.Controllers
{
    [Authorize]
    public class Log4NetViewerController : Controller
    {
        private const string LogDatabaseCookieName = "ldbcn";
        private const string LogTopCookieName = "ldbtop";

        private List<ConnectionStringSettings> _logDatabaseConnectionStrings;

        /// <summary>
        /// Returns the list of connection strings configured in Web.config, filtering out any whose 
        /// name doesn't start with &quot;log-database-&quot;.
        /// </summary>
        protected List<ConnectionStringSettings> LogDatabaseConnectionStrings
        {
            get
            {
                return _logDatabaseConnectionStrings ??
                    (_logDatabaseConnectionStrings = ConfigurationManager.ConnectionStrings
                        .Cast<ConnectionStringSettings>()
                        .Where(css => css.Name.StartsWith(LogDatabase.ConnectionStringPrefix))
                        .ToList());
            }
        }

        /// <summary>
        /// Creates a database connection that can be profiled by MiniProfiler.
        /// </summary>
        protected DbConnection CreateProfiledDbConnection()
        {
            var connStrName = GetSelectedConnectionStringName();
            var connStrSettings = LogDatabaseConnectionStrings.Single(css => css.Name.Equals(connStrName, StringComparison.OrdinalIgnoreCase));

            return new ProfiledDbConnection(new SqlConnection(connStrSettings.ConnectionString), MiniProfiler.Current);
        }


        #region Protected Methods

        /// <summary>
        /// Checks to see if the given connection string name exists in Web.config.
        /// </summary>
        protected bool IsConnectionStringInWebConfig(string connectionStringName)
        {
            var logDbConnectionStringNames = LogDatabaseConnectionStrings
                .Select(css => css.Name)
                .ToList();

            return logDbConnectionStringNames.Any(csn => csn.Equals(connectionStringName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Stores the user's current choice of log database in a cookie.
        /// </summary>
        protected void SetConnectionStringCookie(string logDbConnectionStringName)
        {
            if (string.IsNullOrWhiteSpace(logDbConnectionStringName))
            {
                throw new ArgumentException("Log database connection string name can't be null or whitespace", "logDbConnectionStringName");
            }

            var cookie = new HttpCookie(LogDatabaseCookieName, logDbConnectionStringName)
            {
                Expires = DateTime.Now.AddDays(30.0),
                HttpOnly = true
            };

            Response.Cookies.Add(cookie);
        }

        protected void SetTopCookie(int topRecords)
        {
            if (topRecords <= 100)
            {
                topRecords = 100;
            }

            var cookie = new HttpCookie(LogTopCookieName, topRecords.ToString())
            {
                Expires = DateTime.Now.AddDays(30.0),
                HttpOnly = true
            };

            Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Attempts to retrieve the selected database connection string name from a cookie. If not found,
        /// or if not in Web.config, it returns the first connection string's name. (First means listed
        /// first in the connection strings section.)
        /// </summary>
        protected string GetSelectedConnectionStringName()
        {
            if (LogDatabaseConnectionStrings.Count == 0)
            {
                throw new Exception("There are no configured log4net database connection strings in Web.config.");
            }

            var connStrName = GetConnectionStringNameFromCookie();

            if (string.IsNullOrWhiteSpace(connStrName) || !IsConnectionStringInWebConfig(connStrName))
            {
                // connStrName cookie is not set, or it is set, but not found in Web.config.
                return LogDatabaseConnectionStrings.First().Name;
            }

            return connStrName;
        }

        protected int GetTopValue()
        {
            int top = GetTopFromCookie();

            if (top <= 100)
                return 100;

            return top;
        }

        protected string GetAllSql(int top)
        {
            string table = GetSelectedConnectionTableName();
            return string.Format("SELECT TOP {1} * FROM {0} WHERE Level <> 'INFO' ORDER BY Id DESC", table, top.ToString());
        }

        protected string GetSearchSql(int top)
        {
            string table = GetSelectedConnectionTableName();
            return string.Format("SELECT TOP {1} * FROM {0} WHERE Level <> 'INFO' AND message LIKE @searchTerm OR exception LIKE @searchTerm ORDER BY Id DESC", table, top.ToString());
        }

        protected string GetByIdSql()
        {
            string table = GetSelectedConnectionTableName();
            return string.Format("SELECT * FROM {0} WHERE Id = @Id", table);
        }

        protected string GetTruncateSql()
        {
            string table = GetSelectedConnectionTableName();
            return string.Format("TRUNCATE TABLE {0}", table);
        }

        #endregion

        #region Private Methods

        private string GetSelectedConnectionTableName()
        {
            string logSource = GetSelectedConnectionStringName().ToFriendlyLogDatabaseName();
            return ConfigurationManager.AppSettings["LogTableName-" + logSource];
        }

        /// <summary>
        /// Attempts to read the user's selected log database connection string name from a cookie.
        /// </summary>
        private string GetConnectionStringNameFromCookie()
        {
            var cookie = Request.Cookies[LogDatabaseCookieName];

            if (cookie == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(cookie.Value))
            {
                return null;
            }

            return cookie.Value;
        }

        private int GetTopFromCookie()
        {
            var cookie = Request.Cookies[LogTopCookieName];

            if (cookie == null)
            {
                return 100;
            }

            if (string.IsNullOrWhiteSpace(cookie.Value))
            {
                return 100;
            }

            return int.Parse(cookie.Value);
        }

        #endregion

    }

}