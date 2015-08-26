using System.Linq;
using System.Web.Mvc;
using Dapper;
using Esynctraining.LogViewer.MvcWeb.Models;
using StackExchange.Profiling;

namespace Esynctraining.LogViewer.WebMvc.Controllers
{
    //CREATE TABLE[dbo].[Log] (
    //    [Id]
    //    [int] IDENTITY(1, 1) NOT NULL,
    //    [Date] [datetime]
    //    NOT NULL,
    //    [Thread] [varchar] (255) NOT NULL,
    //    [Level] [varchar] (50) NOT NULL,
    //    [Logger] [varchar] (255) NOT NULL,
    //    [Message] [varchar] (4000) NOT NULL,
    //    [Exception] [varchar] (2000) NULL
    //)

    public class HomeController : Log4NetViewerController
    {
        public ActionResult Index()
        {
            var model = new HomeIndexModel();
            var profiler = MiniProfiler.Current;

            using (profiler.Step("Getting logs from database"))
            using (var sqlConn = CreateProfiledDbConnection())
            {
                model.Logs.AddRange(sqlConn.Query<Log>("SELECT TOP 100 * FROM Log ORDER BY Id DESC"));
            }

            model.SelectedLogDatabase = GetSelectedConnectionStringName();
            model.LogDatabases.AddRange(LogDatabaseConnectionStrings.Select(css => new SelectListItem
            {
                Text = css.Name.Replace(LogDatabase.ConnectionStringPrefix, string.Empty),
                Value = css.Name,
            }));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HomeIndexModel model)
        {
            if (IsConnectionStringInWebConfig(model.SelectedLogDatabase))
            {
                SetConnectionStringCookie(model.SelectedLogDatabase);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(HomeIndexModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SelectedLogDatabase = GetSelectedConnectionStringName();
                model.LogDatabases.AddRange(LogDatabaseConnectionStrings.Select(css => new SelectListItem
                {
                    Text = css.Name.Replace(LogDatabase.ConnectionStringPrefix, string.Empty),
                    Value = css.Name,
                }));

                return View("Index", model);
            }

            var profiler = MiniProfiler.Current;

            using (profiler.Step("Getting logs from database"))
            using (var sqlConn = CreateProfiledDbConnection())
            {
                var searchTerm = "%" + model.SearchTerm.Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]") + "%";
                model.Logs.AddRange(sqlConn.Query<Log>("SELECT TOP 100 * FROM Log WHERE message LIKE @searchTerm OR exception LIKE @searchTerm ORDER BY Id DESC",
                    new { searchTerm }));
            }

            model.SelectedLogDatabase = GetSelectedConnectionStringName();
            model.LogDatabases.AddRange(LogDatabaseConnectionStrings.Select(css => new SelectListItem
            {
                Text = css.Name.Replace(LogDatabase.ConnectionStringPrefix, string.Empty),
                Value = css.Name,
            }));

            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TruncateLogs()
        {
            var profiler = MiniProfiler.Current;

            using (profiler.Step("Truncating logs in database"))
            using (var sqlConn = CreateProfiledDbConnection())
            {
                sqlConn.Execute("TRUNCATE TABLE [Log]");
            }

            return RedirectToAction("Index", "Home");
        }

    }
}