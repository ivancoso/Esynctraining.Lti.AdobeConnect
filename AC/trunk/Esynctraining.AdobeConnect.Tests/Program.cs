using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;
using Esynctraining.Windsor;

namespace Esynctraining.AdobeConnect.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);
            DIConfig.RegisterComponents(container);

            var logger = IoC.Resolve<ILogger>();
            var accountService = new AdobeConnectAccountService(logger);
            IAdobeConnectProxy ac = accountService.GetProvider(new AdobeConnectAccess("https://connect.esynctraining.com", "sergeyi@esynctraining.com", "Qrazy123"), true);

            var seminarService = new SeminarService(logger);
            var licenses = seminarService.GetAllSeminarLicenses(ac);

            string licenseScoId = licenses.Last().ScoId;
            var seminars = seminarService.GetSeminars(licenseScoId, ac);

            string seminarScoId = seminars.First(x => x.ScoId == "1266912").ScoId;
            var sessions = seminarService.GetSeminarSessions(seminarScoId, ac);

            var seminar = new SeminarUpdateItem
            {
                ScoId = "1429897",
                FolderId = licenseScoId,
                Description = "test dest2 3",
                Name = "isa tests Api - 3",
                Type = ScoType.meeting,
                Language = "en",
                //UrlPath = "isa",
            };

            ScoInfo result = seminarService.SaveSeminar(seminar, ac);

            var res = seminarService.SaveSession(new SeminarSessionDto
            {
                SeminarSessionScoId = "1429898",
                SeminarScoId = result.ScoId,
                Name = "new API 2",
                DateBegin = DateTime.Now.AddDays(3),
                DateEnd = DateTime.Now.AddDays(3).AddMinutes(45),
                ExpectedLoad = 15,
            }, ac);

            //var resDelete = seminarService.DeleteSesson("1406981", ac);

            //var resDelete = seminarService.DeleteSeminar("1406519", ac);
        }

    }

}
