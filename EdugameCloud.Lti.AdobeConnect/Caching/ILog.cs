using System;

namespace EdugameCloud.Lti.AdobeConnect.Caching
{
    public interface ILog
    {
        void WriteLine(string value);

        void WriteLine(Exception ex, string tab = "");

    }

}
