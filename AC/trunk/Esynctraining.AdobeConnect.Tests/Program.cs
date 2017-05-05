using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.Windsor.Diagnostics.DebuggerViews;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;
using Esynctraining.Windsor;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var tests = new AdobeConnectProxyTests();
            tests.WillCreateEventViaProxy();







            //var d = new DateTime(1482489280536);

            ////var tests = new CalcReportTests();
            ////tests.Init();
            ////tests.WillGetRecordingsStats("https://connect.fiu.edu", "mkollen", "e$ync123");

            ////RunUmdRecordingsReport();
            //var container = new WindsorContainer();
            //WindsorIoC.Initialize(container);
            //DIConfig.RegisterComponents(container);

            //var logger = IoC.Resolve<ILogger>();
            ////var accountService = new AdobeConnectAccountService(logger);
            ////IAdobeConnectProxy ac = accountService.GetProvider(new AdobeConnectAccess("http://connectdev.esynctraining.com", "sergeyi@esynctraining.com", "e$ync123"), true);

            ////var chatService = new ChatTranscriptService(ac, new ContentService(logger, ac), logger);
            ////var lastSession = ac.ReportMeetingSessions("458536").Values.Last();

            ////ChatTranscript chat = chatService.GetMeetingChatTranscript("458536", "7", lastSession.DateCreated, lastSession.DateEnd);
            ////var publicChat = chat.GetPublicChat();
            ////var privateChatGroups = chat.GetPrivateChatGroups();
            ////int re = 43;

            //var connectionDetails = new ConnectionDetails(new Uri("http://connectdev.esynctraining.com"));
            //var provider = new AdobeConnectProvider(connectionDetails);
            //var userCredentials = new UserCredentials("sergeyi@esynctraining.com", "pwd");
            //LoginResult result = provider.Login(userCredentials);
            //var meetingScoId = 458536;
            //var res = provider.ReportAssetResponseInfo(meetingScoId.ToString(), "458583");
            //var rr2 = provider.ReportQuizQuestionResponse(meetingScoId.ToString());
            //var rr3 = provider.ReportQuizInteractions(meetingScoId.ToString());
            //var rr4 = provider.ReportQuizQuestionDistribution(meetingScoId.ToString());

            //int rr = 4;


            //var seminarService = new SeminarService(logger);
            //var licenses = seminarService.GetAllSeminarLicenses(ac);

            //string licenseScoId = licenses.Last().ScoId;
            //var seminars = seminarService.GetSeminars(licenseScoId, ac);

            //string seminarScoId = seminars.First(x => x.ScoId == "1266912").ScoId;
            //var sessions = seminarService.GetSeminarSessions(seminarScoId, ac);

            //var seminar = new SeminarUpdateItem
            //{
            //    ScoId = "1429897",
            //    FolderId = licenseScoId,
            //    Description = "test dest2 3",
            //    Name = "isa tests Api - 3",
            //    Type = ScoType.meeting,
            //    Language = "en",
            //    //UrlPath = "isa",
            //};

            //ScoInfo result = seminarService.SaveSeminar(seminar, ac);

            //var res = seminarService.SaveSession(new SeminarSessionDto
            //{
            //    SeminarSessionScoId = "1429898",
            //    SeminarScoId = result.ScoId,
            //    Name = "new API 2",
            //    DateBegin = DateTime.Now.AddDays(3),
            //    DateEnd = DateTime.Now.AddDays(3).AddMinutes(45),
            //    ExpectedLoad = 15,
            //}, ac);

            ////var resDelete = seminarService.DeleteSesson("1406981", ac);

            ////var resDelete = seminarService.DeleteSeminar("1406519", ac);
        }

        void RunUmdRecordingsReport()
        {
            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);
            DIConfig.RegisterComponents(container);

            var logger = IoC.Resolve<ILogger>();
            var accountService = new AdobeConnectAccountService(logger);
            //            IAdobeConnectProxy ac = accountService.GetProvider(new AdobeConnectAccess("https://webmeeting.umd.edu/", "mike+umd@esynctraining.com", "e$ync123UMD"), true);

            var apiUrl = new Uri("https://webmeeting.umd.edu");

            var connectionDetails = new ConnectionDetails(apiUrl);
            var provider = new AdobeConnectProvider(connectionDetails);
            LoginResult loginResult = provider.Login(new UserCredentials("mike+umd@esynctraining.com", "e$ync123UMD"));

            var recordings = provider.GetRecordingsList(null); //some id here

            //            var recordingsWithoutCreate = recordings.Values.Where(x => x.DateCreated == default(DateTime)).ToList();
            //            var recordingsWithoutCreateWithEnd = recordingsWithoutCreate.Where(x => x.EndDate != default(DateTime)).ToList();
            var recordingsWithoutEnd = recordings.Values.Where(x => x.EndDate == default(DateTime)).Select(x => x.ScoId);

            var result =
                recordings.Values.Where(x => recordingsWithoutEnd.All(r => r != x.ScoId) && x.ScoId != "2434849").ToList(); //2434849 - rec without start date, can't calculate duration
            double duration = 0;
            double biggestDuration = 0;
            string biggestRecSco = null;

            var recsAfter_20150601 = result.Where(x => x.DateCreated >= new DateTime(2015, 06, 01));
            foreach (var rec in recsAfter_20150601)
            {
                var scoInfo = provider.GetScoInfo(rec.ScoId);
                var rec1 = provider.GetRecordingsList(scoInfo.ScoInfo.FolderId, rec.ScoId);
                TimeSpan ts;
                if (TimeSpan.TryParse(rec1.Values.First().Duration, out ts))
                {
                    duration += ts.TotalMinutes;
                    if (ts.TotalMinutes > biggestDuration)
                    {
                        biggestDuration = ts.TotalMinutes;
                        biggestRecSco = rec.ScoId;
                    }

                }
                else
                {
                    Console.WriteLine($"There was a problem with scoId={rec.ScoId} . Could not parse Duration={rec1.Values.First().Duration}");
                }
            }
            Console.WriteLine($"Total recordings count after 2015/05/31                   :{recsAfter_20150601.Count()}");
            Console.WriteLine($"Total recordings duration(in minutes) after 2015/05/31    :{duration}");
            Console.WriteLine($"The longest meeting duration(in minutes) after 2015/05/31 :{biggestDuration}, scoId={biggestRecSco}");
            var recsBefore20150601 = result.Where(x => x.DateCreated < new DateTime(2015, 06, 01));
            duration = 0;
            biggestDuration = 0;
            foreach (var rec in recsBefore20150601)
            {
                var scoInfo = provider.GetScoInfo(rec.ScoId);
                var rec1 = provider.GetRecordingsList(scoInfo.ScoInfo.FolderId, rec.ScoId);
                TimeSpan ts;
                if (TimeSpan.TryParse(rec1.Values.First().Duration, out ts))
                {
                    duration += ts.TotalMinutes;
                    if (ts.TotalMinutes > biggestDuration)
                    {
                        biggestDuration = ts.TotalMinutes;
                        biggestRecSco = rec.ScoId;
                    }

                }
                else
                {
                    Console.WriteLine($"There was a problem with scoId={rec.ScoId} . Could not parse Duration={rec1.Values.First().Duration}");
                }
            }
            Console.WriteLine($"Total recordings count before 2015/05/31                   :{recsBefore20150601.Count()}");
            Console.WriteLine($"Total recordings duration(in minutes) before 2015/05/31    :{duration}");
            Console.WriteLine($"The longest meeting duration(in minutes) before 2015/05/31 :{biggestDuration}, scoId={biggestRecSco}");
            Console.ReadLine();
        }

    }

}
