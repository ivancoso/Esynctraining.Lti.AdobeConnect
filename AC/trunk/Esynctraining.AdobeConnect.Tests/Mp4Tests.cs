using System;
using Castle.Windsor;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;
using Esynctraining.Windsor;
using log4net.Config;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.Tests
{
    public class Mp4Tests
    {
        [Test]
        public void WillGetMp4Value()
        {
            var container = new WindsorContainer();
            WindsorIoC.Initialize(container);
            DIConfig.RegisterComponents(container);
            XmlConfigurator.Configure();

            var logger = IoC.Resolve<ILogger>();
            var serviceUrl = "http://connectdev.esynctraining.com/api/xml";
            var proxy = new AdobeConnectProxy(new AdobeConnectProvider(new ConnectionDetails(serviceUrl)), logger, serviceUrl, string.Empty  );
            var loginResult = proxy.Login(new UserCredentials("anton@esynctraining.com", "NEp2NV44Sj"));
            var getFieldValue = proxy.GetAclField("44636", "x-385564");
            
            var mp4 = JsonConvert.DeserializeObject<Mp4Dto>(getFieldValue);
        }
    }


    public class Mp4Dto
    {
        public string mp4 { get; set; }
        public string vtt { get; set; }
    }
}