using System;

namespace EdugameCloud.Lti.AdobeConnect.Caching
{
    internal sealed class DummyLog : ILog
    {
        public void WriteLine(string value)
        {
            // NOTE: do nothing
        }

        public void WriteLine(Exception ex, string tab = "")
        {
            // NOTE: do nothing
        }

    }

}
