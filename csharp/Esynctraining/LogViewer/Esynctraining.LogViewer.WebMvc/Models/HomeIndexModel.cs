using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Esynctraining.LogViewer.MvcWeb.Extensions;

namespace Esynctraining.LogViewer.MvcWeb.Models
{
    public sealed class HomeIndexModel
    {
        public List<Log> Logs { get; private set; }

        public string SelectedLogDatabase { get; set; }

        public int OutputRowCount { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string SearchTerm { get; set; }
        
        public List<SelectListItem> LogDatabases { get; set; }

        public string SelectedLogDatabaseName
        {
            get
            {
                if (SelectedLogDatabase == null)
                {
                    return null;
                }

                return SelectedLogDatabase.ToFriendlyLogDatabaseName();
            }
        }


        public HomeIndexModel()
        {
            Logs = new List<Log>();
            LogDatabases = new List<SelectListItem>();
        }

    }

}