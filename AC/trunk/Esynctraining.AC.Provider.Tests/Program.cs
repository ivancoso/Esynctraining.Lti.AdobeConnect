using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.AC.Provider.DataObjects;
using NUnit.Framework;

namespace Esynctraining.AC.Provider.Tests
{
    using System.Xml;

    using Esynctraining.AC.Provider.EntityParsing;

    class Program
    {
        static void Main(string[] args)
        {
            WillCreateEvent().Wait();
        }

        [Test]
        public void WillGetUserTrainingsTaken()
        {
            var principalId = "1004370065";
            var acApiUrl = "http://rhi.adobeconnect.com/";
            var con = new ConnectionDetails(new Uri(acApiUrl)) ;
            var acProvider = new AdobeConnectProvider(con);
            var loginResult = acProvider.Login(new UserCredentials("mike@esynctraining.com", "e$ync123RHI"));
            if (!loginResult.Success) throw new InvalidOperationException("Can't login as admin");
            var result = acProvider.ReportUserTrainingTaken(principalId);
        }

        [Test]
        public void TestHttpClientCookie()
        {
            var cookieContainer = new CookieContainer();
            var baseAddress = new Uri("https://connectstage.esynctraining.com/");
            cookieContainer.Add(baseAddress,new Cookie("TestCookie", "Mycookie"));
            var handler = new HttpClientHandler(){CookieContainer = cookieContainer };
            
            using (var client = new HttpClient(handler){BaseAddress = baseAddress})
            {
                var result = client.PostAsync("test", new StringContent("test-content")).Result;
            }
        }

        [Test]
        public static async Task WillCreateEvent()
        {
            var acApiUrl = "https://connectstage.esynctraining.com/";
            var con = new ConnectionDetails(new Uri(acApiUrl));
            var acProvider = new AdobeConnectProvider(con);
            var login = "nastya@esynctraining.com";
            var password = "Welcome1";
           
            var saveEventFields = new SaveEventFields()
            {
                StartDate = DateTime.Now,
                EventTemplateId = "11036",
                Name = $"__newMyTest{DateTime.Now:yyyy-M-d hh-mm-ss}",
                EventInfo = "info",
                EndDate = DateTime.Now.AddHours(1),
                EventType = "meeting",
                ListScoId = "304292",
                OwnerPermissionId = "host",
                EventCategory = "live",
                LoggedInAccess = "view",
                PasswordByPass = false,
                CatalogView = "remove",
                DefaultCatalogView = "view",
                ShowInCatalog = true,
                DefaultRegistrationType = "advance",
                Description = String.Empty,
                UrlPath = string.Empty,
                TimeZoneId = 4,
                Lang = "en",
                Feature = "Next >",
                RegistrationLimitEnabled = false,
                AdminUser = new UserCredentials(login, password)
            };
            var result = await acProvider.CreateEvent(saveEventFields);
        }
    }
}
