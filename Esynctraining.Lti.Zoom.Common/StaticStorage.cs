using System;
using System.Collections.Concurrent;

namespace Esynctraining.Lti.Zoom.Common
{
    public class StaticStorage
    {
        public static ConcurrentBag<Guid> RequestedLicenses = new ConcurrentBag<Guid>();
        public static NamedLocker NamedLocker = new NamedLocker();
    }
}