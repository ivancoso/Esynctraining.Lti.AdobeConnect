using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EdugameCloud.Tests
{
    using System.Threading;

    using Weborb.Messaging.Api.Service;
    using Weborb.Messaging.Net.RTMP;
    using Weborb.Types;

    [TestClass]
    public class RTMPTest
    {

        public static Guid? clientId = null;

        public void Connect()
        {
            // Create client
            rtmpClient = new RTMPClient();
            // Connect to the server. Connection notification is asynchronous.
            rtmpClient.connect("localhost", 2037, "DBChangesNotifier", null, new ConnectionHandler());
        }

        [TestInitialize]
        public void StartUp()
        {
            Connect();
        }

        [TestMethod]
        public void GetAllocatedClientId()
        {
            rtmpClient.invoke("getAllocatedClientId", new ClientIdHandler());
            int counter = 0;
            while (clientId == null && counter < 60)
            {
                Thread.Sleep(1000);
                counter++;
            }

            Assert.IsFalse(clientId == Guid.Empty);
        }
        private RTMPClient rtmpClient;
    }

    public class ConnectionHandler : IPendingServiceCallback
    {
        /// <summary>
        /// The result received.
        /// </summary>
        /// <param name="call">
        /// The call.
        /// </param>
        public void resultReceived(IPendingServiceCall call)
        {
            Console.WriteLine("RTMP Client is connected.");
        }
    }

    public class ClientIdHandler : IPendingServiceCallback
    {
        /// <summary>
        /// The result received.
        /// </summary>
        /// <param name="call">
        /// The call.
        /// </param>
        public void resultReceived(IPendingServiceCall call)
        {
            var stringObbj = (IAdaptingType)call.getResult();
            var res = (string)stringObbj.adapt(typeof(string));
            Console.WriteLine("RTMP Client Id = " + res);
            RTMPTest.clientId = Guid.Parse(res);
        }
    }

}
