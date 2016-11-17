using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.AdobeConnect.Tests;

namespace Esynctraining.ReportRunner
{
    public class Program
    {
        static void Main()
        {
            var report = new CalcReportTests();
            report.Init();
            report.WillGetRecordingsStatsForCCA("https://cca.acms.com/api/xml", "developer@esynctraining.com", "Welcome1");
            
        }
    }
}
