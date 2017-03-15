using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
