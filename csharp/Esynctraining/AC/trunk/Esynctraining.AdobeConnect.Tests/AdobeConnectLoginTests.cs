using System;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.Tests
{
    public class AdobeConnectLoginTests
    {
        [Test]
        public void WillConnectWithMultipleAcc()// using Thomas creds
        {

            var login = "tbarrene";
            var password = "NEp2NV44Sj";
            var apiUrl = new Uri("https://fiustg.adobeconnect.com");
            var connectionDetails = new ConnectionDetails(apiUrl);
            var provider = new AdobeConnectProvider(connectionDetails);

            var userCredentials = new UserCredentials(login, password, "965886535");
            LoginResult loginResult = provider.Login(userCredentials);
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");
        }

        [Test]
        public void WillGetAccountDetails()
        {
            //var login = "tbarrene";
            //var password = "NEp2NV44Sj";
            // !! User should be an admin!
            var login = "ssotest";
            var password = "Ch@ngeTEST1";
            var apiUrl = new Uri("https://fiustg.adobeconnect.com");
            var connectionDetails = new ConnectionDetails(apiUrl);
            var provider = new AdobeConnectProvider(connectionDetails);

            var userCredentials = new UserCredentials(login, password, "965886535");
            LoginResult loginResult = provider.Login(userCredentials);
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");

            var fakeLogger = new FakeLogger();
            var service = new AdobeConnectAccountService(fakeLogger);
            var principalId = loginResult.User.UserId;
            var proxy = new AdobeConnectProxy(provider, fakeLogger, apiUrl, principalId);
            var details = service.GetAccountDetails(proxy);
            var t = 1;
        }

        [Test]
        public void WillGetAccountDetailsOnLocalAC()
        {
            var login = "mkollen";
            var password = "e$yncFIU2016";
            var apiUrl = new Uri("https://connect.fiu.edu");
            var connectionDetails = new ConnectionDetails(apiUrl);
            var provider = new AdobeConnectProvider(connectionDetails);

            var userCredentials = new UserCredentials(login, password);
            LoginResult loginResult = provider.Login(userCredentials);
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");

            var fakeLogger = new FakeLogger();
            var service = new AdobeConnectAccountService(fakeLogger);
            var principalId = loginResult.User.UserId;
            var proxy = new AdobeConnectProxy(provider, fakeLogger, apiUrl, principalId);
            var details = service.GetAccountDetails(proxy);
            var t = 1;
        }

        [Test]
        public void WillTestHowBreezeSessionStored()
        {
            //var login = "ssotest";
            //var password = "Ch@ngeTEST1";
            var serverAc = new Uri("https://fiustg.adobeconnect.com");
            //var apiUrl = "https://fiustg.adobeconnect.com";
            //var connectionDetails = new ConnectionDetails(apiUrl);
            //var provider = new AdobeConnectProvider(connectionDetails);

            var acConnect = new AdobeConnectAccountService(new FakeLogger());
            //var breeze = "na2breez22yvafwxdrmu8ov3";
            var breeze = "na2breezhcan5ks479qwvmsf";
            var result = acConnect.GetProvider2(new AdobeConnectAccess2(serverAc, breeze));

            //var userCredentials = new UserCredentials(login, password, "965886535");
            //LoginResult loginResult = provider.Login(userCredentials);
            //if (!loginResult.Success)
            //    throw new InvalidOperationException("Invalid login");

            var commonInfo = result.ReportMyMeetings();
        }
    }
}