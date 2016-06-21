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
        [TestCase("https://connect.fiu.edu/api/xml", "mkollen", "e$ync123")]
        public void WillGetRecordingsStats(string apiUrl, string login, string password, int totalObjCount = 0)
        {
            _connectionDetails.ServiceUrl = apiUrl;
            _provider = new AdobeConnectProvider(_connectionDetails);
            var provider = _provider;
            LoginResult loginResult = provider.Login(new UserCredentials(login, password));
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");
            var recordings = provider.ReportRecordingsPaged(totalObjCount);
            
            var reports = GetRecLengthStats(recordings);
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
                    var recDuration = GetDurationOfSingleRec(rec.ScoId);
                    duration += recDuration;
                    _logger.Info($"sco-id {rec.ScoId}, duration {recDuration}");
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