using System;
using Esynctraining.Core;

namespace Esynctraining.Lti.Zoom.Api
{
    public class ZoomLicenseException : Exception, IUserMessageException
    {
        public ZoomLicenseException(Guid key, string message) : base(message)
        {
            Key = key;
        }

        public Guid Key { get; set; }
    }
}