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
            var apiUrl = "https://fiustg.adobeconnect.com/api/xml";
            var connectionDetails = new ConnectionDetails(apiUrl);
            var provider = new AdobeConnectProvider(connectionDetails);

            var userCredentials = new UserCredentials(login, password, "965886535");
            LoginResult loginResult = provider.Login(userCredentials);
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");
        }
    }
}