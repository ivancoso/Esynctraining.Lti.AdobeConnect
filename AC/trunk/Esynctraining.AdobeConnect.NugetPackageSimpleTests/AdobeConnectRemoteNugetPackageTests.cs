using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.NugetPackageSimpleTests
{
    public class AdobeConnectRemoteNugetPackageTests
    {
        [Test]
        public void TestNewEventFieldsInScoInfo()
        {
            var acApiUrl = new Uri("https://connect.esynctraining.com");
            var con = new ConnectionDetails(acApiUrl);
            var acProvider = new AdobeConnectProvider(con);
            var proxy = new AdobeConnectProxy(acProvider, new FakeLogger(), acApiUrl);
            proxy.Login(new UserCredentials("nastya@esynctraining.com", "Nastya123"));
            var eventInfo = proxy.GetScoInfo("2957329");
            Assert.Equals(eventInfo.ScoInfo.EventTemplateScoId, 56489);
        }
    }
}
