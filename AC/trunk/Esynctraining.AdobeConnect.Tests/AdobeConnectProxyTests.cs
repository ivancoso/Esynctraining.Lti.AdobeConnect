using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using NSubstitute;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.Tests
{
    public class AdobeConnectProxyTests
    {

        [Test]
        public void WillReportUserTrainingsTaken()
        {
            var principalId = "1004370065";
            var acApiUrl = new Uri("http://rhi.adobeconnect.com");
            var con = new ConnectionDetails(acApiUrl);
            var acProvider = new AdobeConnectProvider(con);
            var proxy = new AdobeConnectProxy(acProvider, new FakeLogger(), acApiUrl, String.Empty);
            //proxy.report
            proxy.Login(new UserCredentials("mike@esynctraining.com", "e$ync123RHI"));//admin
            var result = proxy.ReportUserTrainingsTaken(principalId);
        }

        [Test]
        public void WillGetAllByPrincipalIds()
        {
            //var proxy = Substitute.For<IAdobeConnectProxy>();
            var ids = new List<string>();
            var apiUrl = new Uri("https://fiustg.adobeconnect.com");
            var connectionDetails = new ConnectionDetails(apiUrl);
            var login = "ssotest";
            var password = "Ch@ngeTEST1";
            var userCredentials = new UserCredentials(login, password, "965886535");
            var provider = new AdobeConnectProvider(connectionDetails);
            LoginResult loginResult = provider.Login(userCredentials);
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");
            var principalId = loginResult.User.UserId;

            var proxy = new AdobeConnectProxy(provider, new FakeLogger(), apiUrl, principalId);
            //var acProvider = Substitute.ForPartsOf<AdobeConnectProvider>();
            //acProvider.When(x => x.GetAllByPrincipalIds())
            //for (var i = 0; i < 375; i++)
            //{
            //    ids.Add(i);
            //}
            ids = new List<string>() 
            {
                "969980305",
"969994159",
"970000385",
"970031418",
"988325146",
"988325884",
"988333621",
"988336193",
"988345341",
"988345796",
"988392645",
"988392238",
"988392115",
"988390536",
"988386250",
"988385271",
"988384635",
"988383270",
"988383256",
"988382042",
"988381526",
"988374150",
"988372782",
"988372008",
"988371915",
"988371234",
"988364932",
"988363108",
"988362537",
"988361823",
"988466848",
"988464487",
"988463493",
"988463137",
"988462642",
"988462637",
"988458941",
"988458741",
"988458278",
"988458012",
"988456474",
"988449598",
"988441839",
"988441817",
"988439110",
"988438412",
"988437927",
"988437623",
"988433234",
"988432937",
"988429393",
"988423628",
"988421518",
"988419726",
"988419367",
"988417948",
"988416743",
"988415048"

            };
            var result = proxy.GetAllByPrincipalIds(ids.Select(x => x.ToString()).ToArray());

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Values.Any());
        }
    }
}