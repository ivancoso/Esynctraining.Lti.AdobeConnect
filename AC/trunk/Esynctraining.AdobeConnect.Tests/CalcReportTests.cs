using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.Windsor;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;
using Esynctraining.Windsor;
using log4net.Config;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.Tests
{
    public class CalcReportTests
    {
        private ConnectionDetails _connectionDetails;
        private AdobeConnectProvider _provider;
        private ILogger _logger;

        [SetUp]
        public void Init()
        {
            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);
            DIConfig.RegisterComponents(container);
            XmlConfigurator.Configure();

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
            _logger = IoC.Resolve<ILogger>();
        }

        //[TestCase("http://connectdev.esynctraining.com/api/xml", "anton@esynctraining.com", "Welcome1")]
        //[TestCase("https://webmeeting.umd.edu/api/xml", "mike+umd@esynctraining.com", "e$ync123UMD")]
        [TestCase("https://connect.fiu.edu/api/xml", "mkollen", "e$ync123", 30)]
        public void WillGetRecordingsStats(string apiUrl, string login, string password, int totalObjCount = 0)
        {
            _connectionDetails.ServiceUrl = apiUrl;
            _provider = new AdobeConnectProvider(_connectionDetails);
            var provider = _provider;
            LoginResult loginResult = provider.Login(new UserCredentials(login, password));
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");
            var recordings = provider.ReportRecordingsPaged(totalObjCount);
            var report1 = GetLastYearNoViews(recordings);
            var report2 = GetLastViewMoreThan2Years(recordings);
            LogRecStats(report1);
            LogRecStats(report2);
            var report34 = GetRecLengthStats(recordings);
            LogRecStats(report34);
        }

        private void LogRecStats<T>(T report34)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in props)
            {
                _logger.Info(propertyInfo.Name + " " + propertyInfo.GetValue(report34));
                Console.WriteLine(propertyInfo.Name + " " + propertyInfo.GetValue(report34));
            }
        }

        [TestCase("https://connect.fiu.edu/api/xml", "mkollen", "e$ync123", "17131049")]
        public void WillParseDurationForSco(string apiUrl, string login, string password, string sco)
        {
            _connectionDetails.ServiceUrl = apiUrl;
            _provider = new AdobeConnectProvider(_connectionDetails);
            var provider = _provider;
            LoginResult loginResult = provider.Login(new UserCredentials(login, password));
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");
            //var recordings = provider.ReportRecordingsPaged();
            var report34 = GetDurationOfSingleRec(sco);
            LogRecStats(report34);
        }

        private DurationShortStats GetLastYearNoViews(IEnumerable<ScoContentCollectionResult> recordings)
        {
            var result = 0;
            var year = DateTime.Today.Year - 1;
            var month = DateTime.Today.Month;
            var day = DateTime.Today.Day;
            var counter = 0;
            var duration = 0.0;
            //foreach (var recording in recordings)
            Parallel.ForEach(recordings, (recording) =>
            {
                _logger.Info($"Count {recording.Values.Count()}");
                if (recording.Values == null)
                    return;

                //var lastYearRecs = recording.Values.Where(x => x.DateCreated >= new DateTime(year, month, day));
                var data = recording.Values;
                foreach (var rec in data)
                {
                    //var scoInfo = provider.GetScoInfo(lastYearRec.ScoId);
                    var scoInfo = _provider.ReportScoViews(rec.ScoId);
                    if (scoInfo.Values == null)
                        continue;
                    if (scoInfo.Values.Count() > 1)
                        throw new InvalidOperationException("Should be one item");
                    var content = scoInfo.Values.ToList()[0];
                    if (content.Views == 0 && rec.DateCreated > new DateTime(year, month, day))
                    {
                        counter++;
                        duration += GetDurationOfSingleRec(rec.ScoId);
                    }
                }


            });
            _logger.Info($"Number of > 1 year no views {counter}");
            
            return new DurationShortStats()
            {
                Count = counter,
                Duration = duration
            };
        }

        private DurationShortStats GetLastViewMoreThan2Years(IEnumerable<ScoContentCollectionResult> recordings)
        {
            //var year = DateTime.Today.Year - 2;
            //var month = DateTime.Today.Month;
            //var day = DateTime.Today.Day;
            var counter = 0;
            var duration = 0.0;
            Parallel.ForEach(recordings, (recording) =>
            {
                {
                    _logger.Info($"Count {recording.Values.Count()}");
                    if (recording.Values == null)
                        return;

                    //var lastYearRecs = recording.Values.Where(x => x.DateCreated >= new DateTime(year, month, day));
                    var data = recording.Values;

                    foreach (var rec in data)
                    {
                        //var scoInfo = provider.GetScoInfo(lastYearRec.ScoId);
                        var scoInfo = _provider.ReportScoViews(rec.ScoId);
                        if (scoInfo.Values == null)
                            continue;
                        if (scoInfo.Values.Count() > 1)
                            throw new InvalidOperationException("Should be one item");
                        if (scoInfo.Values.ToList()[0].Views > 0 &&
                            scoInfo.Values.ToList()[0].LastViewedDate + TimeSpan.FromDays(365*2) < DateTime.Today)
                        {
                            counter++;
                            duration += GetDurationOfSingleRec(rec.ScoId);
                        }
                    }
                }
            });
                
            _logger.Info($"Number of > 1 year no views {counter}");
            return new DurationShortStats()
            {
                Count = counter,
                Duration = duration
            };
        }

        public double GetDurationOfSingleRec(string scoId)
        {
            var scoInfo = _provider.GetScoInfo(scoId);
            if (scoInfo.ScoInfo == null)
            {
                _logger.Info($"sco-id has no sco-info sco-id={scoId}");
                return 0;
            }
                
            var rec1 = _provider.GetRecordingsList(scoInfo.ScoInfo.FolderId, scoId);
            TimeSpan ts;
            if (rec1.Values == null || !rec1.Values.Any())
            {
                _logger.Info($"sco-id has no Values for list-recordings sco-id={scoId}");
                return 0;
            }

            var duration = rec1.Values.First().Duration;
            if (string.IsNullOrEmpty(duration))
                return 0;
            var result = DurationParser.Parse(duration);
            return result.TotalMinutes;
        }

        public DurationStats GetRecLengthStats(IEnumerable<ScoContentCollectionResult> recordings)
        {
            var counter10hours = 0;
            var counter10hoursDur = 0.0;
            var counter3to10hours = 0;
            var counter3to10hoursDur = 0.0;
            var duration = 0.0;
            double biggestDuration = 0;
            string biggestRecSco = null;

            Parallel.ForEach(recordings.ToList(), (rec, y) =>
            {
                Parallel.ForEach(rec.Values, (x, state) =>
                {

                    var recDuration = GetDurationOfSingleRec(x.ScoId);
                    duration += recDuration;
                    if (recDuration > 10 * 60)
                    {
                        counter10hours++;
                        counter10hoursDur += recDuration;
                    }
                    if ((recDuration > 3 * 60) && (recDuration < 10 * 60))
                    {
                        counter3to10hours++;
                        counter3to10hoursDur += recDuration;
                    }
                    if (recDuration > biggestDuration)
                    {
                        biggestDuration = recDuration;
                        biggestRecSco = x.ScoId;
                    }

                });

            });

            return new DurationStats()
            {
                Counter10hours = counter10hours,
                Counter3to10hours = counter3to10hours,
                BiggestDuration = biggestDuration,
                BiggestSco = biggestRecSco,
                Duration = duration,
                Counter10hoursDur = counter3to10hoursDur,
                Counter3to10hoursDur = counter3to10hoursDur
            };
        }
    }

    public class DurationStats
    {
        public int Counter10hours { get; set; }
        public int Counter3to10hours { get; set; }
        public double Duration { get; set; }
        public double Counter10hoursDur { get; set; }
        public double Counter3to10hoursDur { get; set; }
        public double BiggestDuration { get; set; }
        public string BiggestSco { get; set; }

    }

    public class DurationShortStats
    {
        public int Count { get; set; }
        public double Duration { get; set; }
    }
}