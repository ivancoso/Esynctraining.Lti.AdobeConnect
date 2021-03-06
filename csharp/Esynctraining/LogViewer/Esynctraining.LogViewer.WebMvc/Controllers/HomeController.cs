﻿using System.Linq;
using System.Web.Mvc;
using Dapper;
using Esynctraining.LogViewer.MvcWeb.Models;
using StackExchange.Profiling;

namespace Esynctraining.LogViewer.WebMvc.Controllers
{
    public class HomeController : Log4NetViewerController
    {
        public ActionResult Index()
        {
            var model = new HomeIndexModel();
            var profiler = MiniProfiler.Current;

            using (profiler.Step("Getting logs from database"))
            using (var sqlConn = CreateProfiledDbConnection())
            {
                model.Logs.AddRange(sqlConn.Query<Log>(GetAllSql(GetTopValue())));
            }

            model.SelectedLogDatabase = GetSelectedConnectionStringName();
            model.OutputRowCount = GetTopValue();
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
                SetTopCookie(model.OutputRowCount);
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
                model.OutputRowCount = GetTopValue();
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
                model.Logs.AddRange(sqlConn.Query<Log>(GetSearchSql(GetTopValue()),
                    new { searchTerm }));
            }

            model.SelectedLogDatabase = GetSelectedConnectionStringName();
            model.OutputRowCount = GetTopValue();
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
                sqlConn.Execute(GetTruncateSql());
            }

            return RedirectToAction("Index", "Home");
        }

    }
}