using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Windsor;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.Windsor;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.Tests
{
    public class CalcReportTests
    {
        private ConnectionDetails _connectionDetails;
        private AdobeConnectProvider _provider;

        [SetUp]
        public void Init()
        {
            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);
            DIConfig.RegisterComponents(container);

            _connectionDetails = new ConnectionDetails
            {
                EventMaxParticipants = 10,
                Proxy = new ProxyCredentials
                    {
                        Domain = string.Empty,
                        Login = string.Empty,
                        Password = string.Empty,
                        Url = string.Empty,
                    },
            };
        }

        //[TestCase("http://connectdev.esynctraining.com/api/xml", "anton@esynctraining.com", "Welcome1")]
        //[TestCase("https://webmeeting.umd.edu/api/xml", "mike+umd@esynctraining.com", "e$ync123UMD")]
        [TestCase("https://connect.fiu.edu/api/xml", "mkollen", "e$ync123")]
        public void WillGetRecordingsStats(string apiUrl, string login, string password)
        {
            _connectionDetails.ServiceUrl = apiUrl;
            _provider = new AdobeConnectProvider(_connectionDetails);
            var provider = _provider;
            LoginResult loginResult = provider.Login(new UserCredentials(login, password));
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");
            var recordings = provider.ReportRecordingsPaged();
            //var report1 = GetLastYearNoViews(recordings);
            //var report2 = GetLastViewMoreThan2Years(recordings);
            var report34 = GetRecLengthStats(recordings);
        }

        private int GetLastYearNoViews(IEnumerable<ScoContentCollectionResult> recordings)
        {
            var result = 0;
            var year = DateTime.Today.Year - 1;
            var month = DateTime.Today.Month;
            var day = DateTime.Today.Day;
            var counter = 0;
            foreach (var recording in recordings)
            {
                Console.WriteLine(@"Count {0}", recording.Values.Count());
                if (recording.Values == null)
                    continue;
                    
                var lastYearRecs = recording.Values.Where(x => x.DateCreated >= new DateTime(year, month, day));

                foreach (var lastYearRec in lastYearRecs)
                {
                    //var scoInfo = provider.GetScoInfo(lastYearRec.ScoId);
                    var scoInfo = _provider.ReportScoViews(lastYearRec.ScoId);
                    if (scoInfo.Values == null)
                        continue;
                    if (scoInfo.Values.Count() > 1) 
                        throw  new InvalidOperationException("Should be one item");
                    if (scoInfo.Values.ToList()[0].Views == 0)
                    {
                        counter++;
                    }
                }
                
            }
            Console.WriteLine(@"Number of > 1 year no views {0}", counter);
            return counter;
        }

        private int GetLastViewMoreThan2Years(IEnumerable<ScoContentCollectionResult> recordings)
        {
            //var year = DateTime.Today.Year - 2;
            //var month = DateTime.Today.Month;
            //var day = DateTime.Today.Day;
            var counter = 0;
            foreach (var recording in recordings)
            {
                Console.WriteLine(@"Count {0}", recording.Values.Count());
                if (recording.Values == null)
                    continue;

                //var lastYearRecs = recording.Values.Where(x => x.DateCreated >= new DateTime(year, month, day));
                var data = recording.Values;

                foreach (var lastYearRec in data)
                {
                    //var scoInfo = provider.GetScoInfo(lastYearRec.ScoId);
                    var scoInfo = _provider.ReportScoViews(lastYearRec.ScoId);
                    if (scoInfo.Values == null)
                        continue;
                    if (scoInfo.Values.Count() > 1)
                        throw new InvalidOperationException("Should be one item");
                    if (scoInfo.Values.ToList()[0].Views > 0 && scoInfo.Values.ToList()[0].LastViewedDate + TimeSpan.FromDays(365*2) < DateTime.Today)
                    {
                        counter++;
                    }
                }

            }
            Console.WriteLine(@"Number of > 1 year no views {0}", counter);
            return counter;
        }

        public DurationStats GetRecLengthStats(IEnumerable<ScoContentCollectionResult> recordings)
        {
            var counter10hours = 0;
            var counter3to10hours = 0;

            Parallel.ForEach(recordings.ToList(), (rec, y) =>
            {
                Parallel.ForEach(rec.Values, (x, state) =>
                {
                    var scoInfo = _provider.GetScoInfo(x.ScoId);
                    if (scoInfo.ScoInfo == null)
                        return;
                    var rec1 = _provider.GetRecordingsList(scoInfo.ScoInfo.FolderId, x.ScoId);
                    TimeSpan ts;
                    if (TimeSpan.TryParse(rec1.Values.First().Duration, out ts))
                    {

                        if (ts.TotalMinutes > 10 * 60)
                        {
                            counter10hours++;
                        }
                        if ((ts.TotalMinutes > 3 * 60) && (ts.TotalMinutes < 10 * 60))
                        {
                            counter3to10hours++;
                        }
                    }
                    else
                    {
                        Console.WriteLine(
                            $"There was a problem with scoId={x.ScoId} . Could not parse Duration={rec1.Values.First().Duration}");
                    }
                });
                
            });
            return new DurationStats()
            {
                Counter10hours = counter10hours,
                Counter3to10hours = counter3to10hours
            };
        }
    }

    public class DurationStats
    {
        public int Counter10hours { get; set; }
        public int Counter3to10hours { get; set; }
        
    }
}