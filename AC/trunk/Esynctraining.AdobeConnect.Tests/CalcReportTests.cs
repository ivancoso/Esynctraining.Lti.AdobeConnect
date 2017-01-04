using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.Tests
{
    [TestFixture]
    public class CalcReportTests
    {
        private ConnectionDetails _connectionDetails;
        private AdobeConnectProvider _provider;
        private ILogger _logger;
        private ConcurrentDictionary<RecIdentity, double> _recStorage = new ConcurrentDictionary<RecIdentity, double>();
        static public IConfigurationRoot Configuration { get; set; }

        [SetUp]
        public void Init()
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
           //.AddJsonFile("appsettings.json");

            //Configuration = builder.Build();

            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);
            DIConfig.RegisterComponents(container);
            XmlConfigurator.Configure();

            _logger = IoC.Resolve<ILogger>();
        }

        //[TestCase("https://middlebury.adobeconnect.com", "mike+wtu@esynctraining.com", "2016", 0)]
        //[TestCase("https://caminos.adobeconnect.com", "mesync", "user123", 0)]
        //[TestCase("http://connectdev.esynctraining.com", "anton@esynctraining.com", "Welcome1")]
        //[TestCase("https://webmeeting.umd.edu", "mike+umd@esynctraining.com", "e$ync123UMD")]
        //[TestCase("https://connect.fiu.edu", "mkollen", "e$ync123")]
        //[TestCase("http://connect.uthsc.edu", "itsadmin", "Memphis2016", 0)]
        public void WillGetRecordingsStats(string apiUrl, string login, string password, int totalObjCount = 0)
        {
            _connectionDetails = new ConnectionDetails(new Uri(apiUrl));
            _provider = new AdobeConnectProvider(_connectionDetails);
            var provider = _provider;
            LoginResult loginResult = provider.Login(new UserCredentials(login, password));
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");
            var recordings = provider.ReportRecordingsPaged(totalObjCount);

            var reports = GetRecLengthStats(recordings);
            foreach (var rec in _recStorage)
            {
                _logger.Info($"key: {rec.Key.Sco}, {rec.Key.FolderId}, {rec.Value}");
            }
            LogRecStats(reports);
        }

        [TestCase("https://rhi.adobeconnect.com", "mike@esynctraining.com", "Esync321", 0)]
        public void WillGetRecordingsStatsForRHI(string apiUrl, string login, string password, int totalObjCount = 0)
        {
            _connectionDetails = new ConnectionDetails(new Uri(apiUrl));
            _provider = new AdobeConnectProvider(_connectionDetails);
            var provider = _provider;
            LoginResult loginResult = provider.Login(new UserCredentials(login, password));
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");
            var recordings = provider.ReportRecordingsPaged(totalObjCount);

            var reports = GetRecLengthStats(recordings);
            foreach (var rec in _recStorage)
            {
                _logger.Info($"key: {rec.Key.Sco}, {rec.Key.FolderId}, {rec.Value}");
            }
            LogRecStats(reports);

        }

        [TestCase("https://cca.acms.com", "developer@esynctraining.com", "Welcome1", 0)]
        public void WillGetRecordingsStatsForCCA(string apiUrl, string login, string password, int totalObjCount = 0)
        {
            _connectionDetails = new ConnectionDetails(new Uri(apiUrl));
            _provider = new AdobeConnectProvider(_connectionDetails);
            var provider = _provider;
            LoginResult loginResult = provider.Login(new UserCredentials(login, password));
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");
            var recordings = provider.ReportRecordingsPaged(totalObjCount);

            var reports = GetRecLengthStats(recordings);
            foreach (var rec in _recStorage)
            {
                _logger.Info($"key: {rec.Key.Sco}, {rec.Key.FolderId}, {rec.Value}");
            }
            LogRecStats(reports);

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

        [TestCase("https://connect.fiu.edu", "mkollen", "e$ync123", "17131049")]
        public void WillParseDurationForSco(string apiUrl, string login, string password, string sco)
        {
            _connectionDetails = new ConnectionDetails(new Uri(apiUrl));
            _provider = new AdobeConnectProvider(_connectionDetails);
            var provider = _provider;
            LoginResult loginResult = provider.Login(new UserCredentials(login, password));
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");
            //var recordings = provider.ReportRecordingsPaged();
            var reports = GetDurationOfSingleRec(sco);
            LogRecStats(reports);
        }

        public double GetDurationOfSingleRec(string scoId)
        {
            if (string.IsNullOrEmpty(scoId))
            {
                _logger.Error("sco-id should not be empty");
                return 0;
            }
            var scoInfo = _provider.GetScoInfo(scoId);
            if (scoInfo.ScoInfo == null)
            {
                _logger.Error($"sco-id has no sco-info sco-id={scoId}");
                return 0;
            }

            var rec1 = _provider.GetRecordingsList(scoInfo.ScoInfo.FolderId, scoId);
            if (rec1.Values == null || !rec1.Values.Any())
            {
                _logger.Error($"sco-id has no Values for list-recordings sco-id={scoId}");
                return 0;
            }

            var duration = rec1.Values.First().Duration;
            if (string.IsNullOrEmpty(duration))
                return 0;

            var result = DurationParser.Parse(duration);

            var recIdentity = new RecIdentity(scoId, scoInfo.ScoInfo.FolderId);

            var addOperationResult = _recStorage.TryAdd(recIdentity, result.TotalMinutes);
            if (!addOperationResult) return 0;

            _logger.Info(
                $"folderId {scoInfo.ScoInfo.FolderId} sco-id {scoId}, incoming duration string {duration}, duration totalMins {result.TotalMinutes}");
            Console.WriteLine(
                $"folderId {scoInfo.ScoInfo.FolderId} sco-id {scoId}, incoming duration string {duration}, duration totalMins {result.TotalMinutes}");
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
            var noViewCounter = 0;
            var noViewDur = 0.0;
            var year2Dur = 0.0;
            var year2Count = 0;
            var pivotDate = new DateTime(2016, 7, 1);

            Parallel.ForEach(recordings.ToList(), (recCollectionResult, y) =>
            {
                Parallel.ForEach(recCollectionResult.Values, (rec, state) =>
                {
                    if (string.IsNullOrEmpty(rec.ScoId))
                    {
                        _logger.Error("sco-id should not be empty");
                        return;
                    }
                    try
                    {
                        var recDuration = GetDurationOfSingleRec(rec.ScoId);
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
                            biggestRecSco = rec.ScoId;
                        }

                        var scoInfo = _provider.ReportScoViews(rec.ScoId);
                        if (scoInfo.Values == null)
                        {
                            _logger.Warn($"sco-info values are null {rec.ScoId}");
                            return;
                        }
                        if (scoInfo.Values.Count() > 1)
                        {
                            _logger.Error($"sco-info values more than 1 {rec.ScoId}");
                            return;
                        }
                        var content = scoInfo.Values.ToList()[0];

                        if (content.Views == 0 && rec.DateCreated < new DateTime(pivotDate.Year - 1, pivotDate.Month, pivotDate.Day))
                        {
                            noViewCounter++;
                            noViewDur += GetDurationOfSingleRec(rec.ScoId);
                        }
                        // 2nd report > 2 year, views >= 1
                        if (content.Views > 0 && content.LastViewedDate < new DateTime(pivotDate.Year - 2, pivotDate.Month, pivotDate.Day))
                        {
                            year2Count++;
                            year2Dur += GetDurationOfSingleRec(rec.ScoId);
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.ErrorFormat("error getting rec duration", exception);
                        Console.WriteLine($"Rec with id {rec.ScoId} was skipped, coz when getting duration there was an error");
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
                Counter10hoursDur = counter10hoursDur,
                Counter3to10hoursDur = counter3to10hoursDur,
                NoViewsCount = noViewCounter,
                NoViewsDuration = noViewDur,
                Year2Count = year2Count,
                Year2Duration = year2Dur
            };
        }
    }
}