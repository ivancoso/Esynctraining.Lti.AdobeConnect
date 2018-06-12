using System.Collections.Generic;
using Esynctraining.Zoom.ApiWrapper;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class ZoomApiWrapperContainer
    {
        private static readonly Dictionary<string, ZoomApiWrapper> _clients = new Dictionary<string, ZoomApiWrapper>(); //todo: lifestyle?
        public ZoomApiWrapper ZoomApiWrapper { get; private set; }
        public void Set(ZoomApiOptions options)
        {
            if (!_clients.ContainsKey(options.ZoomApiKey))
            {
                _clients.Add(options.ZoomApiKey, new ZoomApiWrapper(options));
            }

            ZoomApiWrapper = _clients[options.ZoomApiKey];
        }
    }
}