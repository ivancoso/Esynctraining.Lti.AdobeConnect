using System.Linq;
using System.Web.Mvc;
using Dapper;
using Esynctraining.LogViewer.MvcWeb.Extensions;
using Esynctraining.LogViewer.MvcWeb.Models;
using StackExchange.Profiling;

namespace Esynctraining.LogViewer.WebMvc.Controllers
{
    public class LogController : Log4NetViewerController
    {
        public ActionResult Detail(int id)
        {
            var model = new LogDetailModel
            {
                LogDatabaseName = GetSelectedConnectionStringName().ToFriendlyLogDatabaseName(),
            };
            var profiler = MiniProfiler.Current;

            using (profiler.Step("Getting log details from database"))
            using (var sqlConn = CreateProfiledDbConnection())
            {
                model.Log = sqlConn
                    .Query<Log>("SELECT * FROM Log WHERE Id = @Id", new { Id = id })
                    .SingleOrDefault();
            }

            if (model.Log == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

    }

}