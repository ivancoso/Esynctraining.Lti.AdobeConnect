using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.AdobeConnect.Tests;
using Microsoft.Extensions.Configuration;


//using Esynctraining.AdobeConnect.Tests;
//using Microsoft.Extensions.Configuration;

namespace Esynctraining.ReportRunner
{
    public class Program
    {
        
        static void Main()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var acUrl = config["ac:url"];
            var login = config["ac:user"];
            var pass = config["ac:password"];

            var report = new CalcReportTests();
            var smtpSettings = new SmtpSettings(config["email:host"], config["email:username"], config["email:password"], config["email:sendFrom"], config["email:sendTo"]);
            report.Init(smtpSettings);
            report.WillGetRecordingsStatsForCCA(acUrl, login, pass);
        }
    }
}
